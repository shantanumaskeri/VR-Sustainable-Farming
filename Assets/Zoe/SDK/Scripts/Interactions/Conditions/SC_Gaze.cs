using Gaze;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SC_Gaze")]
    public class SC_Gaze : S_AbstractCondition
    {
        public S_InteractiveObject TargetObject;
        public S_HoverStates GazeHoverState = S_HoverStates.GAZE_IN;
        public int IndexGazeHoverState = -1;
        public bool IsBeingGazed = false;

        public SC_Gaze()
        {
        }

        public override void Dispose()
        {
            S_EventManager.OnGazeEvent -= OnGazeEvent;
        }

        public override void Reload()
        {
            // Invalidate if we use in or out, but not stay (It can remain valid)
            if (GazeHoverState == S_HoverStates.GAZE_IN
                || GazeHoverState == S_HoverStates.GAZE
                || GazeHoverState == S_HoverStates.GAZE_OUT)
            {
                Invalidate();
            }
        }

        public override void Setup()
        {
            S_EventManager.OnGazeEvent += OnGazeEvent;

            // The logic in CheckForAlreadyGazedAtObjects was here but was leading to cases
            // where the condition was validated in Setup which got the whole Interaction stuck
            // We now wait for the next frame in order to decouple the Setup and the validation
            S_Scheduler.AddUniqueTaskAtNextFrame(CheckForAlreadyGazedAtObjects);
        }

        private void OnGazeEvent(S_GazeEventArgs _e)
        {
            if (_e.GazedObject == TargetObject)
            {
                IsBeingGazed = _e.HoverState == S_HoverStates.GAZE_IN || _e.HoverState == S_HoverStates.GAZE;

                if (IsValid) return;

                if (IsDuration && (FocusDuration > 0))
                {
                    switch (_e.HoverState)
                    {
                        case S_HoverStates.GAZE_IN:
                            ActivateDuration();
                            break;

                        case S_HoverStates.GAZE:
                            ActivateDuration();
                            break;

                        case S_HoverStates.GAZE_OUT:
                            DeactivateDuration();
                            break;
                    }
                }
                else
                {
                    if (_e.HoverState == GazeHoverState)
                    {
                        if (transform.GetComponentInParent<S_InteractiveObject>() != null)
                        {
                            // Debug.LogError("Gaze In Event::TARGET["+ TargetObject + "][" + transform.GetComponentInParent<S_InteractiveObject>().name + "]::VALIDATION CONDITION++++++++++++++++++++");
                        }

                        Validate();
                    }
                    else
                    {
                        Invalidate();
                    }
                }
            }
        }

        private void CheckForAlreadyGazedAtObjects()
        {
            //If the camera was already gazing the object, we need to check in the previous gazed object
            if (S_InputManager.Instance.GetComponentInChildren<S_CameraRaycaster>().IsInteractiveObjectBeingGazed(TargetObject))
            {
                OnGazeEvent(new S_GazeEventArgs() { GazedObject = TargetObject, HoverState = S_HoverStates.GAZE_IN, Sender = S_InputManager.Instance.GetComponentInChildren<S_CameraRaycaster>() });
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            TargetObject = SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[0].ToString());
            GazeHoverState = (S_HoverStates)creationData[1];
        }
    }

    /// <summary>
    /// Wrapper for the API in order to create a gaze condition
    /// </summary>
    public static partial class APIExtensions
    {
        public static SC_Gaze CreateGazeCondition(this S_InteractionDefinition _def, string _objectToGazeAtGUID, S_HoverStates _hoverState)
        {
            return _def.CreateCondition<SC_Gaze>(_objectToGazeAtGUID, _hoverState);
        }
    }
}