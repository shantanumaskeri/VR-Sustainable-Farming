using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SA_ActivateObjects")]
    public class SA_ActivateGameObjects : S_AbstractAction
    {
        public List<Transform> TargetsToActivate = new List<Transform>();
        public List<Transform> TargetsToDeactivate = new List<Transform>();

        private List<S_VisibilityBehaviour> m_VisibilityBehaviours;

        public SA_ActivateGameObjects(){}

        private void Start()
        {
            if (m_VisibilityBehaviours == null)
            {
                m_VisibilityBehaviours = new List<S_VisibilityBehaviour>();
                List<GameObject> objectsToActivate = new List<GameObject>();
                List<GameObject> objectsToDeactivate = new List<GameObject>();
                for (int i = 0; i < TargetsToActivate.Count; i++)
                {
                    if (TargetsToActivate[i] != null)
                    {
                        objectsToActivate.Add(TargetsToActivate[i].gameObject);
                    }                    
                }
                if (objectsToActivate.Count > 0)
                {
                    m_VisibilityBehaviours.Add(new S_VisibilityBehaviour(objectsToActivate, 0, true));
                }
                for (int i = 0; i < TargetsToDeactivate.Count; i++)
                {
                    if (TargetsToDeactivate[i] != null)
                    {
                        objectsToDeactivate.Add(TargetsToDeactivate[i].gameObject);
                    }
                }
                if (objectsToDeactivate.Count > 0)
                {
                    m_VisibilityBehaviours.Add(new S_VisibilityBehaviour(objectsToDeactivate, 0, false));
                }
            }
        }

        public void OnDestroy()
        {
            foreach (S_VisibilityBehaviour item in m_VisibilityBehaviours)
            {
                item.Kill();
            }
            m_VisibilityBehaviours.Clear();
        }

        protected override void ActionLogic()
        {
            if (m_VisibilityBehaviours != null)
            {
                //Debug.Log("=========ACTIVATION OF VISIBILITY BEHAVIOURS::IO[" + this.gameObject.transform.root.name + "]==========");
                foreach (S_VisibilityBehaviour item in m_VisibilityBehaviours)
                {
                    item.Start();
                }
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            float delayTime = (float)creationData[0];
            GameObject toChangeVisibility = SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[1].ToString()).gameObject;
            bool visibility = (bool)creationData[2];
            m_VisibilityBehaviours = new List<S_VisibilityBehaviour>();
            List<GameObject> objectsToChange = new List<GameObject>();
            objectsToChange.Add(toChangeVisibility);
            m_VisibilityBehaviours.Add(new S_VisibilityBehaviour(objectsToChange, 0, visibility));
        }
    }

    public static partial class APIExtensions
    {
        public static SA_ActivateGameObjects CreateVisibilityBehaviour(this S_InteractionDefinition _def, float _time, string _toMoveGUID, bool _visibility)
        {
            return _def.CreateAction<SA_ActivateGameObjects>(_time, _toMoveGUID, _visibility);
        }
    }
}