using System;
using Gaze;
using UnityEngine;

namespace SpatialStories
{
    public class SC_ActionReleased : SC_ActionButton
    {
        public bool LeftHand = true;
        public bool RightHand = true;

        public SC_ActionReleased()
        {
            S_InputsMapEntry lefthandOculus = InputsMap.Add();
            lefthandOculus.UISelectedPlatform = S_Controllers.OCULUS_TOUCH;
            lefthandOculus.UIControllerSpecificInput = (int)S_OculusInputTypes.LeftJoystickIndexTriggerRelease;
            lefthandOculus.InputType = S_InputTypes.LEFT_TRIGGER_BUTTON_RELEASE;

            S_InputsMapEntry righthandOculus = InputsMap.Add();
            righthandOculus.UISelectedPlatform = S_Controllers.OCULUS_TOUCH;
            righthandOculus.UIControllerSpecificInput = (int)S_OculusInputTypes.RightJoystickIndexTriggerRelease;
            righthandOculus.InputType = S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE;
        }

        public override void Setup()
        {
            if (!m_listenersSetUp && !m_hasBeenDestroyed)
            {
                m_listenersSetUp = true;
                string[] platformsNames = Enum.GetNames(typeof(S_Controllers));
                string[] oculusInputNames = Enum.GetNames(typeof(S_OculusInputTypes));

                if (RightHand) S_InputManager.Instance.RightTriggerButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);
                if (LeftHand) S_InputManager.Instance.LeftTriggerButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);
            }
        }

        public override void Dispose()
        {
            if (m_listenersSetUp && !m_hasBeenDestroyed)
            {
                m_listenersSetUp = false;
                if (RightHand) S_InputManager.Instance.RightTriggerButtonReleaseEvent -= OnInputUpEvent;
                if (LeftHand) S_InputManager.Instance.LeftTriggerButtonReleaseEvent -= OnInputUpEvent;
            }
        }

        public override void OnDestroy()
        {
            m_listenersSetUp = true;
            Dispose();
            m_hasBeenDestroyed = true;
        }

        public override void Reload()
        {
            Invalidate();
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
        }

    }

    /// <summary>
    /// Wrapper for the API in order to create a collision condition
    /// </summary>
    public static partial class APIExtensions
    {
        public static SC_ActionReleased CreateActionReleasedCondition(this S_InteractionDefinition _def, string _objectToCollideAtGUID)
        {
            return _def.CreateCondition<SC_ActionReleased>(_objectToCollideAtGUID);
        }
    }
}