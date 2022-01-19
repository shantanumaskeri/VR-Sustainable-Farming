using Gaze;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    /// <summary>
    /// This class is represents the sensors of the user's hands, it decides
    /// if the hand is able to interact with some object
    /// </summary>
    public class S_HandIODetectorKernel
    {
        public RaycastHit[] Hits;

        /// <summary>
        /// Used to raycast every frame when touching an IO or at the specified interval.
        /// </summary>
        public bool ObjectPointerInPreviousFrame = false;

        private float m_NextAllowedUpdate = 0;
        private int m_PointLayerMask;
        private const float BASE_RAYCAST_INTERVAL = 0.2f;

        public List<GameObject> RaycastIOs;
        private S_GrabManager m_GrabManager;
        private S_HandIODetectorFeedback m_FeedbackManager;
        private S_ControllerPointingEventArgs m_GazeControllerPointingEventArgs;
        private KeyValuePair<UnityEngine.XR.XRNode, GameObject> m_KeyValue;

        private string m_requestRaycastTouch = "";

        public S_HandIODetectorKernel(S_GrabManager _owner)
        {
            m_GrabManager = _owner;
            m_PointLayerMask = 1 << LayerMask.NameToLayer(S_UserLayers.HANDHOVER);
            RaycastIOs = new List<GameObject>();

            BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);
        }

        public void Setup()
        {
            m_FeedbackManager = m_GrabManager.IoDetectorFeedback;
            m_GrabManager.DistantGrabOrigin = m_GrabManager.gameObject.GetComponentInChildren<S_DistantGrabPointer>().gameObject;
            m_GazeControllerPointingEventArgs = new S_ControllerPointingEventArgs(m_GrabManager.gameObject, m_KeyValue, true);
            m_GazeControllerPointingEventArgs.Sender = m_GrabManager.gameObject;
            m_KeyValue = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>();
        }

        public void Update()
        {
            if(Time.time > m_NextAllowedUpdate || m_FeedbackManager.ActualFeedbackMode == S_HandIODetectorFeedback.FeedbackModes.Colliding)
            {
                FindDistantObjects();
                m_NextAllowedUpdate = Time.time + BASE_RAYCAST_INTERVAL;
            }
        }

        public void FindDistantObjects()
        {
            if (m_GrabManager.DistantGrabOrigin == null)
                return;

            Vector3 positionGragOrigin = m_GrabManager.DistantGrabOrigin.transform.position;
            Vector3 forwardGragOrigin = m_GrabManager.DistantGrabOrigin.transform.forward;

            Hits = Physics.RaycastAll(positionGragOrigin, forwardGragOrigin, m_PointLayerMask);
            m_GrabManager.HitsIos.Clear();
            m_GrabManager.CloserIO = null;
            m_GrabManager.AnyIntersectedIO = null;

            float visualRayLength = float.MaxValue;

            // if the raycast hits nothing
            if (Hits.Length < 1)
            {
                // notify every previously pointed object they are no longer pointed
                for (int i = 0; i < RaycastIOs.Count; i++)
                {
                    m_GazeControllerPointingEventArgs.IsPointed = false;
                    S_EventManager.FireControllerPointingEvent(m_GazeControllerPointingEventArgs);
                }

                // clear the list
                RaycastIOs.Clear();

                ObjectPointerInPreviousFrame = false;
            }
            else
            {
                // 1° notify new raycasted objects in hits
                for (int i = 0; i < Hits.Length; i++)
                {
                    if (Hits[i].collider.GetComponent<S_HandHover>() != null)
                    {
                        // get the pointed object
                        S_InteractiveObject interactiveObject = Hits[i].collider.transform.GetComponentInParent<S_InteractiveObject>();

                        if (interactiveObject.ManipulationMode == S_ManipulationModes.NONE)
                            continue;

                        // populate the list of IOs hit
                        m_GrabManager.HitsIos.Add(interactiveObject.gameObject);

                        // Add a dynamic grab position points to objects that doesn't need to snap.
                        if (!interactiveObject.SnapOnGrab && !interactiveObject.GrabLogic.IsBeingGrabbed)
                            interactiveObject.GrabLogic.AddDynamicGrabPositioner(Hits[i].point, m_GrabManager);

                        // if pointed object is not in the list
                        if (!RaycastIOs.Contains(interactiveObject.gameObject))
                        {
                            // notify the new pointed object
                            RaycastIOs.Add(interactiveObject.gameObject);
                            m_GazeControllerPointingEventArgs.Dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(m_GrabManager.IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, interactiveObject.gameObject);
                            m_GazeControllerPointingEventArgs.IsPointed = true;
                            S_EventManager.FireControllerPointingEvent(m_GazeControllerPointingEventArgs);
                        }

                        if (!interactiveObject.GrabLogic.IsBeingGrabbed && interactiveObject.IsGrabEnabled && interactiveObject.GrabModeIndex.Equals((int)S_GrabMode.ATTRACT) && Hits[i].distance > interactiveObject.GrabDistance)
                        {
                            if (interactiveObject.EnableManipulation)
                            {
                                m_GrabManager.AnyIntersectedIO = interactiveObject;
                            }
                        }
                        if (!interactiveObject.GrabLogic.IsBeingGrabbed && interactiveObject.IsGrabEnabled && interactiveObject.GrabModeIndex.Equals((int)S_GrabMode.ATTRACT) && Hits[i].distance < interactiveObject.GrabDistance)
                        {
                            m_GrabManager.CloserIO = interactiveObject;
                            m_GrabManager.CloserDistance = Hits[i].distance;
                            break;
                        }
                        if (interactiveObject.IsTouchEnabled && (Hits[i].distance < interactiveObject.TouchDistance) && (interactiveObject.TouchDistance > 0))
                        {
                            bool dispatchEvent = false;
                            if (m_GrabManager.CloserIO != interactiveObject)
                            {
                                dispatchEvent = true;
                            }
                            m_GrabManager.CloserIO = interactiveObject;
                            m_GrabManager.CloserDistance = Hits[i].distance;
                            if (dispatchEvent)
                            {
                                if (m_requestRaycastTouch.Length == 0)
                                {
                                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(SC_PointAndClick.EVENT_S_PROXIMITY_CHECK_TOUCH_COLLISION, m_GrabManager.DistantGrabOrigin, interactiveObject, Hits[i].point);
                                }
                                else
                                {
                                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(m_requestRaycastTouch, m_GrabManager.DistantGrabOrigin, interactiveObject, Hits[i].point, m_GrabManager.IsLeftHand);
                                    m_requestRaycastTouch = "";
                                }
                            }
                            break;
                        }
                        if (interactiveObject.IsGrabEnabled && interactiveObject.GrabModeIndex.Equals((int)S_GrabMode.LEVITATE) && Hits[i].distance < interactiveObject.GrabDistance && !interactiveObject.GrabLogic.IsBeingGrabbed)
                        {
                            // update the hit position until we grab something
                            if (m_GrabManager.GrabState != S_GrabManagerState.GRABBED)
                                m_GrabManager.HitPosition = Hits[i].point;

                            m_GrabManager.CloserIO = interactiveObject;
                            m_GrabManager.CloserDistance = Hits[i].distance;
                            break;
                        }

                        // Get the visual ray length
                        visualRayLength = interactiveObject.IsGrabEnabled && interactiveObject.GrabModeIndex.Equals((int)S_GrabMode.ATTRACT) && visualRayLength > Hits[i].distance ? Hits[i].distance : visualRayLength;
                    }
                }

                // 2 : notify no longer raycasted objects in raycastIOs
                for (int i = 0; i < RaycastIOs.Count; i++)
                {
                    if (!m_GrabManager.HitsIos.Contains(RaycastIOs[i]))
                    {
                        // notify
                        m_GazeControllerPointingEventArgs.Dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(m_GrabManager.IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, RaycastIOs[i]);
                        m_GazeControllerPointingEventArgs.IsPointed = false;

                        S_EventManager.FireControllerPointingEvent(m_GazeControllerPointingEventArgs);

                        // remove it
                        RaycastIOs.RemoveAt(i);
                    }
                }

                if (m_GrabManager.HitsIos.Count > 0)
                    ObjectPointerInPreviousFrame = true;
            }

            bool isThereIntesectionWithIO = (m_GrabManager.AnyIntersectedIO != null) || (m_GrabManager.CloserIO != null);
            if (!isThereIntesectionWithIO)
            {
                m_FeedbackManager.ActualFeedbackMode = S_HandIODetectorFeedback.FeedbackModes.Default;
            }
            else
            {
                m_FeedbackManager.ShowDistantGrabFeedbacks(positionGragOrigin, forwardGragOrigin, visualRayLength, isThereIntesectionWithIO, (m_GrabManager.CloserIO != null), m_GrabManager.GrabLaserStartWidth, m_GrabManager.GrabLaserEndWidth);
                m_FeedbackManager.ActualFeedbackMode = S_HandIODetectorFeedback.FeedbackModes.Colliding;
            }

            if (m_requestRaycastTouch.Length > 0)
            {
                string requestRaycastTouch = m_requestRaycastTouch;
                m_requestRaycastTouch = "";
                BasicSystemEventController.Instance.DispatchBasicSystemEvent(requestRaycastTouch);
            }
        }

        private void OnBasicSystemEvent(string _nameEvent, object[] _list)
        {
            if ((_nameEvent == SC_PointAndClick.EVENT_S_PROXIMITY_REQUEST_RAYCAST_TOUCH) || (_nameEvent == SC_PointAndClick.EVENT_S_PROXIMITY_REQUEST_RAYCAST_OTHER))
            {
                S_HandsEnum handsToCheck = (S_HandsEnum)_list[0];
                switch (handsToCheck)
                {
                    case S_HandsEnum.BOTH:
                        bool checkBothHands = (bool)_list[1];
                        break;

                    case S_HandsEnum.LEFT:
                        if (!m_GrabManager.IsLeftHand)
                        {
                            return;
                        }
                        break;

                    case S_HandsEnum.RIGHT:
                        if (m_GrabManager.IsLeftHand)
                        {
                            return;
                        }
                        break;
                }
                if (_nameEvent == SC_PointAndClick.EVENT_S_PROXIMITY_REQUEST_RAYCAST_TOUCH)
                {
                    m_requestRaycastTouch = SC_PointAndClick.EVENT_S_PROXIMITY_RESPONSE_RAYCAST_TOUCH;
                }
                else if (_nameEvent == SC_PointAndClick.EVENT_S_PROXIMITY_REQUEST_RAYCAST_OTHER)
                {
                    m_requestRaycastTouch = SC_PointAndClick.EVENT_S_PROXIMITY_RESPONSE_RAYCAST_OTHER;
                }
            }
        }

        public void ClearRaycasts()
        {
            RaycastIOs.Clear();
        }

        public void RemoveDestroyedIOFromRaycasts(GameObject _destoyedObj)
        {
            if (RaycastIOs != null)
                RaycastIOs.Remove(_destoyedObj);
        }

    }
}
