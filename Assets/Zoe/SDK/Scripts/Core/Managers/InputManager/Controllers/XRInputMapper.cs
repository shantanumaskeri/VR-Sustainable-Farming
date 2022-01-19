using UnityEngine;
using UnityEngine.XR;

namespace SpatialStories
{
    public class XRInputMapper
    {
        // This value is set arbitrarily set. It wasn't used before but it seems
        // controllers when using the quest send an axis value for the joysticks
        // that is slightly above 0.0
        // Update (11.09.20): Increasing the value from 0.01f to 0.1f due to the HP
        // WMR sending a non-negligible value. If it ever causes issues with other
        // controllers, make this value specific to WMR
        public const float AXIS_TOLERANCE = 0.1f;
        public const float TRIGGER_SENSIBILITY = 0.8f;

        public const float STICK_TRIGGER_SENSIBILITY = 0.2f;

        private S_InputManager m_InputManager;

        public XRInputMapper(S_InputManager _inputManager)
        {
            m_InputManager = _inputManager;
        }

        public bool CheckIfControllerConnected()
        {
            return XRRig.Instance.LeftController.isValid && XRRig.Instance.RightController.isValid;
        }

        private void LogicButtons()
        {
            LogicButtonTouch();
            LogicButtonPress();
        }

        private bool m_RightPrimaryButtonTouched;
        private bool m_RightSecondaryButtonTouched;
        private bool m_LeftPrimaryButtonTouched;
        private bool m_LeftSecondaryButtonTouched;

        private void LogicButtonTouch()
        {
            // Left primary button touch
            bool leftPrimraryButtonTouchedThisFrame = false;
            XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.primaryTouch, out leftPrimraryButtonTouchedThisFrame);

            if (leftPrimraryButtonTouchedThisFrame && !m_LeftPrimaryButtonTouched)
            {
                S_InputManager.Instance.FireLeftPrimaryButtonTouchEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_BUTTON_TOUCH));
            }

            if (!leftPrimraryButtonTouchedThisFrame && m_LeftPrimaryButtonTouched)
            {
                S_InputManager.Instance.FireLeftPrimaryButtonUntouchEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_BUTTON_UNTOUCH));
            }

            m_LeftPrimaryButtonTouched = leftPrimraryButtonTouchedThisFrame;

            // Left secondary button touch
            bool leftSecondaryButtonTouchedThisFrame = false;
            XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.secondaryTouch, out leftSecondaryButtonTouchedThisFrame);

            if (leftSecondaryButtonTouchedThisFrame && !m_LeftSecondaryButtonTouched)
            {
                S_InputManager.Instance.FireLeftSecondaryButtonTouchEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_SECONDARY_BUTTON_TOUCH));
            }

            if (!leftSecondaryButtonTouchedThisFrame && m_LeftSecondaryButtonTouched)
            {
                S_InputManager.Instance.FireLeftSecondaryButtonUntouchEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_SECONDARY_BUTTON_UNTOUCH));
            }

            m_LeftSecondaryButtonTouched = leftSecondaryButtonTouchedThisFrame;

            // Right primrary button touch
            bool rightPrimaryButtonTouchedThisFrame = false;
            XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.primaryButton, out rightPrimaryButtonTouchedThisFrame);

            if (rightPrimaryButtonTouchedThisFrame && !m_RightPrimaryButtonTouched)
            {
                S_InputManager.Instance.FireRightPrimaryButtonTouchEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_BUTTON_TOUCH));
            }

            if (!rightPrimaryButtonTouchedThisFrame && m_RightPrimaryButtonTouched)
            {
                S_InputManager.Instance.FireRightPrimaryButtonUntouchEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_BUTTON_UNTOUCH));
            }

            m_RightPrimaryButtonTouched = rightPrimaryButtonTouchedThisFrame;

            // Right secondary button touch
            bool rightSecondaryButtonTouchedThisFrame = false;
            XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.secondaryTouch, out rightSecondaryButtonTouchedThisFrame);

            if (rightSecondaryButtonTouchedThisFrame && !m_RightSecondaryButtonTouched)
            {
                S_InputManager.Instance.FireRightSecondaryButtonTouchEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_SECONDARY_BUTTON_TOUCH));
            }

            if (!rightSecondaryButtonTouchedThisFrame && m_RightSecondaryButtonTouched)
            {
                S_InputManager.Instance.FireRightSecondaryButtonUntouchEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_SECONDARY_BUTTON_UNTOUCH));
            }

            m_RightSecondaryButtonTouched = rightSecondaryButtonTouchedThisFrame;
        }

        private bool m_RightPrimaryButtonPressed;
        private bool m_RightSecondaryButtonPressed;
        private bool m_LeftPrimaryButtonPressed;
        private bool m_LeftSecondaryButtonPressed;
        private bool m_LeftMenuButtonPressed;
        private bool m_RightMenuButtonPressed;

        private void LogicButtonPress()
        {
            // Left primary button press
            bool leftPrimraryButtonPressedThisFrame = false;
            XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.primaryButton, out leftPrimraryButtonPressedThisFrame);

            if (leftPrimraryButtonPressedThisFrame && !m_LeftPrimaryButtonPressed)
            {
                S_InputManager.Instance.FireLeftPrimaryButtonPressEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_BUTTON_PRESS));
            }

            if (!leftPrimraryButtonPressedThisFrame && m_LeftPrimaryButtonPressed)
            {
                S_InputManager.Instance.FireLeftPrimaryButtonReleaseEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_BUTTON_RELEASE));
            }

            m_LeftPrimaryButtonPressed = leftPrimraryButtonPressedThisFrame;

            // Left secondary button press
            bool leftSecondaryButtonPressedThisFrame = false;
            XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.secondaryButton, out leftSecondaryButtonPressedThisFrame);

            if (leftSecondaryButtonPressedThisFrame && !m_LeftSecondaryButtonPressed)
            {
                S_InputManager.Instance.FireLeftSecondaryButtonPressEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_SECONDARY_BUTTON_PRESS));
            }

            if (!leftSecondaryButtonPressedThisFrame && m_LeftSecondaryButtonPressed)
            {
                S_InputManager.Instance.FireLeftSecondaryButtonReleaseEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_SECONDARY_BUTTON_RELEASE));
            }

            m_LeftSecondaryButtonPressed = leftSecondaryButtonPressedThisFrame;

            // Right primrary button press
            bool rightPrimaryButtonPressedThisFrame = false;
            XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.primaryButton, out rightPrimaryButtonPressedThisFrame);

            if (rightPrimaryButtonPressedThisFrame && !m_RightPrimaryButtonPressed)
            {
                S_InputManager.Instance.FireRightPrimaryButtonPressEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_BUTTON_PRESS));
            }

            if (!rightPrimaryButtonPressedThisFrame && m_RightPrimaryButtonPressed)
            {
                S_InputManager.Instance.FireRightPrimaryButtonReleaseEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_BUTTON_RELEASE));
            }

            m_RightPrimaryButtonPressed = rightPrimaryButtonPressedThisFrame;

            // Right secondary button press
            bool rightSecondaryButtonPressedThisFrame = false;
            XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.secondaryButton, out rightSecondaryButtonPressedThisFrame);

            if (rightSecondaryButtonPressedThisFrame && !m_RightSecondaryButtonPressed)
            {
                S_InputManager.Instance.FireRightSecondaryButtonPressEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_SECONDARY_BUTTON_PRESS));
            }

            if (!rightSecondaryButtonPressedThisFrame && m_RightSecondaryButtonPressed)
            {
                S_InputManager.Instance.FireRightSecondaryButtonReleaseEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_SECONDARY_BUTTON_RELEASE));
            }

            m_RightSecondaryButtonPressed = rightSecondaryButtonPressedThisFrame;

            // Left Menu button press
            bool leftMenuButtonPressedThisFrame = false;
            XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.menuButton, out leftMenuButtonPressedThisFrame);

            if (leftMenuButtonPressedThisFrame && !m_LeftMenuButtonPressed)
            {
                switch (XRRig.Instance.Type)
                {
                    case XRRigType.ViveFocusPlus:
                    case XRRigType.VivePro:
                    case XRRigType.WindowsMixedReality:
                        S_InputManager.Instance.FireLeftPrimaryButtonPressEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_BUTTON_PRESS));
                        break;
                    default:
                        S_InputManager.Instance.FireMenuButtonPressEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.MENU_BUTTON));
                        break;
                }
            }

            if (!leftMenuButtonPressedThisFrame && m_LeftMenuButtonPressed)
            {
                switch (XRRig.Instance.Type)
                {
                    case XRRigType.ViveFocusPlus:
                    case XRRigType.VivePro:
                    case XRRigType.WindowsMixedReality:
                        S_InputManager.Instance.FireLeftPrimaryButtonReleaseEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_BUTTON_RELEASE));
                        break;
                    default:
                        S_InputManager.Instance.FireMenuButtonReleaseEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.MENU_BUTTON));
                        break;
                }
            }

            m_LeftMenuButtonPressed = leftMenuButtonPressedThisFrame;

            // Right Menu button press
            bool rightMenuButtonPressedThisFrame = false;
            XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.menuButton, out rightMenuButtonPressedThisFrame);

            if (rightMenuButtonPressedThisFrame && !m_RightMenuButtonPressed)
            {
                switch (XRRig.Instance.Type)
                {
                    case XRRigType.ViveFocusPlus:
                    case XRRigType.VivePro:
                    case XRRigType.WindowsMixedReality:
                        S_InputManager.Instance.FireRightPrimaryButtonPressEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_BUTTON_PRESS));
                        break;
                }
            }

            if (!rightMenuButtonPressedThisFrame && m_RightMenuButtonPressed)
            {
                switch (XRRig.Instance.Type)
                {
                    case XRRigType.ViveFocusPlus:
                    case XRRigType.VivePro:
                    case XRRigType.WindowsMixedReality:
                        S_InputManager.Instance.FireRightPrimaryButtonReleaseEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_BUTTON_RELEASE));
                        break;
                }
            }

            m_RightMenuButtonPressed = rightMenuButtonPressedThisFrame;
        }

        private bool m_RightTriggerButtonPressed;
        private bool m_LeftTriggerButtonPressed;

        private void LogicTrigger()
        {
            // Right trigger button press
            bool rightTriggerButtonPressedThisFrame = false;
            XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightTriggerButtonPressedThisFrame);

            if (rightTriggerButtonPressedThisFrame && !m_RightTriggerButtonPressed)
            {
                S_InputManager.Instance.FireRightTriggerButtonPressEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_TRIGGER_BUTTON_PRESS));
            }

            if (!rightTriggerButtonPressedThisFrame && m_RightTriggerButtonPressed)
            {
                S_InputManager.Instance.FireRightTriggerButtonReleaseEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE));
            }

            m_RightTriggerButtonPressed = rightTriggerButtonPressedThisFrame;

            // Left trigger button press
            bool leftTriggerButtonPressedThisFrame = false;
            XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftTriggerButtonPressedThisFrame);

            if (leftTriggerButtonPressedThisFrame && !m_LeftTriggerButtonPressed)
            {
                S_InputManager.Instance.FireLeftTriggerButtonPressEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_TRIGGER_BUTTON_PRESS));
            }

            if (!leftTriggerButtonPressedThisFrame && m_LeftTriggerButtonPressed)
            {
                S_InputManager.Instance.FireLeftTriggerButtonReleaseEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_TRIGGER_BUTTON_RELEASE));
            }

            m_LeftTriggerButtonPressed = leftTriggerButtonPressedThisFrame;
        }

        private bool m_RightGripButtonPressed;
        private bool m_LeftGripButtonPressed;

        private void LogicGrip()
        {
            // Right grip button press
            bool rightGripButtonPressedThisFrame = false;
            XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGripButtonPressedThisFrame);

            if (rightGripButtonPressedThisFrame && !m_RightGripButtonPressed)
            {
                S_InputManager.Instance.FireRightGripButtonPressEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_GRIP_BUTTON_PRESS));
            }

            if (!rightGripButtonPressedThisFrame && m_RightGripButtonPressed)
            {
                S_InputManager.Instance.FireRightGripButtonReleaseEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE));
            }

            m_RightGripButtonPressed = rightGripButtonPressedThisFrame;

            // Left grip button press
            bool leftGripButtonPressedThisFrame = false;
            XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGripButtonPressedThisFrame);

            if (leftGripButtonPressedThisFrame && !m_LeftGripButtonPressed)
            {
                S_InputManager.Instance.FireLeftGripButtonPressEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_GRIP_BUTTON_PRESS));
            }

            if (!leftGripButtonPressedThisFrame && m_LeftGripButtonPressed)
            {
                S_InputManager.Instance.FireLeftGripButtonReleaseEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_GRIP_BUTTON_RELEASE));
            }

            m_LeftGripButtonPressed = leftGripButtonPressedThisFrame;
        }

        private Vector2 m_AxisValueRight;
        private Vector2 m_AxisValueLeft;
        private bool m_LeftPrimary2DAxisLeftTilt;
        private bool m_LeftPrimary2DAxisRightTilt;
        private bool m_LeftPrimary2DAxisUpTilt;
        private bool m_LeftPrimary2DAxisDownTilt;
        private bool m_RightPrimary2DAxisLeftTilt;
        private bool m_RightPrimary2DAxisRightTilt;
        private bool m_RightPrimary2DAxisUpTilt;
        private bool m_RightPrimary2DAxisDownTilt;

        private void LogicPrimary2DAxis()
        {
            LogicPrimary2DAxisTouch();
            LogicPrimary2DAxisPress();

            // RIGHT STICK
            Vector2 axisValueRight = Vector2.zero;
            switch (XRRig.Instance.Type)
            {
                case XRRigType.VivePro:
                case XRRigType.ViveFocusPlus:
                    // If we are on a trackpad based family of hardware
                    // make sure that primary2DAxis events are queried and sent
                    // only if the trackpad is pressed
                    if (m_RightPrimary2DAxisButtonPressed)
                    {
                        XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisValueRight);
                    }
                    break;
                case XRRigType.WindowsMixedReality:
                    XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.secondary2DAxis, out axisValueRight);
                    break;
                default:
                    XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisValueRight);
                    break;
            }

            if (axisValueRight.magnitude >= AXIS_TOLERANCE)
            {
                S_InputManager.Instance.FireRightPrimary2DAxisEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS, axisValueRight));
            }

            if (m_AxisValueRight.magnitude >= AXIS_TOLERANCE && axisValueRight.magnitude < AXIS_TOLERANCE)
            {
                S_InputManager.Instance.FireRightPrimary2DAxisReleaseEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_RELEASE));
            }

            m_AxisValueRight = axisValueRight;

            bool rightPrimary2DAxisRightTilt = false;
            if (axisValueRight.x > 0.5f)
            {
                rightPrimary2DAxisRightTilt = true;
                S_InputManager.Instance.FireRightPrimary2DAxisRightTiltEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_RIGHT_TILT));
            }

            if (m_RightPrimary2DAxisRightTilt && !rightPrimary2DAxisRightTilt)
            {
                S_InputManager.Instance.FireRightPrimary2DAxisRightTiltReleaseEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_RIGHT_TILT_RELEASE));
            }

            m_RightPrimary2DAxisRightTilt = rightPrimary2DAxisRightTilt;

            bool rightPrimary2DAxisLeftTilt = false;
            if (axisValueRight.x < -0.5f)
            {
                rightPrimary2DAxisLeftTilt = true;
                S_InputManager.Instance.FireRightPrimary2DAxisLeftTiltEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_LEFT_TILT));
            }

            if (m_RightPrimary2DAxisLeftTilt && !rightPrimary2DAxisLeftTilt)
            {
                S_InputManager.Instance.FireRightPrimary2DAxisLeftTiltReleaseEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_LEFT_TILT_RELEASE));
            }

            m_RightPrimary2DAxisLeftTilt = rightPrimary2DAxisLeftTilt;

            bool rightPrimary2DAxisUpTilt = false;
            if (axisValueRight.y > 0.5f)
            {
                rightPrimary2DAxisUpTilt = true;
                S_InputManager.Instance.FireRightPrimary2DAxisUpTiltEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_UP_TILT));
            }

            if (m_RightPrimary2DAxisUpTilt && !rightPrimary2DAxisUpTilt)
            {
                S_InputManager.Instance.FireRightPrimary2DAxisUpTiltReleaseEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_UP_TILT_RELEASE));
            }

            m_RightPrimary2DAxisUpTilt = rightPrimary2DAxisUpTilt;

            bool rightPrimary2DAxisDownTilt = false;
            if (axisValueRight.y < -0.5f)
            {
                rightPrimary2DAxisDownTilt = true;
                S_InputManager.Instance.FireRightPrimary2DAxisDownTiltEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_DOWN_TILT));
            }

            if (m_RightPrimary2DAxisDownTilt && !rightPrimary2DAxisDownTilt)
            {
                S_InputManager.Instance.FireRightPrimary2DAxisDownTiltReleaseEvent(new S_InputEventArgs(this, XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_DOWN_TILT_RELEASE));
            }

            m_RightPrimary2DAxisDownTilt = rightPrimary2DAxisDownTilt;

            // LEFT STICK
            Vector2 axisValueLeft = Vector2.zero;
            switch (XRRig.Instance.Type)
            {
                case XRRigType.VivePro:
                case XRRigType.ViveFocusPlus:
                    // If we are on a trackpad based family of hardware
                    // make sure that primary2DAxis events are queried and sent
                    // only if the trackpad is pressed
                    if (m_LeftPrimary2DAxisButtonPressed)
                    {
                        XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisValueLeft);
                    }
                    break;
                case XRRigType.WindowsMixedReality:
                    XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.secondary2DAxis, out axisValueLeft);
                    break;
                default:
                    XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisValueLeft);
                    break;
            }

            if (axisValueLeft.magnitude >= AXIS_TOLERANCE)
            {
                S_InputManager.Instance.FireLeftPrimary2DAxisEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS, axisValueLeft));
            }

            if (m_AxisValueLeft.magnitude >= AXIS_TOLERANCE && axisValueLeft.magnitude < AXIS_TOLERANCE)
            {
                S_InputManager.Instance.FireLeftPrimary2DAxisReleaseEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_RELEASE));
            }

            m_AxisValueLeft = axisValueLeft;

            bool leftPrimary2DAxisRightTilt = false;
            if (axisValueLeft.x > 0.5f)
            {
                leftPrimary2DAxisRightTilt = true;
                S_InputManager.Instance.FireLeftPrimary2DAxisRightTiltEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_RIGHT_TILT));
            }

            if (m_LeftPrimary2DAxisRightTilt && !leftPrimary2DAxisRightTilt)
            {
                S_InputManager.Instance.FireLeftPrimary2DAxisRightTiltReleaseEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_RIGHT_TILT_RELEASE));
            }

            m_LeftPrimary2DAxisRightTilt = leftPrimary2DAxisRightTilt;

            bool leftPrimary2DAxisLeftTilt = false;
            if (axisValueLeft.x < -0.5f)
            {
                leftPrimary2DAxisLeftTilt = true;
                S_InputManager.Instance.FireLeftPrimary2DAxisLeftTiltEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_LEFT_TILT));
            }

            if (m_LeftPrimary2DAxisLeftTilt && !leftPrimary2DAxisLeftTilt)
            {
                S_InputManager.Instance.FireLeftPrimary2DAxisLeftTiltReleaseEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_LEFT_TILT_RELEASE));
            }

            m_LeftPrimary2DAxisLeftTilt = leftPrimary2DAxisLeftTilt;

            bool leftPrimary2DAxisUpTilt = false;
            if (axisValueLeft.y > 0.5f)
            {
                leftPrimary2DAxisUpTilt = true;
                S_InputManager.Instance.FireLeftPrimary2DAxisUpTiltEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_UP_TILT));
            }

            if (m_LeftPrimary2DAxisUpTilt && !leftPrimary2DAxisUpTilt)
            {
                S_InputManager.Instance.FireLeftPrimary2DAxisUpTiltReleaseEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_UP_TILT_RELEASE));
            }

            m_LeftPrimary2DAxisUpTilt = leftPrimary2DAxisUpTilt;

            bool leftPrimary2DAxisDownTilt = false;
            if (axisValueLeft.y < -0.5f)
            {
                leftPrimary2DAxisDownTilt = true;
                S_InputManager.Instance.FireLeftPrimary2DAxisDownTiltEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_DOWN_TILT));
            }

            if (m_LeftPrimary2DAxisDownTilt && !leftPrimary2DAxisDownTilt)
            {
                S_InputManager.Instance.FireLeftPrimary2DAxisDownTiltReleaseEvent(new S_InputEventArgs(this, XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_DOWN_TILT_RELEASE));
            }

            m_LeftPrimary2DAxisDownTilt = leftPrimary2DAxisDownTilt;
        }

        private bool m_RightPrimary2DAxisTouched;
        private bool m_LeftPrimary2DAxisTouched;

        private void LogicPrimary2DAxisTouch()
        {
            // Right primary 2D Axis touch
            bool rightPrimary2DAxisTouchedThisFrame = false;
            XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out rightPrimary2DAxisTouchedThisFrame);

            if (rightPrimary2DAxisTouchedThisFrame && !m_RightPrimary2DAxisTouched)
            {
                S_InputManager.Instance.FireRightPrimary2DAxisTouchEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_TOUCH));
            }

            if (!rightPrimary2DAxisTouchedThisFrame && m_RightPrimary2DAxisTouched)
            {
                S_InputManager.Instance.FireRightPrimary2DAxisUntouchEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS_UNTOUCH));
            }

            m_RightPrimary2DAxisTouched = rightPrimary2DAxisTouchedThisFrame;

            // Left primary 2D Axis touch
            bool leftPrimary2DAxisTouchedThisFrame = false;
            XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out leftPrimary2DAxisTouchedThisFrame);

            if (leftPrimary2DAxisTouchedThisFrame && !m_LeftPrimary2DAxisTouched)
            {
                S_InputManager.Instance.FireLeftPrimary2DAxisTouchEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_TOUCH));
            }

            if (!leftPrimary2DAxisTouchedThisFrame && m_LeftPrimary2DAxisTouched)
            {
                S_InputManager.Instance.FireLeftPrimary2DAxisUntouchEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS_UNTOUCH));
            }

            m_LeftPrimary2DAxisTouched = leftPrimary2DAxisTouchedThisFrame;
        }

        private bool m_RightPrimary2DAxisButtonPressed;
        private bool m_LeftPrimary2DAxisButtonPressed;

        private void LogicPrimary2DAxisPress()
        {
            // Right primary 2D Axis press
            bool rightPrimary2DAxisPressedThisFrame = false;
            XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out rightPrimary2DAxisPressedThisFrame);

            if (rightPrimary2DAxisPressedThisFrame && !m_RightPrimary2DAxisButtonPressed)
            {
                S_InputManager.Instance.FireRightPrimary2DAxisButtonPressEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS));
            }

            if (!rightPrimary2DAxisPressedThisFrame && m_RightPrimary2DAxisButtonPressed)
            {
                S_InputManager.Instance.FireRightPrimary2DAxisButtonReleaseEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_PRIMARY_2D_AXIS));
            }

            m_RightPrimary2DAxisButtonPressed = rightPrimary2DAxisPressedThisFrame;

            // Left primary 2D Axis press
            bool leftPrimary2DAxisPressedThisFrame = false;
            XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out leftPrimary2DAxisPressedThisFrame);

            if (leftPrimary2DAxisPressedThisFrame && !m_LeftPrimary2DAxisButtonPressed)
            {
                S_InputManager.Instance.FireLeftPrimary2DAxisButtonPressEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS));
            }

            if (!leftPrimary2DAxisPressedThisFrame && m_LeftPrimary2DAxisButtonPressed)
            {
                S_InputManager.Instance.FireLeftPrimary2DAxisButtonReleaseEvent(new S_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, S_InputTypes.LEFT_PRIMARY_2D_AXIS));
            }

            m_LeftPrimary2DAxisButtonPressed = leftPrimary2DAxisPressedThisFrame;
        }

        public Vector2 LeftAxisStick()
        {
            Vector2 axisValueLeft;
            switch (XRRig.Instance.Type)
            {
                case XRRigType.WindowsMixedReality:
                    XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.secondary2DAxis, out axisValueLeft);
                    break;
                default:
                    XRRig.Instance.LeftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisValueLeft);
                    break;
            }

            if (!m_LeftPrimary2DAxisButtonPressed &&
                (XRRig.Instance.Type == XRRigType.VivePro ||
                XRRig.Instance.Type == XRRigType.ViveFocusPlus))
            {
                return Vector2.zero;
            }

            if (axisValueLeft.magnitude < AXIS_TOLERANCE)
            {
                return Vector2.zero;
            }

            return axisValueLeft;
        }

        public Vector2 RightAxisStick()
        {
            Vector2 axisValueRight;
            switch (XRRig.Instance.Type)
            {
                case XRRigType.WindowsMixedReality:
                    XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.secondary2DAxis, out axisValueRight);
                    break;
                default:
                    XRRig.Instance.RightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisValueRight);
                    break;
            }

            if (!m_RightPrimary2DAxisButtonPressed &&
                (XRRig.Instance.Type == XRRigType.VivePro ||
                XRRig.Instance.Type == XRRigType.ViveFocusPlus))
            {
                return Vector2.zero;
            }

            if (axisValueRight.magnitude < AXIS_TOLERANCE)
            {
                return Vector2.zero;
            }

            return axisValueRight;
        }

        public void Logic()
        {
            LogicButtons();
            LogicTrigger();
            LogicGrip();
            LogicPrimary2DAxis();
        }
    }
}


