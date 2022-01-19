using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public class SA_DestroyBehaviour : S_AbstractAction
    {
        public List<Transform> TargetsToDestroy = new List<Transform>();
        public float TransitionTime;

        private List<S_DestroyBehaviour> m_DestroyBehaviours;

        public SA_DestroyBehaviour(){}

        private void Start()
        {
            if (m_DestroyBehaviours == null)
            {
                m_DestroyBehaviours = new List<S_DestroyBehaviour>();
                List<GameObject> objectsToDestroy = new List<GameObject>();
                for (int i = 0; i < TargetsToDestroy.Count; i++)
                {
                    if (TargetsToDestroy[i] != null)
                    {
                        objectsToDestroy.Add(TargetsToDestroy[i].gameObject);
                    }
                }
                if (objectsToDestroy.Count > 0)
                {
                    m_DestroyBehaviours.Add(new S_DestroyBehaviour(objectsToDestroy, TransitionTime));
                }
            }                
        }

        protected override void ActionLogic()
        {
            if (m_DestroyBehaviours != null)
            {
                foreach (S_DestroyBehaviour item in m_DestroyBehaviours)
                {
                    item.Start();
                }
            }
        }

        public void OnDestroy()
        {
            foreach (S_DestroyBehaviour item in m_DestroyBehaviours)
            {
                item.Kill();
            }
            m_DestroyBehaviours.Clear();
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            TransitionTime = (float)creationData[0];
            GameObject toDestroyObject = SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[1].ToString()).gameObject;
            List<GameObject> objectsToDestroy = new List<GameObject>();
            objectsToDestroy.Add(toDestroyObject);
            m_DestroyBehaviours = new List<S_DestroyBehaviour>();
            m_DestroyBehaviours.Add(new S_DestroyBehaviour(objectsToDestroy, TransitionTime));
        }
    }

    public static partial class APIExtensions
    {
        public static SA_DestroyBehaviour CreateDestroyBehaviour(this S_InteractionDefinition _def, float _time)
        {
            return _def.CreateAction<SA_DestroyBehaviour>(_time);
        }
    }
}
