using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [Serializable]
    public class S_Interaction : S_AbstractEntity
    {
        public delegate void OnInteractionEventHandler(S_Interaction _conditionsManager);

        /// <summary>
        /// Fired when all the conditions of the manager are satisfied (used for dependencies)
        /// </summary>
        public event OnInteractionEventHandler OnInteractionSatisfied;

        /// <summary>
        /// Fired when the all the conditions are reloaded
        /// </summary>
        public event OnInteractionEventHandler OnInteractionReload;
        private void FireOnInteractionReload(S_Interaction _conditionManager)
        {
            if (OnInteractionReload != null)
                OnInteractionReload(_conditionManager);
        }

        /// <summary>
        /// All the conditions that need to be satisfied
        /// </summary>
        public S_AbstractCondition[] Conditions;

        /// <summary>
        /// The amount of time in seconds the conditions will wait before being evaluated
        /// </summary>
        public float Delay;

        /// <summary>
        /// Defines what will be reloaded.
        /// </summary>
        public bool EnableRepeat = false;
        public S_ReloadSubject ReloadSubject;
        public bool IsInfiniteRepeat;
        public int IndexRepeatOptions = -1;

        /// <summary>
        /// Represents the limit in quantity this interaction can be reloaded at.
        /// </summary>
        public S_ReloadLimit ReloadLimit;

        /// <summary>
        /// Is the maximum number of times this interaction can be reloaded (works with ReloadLimit FINITE only)
        /// </summary>
        public int ReloadMax;

        /// <summary>
        /// The amount of time in seconds to reload the condition
        /// </summary>
        public float DelayReload = 0;

        /// <summary>
        /// Defines whether this interaction has any synced condition or not.
        /// </summary>
        private bool m_AnyConditionIsSync = false;

        /// <summary>
        /// Keeps the number of time this interaction has been reloaded
        /// Used to stop reloads when ReloadMax is reached
        /// </summary>
        private int m_ReloadCount;
        public int ReloadCount { get { return m_ReloadCount; } }

        /// <summary>
        /// Defines whether this interaction is active or not.
        /// If not active, nothing is computed.
        /// </summary>
        private bool m_IsActive = true;

        private bool m_ConditionsSetup = false;

        /// <summary>
        /// 
        /// </summary>
        public SC_InteractionDependencies InteractionDependencies;

        /// <summary>
        /// All the actions to perform once this interaction is satisfied.
        /// </summary>
        private S_AbstractAction[] m_Actions;

        /// <summary>
        /// The Interactive Obect this interaction belongs to (the root)
        /// </summary>
        private S_InteractiveObject m_Io;

        /// <summary>
        /// Stores the validity state for each condition.
        /// Used to determine if this interaction is valid (when all the conditions are valid).
        /// </summary>
        private bool[] m_ConditionsValidity;

        /// <summary>
        /// Is the phase in which the state machine is.
        /// </summary>
        private enum InteractionState
        {
            CHECKING_DEPENDENCIES,
            WAITING_DELAY,
            CHECKING_CONDITIONS,
            NOTIFYING,
            CHECKING_RELOAD_COUNT,
            RELOADING,
            FINISHED
        }

        /// <summary>
        /// Actual interaction state processed by the state machine
        /// </summary>
        private InteractionState m_ActualInteractionState;

        /// <summary>
        /// Defines the deactivation dependencies of the interaction
        /// </summary>
        public SC_InteractionDependencies DeactivationDependencies;

        /// <summary>
        /// Indicates to reset the delay or just interrupt it when the deactivation dependencies had been triggered
        /// </summary>
        public S_DelayOptions DelayOptions;

        /// <summary>
        /// Defines the dependencies that the interaction has in order to activate back after
        /// its deactivation
        /// </summary>
        public SC_InteractionDependencies ActivationDependencies;

        /// <summary>
        /// Last tasks sended to the scheduler
        /// </summary>
        private S_SchedulerTask m_LastSchedulerTask;

        /// <summary>
        /// All the info about the interaction used from the editor
        /// </summary>
        public S_InteractionStatus InteractionStatus { get; private set; } = new S_InteractionStatus();

        /// <summary>
        /// Defines the time the interaction is enabled once activated
        /// </summary>
        public bool IsDuration = false;
        public float FocusDuration = 0;
        public int FocusLossModeIndex;
        public float FocusLossSpeed = 1;

        /// <summary>
        /// Adding new custom conditions and custom actions
        /// </summary>
        public S_CustomCondition CustomCondition;
        public int IndexCustomCondition = -1;
        public S_CustomAction CustomAction;
        public int IndexCustomAction = -1;

        private void Start()
        {
            InteractionStatus.DurationActivated = FocusDuration;

            // Allows the system to deactivate the interaction in any time
            SetupDeactivationLogic();
            ProcessState(InteractionState.CHECKING_DEPENDENCIES);
        }


        /// <summary>
        /// Used to progress in the state machine by defining the next state to enter.
        /// </summary>
        /// <param name="_state"></param>
        private void ProcessState(InteractionState _state)
        {
            m_ActualInteractionState = _state;
            bool stateFinnished = false;
            InteractionState nextState = InteractionState.FINISHED;
            switch (_state)
            {
                case InteractionState.CHECKING_DEPENDENCIES:
                    stateFinnished = DependencyState();
                    if (stateFinnished)
                    {
                        ProcessState(InteractionState.WAITING_DELAY);
                    }
                    break;
                case InteractionState.WAITING_DELAY:
                    stateFinnished = DelayState();
                    if (stateFinnished)
                    {
                        ProcessState(InteractionState.CHECKING_CONDITIONS);
                    }
                    break;
                case InteractionState.CHECKING_CONDITIONS:
                    stateFinnished = CheckConditionsState();
                    if (stateFinnished)
                    {
                        ProcessState(InteractionState.NOTIFYING);
                    }
                    break;
                case InteractionState.NOTIFYING:
                    stateFinnished = NotifyState();
                    if (stateFinnished)
                    {
                        ProcessState(InteractionState.CHECKING_RELOAD_COUNT);
                    }
                    break;
                case InteractionState.CHECKING_RELOAD_COUNT:
                    nextState = CheckingReloadCounter();
                    ProcessState(nextState);
                    break;
                case InteractionState.RELOADING:
                    m_LastSchedulerTask = S_Scheduler.AddUniqueTask(DelayReload, ReloadInteractionAction);
                    break;
                default:
                    break;

            }
        }

        private void ReloadInteractionAction()
        {
            InteractionState nextState = ReloadInteraction();
            ProcessState(nextState);
        }

        /// <summary>
        /// Logic to deactivate the interaction when is on the checking dependencies state
        /// </summary>
        private void DeactivateInDependencyState()
        {
            if (InteractionDependencies != null && InteractionDependencies.IsSetup)
                InteractionDependencies.Dispose(false);
        }

        /// <summary>
        /// Checks for dependencies and setup them if any.
        /// </summary>
        /// <returns>true if this interaction has no dependencies</returns>
        private bool DependencyState(bool _deactivating = false)
        {
            InteractionDependencies = GetDependencies();
            if (InteractionDependencies != null && InteractionDependencies.Dependencies.Length > 0)
            {
                InteractionDependencies.Setup(false);
                InteractionDependencies.SetDependenciesSatisfiedCallback(OnDependenciesValid);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Update the state machine and dispose the dependency
        /// </summary>
        public void OnDependenciesValid()
        {
            InteractionDependencies.Dispose(false);
            ProcessState(InteractionState.WAITING_DELAY);
        }

        /// <summary>
        /// Returns the interaction dependencies if any.
        /// </summary>
        /// <returns> the interaction dependencies or null  </returns>
        private SC_InteractionDependencies GetDependencies()
        {
            if (InteractionDependencies != null)
                return InteractionDependencies;

            SC_InteractionDependencies[] dependencies = GetComponents<SC_InteractionDependencies>();

            for (int i = 0; i < dependencies.Length; i++)
            {
                SC_InteractionDependencies dep = dependencies[i];
                if (dep != DeactivationDependencies && dep != ActivationDependencies)
                    return dep;
            }

            return null;
        }
        
        /// <summary>
        /// Logic to deactivate the intearction when is on the deactivate delay state
        /// </summary>
        private void DeactivateInDelayState()
        {
            if (m_LastSchedulerTask != null && m_LastSchedulerTask.IsAlive)
                m_LastSchedulerTask.Interrupt();
        }

        /// <summary>
        /// Logic to deactivate the intearction when is on the deactivate delay state
        /// </summary>
        private void DestroyInDelayState()
        {
            if (m_LastSchedulerTask != null && m_LastSchedulerTask.IsAlive)
                m_LastSchedulerTask.IsAlive = false;
        }

        /// <summary>
        /// Reports if the task is still alive
        /// </summary>
        public bool IsLastSchedulerTaskAlive()
        {
            if (m_LastSchedulerTask != null && m_LastSchedulerTask.IsAlive)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The state where the interaction process the delay if any.
        /// </summary>
        /// <returns>true if there's no delay or if the delay is finished</returns>
        private bool DelayState()
        {
            if (Delay > 0)
            {
                InteractionStatus.SetDelayStarted(true);

                m_LastSchedulerTask = S_Scheduler.AddTask(Delay, (DelayOptions == S_DelayOptions.RESET_DELAY), () =>
                {
                    ProcessState(InteractionState.CHECKING_CONDITIONS);
                    InteractionStatus.SetDelayCompleted(true);
                });
                return false;
            }
            else
            {
                return true;
            }
        }
        

        /// <summary>
        /// Logic to deactivate the interaction when is on the checking conditions state
        /// </summary>
        private void DeactivateInCheckingConditionsState()
        {
            if (Conditions != null && Conditions.Length > 0)
                ConditionsDispose();
        }

        /// <summary>
        /// Gets the conditions and setup them
        /// </summary>
        /// <returns>true if no conditions to setup</returns>
        private bool CheckConditionsState()
        {
            GetConditions();
            if (Conditions.Length > 0)
            {
                ConditionsSetup();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Populates a list of all the conditions but the SC_Dependencies
        /// </summary>
        private void GetConditions()
        {
            // if we already get the conditions, exit
            if (Conditions != null && Conditions.Length > 0)
                return;

            // get all the conditions
            List<S_AbstractCondition> conditionsList = new List<S_AbstractCondition>(GetComponents<S_AbstractCondition>());
            int start = conditionsList.Count - 1;

            // Check if any condition is sync and remove dependencies
            S_AbstractCondition condition;
            for (int i = start; i >= 0; i--)
            {
                // subscribe if it's not a dependency condition
                condition = conditionsList[i];
                if (condition is SC_InteractionDependencies)
                {
                    conditionsList.RemoveAt(i);
                }
                else
                {
                    m_AnyConditionIsSync |= condition.IsSync;
                }
            }

            m_ConditionsValidity = new bool[conditionsList.Count];
            ResetConditionsValidity();
            Conditions = conditionsList.ToArray();
        }

        /// <summary>
        /// Resets the validity of all the conditions in this interaction (set false)
        /// </summary>
        public void ResetConditionsValidity()
        {
            for (int i = 0; i < m_ConditionsValidity.Length; i++)
            {
                m_ConditionsValidity[i] = false;
            }
            InteractionStatus.SetAreConditionsValid(this, false);
        }

        /// <summary>
        /// Set the validity of a certain condition to false
        /// </summary>
        /// <param name="_condition"></param>
        public void InvalidateCondition(S_AbstractCondition _condition)
        {
            S_AbstractCondition cond;
            for (int i = 0; i < Conditions.Length; i++)
            {
                cond = Conditions[i];
                if (_condition == cond)
                {
                    m_ConditionsValidity[i] = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Check if all the conditions are valid
        /// </summary>
        private bool AreAllConditionsValid(S_AbstractCondition _condition)
        {
            bool valid = true;
            S_AbstractCondition cond;
            for (int i = 0; i < Conditions.Length; i++)
            {
                cond = Conditions[i];
                if (_condition == cond)
                {
                    m_ConditionsValidity[i] = true;
                }
                valid &= m_ConditionsValidity[i];
            }

            return valid;
        }

        /// <summary>
        /// Setup all the conditions
        /// </summary>
        public void ConditionsSetup()
        {
            if (m_ConditionsSetup)
            {
                return;
            }

            GetConditions();
            for (int i = 0; i < Conditions.Length; i++)
                Conditions[i].Setup();

            // This is a bit of a hack...
            // Since we made this method public for asynchronous building of play mode
            // scene, the following, which is more important than it should be held the
            // wrong state. Now, make sure we set it to the right state.
            // N.B. This should be temporary and a better solution should be found
            m_ActualInteractionState = InteractionState.CHECKING_CONDITIONS;

            m_ConditionsSetup = true;
        }

        /// <summary>
        ///  Dispose all the conditions
        /// </summary>
        private void ConditionsDispose()
        {
            for (int i = 0; i < Conditions.Length; i++)
                Conditions[i].Dispose();

            m_ConditionsSetup = false;
        }

        /// <summary>
        /// Is called when a condition has been validated.
        /// Check if all conditions are valid to dispose them if true.
        /// Changes the state to NOTIFY.
        /// </summary>
        /// <param name="_condition"></param>
        public void OnConditionValid(S_AbstractCondition _condition)
        {
            InteractionStatus.SetAreConditionsValid(this, AreAllConditionsValid(_condition));

            if (InteractionStatus.AreConditionsValid)
            {
                ConditionsDispose();
                ProcessState(InteractionState.NOTIFYING);
            }
            else
            {
                if (m_AnyConditionIsSync)
                    S_Scheduler.AddUniqueTaskAtNextFrame(InvalidateAllSyncConditions);
            }
        }

        /// <summary>
        /// Invalidate all the synchronized conditions in this interaction
        /// </summary>
        private void InvalidateAllSyncConditions()
        {
            for (int i = 0; i < Conditions.Length; i++)
            {
                if (Conditions[i].IsSync)
                {
                    m_ConditionsValidity[i] = false;
                }
            }
        }

        /// <summary>
        /// Invalidate all the conditions in this interaction
        /// </summary>
        private void InvalidateAllConditions()
        {
            for (int i = 0; i < Conditions.Length; i++)
            {
                m_ConditionsValidity[i] = false;
            }
        }

        /// <summary>
        /// Call the reload method for each condition
        /// </summary>
        private void ReloadConditions()
        {
            for (int i = 0; i < Conditions.Length; i++)
            {
                Conditions[i].Reload();
            }
        }
        

        /// <summary>
        /// Logic to deactivate the interaction when is on the notifiying state
        /// </summary>
        private void DeactivateInNotifyingState()
        {
            if (m_LastSchedulerTask != null && m_LastSchedulerTask.IsAlive)
                m_LastSchedulerTask.Interrupt();
        }

        /// <summary>
        /// Schedules the notify of an interaction for the next update
        /// </summary>
        /// <returns></returns>
        private bool NotifyState()
        {
            m_LastSchedulerTask = S_Scheduler.AddTaskAtNextFrame(Notify);
            return true;
        }

        /// <summary>
        /// Overload of Notify to use as an action with fireEvent=true;
        /// actions.
        /// </summary>
        private void Notify()
        {
            Notify(true);
        }

        /// <summary>
        /// Notify to all the dependencies that the interaction has triggered and executes all the
        /// actions.
        /// This can be called without calling the event to manually trigger the actions.
        /// </summary>
        public void Notify(bool fireEvent)
        {
            if (OnInteractionSatisfied != null && fireEvent)
                OnInteractionSatisfied(this);

            ExecuteActions();
            InteractionStatus.RegisterValidation();
        }

        /// <summary>
        /// Executes all the actions attached to the interaction
        /// </summary>
        private void ExecuteActions()
        {
            // Get the actions
            if (m_Actions == null || m_Actions.Length == 0)
                m_Actions = GetComponents<S_AbstractAction>();

            if ((m_Io == null) && (this != null))
                m_Io = GetComponentInParent<S_InteractiveObject>();

            // Execute the actions
            if (m_Io != null)
            {
                for (int i = 0; i < m_Actions.Length; i++)
                {
                    m_Actions[i].Execute(m_Io);
                }
            }
        }
        

        /// <summary>
        /// Checks if the interaction reached its reload limit if any
        /// </summary>
        /// <returns></returns>
        private InteractionState CheckingReloadCounter()
        {
            if ((ReloadLimit == S_ReloadLimit.FINITE && m_ReloadCount < ReloadMax) || ReloadLimit == S_ReloadLimit.INFINITE)
            {
                m_ReloadCount++;
                return InteractionState.RELOADING;
            }
            return InteractionState.FINISHED;
        }        

        /// <summary>
        /// Logic to reload the interaction when is on the reloading state
        /// </summary>
        private void DeactivateInReloadingState()
        {
            if (m_LastSchedulerTask != null && m_LastSchedulerTask.IsAlive)
                m_LastSchedulerTask.Interrupt();

            S_Scheduler.RemoveTask(ReloadInteractionAction);
        }

        /// <summary>
        /// Reloads the interaction
        /// </summary>
        /// <returns> Returns the next state after the reload of the interaction </returns>
        private InteractionState ReloadInteraction()
        {
            InvalidateAllConditions();
            ReloadConditions();
            InteractionStatus.Reload(ReloadSubject);
            switch (ReloadSubject)
            {
                case S_ReloadSubject.CONDITIONS:
                    FireOnInteractionReload(this);
                    return InteractionState.CHECKING_CONDITIONS;
                case S_ReloadSubject.CONDITIONS_AND_DEPENDENCIES:
                    FireOnInteractionReload(this);
                    return InteractionState.CHECKING_DEPENDENCIES;
                default:
                    return InteractionState.FINISHED;
            }
        }
        


        /// <summary>
        /// Susbscribes the interaction to the deactivation logic
        /// </summary>
        private void SetupDeactivationLogic()
        {
            if (DeactivationDependencies != null)
            {
                DeactivationDependencies.Setup(false);
                DeactivationDependencies.SetDependenciesSatisfiedCallback(OnDeactivate);
            }
        }


        /// <summary>
        /// Subscribes to the activation dependencies
        /// </summary>
        private void SetupActivationLogic()
        {
            if (ActivationDependencies != null)
            {
                ActivationDependencies.Setup(false);
                ActivationDependencies.SetDependenciesSatisfiedCallback(OnActivate);
            }
        }

        /// <summary>
        /// Stops every process being done in this interaction
        /// </summary>
        public void OnDeactivate()
        {
            // Don't listen anymore to the deactivation events
            DeactivationDependencies.Dispose(false);
            StopCurrentStateLogic();
            m_IsActive = false;
            // Begin to listen the activation events (it's the only thing that the inteaction can do now)
            SetupActivationLogic();

            // RESET MAIN DEPENDENCIES
            if (InteractionDependencies != null && InteractionDependencies.Dependencies.Length > 0)
            {
                InteractionDependencies.Dispose(false);
                InteractionDependencies.ResetDependencyStatus();
                InteractionDependencies.Setup(false);
            }
        }

        /// <summary>
        /// Do the appropiate things in order to stop the logic going on on the state machine
        /// </summary>
        private void StopCurrentStateLogic()
        {
            switch (m_ActualInteractionState)
            {
                case InteractionState.CHECKING_DEPENDENCIES:
                    DeactivateInDependencyState();
                    break;
                case InteractionState.WAITING_DELAY:
                    DeactivateInDelayState();
                    break;
                case InteractionState.CHECKING_CONDITIONS:
                    DeactivateInCheckingConditionsState();
                    break;
                case InteractionState.NOTIFYING:
                    DeactivateInNotifyingState();
                    break;
                case InteractionState.RELOADING:
                    DeactivateInReloadingState();
                    break;
                default:
                    break;

            }
        }

        /// <summary>
        /// Reactivates the interaction, rolling back to it's last state
        /// </summary>
        public void OnActivate()
        {
            // Stop listening for the activation events
            ActivationDependencies.Dispose(false);

            m_IsActive = true;

            // If we interrupted a delayed task just reeschedule it using the remaining delay
            if (m_ActualInteractionState == InteractionState.WAITING_DELAY && m_LastSchedulerTask != null)
            {
                m_LastSchedulerTask = m_LastSchedulerTask.ReSchedule();
            }
            else
            {
                ProcessState(m_ActualInteractionState);
            }

            // Allow the interaction to deactivate again if necessary
            SetupDeactivationLogic();
        }

        

        public bool HasConditions()
        {
            GetConditions();
            return Conditions != null && Conditions.Length > 0;
        }

        

        /// <summary>
        /// Forces the interaction to validate
        /// </summary>
        public void ForceValidation()
        {
            // Stop whatever is processing
            StopCurrentStateLogic();
            // Set the current state to notifing
            m_ActualInteractionState = InteractionState.NOTIFYING;
            // Process the notifiing event to get the artificial validation
            ProcessState(m_ActualInteractionState);
        }
        

        private void OnDestroy()
        {
            StopCurrentStateLogic();
        }
    }
}