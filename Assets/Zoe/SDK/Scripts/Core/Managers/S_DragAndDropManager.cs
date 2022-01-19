//-----------------------------------------------------------------------
// <copyright file="LevitationEventArgs.cs" company="apelab sàrl">
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
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2016-01-25</date>
// </copyright>
//-----------------------------------------------------------------------
using SpatialStories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public class S_DragAndDropManager : MonoBehaviour
    {
        #region public Members
        // in unity units
        private bool m_Grabbed, m_IsLevitating;
        private bool m_InProximity;
        private bool m_WasAligned;
        private bool m_Snapped;

        public bool IsSnapped { get { return m_Snapped; } }

        private S_HandController[] m_GrabbingControllers = new S_HandController[2];

        private Vector3 m_StartGrabLocalPosition;
        private Quaternion m_StartGrabLocalRotation;

        private bool m_IsCurrentlyAligned = false;
        private S_InteractiveObject m_IO;
        private Coroutine m_SnapCoroutine;
        private S_InteractiveObject m_InteractiveObject;
        private S_Manipulation m_IOManipulation;
        private S_HandHover m_IOHandHover;
        private bool m_IsCurrentlyDropped = false;

        private Transform m_TargetTransform;
        private S_ManipulationModes m_LastManipulationMode;
        #endregion

        /// <summary>
        /// Stores all the ids of ocupped drag and drop targets
        /// </summary>
        public static HashSet<int> UsedDropTargets = new HashSet<int>();
        private bool m_IsProximityEventSubscribed = false;

        void SubscribeToOnControllerGrabEvent()
        {
            if (S_InputManager.Instance == null)//this could be a scene object which needs to wait for the input manager to connect
            {
                Invoke("SubscribeToOnControllerGrabEvent", 1f);
            }
            else
            {
                S_InputManager.Instance.OnControllerGrabEvent += OnControllerGrabEvent;
                S_InputManager.Instance.OnControllerAttractEvent += OnControllerAttractEvent;
                // Debug.Log("On Controller Grab Event registered - drag and drop manager");
            }
        }

        void OnEnable()
        {
            // to snap/unsnap by grabbing the object
            SubscribeToOnControllerGrabEvent();

            // to snap/unsnap by levitating the object
            S_EventManager.OnLevitationEvent += OnLevitationEvent;

            m_InteractiveObject = GetComponent<S_InteractiveObject>();
        }

        void OnDisable()
        {
            S_EventManager.OnLevitationEvent -= OnLevitationEvent;
            // to snap/unsnap by grabbing the object
            if (S_InputManager.Instance != null)
            {
                S_InputManager.Instance.OnControllerGrabEvent -= OnControllerGrabEvent;
                S_InputManager.Instance.OnControllerGrabEvent -= OnControllerAttractEvent;
            }

            if (m_IsProximityEventSubscribed)
                S_EventManager.OnProximityEvent -= OnProximityEvent;
        }

        void Start()
        {
            m_IO = GetComponent<S_InteractiveObject>();
            m_IOHandHover = m_IO.GetComponentInChildren<S_HandHover>();
            m_IOManipulation = m_IO.GetComponentInChildren<S_Manipulation>();
            m_GrabbingControllers = new S_HandController[2];
        }

        void Update()
        {
            if ((!m_Grabbed && !m_IsLevitating) || !m_InteractiveObject.IsDragAndDropEnabled)
                return;

            m_IsCurrentlyAligned = IsObjectAlignedWithItsTarget();

            // if the value has changed
            if (m_IsCurrentlyAligned != m_WasAligned)
            {
                // if the user aligned the object with its DnD target
                if (m_IsCurrentlyAligned)
                {
                    DropReady();
                }
                // if the user removed the object from its DnD target
                else
                {
                    Remove();
                    S_EventManager.FireDragAndDropEvent(new S_DragAndDropEventArgs(this, gameObject, m_TargetTransform.gameObject, S_DragAndDropStates.DROPREADYCANCELED));
                }

                // update flags
                m_WasAligned = m_IsCurrentlyAligned;
            }
        }

        private void DropReady()
        {
            if (m_InteractiveObject.DnDSnapBeforeDrop)
            {
                Snap(m_InteractiveObject.DnDTimeToSnap);
                m_Snapped = true;
            }

            //Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, targetTransform.gameObject, Gaze_DragAndDropStates.DROPREADY));
            S_EventManager.FireDragAndDropEvent(new S_DragAndDropEventArgs(this, gameObject, m_TargetTransform.gameObject, S_DragAndDropStates.DROPREADY));
        }

        public void Remove()
        {
            m_IsCurrentlyDropped = false;
            if (m_InteractiveObject.DnDSnapBeforeDrop)
            {
                UnSnap();
                m_Snapped = false;
            }
            //Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, targetTransform.gameObject, Gaze_DragAndDropStates.REMOVE));
            S_EventManager.FireDragAndDropEvent(new S_DragAndDropEventArgs(this, gameObject, m_TargetTransform.gameObject, S_DragAndDropStates.REMOVE));

            if (m_IO.ActualGravityState == S_GravityState.LOCKED)
            {
                S_GravityManager.ChangeGravityState(m_IO, S_GravityRequestType.UNLOCK);
                if (!m_IO.GrabLogic.IsBeingGrabbed)
                    S_GravityManager.ChangeGravityState(m_IO, S_GravityRequestType.ACTIVATE_AND_DETACH);
            }
        }

        /// <summary>
        /// This method will be called on every FireNewDragAndDropEvent in order to check
        /// what is the current state of the drop targets, this will allow us to know if a
        /// target is already occuped or not.
        /// </summary>
        /// <param name="e"></param>
        public static void UpdateDropTargetsStates(S_DragAndDropEventArgs e)
        {
            switch (e.State)
            {
                case S_DragAndDropStates.DROP:
                    UsedDropTargets.Add(((GameObject)e.DropTarget).GetInstanceID());
                    break;
                case S_DragAndDropStates.REMOVE:
                    UsedDropTargets.Remove(((GameObject)e.DropTarget).GetInstanceID());
                    break;
            }
        }

        /// <summary>
        /// Check if a drop target is already used or not
        /// </summary>
        /// <param name="_dropTargetToCheck"></param>
        /// <returns></returns>
        public static bool IsDropTargetAlreadyUsed(GameObject _dropTargetToCheck)
        {
            return UsedDropTargets.Contains(_dropTargetToCheck.GetInstanceID());
        }

        private void OnLevelWasLoaded(int level)
        {
            if (UsedDropTargets == null)
                UsedDropTargets = new HashSet<int>();
            //If we are loading a new level just make sure that we don't have references to other levels
            else if (UsedDropTargets != null && UsedDropTargets.Count > 0)
                UsedDropTargets.Clear();
        }

        private void Drop()
        {
            m_IsCurrentlyDropped = true;

            // parent to target object
            transform.SetParent(m_TargetTransform.transform);

            // Don't snap if not necesary
            if (m_IO.DnDSnapOnDrop)
                Snap(m_InteractiveObject.DnDTimeToSnap);

            S_EventManager.FireDragAndDropEvent(new S_DragAndDropEventArgs(this, gameObject, m_TargetTransform.gameObject, S_DragAndDropStates.DROP));
            if (m_IO.DnDAttached)
                m_IO.EnableManipulationMode(S_ManipulationModes.NONE);
        }

        private void PickUp()
        {
            if (m_InteractiveObject.DnDSnapBeforeDrop)
            {
                Snap();
                m_Snapped = true;
            }

            //Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, targetTransform.gameObject, Gaze_DragAndDropStates.PICKUP));
            S_EventManager.FireDragAndDropEvent(new S_DragAndDropEventArgs(this, gameObject, m_TargetTransform.gameObject, S_DragAndDropStates.PICKUP));
        }

        public bool IsInDistance(Vector3 _position, float _tolerance = 0f)
        {
            bool inDistance = false;
            float closestTargetInRangeDistance = float.MaxValue;
            for (int i = 0; i < m_InteractiveObject.DnD_Targets.Count; i++)
            {
                // if target doesn't exist anymore, remove it !
                if (m_InteractiveObject.DnD_Targets[i] == null)
                {
                    m_InteractiveObject.DnD_Targets.RemoveAt(i);
                }
                else
                {
                    float targetDistance = Vector3.Distance(_position, m_InteractiveObject.DnD_Targets[i].transform.position);
                    // compare distance between me (the drop object) and the drop target
                    if (targetDistance <= m_InteractiveObject.DnDMinDistance + _tolerance)
                    {
                        // store the target in proximity's transform if it's closer than any other target in range
                        if (targetDistance < closestTargetInRangeDistance)
                        {
                            // Check if this object wants to be the only one in a target
                            if (!m_IO.DnDDropOnAlreadyOccupiedTargets)
                            {
                                // if the target is not used we can use it
                                if (!S_DragAndDropManager.IsDropTargetAlreadyUsed(m_InteractiveObject.DnD_Targets[i].gameObject))
                                {
                                    m_TargetTransform = m_InteractiveObject.DnD_Targets[i].transform;
                                    closestTargetInRangeDistance = targetDistance;
                                    inDistance = true;
                                }
                            }
                            else // If the object doesn't care about being in the same position of other targets no more checking is needed
                            {
                                m_TargetTransform = m_InteractiveObject.DnD_Targets[i].transform;
                                closestTargetInRangeDistance = targetDistance;
                                inDistance = true;
                            }

                        }
                    }
                }
            }
            return inDistance;
        }

        private bool IsObjectAlignedWithItsTarget()
        {
            // if already snapped, unsnap it
            if (m_Snapped)
                UnSnap();

            if (m_InteractiveObject.DnD_Targets == null)
                return false;

            // get the list size of drop targets for this manager
            int targetCount = m_InteractiveObject.DnD_Targets.Count;

            // exit if there's no target
            if (targetCount < 1)
                return false;

            // for each drop target in the list
            bool isWithinDistance = IsInDistance(transform.position);

            // exit if none of the targets are aligned
            if (!isWithinDistance)
                return false;

            // calculation of dot products
            float[] validArray = { 1, 1 };
            float[] xDotProducts = { Vector3.Dot(transform.right, m_TargetTransform.right), Vector3.Dot(-transform.right, -m_TargetTransform.right) };
            float[] yDotProducts = { Vector3.Dot(transform.up, m_TargetTransform.up), Vector3.Dot(-transform.up, -m_TargetTransform.up) };
            float[] zDotProducts = { Vector3.Dot(transform.forward, m_TargetTransform.forward), Vector3.Dot(-transform.forward, -m_TargetTransform.forward) };

            // is respectAxis checked?
            float[] xAxisSimilarity = m_InteractiveObject.DnDRespectXAxis ? xDotProducts : validArray;
            if (m_InteractiveObject.DnDRespectXAxisMirrored)
            {
                xAxisSimilarity[0] = Mathf.Abs(xAxisSimilarity[0]);
                xAxisSimilarity[1] = Mathf.Abs(xAxisSimilarity[1]);
            }

            float[] yAxisSimilarity = m_InteractiveObject.DnDRespectYAxis ? yDotProducts : validArray;
            if (m_InteractiveObject.DnDRespectYAxisMirrored)
            {
                yAxisSimilarity[0] = Mathf.Abs(yAxisSimilarity[0]);
                yAxisSimilarity[1] = Mathf.Abs(yAxisSimilarity[1]);
            }

            float[] zAxisSimilarity = m_InteractiveObject.DnDRespectZAxis ? zDotProducts : validArray;
            if (m_InteractiveObject.DnDRespectZAxisMirrored)
            {
                zAxisSimilarity[0] = Mathf.Abs(zAxisSimilarity[0]);
                zAxisSimilarity[1] = Mathf.Abs(zAxisSimilarity[1]);
            }

            // check if rotations are valid
            bool xValidRotation = xAxisSimilarity[0] > (m_InteractiveObject.DnDAngleThreshold / 100) || xAxisSimilarity[1] > (m_InteractiveObject.DnDAngleThreshold / 100);
            bool yValidRotation = yAxisSimilarity[0] > (m_InteractiveObject.DnDAngleThreshold / 100) || yAxisSimilarity[1] > (m_InteractiveObject.DnDAngleThreshold / 100);
            bool zValidRotation = zAxisSimilarity[0] > (m_InteractiveObject.DnDAngleThreshold / 100) || zAxisSimilarity[1] > (m_InteractiveObject.DnDAngleThreshold / 100);
            bool validRotation = xValidRotation && yValidRotation && zValidRotation;


            if (validRotation)
            {
                if (m_Snapped)
                {
                    // resnap
                    if (m_IO.DnDSnapOnDrop)
                        Snap();
                }
                return true;
            }
            else
                return false;

        }

        private void Snap(float timeToSnap = 0f)
        {
            if (timeToSnap == 0)
            {
                transform.position = m_TargetTransform.position;
                transform.rotation = m_TargetTransform.transform.rotation;
                S_GravityManager.ChangeGravityState(GetComponent<S_InteractiveObject>(), S_GravityRequestType.DEACTIVATE_AND_ATTACH);
                S_GravityManager.ChangeGravityState(GetComponent<S_InteractiveObject>(), S_GravityRequestType.LOCK);
            }
            else
            {
                m_SnapCoroutine = StartCoroutine(SnapCoroutine(timeToSnap));
            }
        }

        private IEnumerator SnapCoroutine(float timeToSnap)
        {
            // Don't teleport if an object is snapping (This is a hack for placing objects on the head)
            bool wasTeleportAllowed = S_Teleporter.IsTeleportAllowed;
            S_Teleporter.IsTeleportAllowed = false;
            float time = 0f;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            S_GravityManager.ChangeGravityState(GetComponent<S_InteractiveObject>(), S_GravityRequestType.DEACTIVATE_AND_ATTACH);
            S_GravityManager.ChangeGravityState(GetComponent<S_InteractiveObject>(), S_GravityRequestType.LOCK);

            while (time < timeToSnap)
            {
                float eased = QuadEaseOut(time, 0f, 1f, timeToSnap);
                transform.position = Vector3.Lerp(startPos, m_TargetTransform.position, eased);
                transform.rotation = Quaternion.Lerp(startRot, m_TargetTransform.rotation, eased);
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = m_TargetTransform.position;
            transform.rotation = m_TargetTransform.transform.rotation;

            if (wasTeleportAllowed)
                S_Teleporter.IsTeleportAllowed = true;
        }

        private float QuadEaseOut(float time, float startVal, float changeInVal, float duration)
        {
            float elapsedTime = (time > duration) ? 1.0f : time / duration;
            return -changeInVal * elapsedTime * (elapsedTime - 2) + startVal;
        }

        private void UnSnap()
        {
            if (m_SnapCoroutine != null)
            {
                StopCoroutine(m_SnapCoroutine);
                m_SnapCoroutine = null;
            }

            transform.SetParent(null);
        }

        public void Grab(GameObject _controller)
        {
            m_Grabbed = true;

            m_GrabbingControllers[1] = _controller.GetComponentInChildren<S_HandController>();
            m_StartGrabLocalPosition = transform.localPosition;
            m_StartGrabLocalRotation = transform.localRotation;
            if (m_SnapCoroutine != null)
            {
                StopCoroutine(m_SnapCoroutine);
                m_SnapCoroutine = null;
            }
            if (m_WasAligned)
            {
                PickUp();
            }
        }

        private void Ungrab()
        {
            m_Grabbed = false;
            if (m_WasAligned)
            {
                Drop();
            }
            // remove controllers
            m_GrabbingControllers[0] = null;
            m_GrabbingControllers[1] = null;
        }

        private void Levitate()
        {
            m_IsLevitating = true;
            if (m_WasAligned)
            {
                PickUp();
            }
        }

        private void Unlevitate()
        {
            m_IsLevitating = false;
            if (m_WasAligned)
            {
                Drop();
            }
            // remove controllers
            if (m_GrabbingControllers != null && m_GrabbingControllers.Length == 2)
            {
                m_GrabbingControllers[0] = null;
                m_GrabbingControllers[1] = null;
            }
        }

        private void OnProximityEvent(S_ProximityEventArgs e)
        {
            //TODO:(Unregister this properly)
            if (this == null)
                return;

            m_InteractiveObject = GetComponent<S_InteractiveObject>();

            // if the sender is me (the DropObject)
            if (((S_InteractiveObject)e.Sender).gameObject.Equals(gameObject))
            {
                // get the drop target from the received event
                GameObject dropTarget = e.Other.GetComponentInParent<S_InteractiveObject>().gameObject;

                // get the list size of drop targets for this manager
                int targetCount = m_InteractiveObject.DnD_Targets.Count;

                // for each drop target in the list
                for (int i = 0; i < targetCount; i++)
                {
                    // check if the target is in the list
                    if (dropTarget.Equals(m_InteractiveObject.DnD_Targets[i]))
                    {
                        m_InProximity = e.IsInProximity;

                        // if it's not in proximity
                        if (!m_InProximity)
                        {
                            // but was previously aligned
                            if (m_WasAligned)
                            {
                                m_WasAligned = false;

                                // that means the user removed the drop object from its target
                                Remove();
                            }
                        }
                    }
                }
            }
        }

        private void OnControllerGrabEvent(S_ControllerGrabEventArgs e)
        {
            if (e.ControllerObjectPair.Value == gameObject)
            {
                if (e.IsGrabbing)
                {
                    // to know if the object has entered its DnD target or not
                    m_IsProximityEventSubscribed = true;
                    S_EventManager.OnProximityEvent += OnProximityEvent;

                    if (e.ControllerObjectPair.Key.Equals(UnityEngine.XR.XRNode.LeftHand))
                        Grab(S_InputManager.Instance.LeftController);
                    else if (e.ControllerObjectPair.Key.Equals(UnityEngine.XR.XRNode.RightHand))
                        Grab(S_InputManager.Instance.RightController);
                }
                else
                {
                    Ungrab();
                    m_IsProximityEventSubscribed = false;
                    S_EventManager.OnProximityEvent -= OnProximityEvent;
                }
            }
        }

        private void OnControllerAttractEvent(S_ControllerGrabEventArgs e)
        {
            if (e.ControllerObjectPair.Value == gameObject)
            {

            }
        }

        private void OnLevitationEvent(S_LevitationEventArgs e)
        {
            if (S_Utils.AreUnderSameIO(S_Utils.ConvertIntoGameObject(e.Sender), this.gameObject))
            {
                if (e.Type.Equals(S_LevitationTypes.LEVITATE_START))
                {
                    Levitate();
                }
                else if (e.Type.Equals(S_LevitationTypes.LEVITATE_STOP))
                {
                    Unlevitate();
                }
            }
        }

        public void ChangeAttach(bool attach)
        {
            if (m_IsCurrentlyDropped)
            {
                if (!attach)
                    m_IO.EnableManipulationMode(m_LastManipulationMode);
                else
                    m_IO.EnableManipulationMode(S_ManipulationModes.NONE);

            }
        }

        /// <summary>
        /// Allows object to be dropped even if they are super far
        /// </summary>
        public void AutoDrop(Transform _target)
        {
            m_TargetTransform = _target;
            transform.position = m_TargetTransform.position;
            m_WasAligned = true;
            Drop();
        }
    }
}