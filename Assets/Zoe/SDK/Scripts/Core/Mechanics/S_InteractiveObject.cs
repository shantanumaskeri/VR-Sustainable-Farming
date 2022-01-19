// <copyright file="Gaze_InteractiveObject.cs" company="apelab sàrl">
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

using Gaze;
#if PHOTON_INSTALLED
using Photon.Pun;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("S_GravityManager")]
namespace SpatialStories
{
    //[RequireComponent(typeof(PhotonView))]
    //[RequireComponent(typeof(S_PhotonInteractiveObjectManager))]
    [Serializable]
    [SelectionBase]
    public class S_InteractiveObject : S_AbstractEntity, IEquatable<S_InteractiveObject>
    {
        public bool Equals(S_InteractiveObject other)
        {
            return (other == this);
        }

#region Members

        /// <summary>
        /// If this is active the gameobject won't be detached of the hand
        /// </summary>
        public bool IsSticky { get { return isSticky; } }
        private bool isSticky = false;


        /// <summary>
        /// Defines if the IO is affected by gravity
        /// </summary>
        public bool AffectedByGravity; 


        /// <summary>
        /// Defines how the user can interact with the IO by using 
        /// his controllers.
        /// </summary>
        public bool EnableManipulation { get { return ManipulationMode != S_ManipulationModes.NONE; } }
        public S_ManipulationModes ManipulationMode;

        public Transform LeftHandSnapPoint;
        public Transform RightHandSnapPoint;


        /// <summary>
        /// Can this object be grabbed. If networked, no other user must have started grabbing it before.
        /// </summary>
        public bool IsGrabEnabled 
        { get 
            {
                bool isEnabled = false;
#if PHOTON_INSTALLED
                if (myManager != null && myPhotonView != null)
                {                    
                    isEnabled = ((myManager.heldBy == 0 || myManager.heldBy == PhotonNetwork.LocalPlayer.ActorNumber)
                                &&
                                (ManipulationMode == S_ManipulationModes.GRAB || ManipulationMode == S_ManipulationModes.LEVITATE));                    
                }
                else
                {     
#endif
                    isEnabled = (ManipulationMode == S_ManipulationModes.GRAB || ManipulationMode == S_ManipulationModes.LEVITATE);
#if PHOTON_INSTALLED
                }
#endif
                return isEnabled;
            }
        }

        /// <summary>
        /// Can this object be activated by :
        /// TOUCH, POINT or BOTH
        /// </summary>
        public bool IsTouchEnabled { get { return ManipulationMode == S_ManipulationModes.POINT_AND_CLICK; } }
        public int TouchModeIndex;

        /// <summary>
        /// The grab mode is either : Attract or Levitate
        /// TODO(4nc3str4l): This is here as a workarround of the new SDK but with the new
        /// manipulation logic this should be removed and everything should be managed with\
        /// the new manipulation mode attribute.
        /// </summary>
        public int GrabModeIndex
        {
            get
            {
                if (ManipulationMode == S_ManipulationModes.GRAB)
                    return (int)S_GrabMode.ATTRACT;
                else if (ManipulationMode == S_ManipulationModes.LEVITATE)
                    return (int)S_GrabMode.LEVITATE;
                else
                    return -1;
            }
        }

        /// <summary>
        /// It's the collider used to catch the object.
        /// </summary>
        //public Collider catchableHandle;

        /// <summary>
        /// It's the collider used to manipulate the object.
        /// </summary>
        public Collider ManipulableHandle;

        public bool HasGrabPositionner = false;
        public Collider GrabPositionnerCollider;

        public Transform GrabPositionnerTransform { get { return GrabPositionnerCollider.gameObject.transform; } }


        public Transform GrabPositioner;

        /// <summary>
        /// If true, the object being catched will vibrate the controllers while grabbed.
        /// </summary>
        public bool VibratesOnGrab = false;

        /// <summary>
        /// Defines the maximum distance that an object can be cached
        /// </summary>
        public float GrabDistance = 3.0f;

        /// <summary>
        /// Defines the maximum distance that an object can be distanced touched
        /// </summary>
        public float TouchDistance = 3.0f;

        /// <summary>
        /// Defines the speed that a certain object will be atracted to the hand
        /// </summary>
        public float AttractionSpeed = 10.0f;


        /// <summary>
        /// Defines if an object can be grabbed from no matter where once is grabbed
        /// </summary>
        public bool IsManipulable = true;

        public bool SnapOnGrab = false;

        /// <summary>
        /// Is this catchable object using gravity
        /// </summary>
        //		public bool hasGravity;

        // TODO test if this works with a FBX that already has a root motion
        public Transform RootMotion;
#if PHOTON_INSTALLED
        //this is the interactive object's photon view
        private PhotonView myPhotonView;

        //this is the interactive'sobject photon interactive object manager
        private S_PhotonInteractiveObjectManager myManager;
#endif    
        private const float DISABLE_MANIPULATION_TIME = 1f;

        private GameObject m_ActualGrabPoint = null;
        //private Gaze_Handle handle;

        public Coroutine CancelManipulation;

        private S_GrabLogic m_GrabLogic;
        public S_GrabLogic GrabLogic { get { if (m_GrabLogic == null) m_GrabLogic = new S_GrabLogic(this); return m_GrabLogic; } }


        public S_GravityState ActualGravityState { get { return m_ActualGravityState; } }
        private S_GravityState m_ActualGravityState;

        public S_GravityState InitialGravityState { get; private set; }

        public S_Transform InitialTransform { get { return m_InitialTransform; } }

        public bool DnDSnapOnDrop = true;

        private S_Transform m_InitialTransform;

        public bool IsDragAndDropEnabled = false;
        public float DnDMinDistance = 1f;
        // in unity units
        public float DnDAngleThreshold = 1f;
        // 0 is perpendicular, 1 is same direction
        public bool DnDRespectXAxis = false;
        public bool DnDRespectXAxisMirrored = false;
        public bool DnDRespectYAxis = false;
        public bool DnDRespectYAxisMirrored = false;
        public bool DnDRespectZAxis = false;
        public bool DnDRespectZAxisMirrored = false;
        public bool DnDSnapBeforeDrop = true;
        public float DnDTimeToSnap = 0.5f;
        public bool DnDDropOnAlreadyOccupiedTargets = false;

        /// <summary>
        /// If TRUE, once dropped, the object can't be grabbed again.
        /// </summary>
        public bool DnDAttached;
        //public bool DnD_IsTarget = false;
        public List<GameObject> DnD_Targets;

        public S_DragAndDropManager DragAndDropManager;

        private S_Proximity m_Proximity;
        public S_Proximity Proximity
        {
            get
            {
                if (m_Proximity == null)
                    m_Proximity = GetProximity();
                return m_Proximity;
            }
        }

        private S_Manipulation m_Manipulation;
        public S_Manipulation Manipulation
        {
            get
            {
                if (m_Manipulation == null)
                    m_Manipulation = GetManipulation();
                return m_Manipulation;
            }
        }
#endregion Members

        private void Awake()
        {

            
            CreateSchedulerIfNeeded();
            
            m_GrabLogic = new S_GrabLogic(this);
            SetActualGravityStateAsDefault();

            m_InitialTransform = new S_Transform(transform);

            S_SnapPosition[] snapPositions = GetComponentsInChildren<S_SnapPosition>();
            foreach (S_SnapPosition pos in snapPositions)
            {
                if (pos.ActualHand == S_SnapPosition.HAND.LEFT)
                {
                    LeftHandSnapPoint = pos.transform;
                }
                else
                {
                    RightHandSnapPoint = pos.transform;
                }
            }

            if (RootMotion != null)
            {
                transform.SetParent(RootMotion);
            }
        }

        void subscribeToOnControllerGrabEvent() 
        {
            if (S_InputManager.Instance == null)//this could be a scene object which needs to wait for the input manager to connect
            {
                Invoke("subscribeToOnControllerGrabEvent", 1f);
            }
            else
            {
                S_InputManager.Instance.OnControllerGrabEvent += OnControllerGrabEvent;
                S_InputManager.Instance.OnControllerAttractEvent += OnControllerAttractEvent;

                //Debug.Log("On Controller Grab Event registered - Interactive Object");
            }
        }

        private void OnEnable()
        {
#if PHOTON_INSTALLED
            myPhotonView = GetComponent<PhotonView>();
            myManager = GetComponent<S_PhotonInteractiveObjectManager>();
            if (myManager != null)
            {
                subscribeToOnControllerGrabEvent();
            }
#endif
            GrabLogic.SubscribeToEvents();

            S_EventManager.OnControllerPointingEvent += OnControllerPointingEvent;
            DragAndDropManager = gameObject.AddComponent<S_DragAndDropManager>();

        }



        private void RemoveAllListeners()
        {
            if (m_GrabLogic != null)
                m_GrabLogic.UnsubscribeToEvents();
            S_EventManager.OnControllerPointingEvent -= OnControllerPointingEvent;


            //TODO: Unsuscribe from on controller grab event too
            //S_InputManager.Instance.OnControllerGrabEvent -= OnControllerGrabEvent;
            //S_InputManager.Instance.OnControllerAttractEvent -= OnControllerAttractEvent;
        }

        

        public void OnControllerGrabEvent(S_ControllerGrabEventArgs _e)
        {
#if PHOTON_INSTALLED
            if (_e.ControllerObjectPair.Value == gameObject)
            {
                if (_e.IsGrabbing)
                {
                    myManager.StartUsing(PhotonNetwork.LocalPlayer.ActorNumber);
                }
                else
                {
                    myManager.StopUsing();
                }
            }
#endif
        }

        public void OnControllerAttractEvent(S_ControllerGrabEventArgs _e)
        {

#if PHOTON_INSTALLED
            if (_e.ControllerObjectPair.Value == gameObject)
            {
                //Debug.Log("Is attracting " + gameObject.name);
                myManager.StartUsing(PhotonNetwork.LocalPlayer.ActorNumber);
            }
#endif
        }

        private void OnDisable()
        {
            RemoveAllListeners();
        }

        private S_Proximity GetProximity()
        {
            return transform.GetComponentInDirectChildren<S_Proximity>();
        }

        private S_Manipulation GetManipulation()
        {
            return transform.GetComponentInDirectChildren<S_Manipulation>();
        }

        /// <summary>
        /// Changes the sticky state of a game object
        /// </summary>
        /// <param name="_isSticky">If true the object will be attached to the hand</param>
        /// <param name="_dropInmediatelly">If true the grabmanager will detach the object</param>
        public void SetStickykMode(bool _isSticky, bool _dropInmediatelly = true)
        {
            isSticky = _isSticky;
            if (_dropInmediatelly && !isSticky)
                GrabLogic.GrabbingManager.TryDetach();
        }

        public void ContinueManipulation()
        {
            if (CancelManipulation == null) return;
            StopCoroutine(CancelManipulation);
            CancelManipulation = null;
        }

        public IEnumerator DisableManipulationModeInTime()
        {
            yield return new WaitForSeconds(DISABLE_MANIPULATION_TIME);
            GrabLogic.DisableManipulationMode();
        }

        public void ChangeDnDAttach(bool attach)
        {
            DragAndDropManager.ChangeAttach(attach);
        }

        /// <summary>
        /// Notify to everyone that this IO has been destroyed 
        /// </summary>
        private void OnDestroy()
        {
            RemoveAllListeners();
            S_EventManager.FireOnIODestroyed(new S_IODestroyEventArgs(this, this));
        }

        void FixedUpdate()
        {
            GrabLogic.Update();
        }

#region GravityManagement
        /// <summary>
        /// Checks the actual gravity state and store it as default
        /// </summary>
        internal void SetActualGravityStateAsDefault()
        {
            Rigidbody rigidBody = GetRigitBodyOrError();
            if (rigidBody == null)
                return;

            if (rigidBody.isKinematic)
            {

                InitialGravityState = rigidBody.useGravity ? S_GravityState.ACTIVE_KINEMATIC : S_GravityState.UNACTIVE_KINEMATIC;
            }
            else
            {
                InitialGravityState = rigidBody.useGravity ? S_GravityState.ACTIVE_NOT_KINEMATIC : S_GravityState.UNACTIVE_NOT_KINEMATIC;
            }

            m_ActualGravityState = InitialGravityState;
        }

        public void notHoldingAnyMore()
        {
            ReturnToInitialGravityState();
        }

        public void startHolding()
        {            
            SetGravity(false);
        }

        /// <summary>
        /// If the gravity of the IO is not locked it will return to its default state.
        /// </summary>
        internal void ReturnToInitialGravityState()
        {
            Rigidbody rigidBody = GetRigitBodyOrError();

            if (rigidBody == null)
                return;

            switch (InitialGravityState)
            {
                case S_GravityState.ACTIVE_KINEMATIC:
                    rigidBody.useGravity = true;
                    rigidBody.isKinematic = true;
                    break;
                case S_GravityState.ACTIVE_NOT_KINEMATIC:
                    rigidBody.useGravity = true;
                    rigidBody.isKinematic = false;
                    break;
                case S_GravityState.UNACTIVE_KINEMATIC:
                    rigidBody.useGravity = false;
                    rigidBody.isKinematic = true;
                    break;
                case S_GravityState.UNACTIVE_NOT_KINEMATIC:
                    rigidBody.useGravity = false;
                    rigidBody.isKinematic = false;
                    break;
            }
        }

        // Make the IO not listen gravity requests
        internal void LockGravity()
        {
            m_ActualGravityState = S_GravityState.LOCKED;
        }

        /// <summary>
        /// Make the IO listen for gravity requests.
        /// </summary>
        internal void UnlockGravity()
        {
            Rigidbody rigidBody = GetRigitBodyOrError();
            if (rigidBody == null)
                return;

            if (rigidBody.isKinematic)
            {
                m_ActualGravityState = rigidBody.useGravity ? S_GravityState.ACTIVE_KINEMATIC : S_GravityState.UNACTIVE_KINEMATIC;
            }
            else
            {
                m_ActualGravityState = rigidBody.useGravity ? S_GravityState.ACTIVE_NOT_KINEMATIC : S_GravityState.UNACTIVE_NOT_KINEMATIC;
            }
        }

        /// <summary>
        /// Sets ONLY the gravity of a game object to true or false and changes
        /// its state.
        /// </summary>
        /// <param name="_hasGravity"></param>
        internal void SetGravity(bool _hasGravity)
        {
            if (IsGravityLocked())
            {
                WarnUnauthorizedGravityChange();
                return;
            }


            Rigidbody rb = GetRigitBodyOrError();

            if (rb == null)
                return;
            rb.useGravity = _hasGravity;

            if (rb.isKinematic)
            {
                m_ActualGravityState = _hasGravity ? S_GravityState.ACTIVE_KINEMATIC : S_GravityState.UNACTIVE_KINEMATIC;
            }
            else
            {
                m_ActualGravityState = _hasGravity ? S_GravityState.ACTIVE_NOT_KINEMATIC : S_GravityState.UNACTIVE_NOT_KINEMATIC;
            }
        }

        /// <summary>
        /// Sets the mass of the IO
        /// </summary>
        /// <param name="_mass"></param>
        internal void SetMass(float _mass)
        {
            Rigidbody rb = GetRigitBodyOrError();

            if (rb == null)
                return;
            rb.mass = _mass;
        }

        /// <summary>
        /// Sets the gravity of a game object and also the kinematic state in order to 
        /// attach it or not.
        /// </summary>
        /// <param name="_hasGravity"></param>
        internal void SetGravityAndAttach(bool _hasGravity)
        {
            if (IsGravityLocked())
            {
                WarnUnauthorizedGravityChange();
                return;
            }

            Rigidbody rb = GetRigitBodyOrError();

            if (rb == null)
                return;

            rb.isKinematic = !_hasGravity;
            rb.useGravity = _hasGravity;

            m_ActualGravityState = _hasGravity ? S_GravityState.ACTIVE_NOT_KINEMATIC : S_GravityState.UNACTIVE_NOT_KINEMATIC;

        }

        internal Rigidbody GetRigitBodyOrError()
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null && S_GravityManager.SHOW_GRAVITY_WARNINGS)
            {
                Debug.LogWarning(String.Format("The interactive object {0} has not a rigidbody and it should, please add a rigidbody to it.", gameObject.name));
            }

            return rb;
        }

        /// <summary>
        /// This method is called if we detect gravity inconsistencies in order to prevent
        /// developpers to change the gravity directly, 
        /// </summary>
        public void WarnUnauthorizedGravityChange()
        {
            Debug.LogWarning("Gravity chages should be only performed by the GravityManager!");
        }

        public bool IsGravityLocked()
        {
            return m_ActualGravityState == S_GravityState.LOCKED;
        }

        public bool IsAffectedByGravity()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            return rb != null && !rb.isKinematic && rb.useGravity;
        }
#endregion GravityManagement

#region ManipulationManagement
        public void EnableManipulationMode(S_ManipulationModes _manipulationMode)
        {
                ManipulationMode = _manipulationMode;
                           
        }

        public void DisableManipulationMode(S_ManipulationModes manipulationMode)
        {            
                if (ManipulationMode == manipulationMode)
                    ManipulationMode = S_ManipulationModes.NONE;
        }

        public bool IsPointedWithLeftHand;

        public bool IsPointedWithRightHand;

        private void OnControllerPointingEvent(S_ControllerPointingEventArgs e)
        {
            if (e.Dico.Value.Equals(gameObject))
            {
                if (e.KeyValue.Key == UnityEngine.XR.XRNode.LeftHand)
                {
                    IsPointedWithLeftHand = e.IsPointed;
                }
                else
                {
                    IsPointedWithRightHand = e.IsPointed;
                }
            }
        }

#endregion ManipulationManagement

        private static void CreateSchedulerIfNeeded()
        {
            S_Scheduler scheduler = FindObjectOfType<S_Scheduler>();
            if (scheduler == null)
            {
                new GameObject("Scheduler").AddComponent<S_Scheduler>();
            }
        }
    }
}
