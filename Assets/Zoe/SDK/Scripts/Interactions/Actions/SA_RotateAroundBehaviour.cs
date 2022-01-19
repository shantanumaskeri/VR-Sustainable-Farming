using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SA_RotateAround")]
    public class SA_RotateAroundBehaviour : S_AbstractAction
    {
        public List<S_RotateAroundBehaviour> RotateBehaviour = new List<S_RotateAroundBehaviour>();

        public SA_RotateAroundBehaviour() { }

        protected override void ActionLogic()
        {
            foreach (S_RotateAroundBehaviour rotateBehaviour in RotateBehaviour)
            {
                rotateBehaviour.ResetCurrentRotation();
                rotateBehaviour.Start();
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            float transitionTime = (float)creationData[0];
            GameObject targetGameObject = SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[1].ToString()).gameObject;
            GameObject pivotGameObject = SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[2].ToString()).gameObject;
            Vector3 targetRotationVector = (Vector3)creationData[3];
            List<GameObject> objectsToRotateAround = new List<GameObject>();
            objectsToRotateAround.Add(targetGameObject);

            bool loop = (bool)creationData[4];

            RotateBehaviour.Add(new S_RotateAroundBehaviour(objectsToRotateAround, pivotGameObject, transitionTime, targetRotationVector, loop));
        }
    }

    public static partial class APIExtensions
    {
        public static SA_RotateAroundBehaviour CreateRotateAround(this S_InteractionDefinition _def, float _time, string _targetMoveGUID, string _pivotAroundGUID, Vector3 _targetRotation, bool _loop)
        {
            return _def.CreateAction<SA_RotateAroundBehaviour>(_time, _targetMoveGUID, _pivotAroundGUID, _targetRotation, _loop);
        }
    }
}