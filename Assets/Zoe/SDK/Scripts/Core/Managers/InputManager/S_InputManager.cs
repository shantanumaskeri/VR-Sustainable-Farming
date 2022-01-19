// <copyright file="S_InputManager.cs" company="apelab sàrl">
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
using UnityEngine;

namespace SpatialStories
{
    public class S_InputManager : MonoBehaviour
    {
        // ----------------------------------------------
        // SINGLETON
        // ----------------------------------------------	
        private static S_InputManager _instance;

        public static S_InputManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType(typeof(S_InputManager)) as S_InputManager;
                }
                return _instance;
            }
        }

        public const float AXIS_TOLERANCE = 0.1f;
        public const float TRIGGER_SENSIBILITY = 0.8f;

        private const float TRACKPAD_CENTER_AREA = 0.5f;

        // PUBLIC MEMBERS INSPECTOR
        public bool LeftHandActive = true;
        public bool RightHandActive = true;
        public AudioClip hapticAudioClipMin, hapticAudioClipMax;
        public float padTouchDirectionThreshold = .5f;
        public bool debug = false;

        public S_InputsMapEntry DefaultGrabAction;

        public bool Navigation = false;
        public S_NavigationTypes TeleportType = S_NavigationTypes.TELEPORT;
        public S_InputsMapEntry DefaultTeleportAction;

        // TELEPORT PARAMETERS
        public S_HandsEnum TeleportJoystickSelected = S_HandsEnum.BOTH;
        public float MaxTeleportDistance = 3;
        public float MaxTeleportHeight = 4;
        public bool OrientOnTeleport = false;

        // INPUT MANAGEMENT
        public float HoldDuration = 0.5f;
        public float InputSensitivity = 1f;
        public float Cooldown = 0.2f;

        // VISUAL PROPERTIES
        public GameObject TeleportTarget;
        public Color ColorAllowedDestination = Color.blue;
        public Color ColorRestrictedDestination = Color.red;
        public float LineWidth = 0.01f;
        public Material LineMaterial;

        // JOYSTICK NAVIGATION
        public S_InputsMapEntry DefaultJoystickNavigation;
        public S_HandsEnum NavigationJoystickSelected = S_HandsEnum.BOTH;
        public float SpeedJoystick = 2;
        public bool EnableRotation = false;
        public bool SmoothRotation = false;
        public float RotationTimeJoystick = 1;
        public S_HandsEnum RotationJoystickSelected = S_HandsEnum.BOTH;
        public float AngleRotationJoystick = 0;
        private Transform m_headCamera;
        private Transform m_targetCamera;
        private Vector3 m_originalRotationCam;
        private float m_applyRotation = 0;
        private int m_rotationDirection = 1;

        // DEFAULT LASER PARAMETERS
        public bool HideLaser = false;
        public S_HandsEnum HideLaserSelected = S_HandsEnum.BOTH;
        public float DefaultLaserStartWidth = 0.04f;
        public float DefaultLaserLength = 5f;
        public float DefaultLaserEndWidth = 0.02f;
        public Material DefaultLaserMaterial;
        public GameObject DefaultLaserPointObject;
        public Color DefaultLaserColor = Color.white;
        
        // GRAB LASER PARAMETERS
        public GameObject GrabTargetObject;
        public bool HideGrabPoint = false;
        public GameObject GrabTargetPositionLeft;
        public GameObject GrabTargetPositionRight;
        public float GrabLaserStartWidth = 0.04f;
        public float GrabLaserLength = 5f;
        public float GrabLaserEndWidth = 0.02f;
        public GameObject GrabLaserNonInteractablePointObject;
        public GameObject GrabLaserInteractablePointObject;
        public Color GrabLaserInRangeColor = Color.white;
        public Color GrabLaserOutRangeColor = Color.white;

        /// <summary>
        /// Fired when the player has found two items that creates a Totem.
        /// </summary>
        public delegate void ControllerCollisionHandler(S_ControllerCollisionEventArgs e);
        public event ControllerCollisionHandler OnControllerCollisionEvent;
        public void FireControllerCollisionEvent(S_ControllerCollisionEventArgs e) { if (OnControllerCollisionEvent != null) OnControllerCollisionEvent(e); }

        /// <summary>
        /// Fired when the player is grabbing an object with the controller.
        /// </summary>
        public delegate void ControllerGrabHandler(S_ControllerGrabEventArgs e);
        public event ControllerGrabHandler OnControllerGrabEvent;
        public void FireControllerGrabEvent(S_ControllerGrabEventArgs e) { if (OnControllerGrabEvent != null) OnControllerGrabEvent(e); }

        /// <summary>
        /// Fired when the player starts attracting an object with the controller.
        /// </summary>
        public delegate void ControllerAttractHandler(S_ControllerGrabEventArgs e);
        public event ControllerAttractHandler OnControllerAttractEvent;
        public void FireControllerAttractEvent(S_ControllerGrabEventArgs e) { if (OnControllerAttractEvent != null) OnControllerAttractEvent(e); }

        /// <summary>
        /// Fired when the player is touching an object with the controller.
        /// </summary>
        public delegate void ControllerTouchHandler(S_ControllerTouchEventArgs e);
        public event ControllerTouchHandler OnControllerTouchEvent;
        public void FireControllerTouchEvent(S_ControllerTouchEventArgs e) { if (OnControllerTouchEvent != null) OnControllerTouchEvent(e); }

        public delegate void InputEvent(S_InputEventArgs e);

        public event InputEvent MenuButtonPressEvent;
        public event InputEvent MenuButtonReleaseEvent;

        public void FireMenuButtonPressEvent(S_InputEventArgs args) { if (MenuButtonPressEvent != null) MenuButtonPressEvent(args); }
        public void FireMenuButtonReleaseEvent(S_InputEventArgs args) { if (MenuButtonReleaseEvent != null) MenuButtonReleaseEvent(args); }

        public event InputEvent RightPrimaryButtonPressEvent;
        public event InputEvent RightPrimaryButtonReleaseEvent;
        public event InputEvent RightSecondaryButtonPressEvent;
        public event InputEvent RightSecondaryButtonReleaseEvent;
        public event InputEvent LeftPrimaryButtonPressEvent;
        public event InputEvent LeftPrimaryButtonReleaseEvent;
        public event InputEvent LeftSecondaryButtonPressEvent;
        public event InputEvent LeftSecondaryButtonReleaseEvent;

        public void FireRightPrimaryButtonPressEvent(S_InputEventArgs args) { if (RightPrimaryButtonPressEvent != null) RightPrimaryButtonPressEvent(args); }
        public void FireRightPrimaryButtonReleaseEvent(S_InputEventArgs args) { if (RightPrimaryButtonReleaseEvent != null) RightPrimaryButtonReleaseEvent(args); }
        public void FireRightSecondaryButtonPressEvent(S_InputEventArgs _args) { if (RightSecondaryButtonPressEvent != null) RightSecondaryButtonPressEvent(_args); }
        public void FireRightSecondaryButtonReleaseEvent(S_InputEventArgs _args) { if (RightSecondaryButtonReleaseEvent != null) RightSecondaryButtonReleaseEvent(_args); }
        public void FireLeftPrimaryButtonPressEvent(S_InputEventArgs _args) { if (LeftPrimaryButtonPressEvent != null) LeftPrimaryButtonPressEvent(_args); }
        public void FireLeftPrimaryButtonReleaseEvent(S_InputEventArgs _args) { if (LeftPrimaryButtonReleaseEvent != null) LeftPrimaryButtonReleaseEvent(_args); }
        public void FireLeftSecondaryButtonPressEvent(S_InputEventArgs _args) { if (LeftSecondaryButtonPressEvent != null) LeftSecondaryButtonPressEvent(_args); }
        public void FireLeftSecondaryButtonReleaseEvent(S_InputEventArgs _args) { if (LeftSecondaryButtonReleaseEvent != null) LeftSecondaryButtonReleaseEvent(_args); }

        public event InputEvent LeftTriggerButtonPressEvent;
        public event InputEvent LeftTriggerButtonReleaseEvent;
        public event InputEvent RightTriggerButtonPressEvent;
        public event InputEvent RightTriggerButtonReleaseEvent;

        public void FireLeftTriggerButtonPressEvent(S_InputEventArgs _args) { if (LeftTriggerButtonPressEvent != null) LeftTriggerButtonPressEvent(_args); }
        public void FireLeftTriggerButtonReleaseEvent(S_InputEventArgs _args) { if (LeftTriggerButtonReleaseEvent != null) LeftTriggerButtonReleaseEvent(_args); }
        public void FireRightTriggerButtonPressEvent(S_InputEventArgs _args) { if (RightTriggerButtonPressEvent != null) RightTriggerButtonPressEvent(_args); }
        public void FireRightTriggerButtonReleaseEvent(S_InputEventArgs _args) { if (RightTriggerButtonReleaseEvent != null) RightTriggerButtonReleaseEvent(_args); }

        public event InputEvent LeftGripButtonPressEvent;
        public event InputEvent LeftGripButtonReleaseEvent;

        public void FireLeftGripButtonPressEvent(S_InputEventArgs _args) { if (LeftGripButtonPressEvent != null) LeftGripButtonPressEvent(_args); }
        public void FireLeftGripButtonReleaseEvent(S_InputEventArgs _args) { if (LeftGripButtonReleaseEvent != null) LeftGripButtonReleaseEvent(_args); }

        public event InputEvent RightSecondaryButtonTouchEvent;
        public event InputEvent RightSecondaryButtonUntouchEvent;
        public event InputEvent RightPrimaryButtonTouchEvent;
        public event InputEvent RightPrimaryButtonUntouchEvent;
        public event InputEvent LeftPrimaryButtonUntouchEvent;
        public event InputEvent LeftPrimaryButtonTouchEvent;
        public event InputEvent LeftSecondaryButtonTouchEvent;
        public event InputEvent LeftSecondaryButtonUntouchEvent;

        public void FireRightSecondaryButtonTouchEvent(S_InputEventArgs args) { if (RightSecondaryButtonTouchEvent != null) RightSecondaryButtonTouchEvent(args); }
        public void FireRightSecondaryButtonUntouchEvent(S_InputEventArgs args) { if (RightSecondaryButtonUntouchEvent != null) RightSecondaryButtonUntouchEvent(args); }
        public void FireRightPrimaryButtonTouchEvent(S_InputEventArgs args) { if (RightPrimaryButtonTouchEvent != null) RightPrimaryButtonTouchEvent(args); }
        public void FireRightPrimaryButtonUntouchEvent(S_InputEventArgs args) { if (RightPrimaryButtonUntouchEvent != null) RightPrimaryButtonUntouchEvent(args); }
        public void FireLeftPrimaryButtonUntouchEvent(S_InputEventArgs args) { if (LeftPrimaryButtonUntouchEvent != null) LeftPrimaryButtonUntouchEvent(args); }
        public void FireLeftPrimaryButtonTouchEvent(S_InputEventArgs args) { if (LeftPrimaryButtonTouchEvent != null) LeftPrimaryButtonTouchEvent(args); }
        public void FireLeftSecondaryButtonTouchEvent(S_InputEventArgs args) { if (LeftSecondaryButtonTouchEvent != null) LeftSecondaryButtonTouchEvent(args); }
        public void FireLeftSecondaryButtonUntouchEvent(S_InputEventArgs args) { if (LeftSecondaryButtonUntouchEvent != null) LeftSecondaryButtonUntouchEvent(args); }

        public event InputEvent OnButtonIndexTouch;
        public event InputEvent OnButtonIndexUntouch;
        public event InputEvent OnButtonThumbrestTouch;
        public event InputEvent OnButtonThumbrestUntouch;
        public event InputEvent OnButtonThumbstickUntouch;
        public event InputEvent OnButtonThumbstickTouch;

        public void FireOnButtonIndexTouch(S_InputEventArgs args) { if (OnButtonIndexTouch != null) OnButtonIndexTouch(args); }
        public void FireOnButtonIndexUntouch(S_InputEventArgs args) { if (OnButtonIndexUntouch != null) OnButtonIndexUntouch(args); }
        public void FireOnButtonThumbrestTouch(S_InputEventArgs args) { if (OnButtonThumbrestTouch != null) OnButtonThumbrestTouch(args); }
        public void FireOnButtonThumbrestUntouch(S_InputEventArgs args) { if (OnButtonThumbrestUntouch != null) OnButtonThumbrestUntouch(args); }
        public void FireOnButtonThumbstickUntouch(S_InputEventArgs args) { if (OnButtonThumbstickUntouch != null) OnButtonThumbstickUntouch(args); }
        public void FireOnButtonThumbstickTouch(S_InputEventArgs args) { if (OnButtonThumbstickTouch != null) OnButtonThumbstickTouch(args); }

        public event InputEvent OnButtonLeftIndexTouch;
        public event InputEvent OnButtonLeftIndexUntouch;
        public event InputEvent OnButtonLeftThumbrestTouch;
        public event InputEvent OnButtonLeftThumbrestUntouch;
        public event InputEvent LeftPrimary2DAxisUntouchEvent;
        public event InputEvent LeftPrimary2DAxisTouchEvent;

        public void FireOnButtonLeftIndexTouch(S_InputEventArgs args) { if (OnButtonLeftIndexTouch != null) OnButtonLeftIndexTouch(args); }
        public void FireOnButtonLeftIndexUntouch(S_InputEventArgs args) { if (OnButtonLeftIndexUntouch != null) OnButtonLeftIndexUntouch(args); }
        public void FireOnButtonLeftThumbrestTouch(S_InputEventArgs args) { if (OnButtonLeftThumbrestTouch != null) OnButtonLeftThumbrestTouch(args); }
        public void FireOnButtonLeftThumbrestUntouch(S_InputEventArgs args) { if (OnButtonLeftThumbrestUntouch != null) OnButtonLeftThumbrestUntouch(args); }
        public void FireLeftPrimary2DAxisUntouchEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisUntouchEvent != null) LeftPrimary2DAxisUntouchEvent(args); }
        public void FireLeftPrimary2DAxisTouchEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisTouchEvent != null) LeftPrimary2DAxisTouchEvent(args); }

        public event InputEvent RightPrimary2DAxisTouchEvent;
        public event InputEvent RightPrimary2DAxisUntouchEvent;

        public void FireRightPrimary2DAxisTouchEvent(S_InputEventArgs args) { if (RightPrimary2DAxisTouchEvent != null) RightPrimary2DAxisTouchEvent(args); }
        public void FireRightPrimary2DAxisUntouchEvent(S_InputEventArgs args) {  if (RightPrimary2DAxisUntouchEvent != null) RightPrimary2DAxisUntouchEvent(args); }

        public event InputEvent RightGripButtonPressEvent;
        public event InputEvent RightGripButtonReleaseEvent;

        public void FireRightGripButtonPressEvent(S_InputEventArgs args) { if (RightGripButtonPressEvent != null) RightGripButtonPressEvent(args); }
        public void FireRightGripButtonReleaseEvent(S_InputEventArgs args) { if (RightGripButtonReleaseEvent != null) RightGripButtonReleaseEvent(args); }

        public event InputEvent LeftPrimary2DAxisEvent;
        public event InputEvent LeftPrimary2DAxisReleaseEvent;
        public event InputEvent LeftPrimary2DAxisButtonPressEvent;
        public event InputEvent LeftPrimary2DAxisButtonReleaseEvent;

        public void FireLeftPrimary2DAxisEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisEvent != null) LeftPrimary2DAxisEvent(args); }
        public void FireLeftPrimary2DAxisReleaseEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisReleaseEvent != null) LeftPrimary2DAxisReleaseEvent(args); }
        public void FireLeftPrimary2DAxisButtonPressEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisButtonPressEvent != null) LeftPrimary2DAxisButtonPressEvent(args); }
        public void FireLeftPrimary2DAxisButtonReleaseEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisButtonReleaseEvent != null) LeftPrimary2DAxisButtonReleaseEvent(args); }

        public event InputEvent RightPrimary2DAxisEvent;
        public event InputEvent RightPrimary2DAxisReleaseEvent;
        public event InputEvent RightPrimary2DAxisButtonPressEvent;
        public event InputEvent RightPrimary2DAxisButtonReleaseEvent;

        public void FireRightPrimary2DAxisEvent(S_InputEventArgs args) { if (RightPrimary2DAxisEvent != null) RightPrimary2DAxisEvent(args); }
        public void FireRightPrimary2DAxisReleaseEvent(S_InputEventArgs args) { if (RightPrimary2DAxisReleaseEvent != null) RightPrimary2DAxisReleaseEvent(args); }
        public void FireRightPrimary2DAxisButtonPressEvent(S_InputEventArgs args) { if (RightPrimary2DAxisButtonPressEvent != null) RightPrimary2DAxisButtonPressEvent(args); }
        public void FireRightPrimary2DAxisButtonReleaseEvent(S_InputEventArgs args) { if (RightPrimary2DAxisButtonReleaseEvent != null) RightPrimary2DAxisButtonReleaseEvent(args); }

        public event InputEvent LeftPrimary2DAxisLeftTiltEvent;
        public event InputEvent LeftPrimary2DAxisLeftTiltReleaseEvent;
        public event InputEvent LeftPrimary2DAxisRightTiltEvent;
        public event InputEvent LeftPrimary2DAxisRightTiltReleaseEvent;
        public event InputEvent LeftPrimary2DAxisUpTiltEvent;
        public event InputEvent LeftPrimary2DAxisUpTiltReleaseEvent;
        public event InputEvent LeftPrimary2DAxisDownTiltEvent;
        public event InputEvent LeftPrimary2DAxisDownTiltReleaseEvent;

        public void FireLeftPrimary2DAxisLeftTiltEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisLeftTiltEvent != null) LeftPrimary2DAxisLeftTiltEvent(args); }
        public void FireLeftPrimary2DAxisLeftTiltReleaseEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisLeftTiltReleaseEvent != null) LeftPrimary2DAxisLeftTiltReleaseEvent(args); }
        public void FireLeftPrimary2DAxisRightTiltEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisRightTiltEvent != null) LeftPrimary2DAxisRightTiltEvent(args); }
        public void FireLeftPrimary2DAxisRightTiltReleaseEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisRightTiltReleaseEvent != null) LeftPrimary2DAxisRightTiltReleaseEvent(args); }
        public void FireLeftPrimary2DAxisUpTiltEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisUpTiltEvent != null) LeftPrimary2DAxisUpTiltEvent(args); }
        public void FireLeftPrimary2DAxisUpTiltReleaseEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisUpTiltReleaseEvent != null) LeftPrimary2DAxisUpTiltReleaseEvent(args); }
        public void FireLeftPrimary2DAxisDownTiltEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisDownTiltEvent != null) LeftPrimary2DAxisDownTiltEvent(args); }
        public void FireLeftPrimary2DAxisDownTiltReleaseEvent(S_InputEventArgs args) { if (LeftPrimary2DAxisDownTiltReleaseEvent != null) LeftPrimary2DAxisDownTiltReleaseEvent(args); }

        public event InputEvent RightPrimary2DAxisLeftTiltEvent;
        public event InputEvent RightPrimary2DAxisLeftTiltReleaseEvent;
        public event InputEvent RightPrimary2DAxisRightTiltEvent;
        public event InputEvent RightPrimary2DAxisRightTiltReleaseEvent;
        public event InputEvent RightPrimary2DAxisUpTiltEvent;
        public event InputEvent RightPrimary2DAxisUpTiltReleaseEvent;
        public event InputEvent RightPrimary2DAxisDownTiltEvent;
        public event InputEvent RightPrimary2DAxisDownTiltReleaseEvent;

        public void FireRightPrimary2DAxisLeftTiltEvent(S_InputEventArgs args) { if (RightPrimary2DAxisLeftTiltEvent != null) RightPrimary2DAxisLeftTiltEvent(args); }
        public void FireRightPrimary2DAxisLeftTiltReleaseEvent(S_InputEventArgs args) { if (RightPrimary2DAxisLeftTiltReleaseEvent != null) RightPrimary2DAxisLeftTiltReleaseEvent(args); }
        public void FireRightPrimary2DAxisRightTiltEvent(S_InputEventArgs args) { if (RightPrimary2DAxisRightTiltEvent != null) RightPrimary2DAxisRightTiltEvent(args); }
        public void FireRightPrimary2DAxisRightTiltReleaseEvent(S_InputEventArgs args) { if (RightPrimary2DAxisRightTiltReleaseEvent != null) RightPrimary2DAxisRightTiltReleaseEvent(args); }
        public void FireRightPrimary2DAxisUpTiltEvent(S_InputEventArgs args) { if (RightPrimary2DAxisUpTiltEvent != null) RightPrimary2DAxisUpTiltEvent(args); }
        public void FireRightPrimary2DAxisUpTiltReleaseEvent(S_InputEventArgs args) { if (RightPrimary2DAxisUpTiltReleaseEvent != null) RightPrimary2DAxisUpTiltReleaseEvent(args); }
        public void FireRightPrimary2DAxisDownTiltEvent(S_InputEventArgs args) { if (RightPrimary2DAxisDownTiltEvent != null) RightPrimary2DAxisDownTiltEvent(args); }
        public void FireRightPrimary2DAxisDownTiltReleaseEvent(S_InputEventArgs args) { if (RightPrimary2DAxisDownTiltReleaseEvent != null) RightPrimary2DAxisDownTiltReleaseEvent(args); }

        public GameObject LeftController { get { return m_LeftHandIO; } }
        public GameObject RightController { get { return m_RightHandIO; } }

        public bool ControllersConnected { get { return m_ControllersConnected; } }

        private GameObject m_LeftHandIO, m_RightHandIO;

        // Methods to query the hands positions
        public Vector3 LeftHandPosition { get { return m_LeftHandIO.transform.position; } }
        public Vector3 RightHandPosition { get { return m_RightHandIO.transform.position; } }

        // Local positions of the hand
        public Vector3 LeftHandLocalPosition { get { return m_LeftHandIO.transform.localPosition; } }
        public Vector3 RightHandLocalPosition { get { return m_RightHandIO.transform.localPosition; } }

        private bool m_ControllersConnected;

        // the names of the connected controllers (HTC, Oculus Touch...)
        private string[] m_controllersNames;
        private bool m_IsHandRightDown = false, m_IsHandLeftDown = false;
        private bool m_IsIndexLeftDown = false, m_IsIndexRightDown = false;
        protected Vector2 m_AxisValue;

        public bool IsHandRightDown
        {
            get { return m_IsHandRightDown; }
            set { m_IsHandRightDown = value; }
        }
        public bool IsHandLeftDown
        {
            get { return m_IsHandLeftDown; }
            set { m_IsHandLeftDown = value; }
        }
        public bool IsIndexLeftDown
        {
            get { return m_IsIndexLeftDown; }
            set { m_IsIndexLeftDown = value; }
        }
        public bool IsIndexRightDown
        {
            get { return m_IsIndexRightDown; }
            set { m_IsIndexRightDown = value; }
        }

        // is left or right handed
        private uint m_Handedness;

        private float m_LastUpdateTime;
        public float UpdateInterval = .2f;

        public XRInputMapper XRInputMapper;
        public GameObject UnpluggedControllerMessage;
        private float m_LocalHandsHeigth;

        /// <summary>
        /// Used to know what is the dominant direction when the user is using a pad or a joystick
        /// </summary>
        public S_InputTypes DominantDirectionLeftPad;
        public S_InputTypes DominantDirectionRightPad;

        /// <summary>
        /// Used to distinguish between a Touch or a Press
        /// </summary>
        private bool m_IsLeftStickDown = false;
        public bool IsLeftStickDown
        {
            get { return m_IsLeftStickDown; }
            set { m_IsLeftStickDown = value; }
        }

        [HideInInspector]
        private bool m_IsRightStickDown = false;
        public bool IsRightStickDown
        {
            get { return m_IsRightStickDown; }
            set { m_IsRightStickDown = value; }
        }

        // -------------------------------------------
        /* 
		 * Awake
		 */
        void Awake()
        {
            _instance = this;

            m_AxisValue = new Vector2();
            m_AxisValue = Vector2.zero;

            if (Navigation && TeleportType == S_NavigationTypes.TELEPORT)
            {
                gameObject.AddComponent<S_Teleporter>();
            }

            if (UnpluggedControllerMessage != null)
            {
                UnpluggedControllerMessage.SetActive(false);
            }

            XRRig.Instance.StartUp();
            S_HapticsManager.StartUp();
        }

        // -------------------------------------------
        /* 
		 * Start
		 */
        void Start()
        {
            GetControllers();

            m_headCamera = this.gameObject.transform;
            m_targetCamera = m_headCamera.GetComponentInChildren<Camera>().transform;

            S_InputManager.Instance.LeftPrimary2DAxisEvent += OnStickAxisManageEvent;
            S_InputManager.Instance.RightPrimary2DAxisEvent += OnStickAxisManageEvent;

            S_InputManager.Instance.OnButtonThumbstickUntouch += OnThumbstickUntouchEvent;
            S_InputManager.Instance.LeftPrimary2DAxisUntouchEvent += OnThumbstickUntouchEvent;
            S_InputManager.Instance.RightPrimary2DAxisUntouchEvent += OnThumbstickUntouchEvent;

#if UNITY_EDITOR
            this.gameObject.AddComponent<S_MoveCameraWithMouse>();
            this.gameObject.GetComponent<S_MoveCameraWithMouse>().Target = m_headCamera;
            this.gameObject.GetComponent<S_MoveCameraWithMouse>().Camera = m_targetCamera;
#endif

            if (TeleportType != S_NavigationTypes.JOYSTICK)
            {
                if (this.gameObject.GetComponent<Rigidbody>() != null)
                {
                    this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    this.gameObject.GetComponent<Rigidbody>().useGravity = false;
                }
                if (this.gameObject.GetComponent<Collider>() != null)
                {
                    this.gameObject.GetComponent<Collider>().enabled = false;
                }
            }

            XRInputMapper = new XRInputMapper(this);
        }

        // -------------------------------------------
        /* 
         * OnEnable
         */
        void OnEnable()
        {
        }

        // -------------------------------------------
        /* 
		 * OnDisable
		 */
        void OnDisable()
        {
        }

        // -------------------------------------------
        /* 
		 * OnDestroy
		 */
        void OnDestroy()
        {
            XRInputMapper = null;
            if (S_InputManager.Instance != null)
            {
                S_InputManager.Instance.LeftPrimary2DAxisEvent -= OnStickAxisManageEvent;
                S_InputManager.Instance.RightPrimary2DAxisEvent -= OnStickAxisManageEvent;

                S_InputManager.Instance.OnButtonThumbstickUntouch -= OnThumbstickUntouchEvent;
                S_InputManager.Instance.LeftPrimary2DAxisUntouchEvent -= OnThumbstickUntouchEvent;
                S_InputManager.Instance.RightPrimary2DAxisUntouchEvent -= OnThumbstickUntouchEvent;
            }

            S_HapticsManager.ShutDown();
            XRRig.Instance.ShutDown();
        }

        // -------------------------------------------
        /* 
		 * GetControllers
		 */
        protected void GetControllers()
        {
            S_HandController[] controllers = GetComponentsInChildren<S_HandController>();
            if (controllers != null && controllers.Length > 0)
            {
                for (int i = 0; i < controllers.Length; i++)
                {
                    if (controllers[i].IsLeftHand)
                        m_LeftHandIO = controllers[i].GetComponentInParent<S_LeftHandRoot>().gameObject;
                    else
                        m_RightHandIO = controllers[i].GetComponentInParent<S_RightHandRoot>().gameObject;
                }
            }

            if (!LeftHandActive)
                m_LeftHandIO.SetActive(false);

            if (!RightHandActive)
                m_RightHandIO.SetActive(false);
        }

        // -------------------------------------------
        /* 
		 * OnStickAxisManageEvent
		 */
        private void OnStickAxisManageEvent(S_InputEventArgs _event)
        {
            m_AxisValue = _event.AxisValue;

            if (EnableRotation)
            {
                if (!((RotationJoystickSelected == S_HandsEnum.LEFT) && (_event.InputType == S_InputTypes.RIGHT_PRIMARY_2D_AXIS)) &&
                    !((RotationJoystickSelected == S_HandsEnum.RIGHT) && (_event.InputType == S_InputTypes.LEFT_PRIMARY_2D_AXIS)))
                {
                    if (m_AxisValue.sqrMagnitude > 0.4f)
                    {
                        if (m_applyRotation <= 0)
                        {
                            m_applyRotation += Time.deltaTime;
                            if (m_AxisValue.x < 0)
                            {
                                if (SmoothRotation)
                                {
                                    m_rotationDirection = -1;
                                    m_originalRotationCam = transform.rotation.eulerAngles;
                                }
                                else
                                {
                                    transform.RotateAround(m_targetCamera.position, Vector3.up, -AngleRotationJoystick);
                                }
                            }
                            else
                            {
                                if (SmoothRotation)
                                {
                                    m_rotationDirection = 1;
                                    m_originalRotationCam = transform.rotation.eulerAngles;
                                }
                                else
                                {
                                    transform.RotateAround(m_targetCamera.position, Vector3.up, AngleRotationJoystick);
                                }
                            }
                        }
                    }

                    if (m_AxisValue.sqrMagnitude < 0.1f)
                    {
                        if (!SmoothRotation)
                        {
                            m_applyRotation = 0;
                        }
                    }
                }
            }
        }

        // -------------------------------------------
        /* 
		 * OnStickAxisManageEvent
		 */
        private void OnThumbstickUntouchEvent(S_InputEventArgs _event)
        {
            if (EnableRotation)
            {
                if (((RotationJoystickSelected == S_HandsEnum.LEFT || RotationJoystickSelected == S_HandsEnum.BOTH) && (_event.InputType == S_InputTypes.LEFT_PRIMARY_2D_AXIS_UNTOUCH)) ||
                    ((RotationJoystickSelected == S_HandsEnum.RIGHT || RotationJoystickSelected == S_HandsEnum.BOTH) && (_event.InputType == S_InputTypes.RIGHT_PRIMARY_2D_AXIS_UNTOUCH)))
                {
                    if (!SmoothRotation)
                    {
                        m_applyRotation = 0;
                    }                        
                }
            }
        }

        private void CheckControllers()
        {
            if (XRInputMapper != null)
                m_ControllersConnected = XRInputMapper.CheckIfControllerConnected();

            if (UnpluggedControllerMessage != null)
            {
                if (!m_ControllersConnected)
                {
                    if (!UnpluggedControllerMessage.activeSelf)
                        UnpluggedControllerMessage.SetActive(true);
                }
                else
                {
                    if (UnpluggedControllerMessage.activeSelf)
                        UnpluggedControllerMessage.SetActive(false);
                }
            }
        }

        // -------------------------------------------
        /* 
		 * CheckAxisForMovement
		 */
        private void CheckAxisForMovement()
        {
            if (TeleportType == S_NavigationTypes.JOYSTICK)
            {
                if (XRInputMapper != null)
                {
                    Vector2 currentAxis = new Vector2();
                    if (NavigationJoystickSelected == S_HandsEnum.LEFT)
                    {
                        currentAxis = XRInputMapper.LeftAxisStick();
                    }
                    if (NavigationJoystickSelected == S_HandsEnum.RIGHT)
                    {
                        currentAxis = XRInputMapper.RightAxisStick();
                    }
                    if (NavigationJoystickSelected == S_HandsEnum.BOTH)
                    {
                        currentAxis = XRInputMapper.RightAxisStick() + XRInputMapper.LeftAxisStick();
                    }

                    if (!currentAxis.Equals(Vector2.zero))
                    {
                        Vector3 forwardCam = S_CloningUtils.Clone(m_targetCamera.forward);
                        forwardCam.y = 0;
                        Vector3 rightCam = S_CloningUtils.Clone(m_targetCamera.right);
                        rightCam.y = 0;

                        Vector3 forward = currentAxis.y * forwardCam * SpeedJoystick * Time.deltaTime;
                        Vector3 lateral = currentAxis.x * rightCam * SpeedJoystick * Time.deltaTime;

                        Vector3 increment = forward + lateral;
                        Vector3 targetDestination = m_headCamera.transform.position + increment;
                        m_headCamera.transform.position = targetDestination;
                    }
                }
            }
        }

        // -------------------------------------------
        /* 
         * Update
         */
        protected virtual void Update()
        {
            if (XRInputMapper != null) XRInputMapper.Logic();

            if (SmoothRotation)
            {
                if (m_applyRotation > 0)
                {
                    float finalAngle = m_rotationDirection * AngleRotationJoystick;
                    m_applyRotation += Time.deltaTime;
                    if (m_applyRotation < RotationTimeJoystick)
                    {
                        float percentageToComplete = (m_applyRotation / RotationTimeJoystick);
                        transform.RotateAround(m_targetCamera.position, Vector3.up, m_rotationDirection * (finalAngle / RotationTimeJoystick) * Time.deltaTime);
                    }
                    else
                    {
                        m_applyRotation = 0;
                    }
                }
            }
        }

        // -------------------------------------------
        /* 
        * FixedUpdate
        */
        void FixedUpdate()
        {
            if (Time.time > m_LastUpdateTime + UpdateInterval)
            {
                CheckControllers();

                // update last time value
                m_LastUpdateTime = Time.time;
            }

            CheckAxisForMovement();
        }
    }
}
