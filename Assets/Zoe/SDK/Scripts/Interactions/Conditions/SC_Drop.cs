using System;
using System.Collections.Generic;
using Gaze;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SC_Drop")]
    public class SC_Drop : SC_Collision
    {
        public S_InteractiveObject SourceObject;
        public List<GameObject> TargetsObjects = new List<GameObject>();

        public S_DragAndDropStates DropEvent;
        public int IndexDropEvent = -1;

        public SC_Drop()
        {
        }

        public override void Setup()
        {
            S_EventManager.OnDragAndDropEvent += OnDragAndDropEvent;
        }

        public override void Dispose()
        {
            S_EventManager.OnDragAndDropEvent -= OnDragAndDropEvent;
        }

        public override void Reload()
        {
        }

        private void OnDragAndDropEvent(S_DragAndDropEventArgs _event)
        {
            S_InteractiveObject objectRoot = S_Utils.GetIOFromGameObject(this.gameObject);
            S_InteractiveObject objectTarget = S_Utils.GetIOFromObject(_event.DropTarget);
            S_InteractiveObject objectDropped = S_Utils.GetIOFromObject(_event.DropObject);
            int indexCollidingObject = IsCollidingObjectsInList(objectTarget, objectDropped);
            if (indexCollidingObject >= 0)
            {
                if (_event.State == DropEvent)
                {
                    Validate();
                }
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            base.SetupUsingApi(_interaction);
            DropEvent = (S_DragAndDropStates)creationData[4];
        }
    }

    /// <summary>
    /// Wrapper for the API in order to create a gaze condition
    /// </summary>
    public static partial class APIExtensions
    {
        public static SC_Drop CreateHoverCondition(this S_InteractionDefinition _def, S_ProximityStates _state, bool _requireAll, bool _addRig, S_DragAndDropStates _dropEvent, params string[] _gameObjectsIDs)
        {
            int minimunNumber = _addRig ? 1 : 2;
            if (_gameObjectsIDs.Length < minimunNumber)
            {
                Debug.LogError(String.Format("SpatialStories API> too few gameobjects added to proximity condition on interaction {0}.", _def.Name));
            }
            return _def.CreateCondition<SC_Drop>(_state, _requireAll, _addRig, _gameObjectsIDs, _dropEvent);
        }
    }
}
