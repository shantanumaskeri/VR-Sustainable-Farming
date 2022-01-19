using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SA_ChangeMaterial")]
    public class SA_ChangeColorBehaviour : S_AbstractAction
    {
        public List<S_ChangeColorBehaviour> ChangeColorhaviour = new List<S_ChangeColorBehaviour>();

        public SA_ChangeColorBehaviour() { }

        protected override void ActionLogic()
        {
            foreach (S_ChangeColorBehaviour changeColorBehaviour in ChangeColorhaviour)
            {
                changeColorBehaviour.ResetCurrentColor();
                changeColorBehaviour.Start();
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            List<GameObject> targetObjects = new List<GameObject>();
            if (creationData[0] is List<string>)
            {
                List<string> targetsToRotate = (List<string>)creationData[0];
                for (int i = 0; i < targetsToRotate.Count; i++)
                {
                    targetObjects.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(targetsToRotate[i]).transform.gameObject);
                }
            }
            else
            {
                targetObjects.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[2].ToString()).transform.gameObject);
            }
            float duration = (float)creationData[1];
            Color targetColor = (Color)creationData[2];
            bool loop = (bool)creationData[3];
            bool backAndForth = (bool)creationData[4];
            ChangeColorhaviour.Add(new S_ChangeColorBehaviour(targetObjects, targetColor, duration, loop, backAndForth));
        }
    }

    public static partial class APIExtensions
    {
        public static SA_ChangeColorBehaviour CreateChangeColorBehaviour(this S_InteractionDefinition _def, string _toColorGUID, Color _color, float _duration = 0, bool _loop = false, bool _forth = false)
        {
            return _def.CreateAction<SA_ChangeColorBehaviour>(_toColorGUID, _duration, _color, _loop, _forth);
        }
        public static SA_ChangeColorBehaviour CreateChangeColorBehaviour(this S_InteractionDefinition _def, float _time, List<string> _toColorGUID, Color _color, float _duration = 0, bool _loop = false, bool _forth = false)
        {
            return _def.CreateAction<SA_ChangeColorBehaviour>(_toColorGUID, _duration, _color, _loop, _forth);
        }
    }
}