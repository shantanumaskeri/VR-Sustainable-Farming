using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SA_ScaleBehaviour")]
    public class SA_ScaleBehaviour : S_AbstractAction
    {
        public List<S_ScalingBehaviour> ScaleBehaviour = new List<S_ScalingBehaviour>();

        public SA_ScaleBehaviour(){}

        protected override void ActionLogic()
        {
            foreach (S_ScalingBehaviour scaleBehaviour in ScaleBehaviour)
            {
                scaleBehaviour.ResetCurrentScale();
                scaleBehaviour.Start();
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            float transitionTime = (float)creationData[0];
            GameObject targetGameObject = SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[1].ToString()).gameObject;
            List<GameObject> objectsToScale = new List<GameObject>();
            objectsToScale.Add(targetGameObject);
            S_ScaleMode scaleMode = (S_ScaleMode)creationData[3];

            switch (scaleMode)
            {
                case S_ScaleMode.SCALE_ABSOLUTE:
                    Vector3 targetScaleVector = (Vector3)creationData[2];
                    ScaleBehaviour.Add(new S_ScalingBehaviour(objectsToScale, transitionTime, targetScaleVector, scaleMode));
                    break;
                case S_ScaleMode.SCALE_FACTOR:
                    float scaleFactor = (float)creationData[2];
                    ScaleBehaviour.Add(new S_ScalingBehaviour(objectsToScale, transitionTime, scaleFactor, scaleMode));
                    break;
            }
        }
    }

    public static partial class APIExtensions
    {
        public static SA_ScaleBehaviour CreateScaleBehaviour(this S_InteractionDefinition _def, float _time, string _toMoveGUID, Vector3 _targetScale)
        {
            return _def.CreateAction<SA_ScaleBehaviour>(_time, _toMoveGUID, _targetScale, S_ScaleMode.SCALE_ABSOLUTE);
        }

        public static SA_ScaleBehaviour CreateScaleBehaviour(this S_InteractionDefinition _def, float _time, string _toMoveGUID, float _scaleFactor)
        {
            return _def.CreateAction<SA_ScaleBehaviour>(_time, _toMoveGUID, _scaleFactor, S_ScaleMode.SCALE_FACTOR);
        }
    }
}