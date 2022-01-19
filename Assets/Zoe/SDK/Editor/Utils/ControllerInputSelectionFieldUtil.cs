using SpatialStories;
using System;
using UnityEditor;

public class ControllerInputSelectionFieldUtil
{
    private static string[] m_ControllerTypeNames = new string[] { "Choose Platform", "Oculus Touch", "Vive Controller", "Window Mixed Reality Controller" };

    private static string[] m_HandNames = new string[] { "Right", "Left" };
    private enum Hand { Right, Left }

    private static string[] m_LeftOculusTouchInputTypeNames = new string[] { "X Button", "Y Button", "Menu Button", "Trigger", "Grip", "Stick" };
    private enum LeftOculusTouchInput { XButton, YButton, MenuButton, Trigger, Grip, Stick };

    private static string[] m_RightOculusTouchInputTypeNames = new string[] { "A Button", "B Button", "Menu Button", "Trigger", "Grip", "Stick" };
    private enum RightOculusTouchInput { AButton, BButton, MenuButton, Trigger, Grip, Stick };

    private static string[] m_ViveControllerInputTypeNames = new string[] { "Menu Button", "Trigger", "Grip", "Trackpad" };
    private enum ViveControllerInput { MenuButton, Trigger, Grip, Trackpad };

    private static string[] m_WindowsMixedRealityInputTypeNames = new string[] { "Menu Button", "Trigger", "Grip", "Stick" };
    private enum WindowsMixedRealityInput { MenuButton, Trigger, Grip, Stick };

    private static string[] m_ButtonStateNames = new string[] { "Pressed", "Released" };

    private static string[] m_AxisProviderStateNames = new string[] { "Pressed", "Released", "Up Tilt", "Up Tilt Release", "Down Tilt", "Down Tilt Release", "Left Tilt", "Left Tilt Release", "Right Tilt", "Right Tilt Release" };
    private enum AxisProviderState { Pressed, Released, Up, UpRelease, Down, DownRelease, Left, LeftRelease, Right, RightRelease };

    public static void RenderControllerInputSelectionField(ref S_Controllers _controller, ref S_SingleHandsEnum _hand, ref S_BaseInputTypes _baseInputType, ref S_DirectionTypes _directionType, ref S_StickDirections _stickDirectionType)
    {
        EditorGUILayout.BeginHorizontal();

        _controller = (S_Controllers)EditorGUILayout.Popup((int)_controller, m_ControllerTypeNames);

        if (_controller == S_Controllers.NOT_DETERMINED)
        {
            EditorGUILayout.EndHorizontal();

            return;
        }

        _hand = HandNameIndexToSingleHandEnum(EditorGUILayout.Popup(SingleHandEnumToHandNameIndex(_hand), m_HandNames));

        switch (_controller)
        {
            case S_Controllers.OCULUS_TOUCH:
                switch (_hand)
                {
                    case S_SingleHandsEnum.LEFT:
                        _baseInputType = LeftOculusTouchInputTypeNameIndexToBaseInputType(EditorGUILayout.Popup(BaseInputTypeToLeftOculusTouchInputTypeNameIndex(_baseInputType), m_LeftOculusTouchInputTypeNames));
                        break;
                    case S_SingleHandsEnum.RIGHT:
                        _baseInputType = RightOculusTouchInputTypeNameIndexToBaseInputType(EditorGUILayout.Popup(BaseInputTypeToRightOculusTouchInputTypeNameIndex(_baseInputType), m_RightOculusTouchInputTypeNames));
                        break;
                }
                break;
            case S_Controllers.VIVE:
                _baseInputType = ViveControllerInputTypeNameInexToBaseInputType(EditorGUILayout.Popup(BaseInputTypeToViveControllerInputTypeNameIndex(_baseInputType), m_ViveControllerInputTypeNames));
                break;
            case S_Controllers.WINDOWS_MIXED_REALITY:
                _baseInputType = WindowsMixedRealityControllerInputTypeNameIndexToBaseInputType(EditorGUILayout.Popup(BaseInputTypeToWindowsMixedRealityInputTypeNameIndex(_baseInputType), m_WindowsMixedRealityInputTypeNames));
                break;
        }

        if (_baseInputType == S_BaseInputTypes.STICK)
        {
            Tuple<S_DirectionTypes, S_StickDirections> stickDirectionAndDirectionType = AxisProviderStateNameIndexToDirectionTypeAndStickDirection(EditorGUILayout.Popup(StickDirectionAndDirectionTypeToAxisProviderStateNameIndex(_stickDirectionType, _directionType), m_AxisProviderStateNames));
            _directionType = stickDirectionAndDirectionType.Item1;
            _stickDirectionType = stickDirectionAndDirectionType.Item2;
        }
        else
        {
            _directionType = (S_DirectionTypes)EditorGUILayout.Popup((int)_directionType, m_ButtonStateNames);
        }

        EditorGUILayout.EndHorizontal();
    }

    private static int SingleHandEnumToHandNameIndex(S_SingleHandsEnum _singleHand)
    {
        switch (_singleHand)
        {
            default:
            case S_SingleHandsEnum.LEFT:
                return (int)Hand.Left;
            case S_SingleHandsEnum.RIGHT:
                return (int)Hand.Right;
        }
    }

    private static S_SingleHandsEnum HandNameIndexToSingleHandEnum(int _handNameIndex)
    {
        switch ((Hand)_handNameIndex)
        {
            default:
            case Hand.Left:
                return S_SingleHandsEnum.LEFT;
            case Hand.Right:
                return S_SingleHandsEnum.RIGHT;
        }
    }

    private static S_BaseInputTypes RightOculusTouchInputTypeNameIndexToBaseInputType(int _rightOculusTouchInputTypeNameIndex)
    {
        switch ((RightOculusTouchInput)_rightOculusTouchInputTypeNameIndex)
        {
            default:
            case RightOculusTouchInput.AButton:
                return S_BaseInputTypes.PRIMARY_BUTTON;
            case RightOculusTouchInput.BButton:
                return S_BaseInputTypes.SECONDARY_BUTTON;
            case RightOculusTouchInput.Grip:
                return S_BaseInputTypes.GRIP;
            case RightOculusTouchInput.MenuButton:
                return S_BaseInputTypes.START_BUTTON;
            case RightOculusTouchInput.Stick:
                return S_BaseInputTypes.STICK;
            case RightOculusTouchInput.Trigger:
                return S_BaseInputTypes.INDEX;
        }
    }

    private static int BaseInputTypeToRightOculusTouchInputTypeNameIndex(S_BaseInputTypes _baseInputType)
    {
        switch (_baseInputType)
        {
            default:
            case S_BaseInputTypes.PRIMARY_BUTTON:
                return (int)RightOculusTouchInput.AButton;
            case S_BaseInputTypes.SECONDARY_BUTTON:
                return (int)RightOculusTouchInput.BButton;
            case S_BaseInputTypes.GRIP:
                return (int)RightOculusTouchInput.Grip;
            case S_BaseInputTypes.START_BUTTON:
                return (int)RightOculusTouchInput.MenuButton;
            case S_BaseInputTypes.STICK:
                return (int)RightOculusTouchInput.Stick;
            case S_BaseInputTypes.INDEX:
                return (int)RightOculusTouchInput.Trigger;
        }
    }

    private static S_BaseInputTypes LeftOculusTouchInputTypeNameIndexToBaseInputType(int _leftOculusTouchInputTypeNameIndex)
    {
        switch ((LeftOculusTouchInput)_leftOculusTouchInputTypeNameIndex)
        {
            default:
            case LeftOculusTouchInput.XButton:
                return S_BaseInputTypes.PRIMARY_BUTTON;
            case LeftOculusTouchInput.YButton:
                return S_BaseInputTypes.SECONDARY_BUTTON;
            case LeftOculusTouchInput.Grip:
                return S_BaseInputTypes.GRIP;
            case LeftOculusTouchInput.MenuButton:
                return S_BaseInputTypes.START_BUTTON;
            case LeftOculusTouchInput.Stick:
                return S_BaseInputTypes.STICK;
            case LeftOculusTouchInput.Trigger:
                return S_BaseInputTypes.INDEX;
        }
    }

    private static int BaseInputTypeToLeftOculusTouchInputTypeNameIndex(S_BaseInputTypes _baseInputType)
    {
        switch (_baseInputType)
        {
            default:
            case S_BaseInputTypes.PRIMARY_BUTTON:
                return (int)LeftOculusTouchInput.XButton;
            case S_BaseInputTypes.SECONDARY_BUTTON:
                return (int)LeftOculusTouchInput.YButton;
            case S_BaseInputTypes.GRIP:
                return (int)LeftOculusTouchInput.Grip;
            case S_BaseInputTypes.START_BUTTON:
                return (int)LeftOculusTouchInput.MenuButton;
            case S_BaseInputTypes.STICK:
                return (int)LeftOculusTouchInput.Stick;
            case S_BaseInputTypes.INDEX:
                return (int)LeftOculusTouchInput.Trigger;
        }
    }

    private static S_BaseInputTypes ViveControllerInputTypeNameInexToBaseInputType(int _viveControllerInputTypeNameIndex)
    {
        switch ((ViveControllerInput)_viveControllerInputTypeNameIndex)
        {
            default:
            case ViveControllerInput.MenuButton:
                return S_BaseInputTypes.PRIMARY_BUTTON;
            case ViveControllerInput.Grip:
                return S_BaseInputTypes.GRIP;
            case ViveControllerInput.Trackpad:
                return S_BaseInputTypes.STICK;
            case ViveControllerInput.Trigger:
                return S_BaseInputTypes.INDEX;
        }
    }

    private static int BaseInputTypeToViveControllerInputTypeNameIndex(S_BaseInputTypes _baseInputType)
    {
        switch (_baseInputType)
        {
            default:
            case S_BaseInputTypes.PRIMARY_BUTTON:
                return (int)ViveControllerInput.MenuButton;
            case S_BaseInputTypes.GRIP:
                return (int)ViveControllerInput.Grip;
            case S_BaseInputTypes.STICK:
                return (int)ViveControllerInput.Trackpad;
            case S_BaseInputTypes.INDEX:
                return (int)ViveControllerInput.Trigger;
        }
    }

    private static S_BaseInputTypes WindowsMixedRealityControllerInputTypeNameIndexToBaseInputType(int _windowsMixedRealityInputTypeNameIndex)
    {
        switch ((WindowsMixedRealityInput)_windowsMixedRealityInputTypeNameIndex)
        {
            default:
            case WindowsMixedRealityInput.MenuButton:
                return S_BaseInputTypes.PRIMARY_BUTTON;
            case WindowsMixedRealityInput.Grip:
                return S_BaseInputTypes.GRIP;
            case WindowsMixedRealityInput.Stick:
                return S_BaseInputTypes.STICK;
            case WindowsMixedRealityInput.Trigger:
                return S_BaseInputTypes.INDEX;
        }
    }

    private static int BaseInputTypeToWindowsMixedRealityInputTypeNameIndex(S_BaseInputTypes _baseInputType)
    {
        switch (_baseInputType)
        {
            default:
            case S_BaseInputTypes.PRIMARY_BUTTON:
                return (int)WindowsMixedRealityInput.MenuButton;
            case S_BaseInputTypes.GRIP:
                return (int)WindowsMixedRealityInput.Grip;
            case S_BaseInputTypes.STICK:
                return (int)WindowsMixedRealityInput.Stick;
            case S_BaseInputTypes.INDEX:
                return (int)WindowsMixedRealityInput.Trigger;
        }
    }

    private static Tuple<S_DirectionTypes, S_StickDirections> AxisProviderStateNameIndexToDirectionTypeAndStickDirection(int _axisProviderStateNameIndex)
    {
        switch ((AxisProviderState)_axisProviderStateNameIndex)
        {
            default:
            case AxisProviderState.Pressed:
                return new Tuple<S_DirectionTypes, S_StickDirections>(S_DirectionTypes.PRESSED, S_StickDirections.PRESSED);
            case AxisProviderState.Released:
                return new Tuple<S_DirectionTypes, S_StickDirections>(S_DirectionTypes.RELEASED, S_StickDirections.RELEASED);
            case AxisProviderState.Up:
                return new Tuple<S_DirectionTypes, S_StickDirections>(S_DirectionTypes.PRESSED, S_StickDirections.UP);
            case AxisProviderState.UpRelease:
                return new Tuple<S_DirectionTypes, S_StickDirections>(S_DirectionTypes.RELEASED, S_StickDirections.UP);
            case AxisProviderState.Down:
                return new Tuple<S_DirectionTypes, S_StickDirections>(S_DirectionTypes.PRESSED, S_StickDirections.DOWN);
            case AxisProviderState.DownRelease:
                return new Tuple<S_DirectionTypes, S_StickDirections>(S_DirectionTypes.RELEASED, S_StickDirections.DOWN);
            case AxisProviderState.Left:
                return new Tuple<S_DirectionTypes, S_StickDirections>(S_DirectionTypes.PRESSED, S_StickDirections.LEFT);
            case AxisProviderState.LeftRelease:
                return new Tuple<S_DirectionTypes, S_StickDirections>(S_DirectionTypes.RELEASED, S_StickDirections.LEFT);
            case AxisProviderState.Right:
                return new Tuple<S_DirectionTypes, S_StickDirections>(S_DirectionTypes.PRESSED, S_StickDirections.RIGHT);
            case AxisProviderState.RightRelease:
                return new Tuple<S_DirectionTypes, S_StickDirections>(S_DirectionTypes.RELEASED, S_StickDirections.RIGHT);
        }
    }

    private static int StickDirectionAndDirectionTypeToAxisProviderStateNameIndex(S_StickDirections _stickDirection, S_DirectionTypes _directionType)
    {
        switch (_stickDirection)
        {
            default:
            case S_StickDirections.PRESSED:
                return (int)AxisProviderState.Pressed;
            case S_StickDirections.RELEASED:
                return (int)AxisProviderState.Released;
            case S_StickDirections.UP:
                if (_directionType == S_DirectionTypes.PRESSED)
                {
                    return (int)AxisProviderState.Up;
                }
                else
                {
                    return (int)AxisProviderState.UpRelease;
                }
            case S_StickDirections.DOWN:
                if (_directionType == S_DirectionTypes.PRESSED)
                {
                    return (int)AxisProviderState.Down;
                }
                else
                {
                    return (int)AxisProviderState.DownRelease;
                }
            case S_StickDirections.LEFT:
                if (_directionType == S_DirectionTypes.PRESSED)
                {
                    return (int)AxisProviderState.Left;
                }
                else
                {
                    return (int)AxisProviderState.LeftRelease;
                }
            case S_StickDirections.RIGHT:
                if (_directionType == S_DirectionTypes.PRESSED)
                {
                    return (int)AxisProviderState.Right;
                }
                else
                {
                    return (int)AxisProviderState.RightRelease;
                }
        }
    }
}
