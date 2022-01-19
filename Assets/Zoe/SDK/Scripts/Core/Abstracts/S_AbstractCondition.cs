using UnityEngine;

namespace SpatialStories
{
    public abstract partial class S_AbstractCondition : S_AbstractInteractionDataMonoBehaviour
    {
        public const string EVENT_CONDITION_VALIDATED           = "EVENT_CONDITION_VALIDATED";
        public const string EVENT_CONDITION_INVALIDATED         = "EVENT_CONDITION_INVALIDATED";
        public const string EVENT_CONDITION_DURATION_PROGRESS   = "EVENT_CONDITION_DURATION_PROGRESS";
        public const string EVENT_CONDITION_DURATION_DEACTIVATED = "EVENT_CONDITION_DURATION_DEACTIVATED";

        public bool ShowOptions = false;

        // DURATION
        public bool IsDuration = false;
        public float FocusDuration = 0;
        public int FocusLossModeIndex;
        public float FocusLossSpeed = 1;

        protected bool m_durationActivated = false;
        protected float m_durationTimeActivated = 0;
        protected float m_durationReport = 0;

        // INITIALITZATION
        protected bool m_listenersSetUp = false;
        protected bool m_hasBeenDestroyed = false;


        public delegate void OnConditionEventHandler(S_AbstractCondition _condition);
        /// <summary>
        /// Fired when the condition are satisfied (used for dependencies)
        /// </summary>
        public event OnConditionEventHandler OnConditionSatisfied;

        public bool IsValid { get; private set; }

        public bool IsSync = false;

        private S_Interaction m_Interaction;
        public S_Interaction ConditionsManager
        {
            get
            {
                if ((m_Interaction == null) && (this != null))
                    m_Interaction = GetComponent<S_Interaction>();
                return m_Interaction;
            }
        }

        private S_InteractiveObject m_InteractiveObject;
        public S_InteractiveObject InteractiveObject
        {
            get
            {
                if (m_InteractiveObject == null)
                    m_InteractiveObject = this.GetComponentInParent<S_InteractiveObject>(true);
                return m_InteractiveObject;
            }
        }

        public abstract void Dispose();

        public abstract void Setup();

        public abstract void Reload();

        public virtual void Validate()
        {
            Validate(true);
        }

        public virtual void Validate(bool runEvent)
        {
            Debug.Log("Validate::[" + this.gameObject.name + "]::ROOT["+ this.gameObject.transform.root.name +"]");
            IsValid = true;
            m_durationTimeActivated = -1;
            m_durationActivated = false;
            ConditionsManager.OnConditionValid(this);

            if (OnConditionSatisfied != null && runEvent)
                OnConditionSatisfied(this);

            if ((this != null) && (this.gameObject != null))
            {
                BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_CONDITION_VALIDATED, this.gameObject);
            }
        }

        protected virtual void Invalidate()
        {
            IsValid = false;
            m_durationTimeActivated = -1;
            m_durationActivated = false;
            ConditionsManager.InvalidateCondition(this);
            if ((this != null) && (this.gameObject != null))
            {
                BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_CONDITION_INVALIDATED, this.gameObject);
            }
        }

        public virtual void OnDestroy()
        {
        }

        protected virtual bool ActivateDuration()
        {
            if (IsDuration)
            {
                m_durationActivated = true;
                if (m_durationTimeActivated == -1)
                {
                    m_durationTimeActivated = 0;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual bool DeactivateDuration()
        {
            if (IsDuration)
            {
                m_durationActivated = false;
                if (m_durationTimeActivated == -1)
                {
                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_CONDITION_DURATION_DEACTIVATED, this.gameObject);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ReportDuration(bool _force)
        {
            m_durationReport += Time.deltaTime;
            if ((m_durationReport > 0.1f) || _force)
            {
                m_durationReport = 0;
                BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_CONDITION_DURATION_PROGRESS, this.gameObject, m_durationTimeActivated);
            }
        }

        protected void UpdateBasicDuration()
        {
            if (IsDuration)
            {
                if (IsValid) return;

                if (m_durationActivated)
                {
                    if (m_durationTimeActivated >= 0)
                    {
                        m_durationTimeActivated += Time.deltaTime;
                        ReportDuration(false);
                        if (m_durationTimeActivated > FocusDuration)
                        {
                            m_durationTimeActivated = -1;
                            m_durationActivated = false;
                            Validate();
                        }
                    }
                }
                else
                {
                    bool reportDurationRestart = false;
                    if (m_durationTimeActivated > 0)
                    {
                        reportDurationRestart = true;
                    }

                    switch ((S_FocusLossMode)FocusLossModeIndex)
                    {
                        case S_FocusLossMode.KEEP_PROGRESS:
                            break;
                        case S_FocusLossMode.DECREASE_PROGRESSIVELY:
                            m_durationTimeActivated -= Time.deltaTime * FocusLossSpeed;
                            if (m_durationTimeActivated < 0)
                            {
                                m_durationTimeActivated = 0;
                                if (reportDurationRestart)
                                {
                                    ReportDuration(true);
                                }
                            }
                            else
                            {
                                ReportDuration(false);
                            }
                            break;
                        case S_FocusLossMode.RESET_PROGRESS:
                            m_durationTimeActivated = 0;
                            if (reportDurationRestart)
                            {
                                ReportDuration(true);
                            }
                            break;
                    }
                }
            }
        }

        public virtual void Update()
        {
            UpdateBasicDuration();
        }
    }
}