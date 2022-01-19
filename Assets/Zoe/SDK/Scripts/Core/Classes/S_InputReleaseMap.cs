using System.Collections.Generic;

namespace Gaze
{
    public class S_InputReleaseMap
    {
        private static Dictionary<S_InputTypes, List<S_InputTypes>> m_PressToReleaseMap = new Dictionary<S_InputTypes, List<S_InputTypes>>()
        {
            // Regular buttons
            // BUTON                                                   RELEASE BUTTONS
            { S_InputTypes.RIGHT_PRIMARY_BUTTON_PRESS, new List<S_InputTypes>() { S_InputTypes.RIGHT_PRIMARY_BUTTON_RELEASE }},

            { S_InputTypes.RIGHT_SECONDARY_BUTTON_PRESS, new List<S_InputTypes>() { S_InputTypes.RIGHT_SECONDARY_BUTTON_RELEASE }},

            { S_InputTypes.LEFT_PRIMARY_BUTTON_PRESS, new List<S_InputTypes>() { S_InputTypes.LEFT_PRIMARY_BUTTON_RELEASE }},

            { S_InputTypes.LEFT_SECONDARY_BUTTON_PRESS, new List<S_InputTypes>() { S_InputTypes.LEFT_SECONDARY_BUTTON_RELEASE }},

            { S_InputTypes.LEFT_TRIGGER_BUTTON_PRESS, new List<S_InputTypes>() { S_InputTypes.LEFT_TRIGGER_BUTTON_RELEASE }},

            { S_InputTypes.RIGHT_TRIGGER_BUTTON_PRESS, new List<S_InputTypes>() { S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE }},

            { S_InputTypes.LEFT_GRIP_BUTTON_PRESS, new List<S_InputTypes>() { S_InputTypes.LEFT_GRIP_BUTTON_RELEASE }},

            { S_InputTypes.RIGHT_GRIP_BUTTON_PRESS, new List<S_InputTypes>() { S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE }},

            { S_InputTypes.LEFT_PRIMARY_2D_AXIS, new List<S_InputTypes>() { S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_RELEASE }},
            { S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_PRESS, new List<S_InputTypes>() { S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_RELEASE }},

            { S_InputTypes.RIGHT_PRIMARY_2D_AXIS, new List<S_InputTypes>() { S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_RELEASE }},
            { S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_PRESS, new List<S_InputTypes>() { S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_RELEASE }},
            
            // Touch
            { S_InputTypes.RIGHT_PRIMARY_BUTTON_TOUCH, new List<S_InputTypes>() { S_InputTypes.RIGHT_PRIMARY_BUTTON_UNTOUCH} },
            { S_InputTypes.RIGHT_SECONDARY_BUTTON_TOUCH, new List<S_InputTypes>() { S_InputTypes.RIGHT_SECONDARY_BUTTON_UNTOUCH} },
            { S_InputTypes.LEFT_PRIMARY_BUTTON_TOUCH, new List<S_InputTypes>() { S_InputTypes.LEFT_PRIMARY_BUTTON_UNTOUCH} },
            { S_InputTypes.LEFT_SECONDARY_BUTTON_TOUCH, new List<S_InputTypes>() { S_InputTypes.LEFT_SECONDARY_BUTTON_UNTOUCH} },
            { S_InputTypes.LEFT_PRIMARY_2D_AXIS_TOUCH, new List<S_InputTypes>() { S_InputTypes.LEFT_PRIMARY_2D_AXIS_UNTOUCH} },
            { S_InputTypes.RIGHT_PRIMARY_2D_AXIS_TOUCH, new List<S_InputTypes>() { S_InputTypes.RIGHT_PRIMARY_2D_AXIS_UNTOUCH} },

        };

        /// <summary>
        /// Checks if the testing input is a "Release" of the base input
        /// </summary>
        /// <param name="_testingInput">The input type that can potentially be a release input of _baseInput</param>
        /// <param name="_baseInput"> The input the can be a potential press event of the _testingInput </param>
        /// <returns></returns>
        public static bool IsReleaseInputtOf(S_InputTypes _testingInput, S_InputTypes _baseInput)
        {
            // If the input doesn't have any release input (probably because is a release input) return false
            if (!m_PressToReleaseMap.ContainsKey(_baseInput))
                return false;

            // If the base input has any release input test if the event is one of them
            return m_PressToReleaseMap[_baseInput].Contains(_testingInput);
        }
    }
}
