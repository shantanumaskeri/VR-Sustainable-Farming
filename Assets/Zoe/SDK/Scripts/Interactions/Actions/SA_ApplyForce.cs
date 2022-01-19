using UnityEngine;

namespace SpatialStories
{
    public class SA_ApplyForce : S_AbstractAction
    {
        public Rigidbody Target;
        public GameObject Destination;
        public float Force = 1;
        public bool HandImpulse = false;

        private bool m_forceApplied = false;
        private bool m_actionExecuted = false;

        private void Start()
        {
            BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);
        }

        private void OnBasicSystemEvent(string _nameEvent, object[] _list)
        {
            if (!m_actionExecuted) return;
            if (m_forceApplied) return;

            if (_nameEvent == SC_PointAndClick.EVENT_POINTANDCLICK_BROADCAST_CONFIRMATION)
            {
                GameObject originObject = (GameObject)_list[0];
                Vector3 originPosition = (Vector3)_list[1];
                Vector3 finalPosition = (Vector3)_list[2];

                if (originObject != null)
                {
                    Vector3 normalImpulse = (finalPosition - originPosition).normalized;
                    Target.AddForce(normalImpulse * Force, ForceMode.Impulse);
                    m_forceApplied = true;
                }
            }
            if (_nameEvent == SC_Manipulate.EVENT_MANIPULATE_RELEASE_CONFIRMATION)
            {
                Vector3 normalImpulse = ((Vector3)_list[0]).normalized;
                Target.AddForce(normalImpulse * Force, ForceMode.Impulse);
                m_forceApplied = true;
            }
        }

        protected override void ActionLogic()
        {
            m_actionExecuted = true;
            if (!m_forceApplied)
            {
                if (Target != null)
                {
                    if (!HandImpulse)
                    {
                        Vector3 normalImpulse = (Destination.transform.position - Target.gameObject.transform.position).normalized;
                        Target.AddForce(normalImpulse * Force, ForceMode.Impulse);
                        m_forceApplied = true;
                    }
                }
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
        }


    }
}
