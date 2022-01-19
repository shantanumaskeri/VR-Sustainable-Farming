// <copyright file="Gaze_GrabManager.cs" company="apelab sàrl">
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Analytics;

/// <summary>
/// Grab manager handles all grab mechanisme.
/// This script is attached, at run time, on the controllers.
[assembly: InternalsVisibleTo("S_HandIODetectorFeedback")]
[assembly: InternalsVisibleTo("S_HandIODetectorKernel")]
namespace SpatialStories
{
    public class S_GrabManager : MonoBehaviour
    {
        private S_InteractiveObject m_OwnerInteractiveObject;
        public S_InteractiveObject OwnerInteractiveObject
        {
            get
            {
                if (m_OwnerInteractiveObject == null)
                {
                    m_OwnerInteractiveObject = GetOwnerInteractiveObject();
                }

                return m_OwnerInteractiveObject;
            }
        }

        #region members
        public GameObject CollidingObject;
        /// <summary>
        /// Stores all the grab managers in order to make operations with them
        /// </summary>
        public static List<S_GrabManager> GrabManagers = new List<S_GrabManager>();
        public GameObject GrabbedObject;
        /// <summary>
        /// true if this is the left hand controller
        /// </summary>
        public bool IsLeftHand;
        public bool DisplayGrabPointer = false;
        public bool DisplayGlobalPointer = false;
        public float LaserStartWidth, LaserEndWidth, LaserLength = 5f;
        public Material LaserMaterial;
        public Color LaserColor = Color.white;
        public GameObject LaserPointObject;
        public Transform ControllerSnapTransform { get { return m_ControllerSnapTransform; } }
        public bool ShowDebugInfo = false;
        public S_InteractiveObject InteractableIO;
        public bool IsTriggerPressed { get { return m_IsTriggerPressed; } }
        public float UpdateInterval = .2f;
        public Transform GrabPosition;
        public List<GameObject> HitsIos;

        public float GrabLaserStartWidth, GrabLaserEndWidth;

        private float m_CurrentUpdateInterval;
        private bool m_IsGrabbing = false;
        private Transform m_ControllerSnapTransform;
        private bool m_IsTriggerPressed = false;
        private List<GameObject> m_ObjectsInManipulation = new List<GameObject>();
        private S_TouchDistanceMode m_SearchDistanceState;
        private Dictionary<string, object> m_AnalyticsDico;
        private S_ControllerTouchEventArgs m_GazeControllerTouchEventArgs;
        private GameObject m_singleLaserPointObject;

        internal GameObject DistantGrabOrigin;
        internal LineRenderer LaserPointer;
        internal bool IntersectsWithInteractiveIO;
        internal S_GrabManagerState m_grabState;
        internal Vector3 HitPosition;
        internal float CloserDistance = 0;
        internal S_InteractiveObject CloserIO;
        internal S_InteractiveObject AnyIntersectedIO;
        internal S_HandIODetectorKernel IoDetectorKernel;
        internal S_HandIODetectorFeedback IoDetectorFeedback;

        // Distant grab feedbacks
        public float DefaultDistantGrabRayLength = 30f;
        
        public GameObject InteractableDistantGrabFeedback;
        public Color InteractableDistantGrabColor = Color.blue;
        public Color InteractableInRangeDistantGrabColor = Color.green;

        public GameObject NotInteractableDistantGrabFeedback;
        public Color NotInteractableDistantGrabColor = Color.white;

        public GameObject NotInteractableDistantAnyFeedback;
        public Color NotInteractableDistantAnyFeedbackColor = Color.white;

        private GameObject m_singleNotInteractableDistantGrabFeedback, m_singleInteractableDistantGrabFeedback;

        public S_GrabManagerState GrabState
        {
            get { return m_grabState; }
            set {
                m_grabState = value;
                // Debug.LogError("m_grabState=" + m_grabState.ToString());
            }
        }
        
        public GameObject SingleLaserPointObject
        {
            get { return m_singleLaserPointObject; }
        }
        public GameObject SingleNotInteractableDistantGrabFeedback
        {
            get { return m_singleNotInteractableDistantGrabFeedback; }
        }
        public GameObject SingleInteractableDistantGrabFeedback
        {
            get { return m_singleInteractableDistantGrabFeedback; }
        }

        #endregion

        #region CachedVariables

        private float m_LastUpdateTime;
        #endregion CachedVariables

        void OnEnable()
        {
            S_InputManager.Instance.RightTriggerButtonPressEvent += OnHandRightDownEvent;
            S_InputManager.Instance.RightTriggerButtonReleaseEvent += OnHandRightUpEvent;
            S_InputManager.Instance.LeftTriggerButtonPressEvent += OnHandLeftDownEvent;
            S_InputManager.Instance.LeftTriggerButtonReleaseEvent += OnHandLeftUpEvent;

            S_InputManager.Instance.RightGripButtonPressEvent += OnHandRightDownEvent;
            S_InputManager.Instance.RightGripButtonReleaseEvent += OnHandRightUpEvent;
            S_InputManager.Instance.LeftGripButtonPressEvent += OnHandLeftDownEvent;
            S_InputManager.Instance.LeftGripButtonReleaseEvent += OnHandLeftUpEvent;

            S_EventManager.OnGrabEvent += OnGrabEvent;
            S_InputManager.Instance.OnControllerCollisionEvent += OnControllerCollisionEvent;
            S_InputManager.Instance.OnControllerGrabEvent += OnControllerGrabEvent;
            S_InputManager.Instance.OnControllerAttractEvent += OnControllerAttractEvent;

            S_EventManager.OnIODestroyed += OnIODestroyed;

            // get the snap location from the controller
            m_ControllerSnapTransform = gameObject.GetComponentInChildren<S_GrabPositionController>().transform;
            GrabState = S_GrabManagerState.EMPTY;

            // Add this grab manager to the list
            GrabManagers.Add(this);
        }

        private void RemoveAllListeners()
        {
            S_EventManager.OnGrabEvent -= OnGrabEvent;
            S_EventManager.OnIODestroyed -= OnIODestroyed;
            if (S_InputManager.Instance != null)
            {
                S_InputManager.Instance.OnControllerCollisionEvent -= OnControllerCollisionEvent;
                S_InputManager.Instance.OnControllerGrabEvent -= OnControllerGrabEvent;
                S_InputManager.Instance.OnControllerAttractEvent -= OnControllerGrabEvent;

                S_InputManager.Instance.RightTriggerButtonPressEvent -= OnHandRightDownEvent;
                S_InputManager.Instance.RightTriggerButtonReleaseEvent -= OnHandRightUpEvent;
                S_InputManager.Instance.LeftTriggerButtonPressEvent -= OnHandLeftDownEvent;
                S_InputManager.Instance.LeftTriggerButtonReleaseEvent -= OnHandLeftUpEvent;

                S_InputManager.Instance.RightGripButtonPressEvent -= OnHandRightDownEvent;
                S_InputManager.Instance.RightGripButtonReleaseEvent -= OnHandRightUpEvent;
                S_InputManager.Instance.LeftGripButtonPressEvent -= OnHandLeftDownEvent;
                S_InputManager.Instance.LeftGripButtonReleaseEvent -= OnHandLeftUpEvent;
            }

            // Remove this grab manager from the list
            GrabManagers.Remove(this);
        }

        void OnDisable()
        {
            RemoveAllListeners();
        }

        /// <summary>
        /// If this game object gets destroyed we need to remove this reference from the list
        /// </summary>
        private void OnDestroy()
        {
            RemoveAllListeners();
        }

        void Start()
        {
            DisplayGlobalPointer = !S_InputManager.Instance.HideLaser;
            LaserStartWidth = S_InputManager.Instance.DefaultLaserStartWidth;
            LaserEndWidth = S_InputManager.Instance.DefaultLaserEndWidth;
            LaserLength = S_InputManager.Instance.DefaultLaserLength;
            LaserMaterial = S_InputManager.Instance.DefaultLaserMaterial;
            LaserColor = S_InputManager.Instance.DefaultLaserColor;
            LaserPointObject = S_InputManager.Instance.DefaultLaserPointObject;

            DisplayGrabPointer = !S_InputManager.Instance.HideGrabPoint;
            GrabLaserStartWidth = S_InputManager.Instance.GrabLaserStartWidth;
            GrabLaserEndWidth = S_InputManager.Instance.GrabLaserEndWidth;
            DefaultDistantGrabRayLength = S_InputManager.Instance.GrabLaserLength;

            InteractableDistantGrabColor = S_InputManager.Instance.GrabLaserInRangeColor;
            InteractableInRangeDistantGrabColor = S_InputManager.Instance.GrabLaserInRangeColor;
            NotInteractableDistantGrabColor = S_InputManager.Instance.GrabLaserOutRangeColor;

            InteractableDistantGrabFeedback = S_InputManager.Instance.GrabLaserInteractablePointObject;
            NotInteractableDistantGrabFeedback = S_InputManager.Instance.GrabLaserNonInteractablePointObject;
            NotInteractableDistantAnyFeedback = S_InputManager.Instance.GrabLaserNonInteractablePointObject;

            switch (S_InputManager.Instance.HideLaserSelected)
            {
                case S_HandsEnum.LEFT:
                    if (IsLeftHand)
                    {
                        if (!DisplayGlobalPointer)
                        {
                            DisplayGrabPointer = false;
                        }
                        GrabPosition = S_InputManager.Instance.GrabTargetPositionLeft.transform;
                    }
                    break;

                case S_HandsEnum.RIGHT:
                    if (!IsLeftHand)
                    {
                        if (!DisplayGlobalPointer)
                        {
                            DisplayGrabPointer = false;
                        }
                        GrabPosition = S_InputManager.Instance.GrabTargetPositionRight.transform;
                    }
                    break;

                case S_HandsEnum.BOTH:
                    if (!DisplayGlobalPointer)
                    {
                        DisplayGrabPointer = false;
                    }
                    break;
            }

            DistantGrabOrigin = GetComponentInChildren<S_DistantGrabPointer>().gameObject;
            if (DisplayGrabPointer)
            {
                if (GetComponent<LineRenderer>() == null)
                    LaserPointer = gameObject.AddComponent<LineRenderer>();
                else
                    LaserPointer = GetComponent<LineRenderer>();
                LaserPointer.enabled = false;
                LaserPointer.startWidth = LaserStartWidth;
                LaserPointer.endWidth = LaserEndWidth;
                LaserPointer.positionCount = 2;
                LaserPointer.material = LaserMaterial;
                LaserPointer.startColor = Color.white;
                LaserPointer.endColor = LaserColor;
                LaserPointer.enabled = true;

                if (LaserPointObject != null)
                {
                    m_singleLaserPointObject = Instantiate(LaserPointObject, transform);
                }
                if (InteractableDistantGrabFeedback != null)
                {
                    m_singleInteractableDistantGrabFeedback = Instantiate(InteractableDistantGrabFeedback, transform);
                }
                if (NotInteractableDistantGrabFeedback != null)
                {
                    m_singleNotInteractableDistantGrabFeedback = Instantiate(NotInteractableDistantGrabFeedback, transform);
                }
            }

            HitsIos = new List<GameObject>();
            m_AnalyticsDico = new Dictionary<string, object>();
            
            m_GazeControllerTouchEventArgs = new S_ControllerTouchEventArgs(this.gameObject);            
            m_LastUpdateTime = Time.time;

            IoDetectorFeedback = new S_HandIODetectorFeedback(this);
            IoDetectorKernel = new S_HandIODetectorKernel(this);

            IoDetectorKernel.Setup();
            IoDetectorFeedback.Setup();
        }
       
        private void FixedUpdate()
        {
            // check if previous frame we hit an IO with the pointing controller

            if (Time.time >= m_LastUpdateTime)
            {
                //Debug.Log(interactableIO +" isTriggerPressed = "+isTriggerPressed);
                if (GrabState == S_GrabManagerState.EMPTY || GrabState == S_GrabManagerState.SEARCHING)
                    IoDetectorKernel.Update();

                // update last time value
                m_LastUpdateTime = Time.time;
            }

            UpdateGrabState();
        }

        public List<GameObject> GetManipulableListOfObjects()
        {
            List<GameObject> manipulableObjects = new List<GameObject>();

            Collider myManipulation = OwnerInteractiveObject.Manipulation.GetComponent<Collider>();

            for (int i = m_ObjectsInManipulation.Count - 1; i >= 0; i--)
            {
                GameObject go = m_ObjectsInManipulation[i];

                if (go == null)
                {
                    m_ObjectsInManipulation.RemoveAt(i);
                    continue;
                }

                S_InteractiveObject io = go.GetComponent<S_InteractiveObject>();

                if ((io.IsGrabEnabled || io.IsTouchEnabled) &&
                    io.Manipulation.GetComponent<Collider>().bounds.Intersects(myManipulation.bounds))
                {
                    manipulableObjects.Add(io.gameObject);
                }
            }

            return manipulableObjects;
        }

        // Really unoptimized... But doing a simple GetComponentInParent<S_InteractiveObject>
        // might return the wrong S_InteractiveObject in complex IO hierarchies. This makes sure
        // we land on our feet.
        // A potential better fix would be a reference set at Editor time
        private S_InteractiveObject GetOwnerInteractiveObject()
        {
            S_InteractiveObject[] interactiveObjects = GetComponentsInParent<S_InteractiveObject>();

            foreach (S_InteractiveObject interactiveObject in interactiveObjects)
            {
                S_GrabManager grabManager = interactiveObject.GetComponentInChildren<S_GrabManager>();
                if (grabManager != null &&
                    grabManager == this)
                {
                    return interactiveObject;
                }
            }

            return null;
        }

        /// <summary>
        /// This list contains objects which are either Grabable, Touchable or Levitable
        /// and colliding with a controller (in proximity).
        /// If an object has been dropped maybe is still on the proximity list but
        /// he wont be removed from it because the colliders has been deactivated
        /// by calling this method all the objects that are in the proximity list
        /// without being catchable anymore will be removed from it.
        /// </summary>
        public void UpdateManipulationList()
        {
            Collider myManipulation = OwnerInteractiveObject.Manipulation.GetComponent<Collider>();

            for (int i = m_ObjectsInManipulation.Count - 1; i >= 0; i--)
            {
                GameObject go = m_ObjectsInManipulation[i];

                if (go == null)
                {
                    m_ObjectsInManipulation.RemoveAt(i);
                    continue;
                }
                S_InteractiveObject io = go.GetComponent<S_InteractiveObject>();

                if (!io.Manipulation.GetComponent<Collider>().bounds.Intersects(myManipulation.bounds))
                {
                    m_ObjectsInManipulation.RemoveAt(i);
                    continue;
                }

                // if it's not an Interactive Object OR it's neither grabbable nor touchable, remove it !
                if (io == null || (!io.IsGrabEnabled && !io.IsTouchEnabled))
                    m_ObjectsInManipulation.RemoveAt(i);
            }
        }

        /// <summary>
        /// This is an state machine thats handles all the logic of grabbing an
        /// object.
        /// </summary>
        private void UpdateGrabState()
        {
            switch (GrabState)
            {
                case S_GrabManagerState.SEARCHING:
                    SearchValidObjects();
                    break;

                case S_GrabManagerState.ATTRACTING:
                    AttractObjectToHand();
                    break;

                case S_GrabManagerState.GRABBING:
                    GrabObject();
                    break;

                case S_GrabManagerState.GRABBED:
                    break;

                case S_GrabManagerState.EMPTY:
                    break;
            }
        }

        private void Touch(S_InteractiveObject _interactableIO)
        {
            if (_interactableIO.IsTouchEnabled)
            {
                // set the dico members
                UnityEngine.XR.XRNode vrNode = IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand;

                // fire the touch event
                m_GazeControllerTouchEventArgs.Dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(vrNode, _interactableIO.gameObject);
                m_GazeControllerTouchEventArgs.Mode = m_SearchDistanceState;
                m_GazeControllerTouchEventArgs.IsTouching = true;
                m_GazeControllerTouchEventArgs.IsTriggerPressed = m_IsTriggerPressed;
                S_InputManager.Instance.FireControllerTouchEvent(m_GazeControllerTouchEventArgs);

                // analytics
                m_AnalyticsDico.Clear();
                m_AnalyticsDico.Add("Grabbed Object", _interactableIO.name);
                m_AnalyticsDico.Add("Is Left Hand", IsLeftHand);
                Analytics.CustomEvent("Grab", m_AnalyticsDico);

                // set state to EMPTY
                GrabState = S_GrabManagerState.EMPTY;
            }
        }

        /// <summary>
        /// Used in the new grab logic in order to search an object to take one important thing about this method is
        /// that it priorizes the objects in manipulation over the distant grab.
        /// Update : this method now search for any valid object which are : Grabable, Touchable OR Levitable
        /// </summary>
        private void SearchValidObjects()
        {
            List<GameObject> manipulableObjects = GetManipulableListOfObjects();

            // if there are no object in proximity, Distant Grab
            if (manipulableObjects.Count == 0)
            {
                // If we've found something to grab, update the grab state
                if (CloserIO != null)
                {
                    InteractableIO = CloserIO;

                    if (InteractableIO.IsGrabEnabled)
                    {
                        if (InteractableIO.GrabLogic.IsBeingGrabbed)
                        {
                            InteractableIO.GrabLogic.SetManipulationMode(true);
                        }

                        if (InteractableIO.GrabModeIndex.Equals((int)S_GrabMode.ATTRACT))
                        {
                            GrabState = S_GrabManagerState.ATTRACTING;
                            InteractableIO.GrabLogic.GrabbingManager = this;
                            KeyValuePair<UnityEngine.XR.XRNode, GameObject> dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, InteractableIO.gameObject);
                            S_InputManager.Instance.FireControllerAttractEvent(new S_ControllerGrabEventArgs(this, dico, true, HitPosition));
                        }
                        else if (InteractableIO.GrabModeIndex.Equals((int)S_GrabMode.LEVITATE))
                        {
                            GrabState = S_GrabManagerState.GRABBED;
                            ClearLaserPointer();

                            KeyValuePair<UnityEngine.XR.XRNode, GameObject> dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, InteractableIO.gameObject);
                            S_InputManager.Instance.FireControllerGrabEvent(new S_ControllerGrabEventArgs(this, dico, true, HitPosition));
                        }

                        // analytics
                        m_AnalyticsDico.Clear();
                        m_AnalyticsDico.Add("Grabbed Object", InteractableIO.name);
                        m_AnalyticsDico.Add("Is Left Hand", IsLeftHand);
                        Analytics.CustomEvent("Grab", m_AnalyticsDico);
                    }

                    if (InteractableIO.IsTouchEnabled)
                    {
                        m_SearchDistanceState = S_TouchDistanceMode.DISTANT;
                        Touch(InteractableIO);
                    }
                }
            }

            // the controller is in collision with an IO
            else
            {
                // get the IO being collided with
                InteractableIO = manipulableObjects.ElementAt(0).GetComponent<S_InteractiveObject>();

                if (InteractableIO.IsManipulable && !InteractableIO.GrabLogic.IsBeingManipulated)
                {
                    InteractableIO.GrabLogic.SetManipulationMode(true);
                }

                // if the grabbed object is being manipulated
                if (InteractableIO.GrabLogic.GrabbingManager != null)
                {
                    InteractableIO.GrabLogic.GrabbingManager.GrabbedObject = null;
                    InteractableIO.GrabLogic.GrabbingManager.GrabState = S_GrabManagerState.EMPTY;
                    InteractableIO.GrabLogic.GrabbingManager.TryDetach();

                    // Set the grab point where the controller actually is in order to avoid weird jumps
                    if (m_ControllerSnapTransform != null && InteractableIO!=null)
                    {
                        InteractableIO.GrabLogic.AddDynamicGrabPositioner(m_ControllerSnapTransform.position, this);
                    }

                    // change the state to grabbing.
                    GrabState = S_GrabManagerState.GRABBING;
                }
                else
                {
                    // If the object is not being manipulated we just try to Find an object by the distance method
                    if (InteractableIO.IsTouchEnabled)
                    {
                        m_SearchDistanceState = S_TouchDistanceMode.PROXIMITY;
                        Touch(InteractableIO);
                    }
                    else if (InteractableIO.IsGrabEnabled)
                    {
                        if (InteractableIO.GrabModeIndex.Equals((int)S_GrabMode.ATTRACT))
                        {
                            GrabState = S_GrabManagerState.GRABBING;
                        }
                        else if (InteractableIO.GrabModeIndex.Equals((int)S_GrabMode.LEVITATE))
                        {
                            Vector3 offset = InteractableIO.transform.position - InteractableIO.transform.GetComponentInChildren<S_Manipulation>().GetComponent<Collider>().bounds.center;
                            InteractableIO.transform.position = DistantGrabOrigin.transform.position + (0.3f * DistantGrabOrigin.transform.forward) + offset;

                            // analytics
                            m_AnalyticsDico.Clear();
                            m_AnalyticsDico.Add("Grabbed Object", InteractableIO.name);
                            m_AnalyticsDico.Add("Is Left Hand", IsLeftHand);
                            Analytics.CustomEvent("Grab", m_AnalyticsDico);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attracts the object that we are taking to the grab manager and then changes the state
        /// </summary>
        private void AttractObjectToHand()
        {
            if (InteractableIO == null)
            {
                GrabState = S_GrabManagerState.SEARCHING;
                return;
            }

            // Check if we need to attract more the object and atract it if needed.
            if (IsAttactionNeeded())
            {
                AttractObject();
            }
            else
            {
                // If the object doesn't need to be attracted we can change the state to grabbing.
                GrabState = S_GrabManagerState.GRABBING;
                InteractableIO.transform.position = m_ControllerSnapTransform.position - InteractableIO.GrabLogic.GetGrabPoint();
            }
        }

        /// <summary>
        /// Performs the grab logic
        /// </summary>
        private void GrabObject()
        {
            ClearLaserPointer();
            TryAttach();

            GrabState = S_GrabManagerState.GRABBED;
           
        }

        private void TryAttach()
        {

            if (InteractableIO == null)
                return;

            // get the snap location from the controller
            Transform controllerSnapLocation = gameObject.GetComponentInChildren<S_GrabPositionController>().transform;

            // snap in position if needed
            if (InteractableIO.GrabPositionnerCollider != null)
            {
                // get the snap location from the IO
                Transform grabbedObjectPositionnerTransform = InteractableIO.GrabPositionnerCollider.transform;

                // get the delta vector between object and hand grab location
                Vector3 delta = controllerSnapLocation.position - grabbedObjectPositionnerTransform.position;

                // add the delta to the IO
                InteractableIO.transform.position = InteractableIO.transform.position + delta;

                // parent grabbed object to the hand
                InteractableIO.transform.SetParent(controllerSnapLocation, true);
            }
            else
            {
                // store hand grab positionner rotation
                Quaternion originalHandGrabPositionRotation = controllerSnapLocation.rotation;

                // rotate hand grab positionner like grabbed object's grab location's rotation
                Quaternion rotation = InteractableIO.GrabLogic.DefaultHandle.transform.rotation;

                controllerSnapLocation.rotation = rotation;

                // get the delta vector between object and hand grab location
                Vector3 delta = controllerSnapLocation.position - InteractableIO.GrabLogic.DefaultHandle.transform.position;

                // add the delta to the IO
                InteractableIO.transform.position = InteractableIO.transform.position + delta;


                // parent grabbed object to the hand
                InteractableIO.transform.SetParent(controllerSnapLocation, true);


                // restore original hand grab positionner's rotation (this will rotate the grabbed object to)
                controllerSnapLocation.rotation = originalHandGrabPositionRotation;
            }


            InteractableIO.transform.SetParent(null);

            if (InteractableIO == null)
                return;

            GrabbedObject = InteractableIO.gameObject;

            // notify
            KeyValuePair<UnityEngine.XR.XRNode, GameObject> grabbedObjects = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, GrabbedObject);
            m_IsGrabbing = true;

            S_InputManager.Instance.FireControllerGrabEvent(new S_ControllerGrabEventArgs(this, grabbedObjects, m_IsGrabbing));
        }

        private void ClearLaserPointer()
        {
            if (LaserPointer != null)
                LaserPointer.enabled = false;

            if (m_singleNotInteractableDistantGrabFeedback != null)
                m_singleNotInteractableDistantGrabFeedback.SetActive(false);

            if (m_singleInteractableDistantGrabFeedback != null)
                m_singleInteractableDistantGrabFeedback.SetActive(false);
        }

        /// <summary>
        /// Moves an object to the hand at a certain speed.
        /// </summary>
        private void AttractObject()
        {
            if (InteractableIO == null)
            {
                GrabState = S_GrabManagerState.SEARCHING;
                return;
            }
            InteractableIO.transform.position = Vector3.MoveTowards(InteractableIO.transform.position, m_ControllerSnapTransform.position - InteractableIO.GrabLogic.GetGrabPoint(), InteractableIO.AttractionSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Checks if we need to attract the object that we are trying to grab.
        /// </summary>
        /// <returns></returns>
        private bool IsAttactionNeeded()
        {
            return Vector3.Distance(InteractableIO.transform.position, m_ControllerSnapTransform.position - InteractableIO.GrabLogic.GetGrabPoint()) > 0.1f;
        }

        public void TryDetach()
        {
            if (GrabbedObject != null)
            {
                if (GrabbedObject.GetComponentInParent<S_InteractiveObject>() && GrabbedObject.GetComponent<S_InteractiveObject>().IsSticky)
                    return;

                GameObject grabbedObj = GrabbedObject;
                grabbedObj.transform.SetParent(null);

                KeyValuePair<UnityEngine.XR.XRNode, GameObject> dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, grabbedObj);
                m_IsGrabbing = false;

                S_InputManager.Instance.FireControllerGrabEvent(new S_ControllerGrabEventArgs(this, dico, m_IsGrabbing));
            }

            ClearGrabbingVariables();
            if (LaserPointer != null)
                LaserPointer.enabled = true;
        }

        private void ClearGrabbingVariables()
        {
            GrabbedObject = null;
            InteractableIO = null;
            m_IsGrabbing = false;
        }

        public bool IsObjectInManipulationList(GameObject _gameObject)
        {
            return m_ObjectsInManipulation.Contains(_gameObject);
        }

        public void RemoveObjectInManipulation(GameObject _gameObject)
        {
            m_ObjectsInManipulation.Remove(_gameObject);
        }

        public void AddNewObjectInManipulation(GameObject _gameObject)
        {
            m_ObjectsInManipulation.Insert(0, _gameObject);
        }

        private void UpdateManipulationList(S_InteractiveObject collidingIO, S_ControllerCollisionEventArgs e)
        {
            if (e.CollisionType.Equals(S_CollisionTypes.COLLIDER_ENTER))
            {
                if (GrabState == S_GrabManagerState.SEARCHING || GrabState == S_GrabManagerState.EMPTY || GrabState == S_GrabManagerState.ATTRACTING)
                {
                    if (!IsObjectInManipulationList(collidingIO.gameObject))
                    {
                        AddNewObjectInManipulation(collidingIO.gameObject);
                    }

                    if (m_IsTriggerPressed && InteractableIO && InteractableIO.IsGrabEnabled && InteractableIO.GrabModeIndex.Equals((int)S_GrabMode.ATTRACT))
                    {
                        InteractableIO = m_ObjectsInManipulation.ElementAt(0).GetComponent<S_InteractiveObject>();
                        GrabState = S_GrabManagerState.GRABBING;
                    }
                }
            }
            else if (e.CollisionType == S_CollisionTypes.COLLIDER_EXIT)
            {
                RemoveObjectInManipulation(collidingIO.gameObject);
            }
        }

        private IEnumerator GrabInCertainTime(float _time, S_InteractiveObject _objectToTake)
        {
            yield return new WaitForSeconds(_time);
            TryDetach();
            InteractableIO = _objectToTake;
            GrabState = S_GrabManagerState.ATTRACTING;
        }

        private void TriggerReleased(GameObject _object)
        {
            // notify the trigger is released
            m_IsTriggerPressed = false;
            if (_object)
                Touch(InteractableIO);
        }

#region StaticMethods
        /// <summary>
        /// Enables the grab manager
        /// </summary>
        public void EnableGrabManager()
        {
            GrabState = S_GrabManagerState.EMPTY;
        }

        /// <summary>
        /// Disables the grab manager until the EnableGrabManger method is
        /// called
        /// </summary>
        public void DisableGrabManager()
        {
            TryDetach();
            GrabState = S_GrabManagerState.BLOCKED;
        }

        /// <summary>
        /// Disables the user ability to take objects for a certain ammount of time
        /// </summary>
        /// <param name="seconds"></param>
        public void DisableGrabbingForAmmountOfSeconds(float seconds)
        {
            DisableGrabManager();
            StartCoroutine(UnBlockGrabbingInTime(seconds));
        }

        /// <summary>
        /// Enables the grab manager is a certain ammoount of time (in seconds)
        /// </summary>
        /// <param name="time">Time in Seconds</param>
        /// <returns></returns>
        IEnumerator UnBlockGrabbingInTime(float time)
        {
            yield return new WaitForSeconds(time);
            EnableGrabManager();
        }

        /// <summary>
        /// Enables all the grab managers
        /// </summary>
        public static void EnableAllGrabManagers()
        {
            foreach (S_GrabManager grabManager in GrabManagers)
                grabManager.EnableGrabManager();
        }

        /// <summary>
        /// Disables all the grab managers until the enalbe grab manager method is called
        /// </summary>
        public static void DisableAllGrabManagers()
        {
            foreach (S_GrabManager grabManager in GrabManagers)
                grabManager.DisableGrabManager();
        }

        /// <summary>
        /// Gets the a grabbing hand (Usefull to know if we are already taking an object if the other hand)
        /// </summary>
        /// <returns></returns>
        public static S_GrabManager GetGrabbingHand()
        {
            return GrabManagers.FirstOrDefault(gm => gm.m_IsGrabbing && gm.InteractableIO != null);
        }

        public static List<S_GrabManager> GetGrabbingHands(S_InteractiveObject io)
        {
            return GrabManagers.Where(gm => gm.InteractableIO == io).ToList();
        }

        /// <summary>
        /// Used to disable all the grab for a certain ammount of time
        /// </summary>
        /// <param name="_time"></param>
        public static void DisableGrabForTime(float _time)
        {
            foreach (S_GrabManager grabManager in GrabManagers)
                grabManager.DisableGrabbingForAmmountOfSeconds(_time);
        }
#endregion StaticMethods

#region EventHandlers
        // if the grabbed object is already grabbed by the other controller, let the object go
        private void OnControllerGrabEvent(S_ControllerGrabEventArgs e)
        {
            if (e.IsGrabbing)
            {
                // if the other controller has grabbed an object (! before the isLeftHand)
                if ((e.ControllerObjectPair.Key.Equals(UnityEngine.XR.XRNode.LeftHand) && !IsLeftHand) || (e.ControllerObjectPair.Key.Equals(UnityEngine.XR.XRNode.RightHand) && IsLeftHand))
                {
                    // and this object is the one I'm currently grabbing
                    if (e.ControllerObjectPair.Value == GrabbedObject)
                    {
                        // drop it to let the other controller grab it
                        GrabbedObject = null;
                    }
                }
            }
        }

        private void OnControllerAttractEvent(S_ControllerGrabEventArgs e)
        {
            if (e.IsGrabbing)
            {
               
            }
        }

        private void OnHandRightDownEvent(S_InputEventArgs e)
        {
            if ((S_InputManager.Instance.DefaultGrabAction.BaseInputType != S_BaseInputTypes.INDEX) && (e.InputType == S_InputTypes.RIGHT_TRIGGER_BUTTON_PRESS)) return;
            if ((S_InputManager.Instance.DefaultGrabAction.BaseInputType != S_BaseInputTypes.GRIP) && (e.InputType == S_InputTypes.RIGHT_GRIP_BUTTON_PRESS)) return;

            if (e.VrNode.Equals(UnityEngine.XR.XRNode.RightHand) && !IsLeftHand)
            {
                GrabState = S_GrabManagerState.SEARCHING;
                m_IsTriggerPressed = true;
            }
        }

        private void OnHandRightUpEvent(S_InputEventArgs e)
        {
            if ((S_InputManager.Instance.DefaultGrabAction.BaseInputType != S_BaseInputTypes.INDEX) && (e.InputType == S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE)) return;
            if ((S_InputManager.Instance.DefaultGrabAction.BaseInputType != S_BaseInputTypes.GRIP) && (e.InputType == S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE)) return;

            if (e.VrNode.Equals(UnityEngine.XR.XRNode.RightHand) && !IsLeftHand)
            {
                if (InteractableIO != null)
                {
                    S_EventManager.FireControllerPointingEvent(new S_ControllerPointingEventArgs(this.gameObject, new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, InteractableIO.gameObject), false));
                    TriggerReleased(InteractableIO.gameObject);
                    ResetGrabStateAfterHandUp();
                }
                GrabState = S_GrabManagerState.EMPTY;

            }
        }

        private void OnHandLeftDownEvent(S_InputEventArgs e)
        {
            if ((S_InputManager.Instance.DefaultGrabAction.BaseInputType != S_BaseInputTypes.INDEX) && (e.InputType == S_InputTypes.LEFT_TRIGGER_BUTTON_PRESS)) return;
            if ((S_InputManager.Instance.DefaultGrabAction.BaseInputType != S_BaseInputTypes.GRIP) && (e.InputType == S_InputTypes.LEFT_GRIP_BUTTON_PRESS)) return;

            if (e.VrNode.Equals(UnityEngine.XR.XRNode.LeftHand) && IsLeftHand)
            {
                GrabState = S_GrabManagerState.SEARCHING;
                m_IsTriggerPressed = true;
            }
        }

        private void OnHandLeftUpEvent(S_InputEventArgs e)
        {
            if ((S_InputManager.Instance.DefaultGrabAction.BaseInputType != S_BaseInputTypes.INDEX) && (e.InputType == S_InputTypes.LEFT_TRIGGER_BUTTON_RELEASE)) return;
            if ((S_InputManager.Instance.DefaultGrabAction.BaseInputType != S_BaseInputTypes.GRIP) && (e.InputType == S_InputTypes.LEFT_GRIP_BUTTON_RELEASE)) return;

            if (e.VrNode.Equals(UnityEngine.XR.XRNode.LeftHand) && IsLeftHand)
            {
                if (InteractableIO != null)
                {
                    Dictionary<UnityEngine.XR.XRNode, GameObject> dico = new Dictionary<UnityEngine.XR.XRNode, GameObject>();
                    dico.Add(IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, InteractableIO.gameObject);
                    S_EventManager.FireControllerPointingEvent(new S_ControllerPointingEventArgs(this.gameObject, new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, InteractableIO.gameObject), false));
                    TriggerReleased(InteractableIO.gameObject);
                    ResetGrabStateAfterHandUp();
                }
                GrabState = S_GrabManagerState.EMPTY;
            }
        }

        /// <summary>
        /// Change the state of the grabmanager to the correct one after releasing the hand
        /// button.
        /// </summary>
        private void ResetGrabStateAfterHandUp()
        {
            KeyValuePair<UnityEngine.XR.XRNode, GameObject> dico;
            //Gaze_GravityManager.ChangeGravityState(interactableIO, Gaze_GravityRequestType.ACTIVATE_AND_DETACH);

            // If we where attracting we need to to add graviy again to the IO
            if (InteractableIO != null)
                dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, InteractableIO.gameObject);
            else
                dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, null);

            S_InputManager.Instance.FireControllerGrabEvent(new S_ControllerGrabEventArgs(this, dico, false));
            S_GravityManager.ChangeGravityState(InteractableIO, S_GravityRequestType.RETURN_TO_DEFAULT);

            // If the IO was on our hands we need to trydeatch
            TryDetach();

            // Reset other variables
            m_IsTriggerPressed = false;
            GrabState = S_GrabManagerState.EMPTY;
            IoDetectorKernel.ClearRaycasts();
        }

        private void OnGrabEvent(S_GrabEventArgs e)
        {
            if (e.GrabManager.GetInstanceID() != GetInstanceID())
                return;

            StartCoroutine(GrabInCertainTime(e.TimeToGrab, e.InteractiveObject));
        }

        /// <summary>
        /// Calls on controller collision event.
        /// Grabbed objects have to be Interactive Object ('Gaze objects')
        /// 'Other' is either the Proximity for Interactive Objects or the object itself.
        /// 'Sender' is always the controller triggering the event
        /// </summary>
        /// <param name="_e">E.</param>
        /// <summary>
        /// Calls on controller collision event.
        /// Grabbed objects have to be Interactive Object ('Gaze objects')
        /// 'Other' is either the Proximity for Interactive Objects or the object itself.
        /// 'Sender' is always the controller triggering the event
        /// </summary>
        /// <param name="_e">E.</param>
        private void OnControllerCollisionEvent(S_ControllerCollisionEventArgs _e)
        {
            // If the grab manager is not this one we dont care
            if (_e.GrabManger != this)
                return;

            GameObject colObject = S_Utils.ConvertIntoGameObject(_e.Other);

            S_InteractiveObject IO = colObject.GetComponentInParent<S_InteractiveObject>();

            if (IO == null)
                return;

            if (IO.ManipulationMode == S_ManipulationModes.NONE)
                return;

            if (IO.GrabLogic.GrabbingManager != null && IO.ManipulationMode == S_ManipulationModes.LEVITATE)
                return;

            // Discart hands
            if (IO.GetComponentInChildren<S_HandController>())
                return;

            // Discard Head
            if (IO.GetComponent<S_Head>() != null)
                return;

            // Check if the colliding object has a gaze handle in order to avoid noise.
            colObject = (GameObject)_e.Other;

            // exit if the sender is not this Controller
            GameObject senderController = ((GameObject)_e.Sender).GetComponentInParent<S_InteractiveObject>().GetComponentInChildren<S_HandController>().gameObject;
            if (senderController != this.gameObject)
                return;

            // if colliding object is a Gaze_InteractiveObject
            S_InteractiveObject collidingIo = ((GameObject)_e.Other).GetComponentInParent<S_InteractiveObject>();

            // If its the object that we are already taking its not necessary to process
            if (collidingIo.GrabLogic.IsBeingGrabbed && collidingIo.GrabLogic.GrabbingManager == this)
                return;

            if (collidingIo != null)
            {
                // if it's catchable, touchable, levitable OR if it's manipulated and we're colliding with the manipulable collider
                if (collidingIo.IsGrabEnabled || 
                    collidingIo.IsTouchEnabled || 
                    (collidingIo.GrabLogic.IsBeingManipulated && 
                    collidingIo.ManipulableHandle.Equals
                    (_e.Other.GetComponent<Collider>())))
                {
                    UpdateManipulationList(collidingIo, _e);
                }
            }
        }

        /// <summary>
        /// Clear all the lists where the destroyed IO was in order to 
        /// prevent null reference exceptions.
        /// </summary>
        /// <param name="args"></param>
        private void OnIODestroyed(S_IODestroyEventArgs args)
        {

            if (IoDetectorFeedback != null)
                IoDetectorKernel.RemoveDestroyedIOFromRaycasts(args.IO.gameObject);

            if (HitsIos != null)
                HitsIos.Remove(args.IO.gameObject);

            if (m_ObjectsInManipulation != null)
                m_ObjectsInManipulation.Remove(args.IO.gameObject);

            if (GrabbedObject == args.IO.gameObject)
                GrabbedObject = null;

            if (CollidingObject == args.IO.gameObject)
                CollidingObject = null;
        }

#endregion EventHandlers

        private void LateUpdate()
        {
            IoDetectorFeedback.Update();
        }
    }
}
