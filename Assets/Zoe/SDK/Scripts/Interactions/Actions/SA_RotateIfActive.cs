using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public class SA_RotateIfActive : S_AbstractAction
    {
        public Vector3 IncrementRotation = new Vector3();
        public List<Transform> Target = new List<Transform>();

        public SA_RotateIfActive(){}

        private void Start()
        {
        }

        protected override void ActionLogic()
        {
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            Target.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[0].ToString()).transform);
            IncrementRotation = (Vector3)creationData[1];
        }

        void Update()
        {
            if (GetComponent<S_Interaction>().InteractionStatus.AreConditionsValid)
            {
                foreach (Transform item in Target)
                {
                    item.Rotate(IncrementRotation * Time.deltaTime);
                }                
            }
        }
    }

    public static partial class APIExtensions
    {
        public static SA_RotateIfActive CreateRotateIfActiveBehaviour(this S_InteractionDefinition _def, string _toMoveGUID, Vector3 _incrementRotation)
        {
            return _def.CreateAction<SA_RotateIfActive>(_toMoveGUID, _incrementRotation);
        }
    }
}
