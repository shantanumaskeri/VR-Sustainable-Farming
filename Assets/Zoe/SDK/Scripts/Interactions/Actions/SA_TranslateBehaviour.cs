using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SA_TranslateBehaviour")]
    public class SA_TranslateBehaviour : S_AbstractAction
    {
        public List<S_TranslateBehaviour> TranslateBehaviours = new List<S_TranslateBehaviour>();

        public SA_TranslateBehaviour(){}

        protected override void ActionLogic()
        {
            foreach (S_TranslateBehaviour translateBehaviour in TranslateBehaviours)
            {
                translateBehaviour.Start();
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            float transitionTime = (float)creationData[0];
            List<Transform> moveToTransforms = new List<Transform>();
            if (creationData[2] is List<string>)
            {
                List<string> listTargets = (List<string>)creationData[2];
                for (int i = 0; i < listTargets.Count; i++)
                {
                    moveToTransforms.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(listTargets[i]).transform);
                }
            }
            else
            {
                moveToTransforms.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[2].ToString()).transform);
            }
            Transform targetToMoveTransform = SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[1].ToString()).transform;
            float parabolicHeight = (float)creationData[3];
            int flips = (int)creationData[4];
            float offset = (float)creationData[5];
            bool rotate = (bool)creationData[6];
            List<GameObject> objectsToTranslate = new List<GameObject>();
            objectsToTranslate.Add(targetToMoveTransform.gameObject);
            TranslateBehaviours.Add(new S_TranslateBehaviour(objectsToTranslate, transitionTime, moveToTransforms, parabolicHeight, rotate, flips, offset));
        }
    }

    public static partial class APIExtensions
    {
        public static SA_TranslateBehaviour CreateTranslateBehaviour(this S_InteractionDefinition _def, float _time, string _toMoveGUID, string _targetGUID, float _parabolicHeight = 0, int _flips = 0, float _offset = 0, bool _interpolateRotation = false)
        {
            return _def.CreateAction<SA_TranslateBehaviour>(_time, _toMoveGUID,  _targetGUID, _parabolicHeight, _flips, _offset, _interpolateRotation);
        }
        public static SA_TranslateBehaviour CreateTranslateBehaviour(this S_InteractionDefinition _def, float _time, string _toMoveGUID, List<string> _targetGUIDs, float _parabolicHeight = 0, int _flips = 0, float _offset = 0, bool _interpolateRotation = false)
        {
            return _def.CreateAction<SA_TranslateBehaviour>(_time, _toMoveGUID, _targetGUIDs, _parabolicHeight, _flips, _offset, _interpolateRotation);
        }
    }
}