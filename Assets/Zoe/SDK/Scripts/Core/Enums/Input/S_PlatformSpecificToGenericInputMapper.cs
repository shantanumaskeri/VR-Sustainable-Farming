using SpatialStories;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public class S_PlatformSpecificToGenericInputMapper
    {
        private static Dictionary<S_OculusInputTypes, S_InputTypes> m_OculusToGeneric = new Dictionary<S_OculusInputTypes, S_InputTypes>()
        {
            { S_OculusInputTypes.LeftJoystickPress, S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_PRESS },
            { S_OculusInputTypes.LeftJoystickRelease, S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_RELEASE },
            { S_OculusInputTypes.LeftJoystickTouch, S_InputTypes.LEFT_PRIMARY_2D_AXIS_TOUCH },
            { S_OculusInputTypes.LeftJoystickHandTriggerPress, S_InputTypes.LEFT_GRIP_BUTTON_PRESS },
            { S_OculusInputTypes.LeftJoystickHandTriggerRelease, S_InputTypes.LEFT_GRIP_BUTTON_RELEASE },
            { S_OculusInputTypes.LeftJoystickIndexTriggerPress, S_InputTypes.LEFT_TRIGGER_BUTTON_PRESS },
            { S_OculusInputTypes.LeftJoystickIndexTriggerRelease, S_InputTypes.LEFT_TRIGGER_BUTTON_RELEASE },

            { S_OculusInputTypes.RightJoystickPress, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_PRESS },
            { S_OculusInputTypes.RightJoystickRelease, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_RELEASE },
            { S_OculusInputTypes.RightJoystickTouch, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_TOUCH },
            { S_OculusInputTypes.RightJoystickHandTriggerPress, S_InputTypes.RIGHT_GRIP_BUTTON_PRESS },
            { S_OculusInputTypes.RightJoystickHandTriggerRelease, S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE },
            { S_OculusInputTypes.RightJoystickIndexTriggerPress, S_InputTypes.RIGHT_TRIGGER_BUTTON_PRESS },
            { S_OculusInputTypes.RightJoystickIndexTriggerRelease, S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE },

            { S_OculusInputTypes.AButtonPress, S_InputTypes.RIGHT_PRIMARY_BUTTON_PRESS },
            { S_OculusInputTypes.AButtonRelease, S_InputTypes.RIGHT_PRIMARY_BUTTON_RELEASE },
            { S_OculusInputTypes.BButtonPress, S_InputTypes.RIGHT_SECONDARY_BUTTON_PRESS },
            { S_OculusInputTypes.BButtonRelease, S_InputTypes.RIGHT_SECONDARY_BUTTON_RELEASE },
            { S_OculusInputTypes.XButtonPress, S_InputTypes.LEFT_PRIMARY_BUTTON_PRESS },
            { S_OculusInputTypes.XButtonRelease, S_InputTypes.LEFT_PRIMARY_BUTTON_RELEASE },
            { S_OculusInputTypes.YButtonPress, S_InputTypes.LEFT_SECONDARY_BUTTON_PRESS },
            { S_OculusInputTypes.YButtonRelease, S_InputTypes.LEFT_SECONDARY_BUTTON_RELEASE },

            { S_OculusInputTypes.StartButtonPress, S_InputTypes.MENU_BUTTON },

            { S_OculusInputTypes.AButtonTouch, S_InputTypes.RIGHT_PRIMARY_BUTTON_TOUCH },
            { S_OculusInputTypes.BButtonTouch, S_InputTypes.RIGHT_SECONDARY_BUTTON_TOUCH },
            { S_OculusInputTypes.XButtonTouch, S_InputTypes.LEFT_PRIMARY_BUTTON_TOUCH },
            { S_OculusInputTypes.YButtonTouch, S_InputTypes.LEFT_SECONDARY_BUTTON_TOUCH },

            { S_OculusInputTypes.AButtonUntouch, S_InputTypes.RIGHT_PRIMARY_BUTTON_UNTOUCH },
            { S_OculusInputTypes.BButtonUntouch, S_InputTypes.RIGHT_SECONDARY_BUTTON_UNTOUCH },
            { S_OculusInputTypes.XButtonUntouch, S_InputTypes.LEFT_PRIMARY_BUTTON_UNTOUCH },
            { S_OculusInputTypes.YButtonUntouch, S_InputTypes.LEFT_SECONDARY_BUTTON_UNTOUCH },

            { S_OculusInputTypes.RightJoystickUntouch, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_UNTOUCH },

            { S_OculusInputTypes.LeftJoystickUntouch, S_InputTypes.LEFT_PRIMARY_2D_AXIS_UNTOUCH },
        };

        public static S_InputTypes ToGenericInput(S_Controllers _platform, int _inputType)
        {
            switch (_platform)
            {
                case S_Controllers.OCULUS_TOUCH:
                    return OculusToGenericInput((S_OculusInputTypes)_inputType);
                default:
                    Debug.LogError("Translation not implemented for this platform: " + _platform);
                    return S_InputTypes.NONE;
            }
        }

        private static S_InputTypes OculusToGenericInput(S_OculusInputTypes _inputType)
        {
            if (m_OculusToGeneric.ContainsKey(_inputType))
            {
                return m_OculusToGeneric[_inputType];
            }
            else
            {
                Debug.LogError("Translation not implemented for this inputType: " + _inputType);
                return S_InputTypes.NONE;
            }
        }
    }
}
