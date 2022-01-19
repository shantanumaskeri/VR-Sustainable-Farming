using Gaze;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SpatialStories
{
    public class S_GrabLogic
    {
        #region constants

        /// <summary>
        /// USed to recalibrate the center of mass when the object is being manipulated.
        /// </summary>
        private const float CENTER_OF_MASS_CORRECTION_THRESHOLD = 0.05f;

        /// <summary>
        /// How many seconds the object will resist far of the point before dropping it.
        /// </summary>
        const float DEFAULT_TIME_UNTIL_DETACH = 5f;

        private float actualTimeUntilDetach = DEFAULT_TIME_UNTIL_DETACH;

        /// <summary>
        /// How much time will the object be considered in maniplation after 
        /// he is not moving anymore and it has been released.
        /// </summary>
        public float DISABLE_MANIPULATION_TIME = 1f;

        /// <summary>
        /// If the distance of the io to to the controler is more than this constant for more thanb
        /// a certain time it will be detached.
        /// </summary>
        public const float DEFAULT_DETACH_DISTANCE = 0.05f;

        private float actualDetachDistance = DEFAULT_DETACH_DISTANCE;

        /// <summary>
        /// This const determines how agressive will be the position correction to track an object
        /// as bigger as it gets more attached to the tracking point the IO will be.
        /// </summary>
        private const float VELOCITY_CONST = 6000f;

        /// <summary>
        /// How fast the rotation of the object will be corrected.
        /// </summary>
        private const float ANGULAR_VELOCITY_CONST = 100f;

        /// <summary>
        /// Expected delta time.
        /// </summary>
        private const float EXPECTED_DELTA_TIME = 0.0111f;

        /// <summary>
        /// Number of samples that will be used to calculate the throw parameters when an object is released.
        /// </summary>
        public const int SAMPLES = 21;

        private const float MAX_VELOCITY_CHANGE = 10f;
        private const float MAX_ANGULAR_VELOCITY_CHANGE = 20f;

        #endregion constants

        #region vars

        /// <summary>
        /// IO
        /// </summary>
        private S_InteractiveObject m_Owner;

        /// <summary>
        /// /History of angular velocities and positions usefull in the moment of calculating the throw force of an object
        /// </summary>
        public Vector3?[] AngularVelocityHistory = new Vector3?[SAMPLES];

        private int m_AngularVelocityTrackingIndex = 0;
        public List<Vector3> PositionHistory = new List<Vector3>();

        /// <summary>
        /// As we are changing the center of mass in order to avoid flickering of long objects
        /// we need to have a way to go back to the initial center of mass.
        /// </summary>
        private Vector3 m_OriginalCenterOfMass;

        /// <summary>
        /// The current grab manager that is grabbing this object
        /// </summary>
        public S_GrabManager GrabbingManager;

        /// <summary>
        /// This flag defines when an object has been grabbed.
        /// </summary>;
        private bool m_IsBeingGrabbed = false;

        public bool IsBeingGrabbed
        {
            get { return m_IsBeingGrabbed; }
        }

        /// <summary>
        /// This bool is used to check if the object is being manipulated
        /// </summary>
        private bool m_IsBeingManipulated = false;

        public bool IsBeingManipulated
        {
            get { return m_IsBeingManipulated; }
        }

        /// <summary>
        /// It's the collider used to manipulate the object.
        /// </summary>
        public Collider ManipulabeHandle;

        /// <summary>
        /// Grab Point Used in ManipualtionF
        /// </summary>
        private GameObject m_ActualGrabPoint = null;

        private readonly Rigidbody m_RigidBody;

        /// <summary>
        /// USefull to check if the object hasn't move since the last frame (Manipulation)
        /// </summary>
        private Vector3 m_LastPosition;

        /// <summary>
        /// If the object is far of the grab point this var will decrease if it reaches 0 it will be detached.
        /// </summary>
        private float m_RemainingTimeUntilDetach = DEFAULT_TIME_UNTIL_DETACH;

        /// <summary>
        /// Determiens where is the grab point of the actual grabbing controller
        /// </summary>
        private Transform m_ControllerGrabLocation;

        private bool m_IsBeingReleasedBecauseOfDistance = false;

        /// <summary>
        /// Determines where the object will be hold if it has not a grab positioner.
        /// </summary>
        private S_Manipulation m_DefaultHandle;

        public S_Manipulation DefaultHandle
        {
            get
            {
                if (m_DefaultHandle == null) m_DefaultHandle = m_Owner.GetComponentInChildren<S_Manipulation>();
                return m_DefaultHandle;
            }
        }

        protected Vector3 m_ExternalVelocity;
        protected Vector3 m_ExternalAngularVelocity;

        private bool m_FirstCatch = true;
        private bool m_IsEnabled = false;
        
        #endregion vars

        public S_GrabLogic(S_InteractiveObject _owner)
        {
            m_Owner = _owner;
            m_RigidBody = m_Owner.GetRigitBodyOrError();
            if (m_RigidBody == null)
                return;
            m_OriginalCenterOfMass = m_RigidBody.centerOfMass;

            Time.fixedDeltaTime = EXPECTED_DELTA_TIME;
            m_RigidBody.drag = 0;
            m_RigidBody.angularDrag = 0.05f;
            m_RigidBody.maxAngularVelocity = MAX_ANGULAR_VELOCITY_CHANGE;
        }

        //TODO: Remove
        int timesTried = 0;
          //needed when instantiating camera instead of having it in the scene
        async void tryAgain()
        {
                await Task.Delay(1000);
                subscribeToOnControllerGrabEvent();
        }
        
        void subscribeToOnControllerGrabEvent()
        {
            if (timesTried>0)
            {
                //Debug.Log("Retrying now...");
            }
            if (S_InputManager.Instance == null) //keep waiting for instance to not being null(which will happen when the user connects
            {
                timesTried++;
               // Debug.Log("Delaying...");
                tryAgain();
                //Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ => subscribeToOnControllerGrabEvent());

            }
            else
            {         
            //needed when instantiating camera instead of having it in the scene

                S_InputManager.Instance.OnControllerGrabEvent += OnControllerGrabEvent;
                S_InputManager.Instance.OnControllerAttractEvent += OnControllerAttractEvent;
               // Debug.Log("On Controller Grab Event registered - grab logic : tries:" + timesTried.ToString());
            }
        }

        public void SubscribeToEvents()
        {
            if (m_IsEnabled)
                return;

            
            subscribeToOnControllerGrabEvent();
            S_EventManager.OnTeleportEvent += OnTeleportEvent;
            S_EventManager.OnDragAndDropEvent += OnDragAndDropEvent;

            m_IsEnabled = true;
        }

        public void UnsubscribeToEvents()
        {
            if (!m_IsEnabled)
                return;
            
            if (S_InputManager.Instance!=null) S_InputManager.Instance.OnControllerGrabEvent -= OnControllerGrabEvent;
            S_EventManager.OnTeleportEvent -= OnTeleportEvent;
            S_EventManager.OnDragAndDropEvent -= OnDragAndDropEvent;

            m_IsEnabled = false;
        }


        /// <summary>
        /// Used to update the grab state of this game s
        /// </summary>
        /// <param name="e"></param>
        public void OnControllerGrabEvent(S_ControllerGrabEventArgs _e)
        {
            // Mark the object as grabbed or ungrabbed
            if (_e.ControllerObjectPair.Value != m_Owner.gameObject) return;

            m_IsBeingGrabbed = _e.IsGrabbing;
            
            // Handle all the manipulation states
            if (m_IsBeingGrabbed)
                GrabObject(_e);
            else
                ReleaseObject();
        }

        public void OnControllerAttractEvent(S_ControllerGrabEventArgs _e)
        {
            // Mark the object as grabbed or ungrabbed
            if (_e.ControllerObjectPair.Value != m_Owner.gameObject) return;

        }

        public void OnDragAndDropEvent(S_DragAndDropEventArgs _e)
        {
            if (!S_Utils.GetIOFromObject(_e.DropObject).GetInstanceID().Equals(m_Owner.GetInstanceID()))
                return;
        }

        public void Update()
        {
            if (m_IsBeingGrabbed && m_Owner.ManipulationMode != S_ManipulationModes.LEVITATE)
            {
                FollowHand();
                AddExternalVelocities();
            }

            if (m_IsBeingManipulated)
                m_LastPosition = m_Owner.transform.position;
        }

        /// <summary>
        /// The IO will follow the grabbing controller
        /// </summary>
        public void FollowHand()
        {
            FollowPoint(m_ControllerGrabLocation);
        }

        /// <summary>
        /// Sets the tolerance of distance between the following point and the object
        /// like that if the follow point goes through a wall the object won't begin the detach process
        /// until this ditance is reached
        /// </summary>
        /// <param name="_distance"></param>
        public void SetDistanceTolerance(float _distance)
        {
            actualDetachDistance = _distance;
        }

        /// <summary>
        /// Set the detach time since the distance has been superated.
        /// </summary>
        /// <param name="_timeTolerance"></param>
        public void SetTimeTolerance(float _timeTolerance)
        {
            actualTimeUntilDetach = _timeTolerance;
        }

        /// <summary>
        /// The IO will follow the specified transform
        /// </summary>
        /// <param name="_transformToFollow">The transform that the IO will follow (Ex: A controller)</param>
        /// <param name="_followOriginTransform"> Optional parameter to choose a custom follow origin point </param>
        public void FollowPoint(Transform _transformToFollow, Transform _followOriginTransform = null)
        {
            FollowPhysicPoint(_transformToFollow, _followOriginTransform);
            AddExternalVelocities();
        }

        /// <summary>
        /// TODO: Spit this method i little ones
        /// </summary>
        /// <param name="_transformToFollow"></param>
        /// <param name="_followOriginTransform"></param>
        private void FollowPhysicPoint(Transform _transformToFollow, Transform _followOriginTransform = null)
        {
            Quaternion rotationDelta;
            Vector3 desiredPosition;

            // This is used for levitating and object
            if (_followOriginTransform != null)
            {
                if (!m_Owner.SnapOnGrab)
                {
                    rotationDelta = CalcRotationDelta(_transformToFollow.transform.rotation, _followOriginTransform.transform.rotation);
                }
                else
                {
                    rotationDelta = CalcRotationDelta(_transformToFollow.transform.rotation, Quaternion.Euler(_followOriginTransform.transform.rotation.eulerAngles + GetSnapPointForHand(GrabbingManager.IsLeftHand).transform.localEulerAngles));
                }
                desiredPosition = CalcDesiredPosition(_transformToFollow.transform.position, _followOriginTransform.transform.position);
            }
            // This is used when the object is snappable
            else if (m_Owner.SnapOnGrab && (!m_Owner.GrabLogic.m_IsBeingManipulated || m_Owner.GrabPositionnerCollider == null))
            {
                rotationDelta = CalcRotationDelta(_transformToFollow.transform.rotation, GetSnapPointForHand(GrabbingManager.IsLeftHand).transform.rotation);
                desiredPosition = CalcDesiredPosition(_transformToFollow.transform.position, GetSnapPointForHand(GrabbingManager.IsLeftHand).transform.position);
            }
            // This is for manipulabe objects
            else if (m_Owner.GrabPositionnerCollider != null)
            {
                rotationDelta = CalcRotationDelta(_transformToFollow.transform.rotation, m_Owner.GrabPositionnerCollider.transform.rotation);
                desiredPosition = CalcDesiredPosition(_transformToFollow.transform.position, m_Owner.GrabPositionnerCollider.transform.position);
            }
            // This shouldn't be called
            else
            {
                rotationDelta = CalcRotationDelta(_transformToFollow.transform.rotation, DefaultHandle.transform.rotation);
                desiredPosition = CalcDesiredPosition(_transformToFollow.transform.position, DefaultHandle.transform.position);
            }

            UpdateObjectAngularVelocity(rotationDelta);
            UpdateObjectVelocity(desiredPosition);
            UpdatePositionHistory(_transformToFollow);
            CheckIfDetachNeeded(_transformToFollow, _followOriginTransform);
        }

        public Quaternion CalcRotationDelta(Quaternion _rotationToFollow, Quaternion _rotationOffset)
        {
            return _rotationToFollow * Quaternion.Inverse(_rotationOffset);
        }

        public Vector3 CalcDesiredPosition(Vector3 _positionToFollow, Vector3 _offset)
        {
            return _positionToFollow - _offset;
        }

        public void UpdateObjectVelocity(Vector3 positionDelta)
        {
            float velocityMagic = VELOCITY_CONST / (Time.fixedDeltaTime / EXPECTED_DELTA_TIME);

            Vector3 velocityTarget = (positionDelta * velocityMagic) * Time.fixedDeltaTime;
            if (float.IsNaN(velocityTarget.x) == false)
            {
                m_RigidBody.velocity = Vector3.MoveTowards(m_RigidBody.velocity, velocityTarget, MAX_VELOCITY_CHANGE);
            }
        }

        public void UpdatePositionHistory(Transform _transformToFollow)
        {
            if (PositionHistory != null)
            {
                m_AngularVelocityTrackingIndex++;

                if (m_AngularVelocityTrackingIndex >= AngularVelocityHistory.Length)
                    m_AngularVelocityTrackingIndex = 0;

                AngularVelocityHistory[m_AngularVelocityTrackingIndex] = m_RigidBody.angularVelocity;

                if (PositionHistory.Count > SAMPLES)
                    PositionHistory.RemoveAt(PositionHistory.Count() - 1);
                PositionHistory.Insert(0, _transformToFollow.transform.position);
            }
        }

        public void UpdateObjectAngularVelocity(Quaternion rotationDelta)
        {
            float angle;
            Vector3 axis;
            float angularVelocityMagic = ANGULAR_VELOCITY_CONST / (Time.fixedDeltaTime / EXPECTED_DELTA_TIME);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
                angle -= 360;

            if (Math.Abs(angle) > 0.01f)
            {
                Vector3 angularTarget = angle * axis;
                if (float.IsNaN(angularTarget.x) == false)
                {
                    angularTarget = (angularTarget * angularVelocityMagic) * Time.fixedDeltaTime;
                    m_RigidBody.angularVelocity = Vector3.MoveTowards(m_RigidBody.angularVelocity, angularTarget,
                        MAX_ANGULAR_VELOCITY_CHANGE);
                }
            }
        }


        public void CheckIfDetachNeeded(Transform _transformToFollow, Transform _followOriginTransform = null)
        {
            if (m_Owner.ManipulationMode != S_ManipulationModes.LEVITATE)
            {
                if (m_Owner.SnapOnGrab)
                {
                    if (m_Owner.GrabPositionnerCollider == null)
                    {
                        if (IsDetachNeeded(m_ControllerGrabLocation.position, GetSnapPointForHand(GrabbingManager.IsLeftHand).transform.position))
                        {
                            UpdateDetachState();
                        }
                    }
                    else
                    {
                        if (!m_Owner.SnapOnGrab && IsDetachNeeded(m_ControllerGrabLocation.position, m_Owner.GrabPositionnerCollider.transform.position))
                        {
                            UpdateDetachState();
                        }
                    }
                }
                else if (m_Owner.GrabPositionnerCollider != null && (!m_Owner.SnapOnGrab && IsDetachNeeded(m_ControllerGrabLocation.position, m_Owner.GrabPositionnerCollider.transform.position)))
                {
                    UpdateDetachState();
                }
                else if (m_Owner.GrabPositionnerCollider == null && IsDetachNeeded(m_ControllerGrabLocation.position, DefaultHandle.transform.position))
                {
                    UpdateDetachState();
                }
                else
                    m_RemainingTimeUntilDetach = actualTimeUntilDetach;
            }
            else
            {
                Transform desiredPos = _followOriginTransform == null ? m_DefaultHandle.transform : _followOriginTransform;
                if (IsDetachNeeded(desiredPos.transform.position, _transformToFollow.transform.position))
                {
                    UpdateDetachState();
                }
                else
                    m_RemainingTimeUntilDetach = actualTimeUntilDetach;
            }
        }

        public bool IsDetachNeeded(Vector3 _desiredPosition, Vector3 _currentPosition)
        {
            return Vector3.Distance(_desiredPosition, _currentPosition) > actualDetachDistance;
        }

        public void UpdateDetachState()
        {
            m_RemainingTimeUntilDetach -= Time.fixedDeltaTime;
            if (m_RemainingTimeUntilDetach <= 0)
                StopGrabbing(true);
        }

        /// <summary>
        /// Fires the grab stop event in order interrupt the grabbing of an object
        /// </summary>
        public void StopGrabbing(bool _tooFar)
        {
            if (_tooFar)
            {
                m_IsBeingReleasedBecauseOfDistance = true;
            }

            KeyValuePair<UnityEngine.XR.XRNode, GameObject> dico =
                new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(GrabbingManager.IsLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand,
                    m_Owner.gameObject);
            S_InputManager.Instance.FireControllerGrabEvent(new S_ControllerGrabEventArgs(GrabbingManager, dico, false));
        }

        public void AddDynamicGrabPositioner(Vector3 _hitPoint, S_GrabManager _grabManager)
        {
            if (!m_Owner.IsManipulable)
                return;

            // If we dont have a grab position collider just create one.
            if (m_Owner.GrabPositionnerCollider == null)
            {
                GameObject go = new GameObject("Dynamic Grab Point");
                m_Owner.GrabPositionnerCollider = go.AddComponent<BoxCollider>();
                go.hideFlags = HideFlags.HideInHierarchy;
                go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                m_Owner.GrabPositionnerCollider.isTrigger = true;
                go.transform.SetParent(m_Owner.transform);
            }

            m_Owner.GrabPositionnerCollider.transform.position = _hitPoint;
            m_Owner.GrabPositionnerCollider.transform.forward = _hitPoint - Camera.main.transform.position;
            _grabManager.gameObject.GetComponentInChildren<S_GrabPositionController>().transform.forward =
                m_Owner.GrabPositionnerCollider.transform.forward;
        }

        /// <summary>
        /// Clears the history of data needed to trow an object. 
        /// </summary>
        protected virtual void CleanPositionsAndVelocityHistory()
        {
            m_AngularVelocityTrackingIndex = 0;
            PositionHistory.Clear();
            AngularVelocityHistory = new Vector3?[SAMPLES];
        }

        /// <summary>
        /// Returns the grab point in order to inform from where this object should be taken
        /// </summary>
        /// <returns></returns>
        public Vector3 GetGrabPoint()
        {
            S_Manipulation handle = m_Owner.GetComponentInChildren<S_Manipulation>();
            if (handle == null)
                Debug.LogAssertion("An interactive object should have a Gaze_Handle child.");
            Vector3 point = m_ActualGrabPoint != null
                ? m_ActualGrabPoint.transform.position
                : m_Owner.GetComponentInChildren<S_Manipulation>().transform.position;
            return point - m_Owner.transform.position;
        }

        /// <summary>
        /// This method will be called when we need to change the grab point of the
        /// gaze interactive object.
        /// </summary>
        /// <param name="_point"></param>
        public void SetGrabPoint(Vector3 _point)
        {
            if (m_ActualGrabPoint != null)
                GameObject.Destroy(m_ActualGrabPoint);

            m_ActualGrabPoint = new GameObject();
            m_ActualGrabPoint.transform.position = _point;
            m_ActualGrabPoint.transform.parent = m_Owner.transform;
        }


        /// <summary>
        /// In order to be able to manipulate a gaze interactive object we need to
        /// set the manipulation mode to on.
        /// </summary>
        /// <param name="_isOn">If set to false the object wont be considered as being manipulated</param>
        /// <param name="_inmeditelly"> is this going to happen now or it will wait the default time </param>
        public void SetManipulationMode(bool _isOn, bool _inmeditelly = false)
        {
            // Don't allow to change the manipulation mode if the object is not manipulable
            if (!m_Owner.IsManipulable)
                return;

            if (_isOn)
                m_IsBeingManipulated = true;
            else
            {
                if (!_inmeditelly)
                    m_Owner.CancelManipulation = m_Owner.StartCoroutine(m_Owner.DisableManipulationModeInTime());
                else
                    CleanManipulationData();
            }
        }

        /// <summary>
        /// Once the object stops being manipulated we need to clean all the variables
        /// </summary>
        private void CleanManipulationData()
        {
            m_Owner.CancelManipulation = null;
            m_IsBeingManipulated = false;

            if (m_ActualGrabPoint != null)
                GameObject.Destroy(m_ActualGrabPoint);

            m_ActualGrabPoint = null;

            if (m_Owner.GrabPositionnerCollider != null)
            {
                GameObject.Destroy(m_Owner.GrabPositionnerCollider.gameObject);
            }
        }

        /// <summary>
        /// The object won't be manipulable after the call of this method.
        /// </summary>
        public void DisableManipulationMode()
        {
            if (Vector3.Distance(m_LastPosition, m_Owner.transform.position) <= 0.05f && !IsBeingGrabbed)
                CleanManipulationData();
            else
                m_Owner.CancelManipulation = m_Owner.StartCoroutine(m_Owner.DisableManipulationModeInTime());
        }


        /// <summary>
        /// Used to throw an object.
        /// </summary>
        protected virtual void ThrowObject()
        {
            if (PositionHistory.Count < SAMPLES)
                return;

            if (m_IsBeingReleasedBecauseOfDistance)
            {
                m_RigidBody.velocity = Vector3.zero;
                m_RigidBody.angularVelocity = Vector3.zero;
            }
            else
            {
                //Help the user a little bit if he is not levitating the object
                float userHelp = m_Owner.ManipulationMode != S_ManipulationModes.LEVITATE ? 2.6f : 1.0f;

                // Throw Impulse
                float meanAcceleration = GetMeanVelocity() * userHelp;

                Vector3 direction = m_Owner.transform.position - PositionHistory[PositionHistory.Count - 1];

                m_RigidBody.velocity = direction * meanAcceleration * Time.deltaTime;

                // Angular Velocity
                Vector3? meanAngularVelocity = GetMeanVector(AngularVelocityHistory);
                if (meanAngularVelocity != null)
                {
                    m_RigidBody.angularVelocity = meanAngularVelocity.Value;
                }
            }


        }

        /// <summary>
        /// Gets the mean velocity of the io by using the history 
        /// </summary>
        /// <returns></returns>
        private float GetMeanVelocity()
        {
            List<float> velocities = new List<float>();
            for (int i = 0; i < PositionHistory.Count - 5; i++)
            {
                velocities.Add(Vector3.Distance(PositionHistory[i], PositionHistory[i + 1]) /
                               Mathf.Pow(Time.deltaTime, 2));
            }

            return velocities.Average();
        }


        /// <summary>
        /// Gets the mean of an array of vectors
        /// </summary>
        /// <param name="_positions"></param>
        /// <returns></returns>
        protected Vector3? GetMeanVector(Vector3?[] _positions)
        {
            float x = 0f;
            float y = 0f;
            float z = 0f;

            int count = 0;
            for (int index = 0; index < _positions.Length; index++)
            {
                if (_positions[index] != null)
                {
                    x += _positions[index].Value.x;
                    y += _positions[index].Value.y;
                    z += _positions[index].Value.z;

                    count++;
                }
            }

            if (count > 0)
            {
                return new Vector3(x / count, y / count, z / count);
            }

            return null;
        }


        protected virtual void AddExternalVelocities()
        {
            if (m_ExternalVelocity != Vector3.zero)
            {
                this.m_RigidBody.velocity = Vector3.Lerp(this.m_RigidBody.velocity, m_ExternalVelocity, 0.5f);
                m_ExternalVelocity = Vector3.zero;
            }

            if (m_ExternalAngularVelocity == Vector3.zero) return;
            this.m_RigidBody.angularVelocity = Vector3.Lerp(this.m_RigidBody.angularVelocity, m_ExternalAngularVelocity,
                0.5f);
            m_ExternalAngularVelocity = Vector3.zero;
        }

        public void AddExternalVelocity(Vector3 _velocity)
        {
            m_ExternalVelocity = m_ExternalVelocity == Vector3.zero
                ? _velocity
                : Vector3.Lerp(m_ExternalVelocity, _velocity, 0.5f);
        }

        public void AddExternalAngularVelocity(Vector3 _angularVelocity)
        {
            m_ExternalAngularVelocity = m_ExternalAngularVelocity == Vector3.zero
                ? _angularVelocity
                : Vector3.Lerp(m_ExternalAngularVelocity, _angularVelocity, 0.5f);
        }

        #region EventHandlers

        public void OnTeleportEvent(S_TeleportEventArgs args)
        {
            if (IsBeingGrabbed)
            {
                m_Owner.transform.position = m_ControllerGrabLocation.transform.position;
            }
        }


        /// <summary>
        /// USed to know if the gravity has been changed during the manipulation of the object
        /// </summary>
        public void GravityChanged()
        {
            if (!IsBeingGrabbed)
                return;

            SetRigidBodyParametersToEnableManipulation();
        }

        private void SetRigidBodyParametersToEnableManipulation()
        {
            // This will serve the object as guide to know how to follow the controllers hand
            m_ControllerGrabLocation = GrabbingManager.GetComponentInChildren<S_GrabPositionController>().transform;
            S_GravityManager.ChangeGravityState(m_Owner, S_GravityRequestType.UNLOCK, false);
            S_GravityManager.ChangeGravityState(m_Owner, S_GravityRequestType.ACTIVATE_AND_DETACH, false);

            m_RigidBody.useGravity = false;
            m_RigidBody.maxAngularVelocity = 100;
            m_RigidBody.drag = 0;
            m_RigidBody.angularDrag = 0.05f;
        }

        /// <summary>
        /// Performs all the opearations needed to grab an object after
        /// the grab event has been received
        /// </summary>
        /// <param name="_e"></param>
        private void GrabObject(S_ControllerGrabEventArgs _e)
        {
            GrabbingManager = (S_GrabManager)_e.Sender;

            if (m_Owner.IsManipulable && !IsBeingManipulated)
                SetManipulationMode(true);

            else if (IsBeingManipulated)
                m_Owner.ContinueManipulation();

            SetRigidBodyParametersToEnableManipulation();

            if (m_Owner.ManipulationMode != S_ManipulationModes.LEVITATE)
            {
                SetDistanceTolerance(DEFAULT_DETACH_DISTANCE);
                SetTimeTolerance(DEFAULT_TIME_UNTIL_DETACH);
            }

            m_RemainingTimeUntilDetach = actualTimeUntilDetach;

            m_OriginalCenterOfMass = m_RigidBody.centerOfMass;

            if (m_Owner.HasGrabPositionner)
            {
                m_RigidBody.centerOfMass = m_Owner.GrabPositionnerCollider.transform.localPosition;
            }
            else
            {
                m_RigidBody.centerOfMass = DefaultHandle.transform.localPosition;
            }


            isCenterOfMassAlreadyCorrected = false;

            m_IsBeingReleasedBecauseOfDistance = false;

        }

        bool isCenterOfMassAlreadyCorrected = false;

        /// <summary>
        /// Perform all the operations needed after the release of an object has been received from an 
        /// event
        /// </summary>
        private void ReleaseObject()
        {
            m_RemainingTimeUntilDetach = actualTimeUntilDetach;

            if (m_Owner.IsManipulable)
                SetManipulationMode(false);

            GrabbingManager = null;
            m_ControllerGrabLocation = null;

            if (m_Owner.InitialGravityState != S_GravityState.ACTIVE_NOT_KINEMATIC)
                S_GravityManager.ChangeGravityState(m_Owner, S_GravityRequestType.DEACTIVATE_AND_ATTACH, false);
            else
            {
                if (m_RigidBody != null)
                    m_RigidBody.useGravity = true;
                ThrowObject();
            }

            if (m_RigidBody)
            {
                m_RigidBody.centerOfMass = m_OriginalCenterOfMass;
            }

            S_GravityManager.ChangeGravityState(m_Owner, S_GravityRequestType.RETURN_TO_DEFAULT, false);
            CleanPositionsAndVelocityHistory();
        }

        private Transform leftSnapPoint, rigtSnapPoint;
        public Transform GetSnapPointForHand(bool _isLeftHand)
        {
            if ((leftSnapPoint == null && S_InputManager.Instance.LeftHandActive) ||
                (rigtSnapPoint == null && S_InputManager.Instance.RightHandActive))
            {
                List<S_SnapPosition> positions = m_Owner.gameObject.GetComponentsInChildrenBFS<S_SnapPosition>();
                int i = 0;
                foreach (S_SnapPosition position in positions)
                {
                    if (i > 1)
                        break;

                    if (position.ActualHand == S_SnapPosition.HAND.LEFT)
                        leftSnapPoint = position.transform;
                    else
                        rigtSnapPoint = position.transform;
                    ++i;
                }

                if (leftSnapPoint == null && S_InputManager.Instance.LeftHandActive)
                {
                    Debug.Log(string.Format("{0} needs an snap left point to be grabbable, " +
                                                 "setting the object transform as a default left snap point",
                        m_Owner.name));
                    leftSnapPoint = m_Owner.transform;
                }
                if (rigtSnapPoint == null && S_InputManager.Instance.RightHandActive)
                {
                    Debug.Log(string.Format("{0} needs an snap right point to be grabbable, " +
                                                 "setting the object transform as a default left snap point",
                        m_Owner.name));
                    rigtSnapPoint = m_Owner.transform;
                }
            }

            return _isLeftHand ? leftSnapPoint : rigtSnapPoint;
        }

        #endregion EventHandlers
    }
}
