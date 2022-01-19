namespace SpatialStories
{
    public class S_InteractionStatus
    {
        public const string EVENT_INTERACTION_STATUS_DURATION_COMPLETED = "EVENT_INTERACTION_STATUS_DURATION_COMPLETED";

        private bool m_areConditionsValid = false;
        private bool m_hasDelayStarted = false;
        private bool m_isDelayCompleted = false;
        private bool m_validated = false;
        private int m_validationsCounter = 0;

        private float m_durationActivated = 0;
        private bool m_durationCompleted = false;
        private S_Interaction m_interaction = null;

        public bool AreConditionsValid { get { return m_areConditionsValid; } }
        public bool HasDelayStarted { get { return m_hasDelayStarted; } }
        public bool IsDelayCompleted { get { return m_isDelayCompleted; } }
        public bool Validated { get { return m_validated; } }
        public int ValidationsCounter { get { return m_validationsCounter; } }

        public float DurationActivated { set { m_durationActivated = value; } }

        public void Reload(S_ReloadSubject _subject)
        {
            if (_subject == S_ReloadSubject.NONE)
                return;

            if ((m_durationActivated > 0) && (!m_durationCompleted))
                return;

            m_areConditionsValid = false;
            m_hasDelayStarted = false;
            m_isDelayCompleted = false;
            m_validated = false;
            m_durationCompleted = false;
        }

        public void RegisterValidation()
        {
            m_validated = true;
            m_validationsCounter++;
        }

        public void SetDelayStarted(bool _value)
        {
            m_hasDelayStarted = _value;
        }

        public void SetDelayCompleted(bool _value)
        {
            m_isDelayCompleted = _value;
        }

        public void SetAreConditionsValid(S_Interaction _interaction, bool _value)
        {
            if ((m_durationActivated <= 0) || (m_durationCompleted))
            {
                m_areConditionsValid = _value;
                m_interaction = null;
                m_durationCompleted = false;
            }
            else
            {
                if ((m_interaction == null) &&  _value)
                {
                    m_areConditionsValid = _value;
                    m_interaction = _interaction;
                    BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);
                    BasicSystemEventController.Instance.DelayBasicSystemEvent(EVENT_INTERACTION_STATUS_DURATION_COMPLETED, m_durationActivated, m_interaction);
                }                
            }
        }

        private void OnBasicSystemEvent(string _nameEvent, object[] _list)
        {
            if (_nameEvent == EVENT_INTERACTION_STATUS_DURATION_COMPLETED)
            {
                if (m_interaction == (S_Interaction)_list[0])
                {
                    m_durationCompleted = true;
                    BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
                    m_interaction.ResetConditionsValidity();
                }
            }
        }
    }
}
