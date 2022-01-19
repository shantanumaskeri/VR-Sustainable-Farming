// <copyright file="Gaze_EventManager.cs" company="apelab sàrl">
// © apelab. All Rights Reserved.
//
// This source is subject to the apelab license.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2014-06-01</date>
using SpatialStories;

namespace Gaze
{
    public static class S_EventManager
    {
        /// <summary>
        /// Fired whenever an object is being gazed and ungazed.
        /// </summary>
        public delegate void GazeHandler(S_GazeEventArgs e);
        public static event GazeHandler OnGazeEvent;
        public static void FireGazeEvent(S_GazeEventArgs e)
        {
            if (OnGazeEvent != null)
                OnGazeEvent(e);
        }


        /// <summary>
        /// Fired on Trigger's audio related event occurs.
        /// </summary>
        public delegate void AudioPlayerHandler(S_AudioPlayerEventArgs e);
        public static event AudioPlayerHandler OnAudioPlayerEvent;
        public static void FireAudioPlayerEvent(S_AudioPlayerEventArgs e)
        {
            if (OnAudioPlayerEvent != null)
                OnAudioPlayerEvent(e);
        }

        /// <summary>
        /// Fired when two Triggers collide.
        /// </summary>
        public delegate void CollisionHandler(S_CollisionEventArgs e);
        public static event CollisionHandler OnCollisionEvent;
        public static void FireCollisionEvent(S_CollisionEventArgs e)
        {
            if (OnCollisionEvent != null)
                OnCollisionEvent(e);
        }

        /// <summary>
        /// Fired when the camera enters the object's proximity zone.
        /// </summary>
        public delegate void ProximityHandler(S_ProximityEventArgs e);
        public static event ProximityHandler OnProximityEvent;
        public static void FireProximityEvent(S_ProximityEventArgs e)
        {
            if (OnProximityEvent != null)
            {
                OnProximityEvent(e);
            }

        }

        /// <summary>
        /// Fired when an object with zoom enabled is gazed.
        /// </summary>
        public delegate void ZoomHandler(S_ZoomEventArgs e);
        public static event ZoomHandler OnZoomEvent;
        public static void FireZoomEvent(S_ZoomEventArgs e)
        {
            if (OnZoomEvent != null)
                OnZoomEvent(e);
        }

        /// <summary>
        /// Fired when the 'Handle' GameObject is collided
        /// </summary>
        public delegate void HandleHandler(S_HandleEventArgs e);
        public static event HandleHandler OnHandleEvent;
        public static void FireHandleEvent(S_HandleEventArgs e)
        {
            if (OnHandleEvent != null)
                OnHandleEvent(e);
        }

        /// <summary>
        /// Fired when the custom condition notifies the Gaze_Conditions
        /// </summary>
        public delegate void CustomConditionHandler(S_CustomConditionEventArgs e);
        public static event CustomConditionHandler OnCustomConditionEvent;
        public static void FireCustomConditionEvent(S_CustomConditionEventArgs e)
        {
            if (OnCustomConditionEvent != null)
                OnCustomConditionEvent(e);
        }

        /// <summary>
        /// Fired when all the dependencies of a trigger have been validated
        /// </summary>
        public delegate void DependenciesValidatedHandler(S_DependenciesValidatedEventArgs e);
        public static event DependenciesValidatedHandler OnDependenciesValidated;
        public static void FireOnDependenciesValidated(S_DependenciesValidatedEventArgs e)
        {
            if (OnDependenciesValidated != null)
                OnDependenciesValidated(e);
        }

        /// <summary>
        /// Fired when the custom condition notifies the Gaze_Conditions
        /// </summary>
        public delegate void GrabHandler(S_GrabEventArgs e);
        public static event GrabHandler OnGrabEvent;
        public static void FireGrabEvent(S_GrabEventArgs e)
        {
            if (OnGrabEvent != null)
                OnGrabEvent(e);
        }

        /// <summary>
        /// Fired when the player is moving a drag and drop object.
        /// </summary>
        public delegate void DragAndDropEvent(S_DragAndDropEventArgs e);
        public static event DragAndDropEvent OnDragAndDropEvent;
        public static void FireDragAndDropEvent(S_DragAndDropEventArgs e)
        {
            S_DragAndDropManager.UpdateDropTargetsStates(e);
            if (OnDragAndDropEvent != null)
                OnDragAndDropEvent(e);
        }

        /// <summary>
        /// Fired when the player tries to levitate an object.
        /// </summary>
        public delegate void LevitationEvent(S_LevitationEventArgs e);
        public static LevitationEvent OnLevitationEvent;
        public static void FireLevitationEvent(S_LevitationEventArgs e)
        {
            if (OnLevitationEvent != null)
                OnLevitationEvent(e);
        }

        /// Fired when a controller points toward an object
        /// </summary>
        public delegate void ControllerPointingHandler(S_ControllerPointingEventArgs e);
        public static event ControllerPointingHandler OnControllerPointingEvent;
        public static void FireControllerPointingEvent(S_ControllerPointingEventArgs e)
        {
            if (OnControllerPointingEvent != null)
                OnControllerPointingEvent(e);
        }

        /// Fired when the controller is triggering a laser raycast
        /// </summary>
        public delegate void LaserHandler(S_LaserEventArgs e);
        public static event LaserHandler OnLaserEvent;
        public static void FireLaserEvent(S_LaserEventArgs e)
        {
            if (OnLaserEvent != null)
                OnLaserEvent(e);
        }

        /// Fired when the controller is teleporting
        /// </summary>
        public delegate void TeleportHandler(S_TeleportEventArgs e);
        public static event TeleportHandler OnTeleportEvent;
        public static void FireTeleportEvent(S_TeleportEventArgs e)
        {
            if (OnTeleportEvent != null)
                OnTeleportEvent(e);
        }

        /// Fired when the controller is teleporting
        /// </summary>
        public delegate void IODestroyHandler(S_IODestroyEventArgs e);
        public static event IODestroyHandler OnIODestroyed;
        public static void FireOnIODestroyed(S_IODestroyEventArgs e)
        {
            if (OnIODestroyed != null)
                OnIODestroyed(e);
        }

        // These couple of events are meant to be used on Mobile for when a S_InteractiveObject is touched
        // and untouched through its Proximity collider
        public delegate void InteractiveObjectTouchHandler(S_InteractiveObject _interactiveOject);
        public static event InteractiveObjectTouchHandler InteractiveObjectTouched;
        public static void FireInteractiveObjectTouched(S_InteractiveObject _touchedInteractiveObject)
        {
            InteractiveObjectTouched?.Invoke(_touchedInteractiveObject);
        }

        public static event InteractiveObjectTouchHandler InteractiveObjectUntouched;
        public static void FireInteractiveObjectUntouched(S_InteractiveObject _touchedInteractiveObject)
        {
            InteractiveObjectUntouched?.Invoke(_touchedInteractiveObject);
        }
    }
}