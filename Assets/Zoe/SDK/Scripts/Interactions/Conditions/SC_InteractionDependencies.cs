using System;
using UnityEngine;

namespace SpatialStories
{
    [Serializable]
    public class SC_InteractionDependencies : S_AbstractCondition
    {
        public bool IsSetup = false;

        public S_Interaction[] Dependencies;

        public bool RequireAll = false;

        public bool AreDependenciesValid = false;
        private bool[] m_DependenciesStatus;
        private Action m_DependenciesSatisfiedCallback;

        protected SC_InteractionDependencies()
        {
        }

        public void SetDependenciesSatisfiedCallback(Action _callback)
        {
            m_DependenciesSatisfiedCallback = _callback;
        }

        public override void Dispose()
        {
            throw new NotImplementedException("Use Dispose(bool _reload) instead");
        }

        public override void Setup()
        {
            throw new NotImplementedException("Use Setup(bool _reload) instead");
        }

        public void Setup(bool _reload)
        {
            AreDependenciesValid = false;
            IsSetup = true;
            SubscribeToDependencies(_reload);
        }

        /// <summary>
        /// Unsubscribe from all the dependencies and set the setup flag to false
        /// in order to not listen to them anymore.
        /// </summary>
        /// <param name="_reload">Are the dependencies to unsubscribe from concerning the reload</param>
        public void Dispose(bool _reload)
        {
            IsSetup = false;
            UnsubscribeToDependencies(_reload);
        }

        /// <summary>
        /// Subscribe to all the dependencies this interaction has
        /// in order to listen when they're satisfied.
        /// </summary>
        private void SubscribeToDependencies(bool _reload)
        {
            if (m_DependenciesStatus == null)
            {
                m_DependenciesStatus = new bool[Dependencies.Length];
                ResetDependencyStatus();
            }

            for (int i = 0; i < Dependencies.Length; i++)
            {
                if (Dependencies[i] != null)
                {
                    if (_reload)
                        Dependencies[i].OnInteractionReload += OnDependencyValidated;
                    else
                        Dependencies[i].OnInteractionSatisfied += OnDependencyValidated;
                }
            }
        }

        public void ResetDependencyStatus()
        {
            for (int i = 0; i < Dependencies.Length; i++)
            {
                m_DependenciesStatus[i] = false;
            }
        }

        /// <summary>
        /// Unsubscribe to all dependencies when satisfied
        /// in order to spare useless processing resources.
        /// </summary>
        /// <param name="_reload">are the dependencies concerning the reload</param>
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
                //Debug.Log("OnDependencyValidated::IO[" + this.gameObject.GetComponentInParent<S_InteractiveObject>().name + "][" + this.gameObject.name + "]::IS VALID DEPENDENCY ++++++");
                AreDependenciesValid = true;
                m_DependenciesSatisfiedCallback();
                ResetDependencyStatus();
            }
            else
            {
                Debug.Log("OnDependencyValidated::IO[" + this.gameObject.GetComponentInParent<S_InteractiveObject>().name + "][" + this.gameObject.name + "]::IS INVALID DEPENDENCY --------");
            }
        }

        public override void Reload()
        {
            Setup();
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            
        }
    }
}