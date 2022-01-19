using UnityEngine;

namespace SpatialStories
{
    public abstract partial class S_AbstractAction : S_AbstractInteractionDataMonoBehaviour
    {
        public const string EVENT_ABSTRACT_ACTION_DELAY_REPORT = "EVENT_ABSTRACT_ACTION_DELAY_REPORT";
        public const string EVENT_ABSTRACT_ACTION_DELAY_TOTAL = "EVENT_ABSTRACT_ACTION_DELAY_TOTAL";

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

        public bool ShowOptions = false;

        // DELAY
        public bool IsDelayed = false;
        public bool Loop = false;
        public float Delay = 0;
        public bool Random = false;
        public float Min = 0;
        public float Max = 1;

        private float m_finalDelay;

        protected abstract void ActionLogic();

        private void ReportDelay()
        {
            m_finalDelay -= (Time.deltaTime * 2);
            if (m_finalDelay < 0) m_finalDelay = 0;
            BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_ABSTRACT_ACTION_DELAY_REPORT, this.gameObject, S_StringUtils.Trim(m_finalDelay.ToString(), 3));
        }

        public void Execute(S_InteractiveObject _io)
        {
            m_InteractiveObject = _io;
            if (((Delay > 0) && IsDelayed) || (Random))
            {
                m_finalDelay = Delay;
                if (Random && (Min < Max))
                {
                    m_finalDelay = UnityEngine.Random.Range(Min, Max);
                }
#if UNITY_EDITOR
                BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_ABSTRACT_ACTION_DELAY_TOTAL, this.gameObject, S_StringUtils.Trim(m_finalDelay.ToString(), 3));
                S_Scheduler.AddTask(m_finalDelay, false, ActionLogic, ReportDelay);
#else
                S_Scheduler.AddTask(m_finalDelay, false, ActionLogic);
#endif
            }
            else
            {
                ActionLogic();
            }
        }
    }
}

