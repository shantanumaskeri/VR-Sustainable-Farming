using System;
using Gaze;
using UnityEngine;

namespace SpatialStories
{
    public class SC_ActionPressed : SC_ActionButton
    {
        public bool LeftHand = true;
        public bool RightHand = true;

        public SC_ActionPressed()
        {
            S_InputsMapEntry lefthandOculus = InputsMap.Add();
            lefthandOculus.UISelectedPlatform = S_Controllers.OCULUS_TOUCH;
            lefthandOculus.UIControllerSpecificInput = (int)S_OculusInputTypes.LeftJoystickIndexTriggerPress;
            lefthandOculus.InputType = S_InputTypes.LEFT_TRIGGER_BUTTON_PRESS;

            S_InputsMapEntry righthandOculus = InputsMap.Add();
            righthandOculus.UISelectedPlatform = S_Controllers.OCULUS_TOUCH;
            righthandOculus.UIControllerSpecificInput = (int)S_OculusInputTypes.RightJoystickIndexTriggerPress;
            righthandOculus.InputType = S_InputTypes.RIGHT_TRIGGER_BUTTON_PRESS;
        }

        public override void Setup()
        {
            if (!m_listenersSetUp && !m_hasBeenDestroyed)
            {
                m_listenersSetUp = true;
                string[] platformsNames = Enum.GetNames(typeof(S_Controllers));
                string[] oculusInputNames = Enum.GetNames(typeof(S_OculusInputTypes));

                if (RightHand) S_InputManager.Instance.RightTriggerButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                if (LeftHand) S_InputManager.Instance.LeftTriggerButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);
            }
        }

        public override void Dispose()
        {
            if (m_listenersSetUp && !m_hasBeenDestroyed)
            {
                m_listenersSetUp = false;
                if (RightHand) S_InputManager.Instance.RightTriggerButtonPressEvent -= OnInputDownEvent;
                if (LeftHand) S_InputManager.Instance.LeftTriggerButtonPressEvent -= OnInputDownEvent;
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
        public static SC_ActionPressed CreateActionPressedCondition(this S_InteractionDefinition _def, string _objectToCollideAtGUID)
        {
            return _def.CreateCondition<SC_ActionPressed>(_objectToCollideAtGUID);
        }
    }
}