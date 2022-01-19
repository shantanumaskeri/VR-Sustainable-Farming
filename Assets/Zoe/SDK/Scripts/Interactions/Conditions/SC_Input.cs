using System;
using System.Collections.Generic;
using Gaze;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SC_Input")]
    public class SC_Input : S_AbstractCondition
    {
        public bool RequireAllInputs = false;
        public S_InputsMap InputsMap = new S_InputsMap();

        private List<S_InputsMapEntry> m_currentInputsEntries = new List<S_InputsMapEntry>();

        public SC_Input()
        {
        }

        public override void Setup()
        {
            if (!m_listenersSetUp && !m_hasBeenDestroyed)
            {
                m_listenersSetUp = true;

                // KEY DOWN
                S_InputManager.Instance.MenuButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimaryButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.RightSecondaryButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimaryButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftSecondaryButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.LeftTriggerButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.RightTriggerButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.LeftGripButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.RightSecondaryButtonTouchEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimaryButtonTouchEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimaryButtonTouchEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftSecondaryButtonTouchEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.OnButtonLeftIndexTouch += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.OnButtonLeftThumbrestTouch += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisTouchEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.RightPrimary2DAxisTouchEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.RightGripButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.LeftPrimary2DAxisEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.RightPrimary2DAxisEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimary2DAxisButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.LeftPrimary2DAxisLeftTiltEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisRightTiltEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisUpTiltEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisDownTiltEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.RightPrimary2DAxisLeftTiltEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimary2DAxisRightTiltEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimary2DAxisUpTiltEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimary2DAxisDownTiltEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.LeftPrimary2DAxisLeftTiltReleaseEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisRightTiltReleaseEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisUpTiltReleaseEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisDownTiltReleaseEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.RightPrimary2DAxisLeftTiltReleaseEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimary2DAxisRightTiltReleaseEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimary2DAxisUpTiltReleaseEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimary2DAxisDownTiltReleaseEvent += new S_InputManager.InputEvent(OnInputDownEvent);

                // KEY UP
                S_InputManager.Instance.RightPrimaryButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);

                S_InputManager.Instance.RightSecondaryButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);
                S_InputManager.Instance.LeftPrimaryButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);
                S_InputManager.Instance.LeftSecondaryButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);

                S_InputManager.Instance.LeftTriggerButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);

                S_InputManager.Instance.RightTriggerButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);

                S_InputManager.Instance.LeftGripButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);

                S_InputManager.Instance.RightSecondaryButtonUntouchEvent += new S_InputManager.InputEvent(OnInputUpEvent);
                S_InputManager.Instance.RightPrimaryButtonUntouchEvent += new S_InputManager.InputEvent(OnInputUpEvent);
                S_InputManager.Instance.LeftPrimaryButtonUntouchEvent += new S_InputManager.InputEvent(OnInputUpEvent);
                S_InputManager.Instance.LeftSecondaryButtonUntouchEvent += new S_InputManager.InputEvent(OnInputUpEvent);

                S_InputManager.Instance.OnButtonLeftIndexUntouch += new S_InputManager.InputEvent(OnInputUpEvent);
                S_InputManager.Instance.OnButtonLeftThumbrestUntouch += new S_InputManager.InputEvent(OnInputUpEvent);
                S_InputManager.Instance.LeftPrimary2DAxisUntouchEvent += new S_InputManager.InputEvent(OnInputUpEvent);

                S_InputManager.Instance.RightPrimary2DAxisUntouchEvent += new S_InputManager.InputEvent(OnInputUpEvent);

                S_InputManager.Instance.RightGripButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);

                S_InputManager.Instance.LeftPrimary2DAxisButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);

                S_InputManager.Instance.RightPrimary2DAxisButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);
            }
        }

        private void OnInputKeepPressedEvent(S_InputEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            if (m_listenersSetUp && !m_hasBeenDestroyed)
            {
                m_listenersSetUp = false;

                // KEY DOWN
                S_InputManager.Instance.MenuButtonPressEvent -= OnInputDownEvent;
                S_InputManager.Instance.RightPrimaryButtonPressEvent -= OnInputDownEvent;

                S_InputManager.Instance.RightSecondaryButtonPressEvent -= OnInputDownEvent;
                S_InputManager.Instance.LeftPrimaryButtonPressEvent -= OnInputDownEvent;
                S_InputManager.Instance.LeftSecondaryButtonPressEvent -= OnInputDownEvent;

                S_InputManager.Instance.LeftTriggerButtonPressEvent -= OnInputDownEvent;

                S_InputManager.Instance.RightTriggerButtonPressEvent -= OnInputDownEvent;

                S_InputManager.Instance.LeftGripButtonPressEvent -= OnInputDownEvent;

                S_InputManager.Instance.RightSecondaryButtonTouchEvent -= OnInputDownEvent;
                S_InputManager.Instance.RightPrimaryButtonTouchEvent -= OnInputDownEvent;
                S_InputManager.Instance.LeftPrimaryButtonTouchEvent -= OnInputDownEvent;
                S_InputManager.Instance.LeftSecondaryButtonTouchEvent -= OnInputDownEvent;

                S_InputManager.Instance.OnButtonLeftIndexTouch -= OnInputDownEvent;
                S_InputManager.Instance.OnButtonLeftThumbrestTouch -= OnInputDownEvent;
                S_InputManager.Instance.LeftPrimary2DAxisTouchEvent -= OnInputDownEvent;

                S_InputManager.Instance.RightPrimary2DAxisTouchEvent -= OnInputDownEvent;

                S_InputManager.Instance.RightGripButtonPressEvent -= OnInputDownEvent;

                S_InputManager.Instance.LeftPrimary2DAxisEvent -= OnInputDownEvent;
                S_InputManager.Instance.LeftPrimary2DAxisButtonPressEvent -= OnInputDownEvent;

                S_InputManager.Instance.RightPrimary2DAxisEvent -= OnInputDownEvent;
                S_InputManager.Instance.RightPrimary2DAxisButtonPressEvent -= OnInputDownEvent;

                S_InputManager.Instance.LeftPrimary2DAxisLeftTiltEvent -= OnInputDownEvent;
                S_InputManager.Instance.LeftPrimary2DAxisRightTiltEvent -= OnInputDownEvent;
                S_InputManager.Instance.LeftPrimary2DAxisUpTiltEvent -= OnInputDownEvent;
                S_InputManager.Instance.LeftPrimary2DAxisDownTiltEvent -= OnInputDownEvent;

                S_InputManager.Instance.RightPrimary2DAxisLeftTiltEvent -= OnInputDownEvent;
                S_InputManager.Instance.RightPrimary2DAxisRightTiltEvent -= OnInputDownEvent;
                S_InputManager.Instance.RightPrimary2DAxisUpTiltEvent -= OnInputDownEvent;
                S_InputManager.Instance.RightPrimary2DAxisDownTiltEvent -= OnInputDownEvent;

                S_InputManager.Instance.LeftPrimary2DAxisLeftTiltReleaseEvent -= new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisRightTiltReleaseEvent -= new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisUpTiltReleaseEvent -= new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftPrimary2DAxisDownTiltReleaseEvent -= new S_InputManager.InputEvent(OnInputDownEvent);

                S_InputManager.Instance.RightPrimary2DAxisLeftTiltReleaseEvent -= new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimary2DAxisRightTiltReleaseEvent -= new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimary2DAxisUpTiltReleaseEvent -= new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightPrimary2DAxisDownTiltReleaseEvent -= new S_InputManager.InputEvent(OnInputDownEvent);

                // KEY UP
                S_InputManager.Instance.RightPrimaryButtonReleaseEvent -= OnInputUpEvent;

                S_InputManager.Instance.RightSecondaryButtonReleaseEvent -= OnInputUpEvent;
                S_InputManager.Instance.LeftPrimaryButtonReleaseEvent -= OnInputUpEvent;
                S_InputManager.Instance.LeftSecondaryButtonReleaseEvent -= OnInputUpEvent;

                S_InputManager.Instance.LeftTriggerButtonReleaseEvent -= OnInputUpEvent;

                S_InputManager.Instance.RightTriggerButtonReleaseEvent -= OnInputUpEvent;

                S_InputManager.Instance.LeftGripButtonReleaseEvent -= OnInputUpEvent;

                S_InputManager.Instance.RightSecondaryButtonUntouchEvent -= OnInputUpEvent;
                S_InputManager.Instance.RightPrimaryButtonUntouchEvent -= OnInputUpEvent;
                S_InputManager.Instance.LeftPrimaryButtonUntouchEvent -= OnInputUpEvent;
                S_InputManager.Instance.LeftSecondaryButtonUntouchEvent -= OnInputUpEvent;

                S_InputManager.Instance.OnButtonLeftIndexUntouch -= OnInputUpEvent;
                S_InputManager.Instance.OnButtonLeftThumbrestUntouch -= OnInputUpEvent;
                S_InputManager.Instance.LeftPrimary2DAxisUntouchEvent -= OnInputUpEvent;

                S_InputManager.Instance.RightPrimary2DAxisUntouchEvent -= OnInputUpEvent;

                S_InputManager.Instance.RightGripButtonReleaseEvent -= OnInputUpEvent;

                S_InputManager.Instance.LeftPrimary2DAxisButtonReleaseEvent -= OnInputUpEvent;

                S_InputManager.Instance.RightPrimary2DAxisButtonReleaseEvent -= OnInputUpEvent;

            }
        }

        public override void OnDestroy()
        {
            m_listenersSetUp = true;
            Dispose();
            m_hasBeenDestroyed = true;
        }

        protected void OnInputDownEvent(S_InputEventArgs _event)
        {
            if (this == null) return;
            if (IsValid) return;

            CheckInputs(_event);
        }

        protected void OnInputUpEvent(S_InputEventArgs _event)
        {
            if (IsValid) return;

            CheckInputs(_event);
        }

        private void CheckInputs(S_InputEventArgs _event)
        {
            foreach (S_InputsMapEntry input in InputsMap.InputsEntries)
            {
                if (input.InputType == _event.InputType &&
                    input.UISelectedPlatform == XRRigTypeToControllerType(XRRig.Instance.Type))
                {
                    if (RequireAllInputs)
                    {
                        m_currentInputsEntries.Add(input);
                    }
                    else
                    {
                        CheckValidation(true);
                    }
                }
            }

            foreach (S_InputsMapEntry input in InputsMap.InputsEntries)
            {
                if (input.OppositeType == _event.InputType &&
                    input.UISelectedPlatform == XRRigTypeToControllerType(XRRig.Instance.Type))
                {
                    if (RequireAllInputs)
                    {
                        m_currentInputsEntries.Remove(input);
                    }
                    else
                    {
                        CheckValidation(false);
                    }
                }
            }

            if (RequireAllInputs)
            {
                if (InputsMap.InputsEntries.Count == m_currentInputsEntries.Count)
                {
                    CheckValidation(true);
                }
                else
                {
                    CheckValidation(false);
                }
            }
        }

        private void CheckValidation(bool _isValid)
        {
            if (IsValid) return;

            if (IsDuration && (FocusDuration > 0))
            {
                if (_isValid)
                {
                    ActivateDuration();
                }
                else
                {
                    DeactivateDuration();
                }
            }
            else
            {
                if (_isValid)
                {
                    Validate();
                }
                else
                {
                    Invalidate();
                }
            }
        }

        public override void Validate()
        {
            base.Validate();
            m_currentInputsEntries.Clear();
        }

        public override void Reload()
        {
            Invalidate();
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
        }

        private S_Controllers XRRigTypeToControllerType(XRRigType _xRRigType)
        {
            switch (_xRRigType)
            {
                default:
                case XRRigType.OculusQuest:
                case XRRigType.OculusRift:
                case XRRigType.OculusRiftS:
                    return S_Controllers.OCULUS_TOUCH;
                case XRRigType.ViveFocusPlus:
                case XRRigType.VivePro:
                    return S_Controllers.VIVE;
                case XRRigType.WindowsMixedReality:
                    return S_Controllers.WINDOWS_MIXED_REALITY;
            }
        }
    }

    /// <summary>
    /// Wrapper for the API in order to create a collision condition
    /// </summary>
    public static partial class APIExtensions
    {
        public static SC_Input CreateInputCondition(this S_InteractionDefinition _def, string _objectToCollideAtGUID)
        {
            return _def.CreateCondition<SC_Input>(_objectToCollideAtGUID);
        }
    }
}