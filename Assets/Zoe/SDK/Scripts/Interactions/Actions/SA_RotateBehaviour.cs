using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SA_RotateBehaviour")]
    public class SA_RotateBehaviour : S_AbstractAction
    {
        public List<S_RotateBehaviour> RotateBehaviour = new List<S_RotateBehaviour>();

        public SA_RotateBehaviour() { }

        protected override void ActionLogic()
        {
            foreach (S_RotateBehaviour rotateBehaviour in RotateBehaviour)
            {
                rotateBehaviour.ResetCurrentRotation();
                rotateBehaviour.Start();
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            float transitionTime = (float)creationData[0];
            bool transitionLoop = (bool)creationData[1];
            List<GameObject> targetObjects = new List<GameObject>();
            List<string> targetsToRotate = (List<string>)creationData[2];
            for (int i = 0; i < targetsToRotate.Count; i++)
            {
                targetObjects.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(targetsToRotate[i]).gameObject);
            }

            Vector3 targetRotationVector = new Vector3();
            if (creationData.Count == 6)
            {
                Vector3 axisRotation = (Vector3)creationData[3];
                float transitionAngle = (float)creationData[4];

                targetRotationVector = axisRotation * transitionAngle;
            }
            else
            {
                targetRotationVector = (Vector3)creationData[3];
            }

            bool backAndForth = (bool)creationData[5];
            RotateBehaviour.Add(new S_RotateBehaviour(targetObjects, transitionTime, targetRotationVector, transitionLoop, backAndForth));
        }
    }

    public static partial class APIExtensions
    {
        public static SA_RotateBehaviour CreateRotateBehaviour(this S_InteractionDefinition _def, float _time, string _toMoveGUID, Vector3 _targetRotation, bool _loop = false)
        {
            List<string> targetsGUIDs = new List<string>
            {
                _toMoveGUID
            };

            return _def.CreateAction<SA_RotateBehaviour>(_time, _loop, targetsGUIDs, _targetRotation);
        }
        public static SA_RotateBehaviour CreateRotateBehaviour(this S_InteractionDefinition _def, float _time, List<string> _toMoveGUIDs, Vector3 _targetRotation, bool _loop = false)
        {
            return _def.CreateAction<SA_RotateBehaviour>(_time, _loop, _toMoveGUIDs, _targetRotation);
        }
        public static SA_RotateBehaviour CreateRotateBehaviour(this S_InteractionDefinition _def, float _time, string _toMoveGUID, Vector3 _targetAxis, bool _loop, float _angle, bool _backAndForth)
        {
            List<string> targetsGUIDs = new List<string>
            {
                _toMoveGUID
            };

            return _def.CreateAction<SA_RotateBehaviour>(_time, _loop, targetsGUIDs, _targetAxis, _angle, _backAndForth);
        }
    }
}