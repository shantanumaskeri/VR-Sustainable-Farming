using Gaze;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(S_InputManager))]
    public class S_InputManagerEditor : S_Editor
    {
        private S_InputManager m_inputManager;

        private Texture2D m_iconInteraction;

        protected SC_Input m_ActionButton;
        protected string[] m_platformsNames;
        protected string[] m_viveInputNames;
        protected string[] m_oculusInputNames;
        protected string[] m_inputsNames;
        protected string[] m_gearVrInputNames;

        protected string[] m_allHandNames;
        protected string[] m_handNames;
        protected string[] m_GrabInputTypeNames = new string[] { "Trigger", "Grip" };
        protected enum GrabInput { Trigger, Grip };
        protected string[] m_teleportInputNames;
        protected string[] m_directionNames;
        protected string[] m_stickDirectionNames;

        public S_BaseInputTypes BaseInputType;
        public S_DirectionTypes DirectionType;

        private S_VisualGraphicSelector m_visualGraphicSelector;

        private string[] m_iconsTeleportEnabled = { "UI/Teleporter/NormalTeleportation_Enabled", "UI/Teleporter/HotspotTeleportation_Enabled", "UI/Teleporter/TriangleTeleport_Enabled" };
        private string[] m_iconsTeleportDisabled = { "UI/Teleporter/NormalTeleportation_Disabled", "UI/Teleporter/HotspotTeleportation_Disabled", "UI/Teleporter/TriangleTeleport_Disabled" };
        private string[] m_namesTeleport = { "Teleportation", "Hotspot Teleportion", "Joystick Navigation" };
        private bool[] m_flagTeleport = { false, false, false };
        private bool[] m_displayButtonsTeleport = { true, false, true };

        private bool m_ShowHandRaycastFoldout = false;

        // -------------------------------------------
        /*
		 * OnEnable
		 */
        private void OnEnable()
        {
            m_visualGraphicSelector = new S_VisualGraphicSelector();

            m_inputManager = (S_InputManager)target;

            m_platformsNames = Enum.GetNames(typeof(S_Controllers));
            m_oculusInputNames = Enum.GetNames(typeof(S_OculusInputTypes));

            m_allHandNames = Enum.GetNames(typeof(S_SingleHandsEnum));
            m_handNames = new string[2];
            for (int i = 0; i < 2; i++)
            {
                m_handNames[i] = m_allHandNames[i];
            }

            m_directionNames = Enum.GetNames(typeof(S_DirectionTypes));
            m_stickDirectionNames = Enum.GetNames(typeof(S_StickDirections));

            m_teleportInputNames = Enum.GetNames(typeof(S_BaseInputTypes));
        }

        // -------------------------------------------
        /*
		 * DisplayInputEntry
		 */
        private int DisplayInputEntry(S_InputsMapEntry _inputEntry, bool _enableDifferenceHands, string[] _baseInputNames)
        {
            EditorGUILayout.BeginHorizontal();

            // we chose to hard core the choice of the Oculus Rift because
            // other hardware's support is now broken
            _inputEntry.UISelectedPlatform = S_Controllers.OCULUS_TOUCH;

            if (_enableDifferenceHands)
            {
                _inputEntry.UIHandSelected = (S_SingleHandsEnum)EditorGUILayout.Popup((int)_inputEntry.UIHandSelected, m_handNames);
            }

            int uiSelection = EditorGUILayout.Popup(BaseInputTypeToGrabInputTypeNameIndex(_inputEntry.UIBaseInputType), _baseInputNames);

            EditorGUILayout.EndHorizontal();

            return uiSelection;
        }

        private int BaseInputTypeToGrabInputTypeNameIndex(S_BaseInputTypes _baseInputType)
        {
            switch (_baseInputType)
            {
                default:
                case S_BaseInputTypes.INDEX:
                    return (int)GrabInput.Trigger;
                case S_BaseInputTypes.GRIP:
                    return (int)GrabInput.Grip;
            }
        }

        private S_BaseInputTypes GrabInputTypeNameIndexToBaseInputType(int _grabInputTypeNameIndex)
        {
            switch ((GrabInput)_grabInputTypeNameIndex)
            {
                default:
                case GrabInput.Trigger:
                    return S_BaseInputTypes.INDEX;
                case GrabInput.Grip:
                    return S_BaseInputTypes.GRIP;
            }
        }

        // -------------------------------------------
        /*
         * SetUpDefaultGrabInput
         */
        private void SetUpDefaultGrabInput()
        {
            int uISelectedInputType = DisplayInputEntry(m_inputManager.DefaultGrabAction, false, m_GrabInputTypeNames);
            S_BaseInputTypes selectedInputType = GrabInputTypeNameIndexToBaseInputType(uISelectedInputType);
            m_inputManager.DefaultGrabAction.Set(S_SingleHandsEnum.BOTH, selectedInputType, S_DirectionTypes.PRESSED, S_StickDirections.DOWN);
        }

        // -------------------------------------------
        /*
         * DisplayTypeNavigation
         */
        private void DisplayTypeNavigation()
        {
            S_EditorUtils.DrawEditorHint("Set the type of navigation you want to use in your experience.");

            m_visualGraphicSelector.DisplayHandsSelectionButtons(m_inputManager);
        }


        // -------------------------------------------
        /*
		 * DisplayNormalTeleport
		 */
        private void DisplayNormalTeleport()
        {
            /*
            // COMMENTED IN ORDER TO RESTORE IN THE FUTURE IF TO SET A CUSTOM BUTTON FOR TELEPORT IS REQUIRED
            S_EditorUtils.DrawSectionTitle("Default Teleportation Input");
            S_EditorUtils.DrawEditorHint("Set the input that will be used for teleport");

            int uISelectedTeleportInputType = DisplayInputEntry(m_inputManager.DefaultTeleportAction, true, m_teleportInputNames);
            m_inputManager.DefaultTeleportAction.Set((S_SingleHandsEnum)m_inputManager.DefaultTeleportAction.UIHandSelected, (S_BaseInputTypes)uISelectedTeleportInputType, S_DirectionTypes.PRESSED, S_StickDirections.DOWN);
            */
            S_EditorLayoutUtils.Space(9);

            int indexTeleportJoystickSelected = -1;
            switch (m_inputManager.TeleportJoystickSelected)
            {
                case S_HandsEnum.LEFT:
                    indexTeleportJoystickSelected = 0;
                    break;

                case S_HandsEnum.RIGHT:
                    indexTeleportJoystickSelected = 1;
                    break;

                case S_HandsEnum.BOTH:
                    indexTeleportJoystickSelected = 2;
                    break;

                default:
                    indexTeleportJoystickSelected = 2;
                    break;
            }
            indexTeleportJoystickSelected = EditorGUILayout.Popup("Controller for teleporting", indexTeleportJoystickSelected, m_allHandNames, GUILayout.MaxWidth(300));
            switch (indexTeleportJoystickSelected)
            {
                case 0:
                    m_inputManager.TeleportJoystickSelected = S_HandsEnum.LEFT;
                    break;

                case 1:
                    m_inputManager.TeleportJoystickSelected = S_HandsEnum.RIGHT;
                    break;

                case 2:
                    m_inputManager.TeleportJoystickSelected = S_HandsEnum.BOTH;
                    break;
            }
            EditorGUILayout.Space();


            // FORCE THE TELEPORT TO WORK WITH THE STICK ALWAYS
            m_inputManager.DefaultTeleportAction = new S_InputsMapEntry();
            if (m_inputManager.TeleportJoystickSelected == S_HandsEnum.LEFT)
            {
                m_inputManager.DefaultTeleportAction.InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_DOWN_TILT;
            }
            else
            {
                m_inputManager.DefaultTeleportAction.InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_DOWN_TILT;
            }

            S_EditorUtils.DrawSectionTitle("Parameters");

            m_inputManager.MaxTeleportDistance = EditorGUILayout.FloatField(new GUIContent("Teleport Distance [m]", ""), m_inputManager.MaxTeleportDistance, GUILayout.Width(400));
            m_inputManager.MaxTeleportHeight = EditorGUILayout.FloatField(new GUIContent("Teleport Height [m]", ""), m_inputManager.MaxTeleportHeight, GUILayout.Width(400));
            m_inputManager.OrientOnTeleport = EditorGUILayout.Toggle(new GUIContent("Orient on Teleport", ""), m_inputManager.OrientOnTeleport, GUILayout.Width(400));
            EditorGUILayout.Space();

            S_EditorUtils.DrawSectionTitle("Input");
            S_EditorUtils.DrawEditorHint("Parameters for refining user's input before teleporting");
            m_inputManager.HoldDuration = EditorGUILayout.FloatField(new GUIContent("Hold Duration [s]", ""), m_inputManager.HoldDuration, GUILayout.Width(400));
            m_inputManager.InputSensitivity = EditorGUILayout.Slider(new GUIContent("Input Sensitivity", ""), m_inputManager.InputSensitivity, 0, 1, GUILayout.Width(400));
            m_inputManager.Cooldown = EditorGUILayout.FloatField(new GUIContent("Pause time [s]", ""), m_inputManager.Cooldown, GUILayout.Width(400));

            EditorGUILayout.Space();

            S_EditorUtils.DrawSectionTitle("Visual Properties");
            S_EditorUtils.DrawEditorHint("Customize the look of the teleportation");

            m_inputManager.TeleportTarget = ((GameObject)EditorGUILayout.ObjectField("Teleport Target", m_inputManager.TeleportTarget, typeof(GameObject), true, GUILayout.Width(320)));
            m_inputManager.ColorAllowedDestination = EditorGUILayout.ColorField("Allowed Destination", m_inputManager.ColorAllowedDestination, GUILayout.Width(400));
            m_inputManager.ColorRestrictedDestination = EditorGUILayout.ColorField("Not Allowed Destination", m_inputManager.ColorRestrictedDestination, GUILayout.Width(400));
            m_inputManager.LineWidth = EditorGUILayout.FloatField(new GUIContent("Line Width", ""), m_inputManager.LineWidth, GUILayout.Width(400));
            m_inputManager.LineMaterial = ((Material)EditorGUILayout.ObjectField("Line Material", m_inputManager.LineMaterial, typeof(Material), true, GUILayout.Width(320)));

            EditorGUILayout.Space();
        }

        // -------------------------------------------
        /*
		 * DisplayHotspotTeleport
		 */
        private void DisplayHotspotTeleport()
        {

        }

        // -------------------------------------------
        /*
		 * DisplayJoystickNavigation
		 */
        private void DisplayJoystickNavigation()
        {
            S_EditorLayoutUtils.Space(9);

            S_EditorUtils.DrawSectionTitle("Movement Joystick");
            S_EditorUtils.DrawEditorHint("Set the joystick to be used for the camera movement.");

            int indexNavigationJoystickSelected = -1;
            switch (m_inputManager.NavigationJoystickSelected)
            {
                case S_HandsEnum.LEFT:
                    indexNavigationJoystickSelected = 0;
                    break;

                case S_HandsEnum.RIGHT:
                    indexNavigationJoystickSelected = 1;
                    break;

                case S_HandsEnum.BOTH:
                    indexNavigationJoystickSelected = 2;
                    break;

                default:
                    indexNavigationJoystickSelected = 2;
                    break;
            }
            indexNavigationJoystickSelected = EditorGUILayout.Popup(indexNavigationJoystickSelected, m_allHandNames, GUILayout.Width(100));
            switch (indexNavigationJoystickSelected)
            {
                case 0:
                    m_inputManager.NavigationJoystickSelected = S_HandsEnum.LEFT;
                    break;

                case 1:
                    m_inputManager.NavigationJoystickSelected = S_HandsEnum.RIGHT;
                    break;

                case 2:
                    m_inputManager.NavigationJoystickSelected = S_HandsEnum.BOTH;
                    break;
            }
            EditorGUILayout.Space();

            S_EditorUtils.DrawSectionTitle("Parameters");

            m_inputManager.SpeedJoystick = EditorGUILayout.FloatField(new GUIContent("Speed [m/s]", ""), m_inputManager.SpeedJoystick, GUILayout.Width(400));
        }

        // -------------------------------------------
        /*
         * DisplayJoystickRotation
         */
        private void DisplayJoystickRotation()
        {
            m_inputManager.EnableRotation = EditorGUILayout.ToggleLeft(new GUIContent(" Enable Camera Rotation", ""), m_inputManager.EnableRotation, GUILayout.Width(400));
            if (m_inputManager.EnableRotation)
            {
                S_EditorUtils.DrawEditorHint("Choose the joystick to be used for rotating the camera");
                int indexRotationJoystickSelected = -1;
                switch (m_inputManager.RotationJoystickSelected)
                {
                    case S_HandsEnum.LEFT:
                        indexRotationJoystickSelected = 0;
                        break;

                    case S_HandsEnum.RIGHT:
                        indexRotationJoystickSelected = 1;
                        break;

                    case S_HandsEnum.BOTH:
                        indexRotationJoystickSelected = 2;
                        break;

                    default:
                        indexRotationJoystickSelected = 2;
                        break;
                }
                indexRotationJoystickSelected = EditorGUILayout.Popup(indexRotationJoystickSelected, m_allHandNames, GUILayout.Width(100));
                switch (indexRotationJoystickSelected)
                {
                    case 0:
                        m_inputManager.RotationJoystickSelected = S_HandsEnum.LEFT;
                        break;

                    case 1:
                        m_inputManager.RotationJoystickSelected = S_HandsEnum.RIGHT;
                        break;

                    case 2:
                        m_inputManager.RotationJoystickSelected = S_HandsEnum.BOTH;
                        break;
                }
                EditorGUILayout.Space();
                m_inputManager.AngleRotationJoystick = EditorGUILayout.Slider(new GUIContent("Angle", ""), m_inputManager.AngleRotationJoystick, 0, 90, GUILayout.Width(400));
            }
        }

        // -------------------------------------------
        /*
         * DisplayNavigation
         */
        private void DisplayNavigation()
        {
            DisplayTypeNavigation();

            switch (m_inputManager.TeleportType)
            {
                case S_NavigationTypes.TELEPORT:
                    DisplayNormalTeleport();
                    break;
                case S_NavigationTypes.HOTSPOT:
                    DisplayHotspotTeleport();
                    break;
                case S_NavigationTypes.JOYSTICK:
                    DisplayJoystickNavigation();
                    break;
            }
        }

        // -------------------------------------------
        /*
		 * DisplayHandRaycast
		 */
        private void DisplayHandRaycast()
        {
            S_EditorUtils.DrawSectionTitle("Default Laser Parameters");
            S_EditorUtils.DrawEditorHint("Settings for the laser coming out of the controllers.");

            m_inputManager.HideLaser = EditorGUILayout.Toggle(new GUIContent("Hide Laser", ""), m_inputManager.HideLaser, GUILayout.Width(400));

            if (m_inputManager.HideLaser)
            {
                int indexHideLaserSelected = -1;
                switch (m_inputManager.HideLaserSelected)
                {
                    case S_HandsEnum.LEFT:
                        indexHideLaserSelected = 0;
                        break;

                    case S_HandsEnum.RIGHT:
                        indexHideLaserSelected = 1;
                        break;

                    case S_HandsEnum.BOTH:
                        indexHideLaserSelected = 2;
                        break;

                    default:
                        indexHideLaserSelected = 2;
                        break;
                }
                indexHideLaserSelected = EditorGUILayout.Popup("Hand to hide laser", indexHideLaserSelected, m_allHandNames, GUILayout.MaxWidth(300));
                switch (indexHideLaserSelected)
                {
                    case 0:
                        m_inputManager.HideLaserSelected = S_HandsEnum.LEFT;
                        break;

                    case 1:
                        m_inputManager.HideLaserSelected = S_HandsEnum.RIGHT;
                        break;

                    case 2:
                        m_inputManager.HideLaserSelected = S_HandsEnum.BOTH;
                        break;
                }
                EditorGUILayout.Space();
            }

            m_inputManager.DefaultLaserStartWidth = EditorGUILayout.FloatField(new GUIContent("Laser Start Width [m]", ""), m_inputManager.DefaultLaserStartWidth, GUILayout.Width(400));
            m_inputManager.DefaultLaserEndWidth = EditorGUILayout.FloatField(new GUIContent("Laser End Width [m]", ""), m_inputManager.DefaultLaserEndWidth, GUILayout.Width(400));
            m_inputManager.DefaultLaserLength = EditorGUILayout.FloatField(new GUIContent("Laser Length [m]", ""), m_inputManager.DefaultLaserLength, GUILayout.Width(400));
            m_inputManager.DefaultLaserMaterial = ((Material)EditorGUILayout.ObjectField("Laser Material", m_inputManager.DefaultLaserMaterial, typeof(Material), true, GUILayout.Width(400)));
            m_inputManager.DefaultLaserPointObject = ((GameObject)EditorGUILayout.ObjectField("Laser Pointer", m_inputManager.DefaultLaserPointObject, typeof(GameObject), true, GUILayout.Width(400)));
            m_inputManager.DefaultLaserColor = EditorGUILayout.ColorField("Laser Color", m_inputManager.DefaultLaserColor, GUILayout.Width(400));
            EditorGUILayout.Space();

            S_EditorUtils.DrawSectionTitle("Manipulation Laser Parameters");
            S_EditorUtils.DrawEditorHint("Settings for the laser when it hits an Interactive Object.");
            EditorGUILayout.Space();

            m_inputManager.HideGrabPoint = EditorGUILayout.Toggle(new GUIContent("Hide Grab Pointer", ""), m_inputManager.HideGrabPoint, GUILayout.Width(400));
            m_inputManager.GrabTargetPositionLeft = ((GameObject)EditorGUILayout.ObjectField("Grab Target Position Left", m_inputManager.GrabTargetPositionLeft, typeof(GameObject), true, GUILayout.Width(400)));
            m_inputManager.GrabTargetPositionRight = ((GameObject)EditorGUILayout.ObjectField("Grab Target Position Right", m_inputManager.GrabTargetPositionRight, typeof(GameObject), true, GUILayout.Width(400)));
            m_inputManager.GrabLaserStartWidth = EditorGUILayout.FloatField(new GUIContent("Grab Laser Start Width [m]", ""), m_inputManager.GrabLaserStartWidth, GUILayout.Width(400));
            m_inputManager.GrabLaserEndWidth = EditorGUILayout.FloatField(new GUIContent("Grab Laser End Width [m]", ""), m_inputManager.GrabLaserEndWidth, GUILayout.Width(400));
            m_inputManager.GrabLaserLength = EditorGUILayout.FloatField(new GUIContent("Grab Laser Length [m]", ""), m_inputManager.GrabLaserLength, GUILayout.Width(400));
            m_inputManager.GrabLaserInteractablePointObject = ((GameObject)EditorGUILayout.ObjectField("Grab Laser In-Range Pointer", m_inputManager.GrabLaserInteractablePointObject, typeof(GameObject), true, GUILayout.Width(400)));
            m_inputManager.GrabLaserInRangeColor = EditorGUILayout.ColorField("Grab Laser In-Range Color", m_inputManager.GrabLaserInRangeColor, GUILayout.Width(400));
            m_inputManager.GrabLaserNonInteractablePointObject = ((GameObject)EditorGUILayout.ObjectField("Grab Laser Out-Range Pointer", m_inputManager.GrabLaserNonInteractablePointObject, typeof(GameObject), true, GUILayout.Width(400)));
            m_inputManager.GrabLaserOutRangeColor = EditorGUILayout.ColorField("Grab Laser Out-Range Color", m_inputManager.GrabLaserOutRangeColor, GUILayout.Width(400));
        }

        // -------------------------------------------
        /*
		 * DisplayInputManagerInspectorUI
		 */
        private void DisplayInputManagerInspectorUI()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("User Settings");
            S_EditorUtils.DrawEditorHint("Options for navigation and for controllers.");
            EditorGUILayout.Space();

            m_inputManager.Navigation = EditorGUILayout.ToggleLeft(" Navigation", m_inputManager.Navigation);

            if (m_inputManager.Navigation)
            {
                DisplayNavigation();
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (((m_inputManager.TeleportType == S_NavigationTypes.TELEPORT) && (m_inputManager.TeleportJoystickSelected != S_HandsEnum.BOTH)) ||
                ((m_inputManager.TeleportType == S_NavigationTypes.JOYSTICK) && (m_inputManager.NavigationJoystickSelected != S_HandsEnum.BOTH)))
            {
                DisplayJoystickRotation();

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
            else
            {
                m_inputManager.EnableRotation = false;
            }

            S_EditorUtils.DrawSectionTitle("Manipulation");
            EditorGUILayout.LabelField("Default Input");
            S_EditorUtils.DrawEditorHint("Set the input to be used for manipulating objects in your experience (both hands).");

            // DEFAULT HAND GRAB INPUT
            SetUpDefaultGrabInput();

            m_ShowHandRaycastFoldout = EditorGUILayout.Foldout(m_ShowHandRaycastFoldout, "Controllers Raycast");
            if (m_ShowHandRaycastFoldout)
            {
                DisplayHandRaycast();
            }
        }

        // -------------------------------------------
        /*
		 * OnEditorUI
		 */
        public override void OnEditorUI()
        {
            DisplayInputManagerInspectorUI();
        }


        // -------------------------------------------
        /*
		 * OnRuntimeUI
		 */
        public override void OnRuntimeUI()
        {
            DisplayInputManagerInspectorUI();
        }
    }
}
