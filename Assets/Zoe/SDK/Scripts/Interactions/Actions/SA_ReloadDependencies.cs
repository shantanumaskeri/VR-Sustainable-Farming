using System;
using UnityEngine;

namespace SpatialStories
{
    [Serializable]
    public class SA_ReloadDependencies : S_AbstractAction
    {
        public bool IsSetup = false;

        public bool RequireAll = true;
        public S_Interaction[] Dependencies;
        public bool AreDependenciesValid = false;

        private bool[] m_DependenciesStatus;
        private Action m_DependenciesSatisfiedCallback;

        public SA_ReloadDependencies(){}
        
        private void Start()
        {
            
        }

        public void OnDestroy()
        {
            Dispose(false);
        }

        protected override void ActionLogic()
        {
            Setup(true);
        }

        public void SetDependenciesSatisfiedCallback(Action _callback)
        {
            m_DependenciesSatisfiedCallback = _callback;
        }

        public void Setup(bool _reload)
        {
            AreDependenciesValid = false;
            IsSetup = true;
            SubscribeToDependencies(_reload);
        }

        public void Dispose(bool _reload)
        {
            IsSetup = false;
            UnsubscribeToDependencies(_reload);
        }

        private void SubscribeToDependencies(bool _reload)
        {
            if (m_DependenciesStatus == null)
            {
                m_DependenciesStatus = new bool[Dependencies.Length];
                ResetDependencyStatus();
            }

            for (int i = 0; i < Dependencies.Length; i++)
            {
                if (_reload)
                    Dependencies[i].OnInteractionReload += OnDependencyValidated;
                else
                    Dependencies[i].OnInteractionSatisfied += OnDependencyValidated;
            }
        }

        public void ResetDependencyStatus()
        {
            for (int i = 0; i < Dependencies.Length; i++)
            {
                m_DependenciesStatus[i] = false;
            }
        }

        private void UnsubscribeToDependencies(bool _reload)
        {
            for (int i = 0; i < Dependencies.Length; i++)
            {
                if (_reload)
                    Dependencies[i].OnInteractionReload -= OnDependencyValidated;
                else
                    Dependencies[i].OnInteractionSatisfied -= OnDependencyValidated;
            }
        }

        private void OnDependencyValidated(S_Interaction _conditionsManager)
        {
            bool valid = true;
            for (int i = 0; i < Dependencies.Length; i++)
            {
                if (Dependencies[i] == _conditionsManager)
                    m_DependenciesStatus[i] = true;

                if (RequireAll)
                    valid &= m_DependenciesStatus[i];
                else
                {
                    if (m_DependenciesStatus[i])
                    {
                        valid = true;
                        break;
                    }
                }
            }
            if (this == null) return;
            if (this.gameObject == null) return;
            if (this.gameObject.GetComponentInParent<S_InteractiveObject>() == null) return;
            if (valid)
            {
                Debug.Log("OnDependencyValidated::IO[" + this.gameObject.GetComponentInParent<S_InteractiveObject>().name + "][" + this.gameObject.name + "]::IS VALID DEPENDENCY ++++++");
                AreDependenciesValid = true;
                m_DependenciesSatisfiedCallback();
                ResetDependencyStatus();
            }
            else
            {
                Debug.Log("OnDependencyValidated::IO[" + this.gameObject.GetComponentInParent<S_InteractiveObject>().name + "][" + this.gameObject.name + "]::IS INVALID DEPENDENCY --------");
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
        }
    }

    public static partial class APIExtensions
    {
        public static SA_ReloadDependencies CreateReloadDependenciesBehaviour(this S_InteractionDefinition _def, float _time, string _toMoveGUID, bool _visibility)
        {
            return _def.CreateAction<SA_ReloadDependencies>(_time, _toMoveGUID, _visibility);
        }
    }
}
