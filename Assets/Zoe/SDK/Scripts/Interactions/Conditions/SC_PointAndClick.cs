using UnityEngine;
using System;
using System.Collections.Generic;
using Gaze;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SC_PointAndClick")]
    public class SC_PointAndClick : SC_Collision
    {
        // ----------------------------------------------
        // EVENTS
        // ----------------------------------------------
        public const string EVENT_S_PROXIMITY_CHECK_TOUCH_COLLISION = "EVENT_S_PROXIMITY_CHECK_TOUCH_COLLISION";

        public const string EVENT_S_PROXIMITY_REQUEST_RAYCAST_TOUCH = "EVENT_S_PROXIMITY_REQUEST_RAYCAST_TOUCH";
        public const string EVENT_S_PROXIMITY_REQUEST_RAYCAST_OTHER = "EVENT_S_PROXIMITY_REQUEST_RAYCAST_OTHER";
        public const string EVENT_S_PROXIMITY_RESPONSE_RAYCAST_TOUCH = "EVENT_S_PROXIMITY_RESPONSE_RAYCAST_TOUCH";
        public const string EVENT_S_PROXIMITY_RESPONSE_RAYCAST_OTHER = "EVENT_S_PROXIMITY_RESPONSE_RAYCAST_OTHER";

        public const string EVENT_POINTANDCLICK_BROADCAST_CONFIRMATION = "EVENT_POINTANDCLICK_BROADCAST_CONFIRMATION";

        // ----------------------------------------------
        // EVENTS
        // ----------------------------------------------
        public bool EnableWithTrigger = true;
        public int TouchState;
        public int PointAndClickIndexAction;

        public bool RequireBothHands = false;
        public S_ProximityStates HoverState;
        public int HoverStateIndexAction;

        // ----------------------------------------------
        // PRIVATE MEMBERS
        // ----------------------------------------------
        private bool m_enableLeftHand = false;
        private bool m_enableRightHand = false;

        private bool m_validCollision = false;

        private bool m_distanceTouch = false;
        private int m_requestedRaycastTouch = -1;
        private S_HandsEnum m_pressedHand;

        private List<S_InteractiveObject> m_handsEnter = new List<S_InteractiveObject>();

        private GameObject m_originObject = null;
        private Vector3 m_originRaycast = Vector3.zero;
        private Vector3 m_collisionPoint = Vector3.zero;

        // ----------------------------------------------
        // FUNCTIONS
        // ----------------------------------------------

        public SC_PointAndClick() { }

        public void Start()
        {
            if (EnableWithTrigger)
            {
                BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);

                if (GameObject.FindObjectOfType<S_LeftHandRoot>() != null)
                {
                    m_enableLeftHand = true;
                }
                if (GameObject.FindObjectOfType<S_RightHandRoot>() != null)
                {
                    m_enableRightHand = true;
                }
            }
        }

        public override void Setup()
        {
            base.Setup();
            S_EventManager.OnProximityEvent -= OnProximityEvent;
            if (EnableWithTrigger)
            {
                S_InputManager.Instance.LeftTriggerButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightTriggerButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftTriggerButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);
                S_InputManager.Instance.RightTriggerButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);
            }
            else
            {
                S_EventManager.OnControllerPointingEvent += OnControllerPointingEvent;

                // The logic in CheckForAlreadyPointedAtObjects was here but was leading to cases
                // where the condition was validated in Setup which got the whole Interaction stuck
                // We now wait for the next frame in order to decouple the Setup and the validation
                S_Scheduler.AddUniqueTaskAtNextFrame(CheckForAlreadyPointedAtObjects);
            }
        }

        public override void Dispose()
        {
            if (EnableWithTrigger)
            {
                S_InputManager myInstance = S_InputManager.Instance;
                if (myInstance != null)  //sometimes the getter does not return an instance - e.g when it cannot find the inputmanager in the scene
                {
                    myInstance.LeftTriggerButtonPressEvent -= OnInputDownEvent;
                    myInstance.RightTriggerButtonPressEvent -= OnInputDownEvent;
                    myInstance.LeftTriggerButtonReleaseEvent -= OnInputUpEvent;
                    myInstance.RightTriggerButtonReleaseEvent -= OnInputUpEvent;
                }
            }
            else
            {
                S_EventManager.OnControllerPointingEvent -= OnControllerPointingEvent;
            }
            base.Dispose();
        }

        public override void Reload()
        {
            if (EnableWithTrigger)
            {
                m_validCollision = false;
                m_distanceTouch = false;
                m_requestedRaycastTouch = -1;
            }
            else
            {
                m_handsEnter.Clear();
            }
            base.Reload();
        }

        private void CheckForAlreadyPointedAtObjects()
        {
            // If we are already pointing at an object, we need to make sure the logic
            // runs just the same. In order to do that, we have this small hack which checks
            // all the grab managers (Supposedly two, left hand and right hand) and checks their
            // IODetectorKernel which is the class responsible for sending ControllerPointingEvents
            foreach (S_GrabManager grabManager in S_GrabManager.GrabManagers)
            {
                foreach (GameObject gameobject in grabManager.IoDetectorKernel.RaycastIOs)
                {
                    S_ControllerPointingEventArgs eventArgs = new S_ControllerPointingEventArgs();
                    eventArgs.Sender = grabManager.gameObject;
                    eventArgs.Dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(grabManager.IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, gameobject);
                    eventArgs.IsPointed = true;
                    OnControllerPointingEvent(eventArgs);
                }
            }
        }


        private void OnControllerPointingEvent(S_ControllerPointingEventArgs _event)
        {
            if (!EnableWithTrigger)
            {
                S_InteractiveObject objectRoot = S_Utils.GetIOFromGameObject(this.gameObject);
                S_InteractiveObject objectTarget = S_Utils.GetIOFromObject(_event.Dico.Value);
                S_InteractiveObject objectSender = S_Utils.GetIOFromObject(_event.Sender);
                int indexCollidingObject = IsCollidingObjectsInList(objectTarget, objectSender);
                if (indexCollidingObject >= 0)
                {
                    switch (HoverState)
                    {
                        case S_ProximityStates.ENTER:
                            if (_event.IsPointed)
                            {
                                if (!RequireBothHands)
                                {
                                    Validate();
                                }
                                else
                                {
                                    if (!m_handsEnter.Contains(objectSender))
                                    {
                                        m_handsEnter.Add(objectSender);
                                    }
                                    if (m_handsEnter.Count == 2)
                                    {
                                        Validate();
                                    }
                                }
                            }
                            else
                            {
                                if (RequireBothHands)
                                {
                                    m_handsEnter.Remove(objectSender);
                                    if (m_handsEnter.Count != 2)
                                    {
                                        Invalidate();
                                    }
                                }
                            }
                            break;

                        case S_ProximityStates.INSIDE:
                            break;

                        case S_ProximityStates.EXIT:
                            if (!_event.IsPointed)
                            {
                                if (!RequireBothHands)
                                {
                                    Validate();
                                }
                                else
                                {
                                    if (!m_handsEnter.Contains(objectSender))
                                    {
                                        m_handsEnter.Add(objectSender);
                                    }
                                    if (m_handsEnter.Count == 2)
                                    {
                                        Validate();
                                    }
                                }
                            }
                            else
                            {
                                if (RequireBothHands)
                                {
                                    m_handsEnter.Remove(objectSender);
                                    if (m_handsEnter.Count != 2)
                                    {
                                        Invalidate();
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        private bool IsTargetObjectDistanceTouch(object[] _list)
        {
            GameObject otherObject = (GameObject)_list[0];
            S_InteractiveObject senderIO = otherObject.GetComponentInParent<S_InteractiveObject>();
            S_InteractiveObject targetIO = (S_InteractiveObject)_list[1];
            S_InteractiveObject currentIO = S_Utils.GetIOFromGameObject(this.gameObject);
            m_originObject = otherObject;
            m_originRaycast = otherObject.transform.position;
            m_collisionPoint = (Vector3)_list[2];
            int indexCollidingObject = IsCollidingObjectsInList(targetIO, senderIO);
            if (indexCollidingObject >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckResultHandControlRaycast(bool _applyDeactivation, object[] _list)
        {
            if (_list.Length > 0)
            {
                if (m_durationActivated)
                {
                    if (!IsTargetObjectDistanceTouch(_list))
                    {
                        DeactivateDuration();
                        return false;
                    }
                }
                else
                {
                    if (IsTargetObjectDistanceTouch(_list))
                    {
                        if (IsDuration && (FocusDuration > 0))
                        {
                            ActivateDuration();
                        }
                        else
                        {
                            Validate();
                        }
                    }
                }
                return true;
            }
            else
            {
                if (_applyDeactivation)
                {
                    if (m_durationActivated)
                    {
                        DeactivateDuration();
                    }
                    else
                    {
                        m_validCollision = false;
                        m_distanceTouch = false;
                    }
                }
                return false;
            }
        }

        private void OnBasicSystemEvent(string _nameEvent, object[] _list)
        {
            if (_nameEvent == EVENT_S_PROXIMITY_CHECK_TOUCH_COLLISION)
            {
                if (IsTargetObjectDistanceTouch(_list))
                {
                    m_validCollision = true;
                    m_distanceTouch = true;
                }
                else
                {
                    m_validCollision = false;
                    m_distanceTouch = false;
                }
            }
            if (_nameEvent == EVENT_S_PROXIMITY_RESPONSE_RAYCAST_TOUCH)
            {
                if (m_requestedRaycastTouch > 0)
                {
                    m_requestedRaycastTouch--;

                    if (m_requestedRaycastTouch == 0)
                    {
                        CheckResultHandControlRaycast(true, _list);
                    }
                    else
                    {
                        if (CheckResultHandControlRaycast(false, _list))
                        {
                            m_requestedRaycastTouch = -1;
                        }
                        else
                        {
                            if (m_pressedHand == S_HandsEnum.LEFT)
                            {
                                BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_S_PROXIMITY_REQUEST_RAYCAST_OTHER, S_HandsEnum.RIGHT, RequireBothHands);
                            }
                            else
                            {
                                BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_S_PROXIMITY_REQUEST_RAYCAST_OTHER, S_HandsEnum.LEFT, RequireBothHands);
                            }
                        }
                    }
                }
            }
            if (_nameEvent == EVENT_S_PROXIMITY_RESPONSE_RAYCAST_OTHER)
            {
                CheckResultHandControlRaycast(true, _list);
            }
        }

        private void CheckValidTouch(S_HandsEnum _pressedHand)
        {
            if (m_validCollision)
            {
                if (m_distanceTouch)
                {
                    if (GetHands() == S_HandsEnum.BOTH)
                    {
                        m_requestedRaycastTouch = 2;
                        m_pressedHand = _pressedHand;
                        BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_S_PROXIMITY_REQUEST_RAYCAST_TOUCH, m_pressedHand, RequireBothHands);
                    }
                    else
                    {
                        m_requestedRaycastTouch = 1;
                        BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_S_PROXIMITY_REQUEST_RAYCAST_TOUCH, GetHands(), RequireBothHands);
                    }
                }
            }
        }

        private void OnInputDownEvent(S_InputEventArgs _event)
        {
            if (!m_validCollision) return;

            if ((S_TouchStates)TouchState == S_TouchStates.OnTouch)
            {
                if (m_enableRightHand)
                {
                    if ((_event.InputType == S_InputTypes.RIGHT_GRIP_BUTTON_PRESS) || (_event.InputType == S_InputTypes.RIGHT_TRIGGER_BUTTON_PRESS) || (_event.InputType == S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_PRESS))
                    {
                        CheckValidTouch(S_HandsEnum.RIGHT);
                        return;
                    }
                }

                if (m_enableLeftHand)
                {
                    if ((_event.InputType == S_InputTypes.LEFT_GRIP_BUTTON_PRESS) || (_event.InputType == S_InputTypes.LEFT_TRIGGER_BUTTON_PRESS) || (_event.InputType == S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_PRESS))
                    {
                        CheckValidTouch(S_HandsEnum.LEFT);
                        return;
                    }
                }
            }
        }

        private void OnInputUpEvent(S_InputEventArgs _event)
        {
            if (!m_validCollision) return;

            if ((S_TouchStates)TouchState == S_TouchStates.OnUntouch)
            {
                if (m_enableRightHand)
                {
                    if ((_event.InputType == S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_RELEASE))
                    {
                        CheckValidTouch(S_HandsEnum.RIGHT);
                        return;
                    }
                }

                if (m_enableLeftHand)
                {
                    if ((_event.InputType == S_InputTypes.LEFT_GRIP_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.LEFT_TRIGGER_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_RELEASE))
                    {
                        CheckValidTouch(S_HandsEnum.LEFT);
                        return;
                    }
                }
            }

            if ((S_TouchStates)TouchState == S_TouchStates.OnTouch)
            {
                if (m_enableRightHand)
                {
                    if ((_event.InputType == S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_RELEASE))
                    {
                        if (m_durationActivated)
                        {
                            DeactivateDuration();
                            return;
                        }
                    }
                }
                if (m_enableLeftHand)
                {
                    if ((_event.InputType == S_InputTypes.LEFT_GRIP_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.LEFT_TRIGGER_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_RELEASE))
                    {
                        if (m_durationActivated)
                        {
                            DeactivateDuration();
                            return;
                        }
                    }
                }
            }
        }

        public override void Validate()
        {
            base.Validate();

            BasicSystemEventController.Instance.DelayBasicSystemEvent(EVENT_POINTANDCLICK_BROADCAST_CONFIRMATION, 0.05f, m_originObject, m_originRaycast, m_collisionPoint);
        }

        protected override bool DeactivateDuration()
        {
            m_requestedRaycastTouch = -1;
            return base.DeactivateDuration();
        }

        protected override bool ActivateDuration()
        {
            m_requestedRaycastTouch = -1;
            return base.ActivateDuration();
        }

        public override void Update()
        {
            UpdateBasicDuration();

            if (m_durationActivated)
            {
                if (GetHands() == S_HandsEnum.BOTH)
                {
                    m_requestedRaycastTouch = 2;
                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_S_PROXIMITY_REQUEST_RAYCAST_TOUCH, m_pressedHand, RequireBothHands);
                }
                else
                {
                    m_requestedRaycastTouch = 1;
                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_S_PROXIMITY_REQUEST_RAYCAST_TOUCH, GetHands(), RequireBothHands);
                }
            }
        }
    }

    public partial class APIExtensions
    {
        public static SC_PointAndClick CreatePointAndClickCondition(this S_InteractionDefinition _def, S_ProximityStates _state, bool _requireAll, bool _addRig, params string[] _gameObjectsIDs)
        {
            int minimunNumber = _addRig ? 1 : 2;
            if(_gameObjectsIDs.Length < minimunNumber)
            {
                Debug.LogError(String.Format("SpatialStories API> too few gameobjects added to touch condition on interaction {0}.", _def.Name));
            }
            return _def.CreateCondition<SC_PointAndClick>(_state, _requireAll, _addRig, _gameObjectsIDs);
        }
    }
}
