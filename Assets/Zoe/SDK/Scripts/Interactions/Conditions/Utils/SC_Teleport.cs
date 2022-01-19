using System;
using System.Collections.Generic;
using Gaze;
using UnityEngine;

namespace SpatialStories
{
    public class SC_Teleport : S_AbstractCondition
    {
        public List<S_InteractiveObject> TargetObjects = new List<S_InteractiveObject>();

        public SC_Teleport()
        {
        }

        public override void Setup()
        {
            S_EventManager.OnTeleportEvent += OnTeleportEvent;
        }

        public override void Dispose()
        {
            S_EventManager.OnTeleportEvent -= OnTeleportEvent;
        }

        private void OnTeleportEvent(S_TeleportEventArgs _event)
        {
            if (_event.Mode == S_TeleportMode.TELEPORT)
            {
                S_InteractiveObject objectTeleported = S_Utils.GetIOFromGameObject(_event.TargetObject);
                if (S_Utils.CheckInteractiveObjectInList(objectTeleported, TargetObjects))
                {
                    Validate();
                }
            }
        }

        public override void Reload()
        {
            Invalidate();
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            TargetObjects.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[0].ToString()));
        }

    }

    /// <summary>
    /// Wrapper for the API in order to create a collision condition
    /// </summary>
    public static partial class APIExtensions
    {
        public static SC_Teleport CreateTeleportCondition(this S_InteractionDefinition _def, string _objectToCollideAtGUID)
        {
            return _def.CreateCondition<SC_Teleport>(_objectToCollideAtGUID);
        }
    }
}