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

// <date>2016-01-25</date>
// </copyright>
//-----------------------------------------------------------------------
using SpatialStories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public class S_LevitationManager : MonoBehaviour
    {
        public float DetachDistance = 0.5f;
        public float DetachTime = 3f;

        #region members
        [Tooltip("The duration in seconds to charge the levitation")]
        public float ChargeDuration = .1f;
        [Tooltip("The amount of force (speed) used to center the levitated object at gaze position")]
        public float AttractionForce = 3f;

        // Used for the ease in / out of the object
        public float TimePressingToReachMaxSpeed = 1.0f;
        private float m_ActualTimePressingTrigger = 0.0f;

        [Tooltip("The closest distance the object can be from the controller")]
        public float ClosestDistance = 1f;
        public float BeamSplineSmoothness = .1f;
        public AudioClip BeamSoundClip;
        [Range(0f, 1f)]
        public float AudioFXVolume = 1f;
        public int BeamNumberOfControlPoints = 5;
        public float BeamWidth = .005f;
        public float BeamOffsetSpeed = 10f, beamOffsetAmplitude = .02f;
        public Material BeamMaterial;
        public float BeamTextureSpeed = .4f;
        public Color DropOffStartColor = Color.white, dropOffEndColor = Color.white;
        public Color DropOnStartColor = Color.white, dropOnEndColor = Color.green;
        public float PointerDiameter = .05f;

        private bool m_IsLeftHand;
        private GameObject m_ControllerLeft, m_ControllerRight;
        private S_HandsEnum m_ActualHand;

        private GameObject m_TargetLocation;

        private float m_ChargeStartTime;
        private bool m_IsCharged;
        private bool m_IsCharging;
        private bool m_IsControllerTrigger;
        private LineRenderer m_Beam;
        private Vector3 m_OldPos, m_NewPos;
        private Vector3 m_ObjectToLevitateVelocity;
        private float m_ObjectDistance;
        private Vector3 m_BeamEndPosition;
        private float m_ChargeProgress;
        private Vector3[] m_BeamControlPoints, m_BeamSplinePoints;
        private Vector3 m_BeamStartPosition;
        private S_LevitationStates m_LevitationState;
        private Vector3 m_HitPosition;

        private GameObject m_AttachPoint;

        private Transform m_HandLocation;
        private bool m_DropReady;
        private IEnumerator m_UpdateBeamColorRoutine;

        // Interaction physX vars
        private S_InteractiveObject m_IOToLevitate;
        private GameObject m_DynamicLevitationPoint;

        // The current drag and drop manager
        private S_DragAndDropManager m_CurrentDragAndDropManager;

        // Is this object being controlled by the drag and drop manager (Snap on Drop)
        private bool m_SnappedByDragAndDrop = false;

        // This is the offset distance between the Dynamic levitaion point and the object position
        private float m_SnappedTolerance = 0f;

        private GameObject m_OriginPoint;
        public GameObject OriginParticlesPrefab;
        public GameObject EndPointPrefab;
        #endregion

        void OnEnable()
        {
            m_ActualHand = GetComponentInChildren<S_GrabManager>().IsLeftHand ? S_HandsEnum.LEFT : S_HandsEnum.RIGHT;

            S_EventManager.OnLevitationEvent += OnLevitationEvent;
            S_EventManager.OnDragAndDropEvent += OnDragAndDropEvent;

            // TODO: Discriminate the hand that we are using
            S_InputManager.Instance.OnControllerGrabEvent += OnControllerGrabEvent;
            
            transform.gameObject.AddComponent<AudioSource>();
            GetComponent<AudioSource>().playOnAwake = false;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().clip = BeamSoundClip;
            GetComponent<AudioSource>().volume = AudioFXVolume;
        }

        void OnDisable()
        {
            S_EventManager.OnLevitationEvent -= OnLevitationEvent;
            S_EventManager.OnDragAndDropEvent -= OnDragAndDropEvent;

            S_InputManager.Instance.OnControllerGrabEvent -= OnControllerGrabEvent;
        }

        private void OnDestroy()
        {
            if (BeamMaterial)
                BeamMaterial.SetTextureOffset("_MainTex", new Vector2(0f, 0f));

            Destroy(m_AttachPoint);
            Destroy(m_OriginPoint);

            S_LevitationAttachPoint.DestroyAllAttachPoints();
        }

        void Start()
        {
            
            m_IsLeftHand = GetComponentInChildren<S_GrabManager>().IsLeftHand;
            m_ActualHand = GetComponentInChildren<S_GrabManager>().IsLeftHand ? S_HandsEnum.LEFT : S_HandsEnum.RIGHT;
            m_IsControllerTrigger = false;
            m_TargetLocation = transform.Find("Levitation Target").gameObject;

            m_Beam = gameObject.AddComponent<LineRenderer>();
            m_Beam.receiveShadows = false;
            m_Beam.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            m_Beam.material = BeamMaterial;
            m_Beam.startColor = DropOffStartColor;
            m_Beam.endColor = dropOffEndColor;
            m_Beam.startWidth = .001f;
            m_Beam.endWidth = BeamWidth;

            ResetCharge();

            // the number of points for the spline is total number of control points (minus 3 to avoid first and 2 lasts) divided by step distance
            m_BeamSplinePoints = new Vector3[(int)((BeamNumberOfControlPoints - 3) / BeamSplineSmoothness + 1)];
            S_CatmullRomSpline.StepDistance = BeamSplineSmoothness;
            S_CatmullRomSpline.SplinePoints = m_BeamSplinePoints;

            m_LevitationState = S_LevitationStates.NEUTRAL;

            StartCoroutine(FindControllers());

            CreateAttachPoint();
            //Debug.Log("Attach Point Created!");

            m_HandLocation = GetComponentInChildren<S_DistantGrabPointer>().transform;
        }


        void FixedUpdate()
        {

            if (m_IOToLevitate != null && m_IOToLevitate.GrabLogic.GrabbingManager != null && m_IOToLevitate.GrabLogic.GrabbingManager.IsLeftHand != m_IsLeftHand)
                return;

            if ((!m_IsCharged && !m_IsCharging) && m_IsControllerTrigger)
            {
                StartCoroutine(Charge());
            }

            if (!m_IsCharged) return;
            if (!m_IsControllerTrigger)
                ResetCharge();
            else
            {
                if (m_IOToLevitate != null && m_IOToLevitate.ManipulationMode == S_ManipulationModes.LEVITATE)
                    Levitate();
                else
                {
                    S_EventManager.FireLevitationEvent(new S_LevitationEventArgs(this, m_IOToLevitate.gameObject, S_LevitationTypes.LEVITATE_STOP, m_ActualHand));
                    m_IsControllerTrigger = false;
                }
            }
        }

        private IEnumerator UpdateBeamFeedback(bool _dropReady)
        {
            if (_dropReady)
            {
                m_Beam.startColor = DropOnStartColor;
                m_Beam.endColor = dropOnEndColor;
                m_AttachPoint.GetComponent<Renderer>().material.color = dropOnEndColor;
            }
            else
            {
                m_Beam.startColor = DropOffStartColor;
                m_Beam.endColor = dropOffEndColor;
                m_AttachPoint.GetComponent<Renderer>().material.color = dropOffEndColor;
            }
            yield return null;
        }

        private IEnumerator FindControllers()
        {
            yield return new WaitForSeconds(1f);
            S_HandController[] handControllers = Camera.main.GetComponentInParent<S_InputManager>().GetComponentsInChildren<S_HandController>();
            for (int i = 0; i < handControllers.Length; ++i)
            {
                if (handControllers[i].IsLeftHand)
                    m_ControllerLeft = handControllers[i].gameObject;
                else
                    m_ControllerRight = handControllers[i].gameObject;
            }
        }

        private void CreateAttachPoint()
        {
            if (m_AttachPoint != null)
                Destroy(m_AttachPoint);


            m_AttachPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_AttachPoint.AddComponent<S_LevitationAttachPoint>();
            Destroy(m_AttachPoint.GetComponent<Collider>());
            m_AttachPoint.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            m_AttachPoint.name = " - Target";
            m_AttachPoint.GetComponent<Renderer>().enabled = false;

            if (EndPointPrefab == null)
            {
                if(m_AttachPoint != null)
                    Destroy(m_AttachPoint);
                m_AttachPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                m_AttachPoint.AddComponent<S_LevitationAttachPoint>();

                m_AttachPoint.transform.localScale = new Vector3(PointerDiameter, PointerDiameter, PointerDiameter);
                m_AttachPoint.GetComponent<SphereCollider>().isTrigger = true;
            }
            else
            {
                m_AttachPoint = GameObject.Instantiate(EndPointPrefab);
                m_AttachPoint.AddComponent<S_LevitationAttachPoint>();
            }

            if (OriginParticlesPrefab == null)
            {
                if (m_OriginPoint != null)
                    Destroy(m_AttachPoint);
                m_OriginPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                m_OriginPoint.GetComponent<SphereCollider>().isTrigger = true;
                m_OriginPoint.transform.localScale = new Vector3(PointerDiameter, PointerDiameter, PointerDiameter);
            }
            else
            {
                m_OriginPoint = GameObject.Instantiate(OriginParticlesPrefab);
                m_OriginPoint.name = "Origin Point";
            }
            m_OriginPoint.SetActive(false);
            m_AttachPoint.SetActive(false);

        }

        private IEnumerator Charge()
        {
            #region INIT
            m_Beam.positionCount = 2;
            m_IsCharging = true;
            m_ChargeStartTime = Time.time;
            #endregion

            m_OriginPoint.SetActive(true);
            m_AttachPoint.SetActive(true);

            #region CHARGING
            GetComponent<AudioSource>().Play();
            while (Time.time - m_ChargeStartTime < ChargeDuration)
            {
                UpdateBeamLength();
                yield return null;
            }
            #endregion

            #region CHARGE FINISHED
            m_IsCharged = true;
            m_IsCharging = false;

            // the 5 control points for the straight line
            m_BeamControlPoints = new Vector3[BeamNumberOfControlPoints];

            // add vertices to be able to curve the line
            m_Beam.positionCount = m_BeamSplinePoints.Length;

            // Set the beam points correctly
            for (int i = 0; i < m_BeamSplinePoints.Length; i++)
            {
                m_Beam.SetPosition(i, m_BeamSplinePoints[i]);
            }

            // set pointer position
            SetAttachPoint();

            // set the distance from controller to object to levitate to the target levitation object
            SetTargetLocation();

            // initialize start position for velocity computations
            m_OldPos = m_AttachPoint.transform.position;

            m_IOToLevitate.GrabLogic.SetDistanceTolerance(DetachDistance);
            m_IOToLevitate.GrabLogic.SetTimeTolerance(DetachTime);

            S_EventManager.FireLevitationEvent(new S_LevitationEventArgs(this, m_IOToLevitate.gameObject, S_LevitationTypes.LEVITATE_START, m_ActualHand));
            #endregion


        }

        private void SetAttachPoint()
        {
            // set position of the pointer to the hit point
            m_AttachPoint.transform.position = m_HitPosition;

            // show attach point
            m_AttachPoint.GetComponent<Renderer>().enabled = true;
        }

        private void ClearAttachPoint()
        {
            // reset pointer when levitation finished
            if (m_AttachPoint)
                m_AttachPoint.GetComponent<Renderer>().enabled = false;
        }

        private void SetTargetLocation()
        {
            // position at hand's location
            m_TargetLocation.transform.localPosition = m_HandLocation.localPosition;

            // find delta between object and hand
            float distance = Vector3.Distance(m_HandLocation.position, m_AttachPoint.transform.position);

            // add distance between object and hand in forward direction of the target location
            m_TargetLocation.transform.localPosition = new Vector3(m_TargetLocation.transform.localPosition.x, m_TargetLocation.transform.localPosition.y, m_TargetLocation.transform.localPosition.z + distance);
        }

        private Vector3 getVectorPointAtDistance(Vector3 _vectorA, Vector3 _vectorB, float _distanceFromVectorAPercentage)
        {
            return _vectorA + (_distanceFromVectorAPercentage * (_vectorB - _vectorA));
        }

        private void UpdateBeamLength()
        {
            if (m_IOToLevitate != null)
            {
                if (!m_HandLocation)
                    m_HandLocation = GetComponentInChildren<S_DistantGrabPointer>().transform;

                m_ObjectDistance = Vector3.Distance(m_AttachPoint.transform.localPosition, m_HandLocation.position);
                m_ChargeProgress = (Time.time - m_ChargeStartTime) / ChargeDuration;
                m_BeamEndPosition = m_HandLocation.position + ((m_HitPosition - m_HandLocation.position) * m_ChargeProgress);

                m_Beam.SetPosition(0, m_HandLocation.position);
                m_Beam.SetPosition(1, m_BeamEndPosition);
            }
        }

        private float GetChargeProgression()
        {
            m_ObjectDistance = Vector3.Distance(m_AttachPoint.transform.localPosition, m_HandLocation.position);
            return (Time.time - m_ChargeStartTime) / ChargeDuration;
        }

        private void Levitate()
        {
            if (m_IOToLevitate == null) return;

            m_Beam.enabled = true;

            // beam update
            UpdateBeamControlPoints();
            m_BeamSplinePoints = S_CatmullRomSpline.getSplinePoints(m_BeamControlPoints);
            for (int i = 0; i < m_BeamSplinePoints.Length; i++)
            {
                m_Beam.SetPosition(i, m_BeamSplinePoints[i]);
            }

            AnimateBeamTexture();

            // push / pull levitating object if needed
            PullPush();

            // move object to target's position
            m_AttachPoint.transform.position = Vector3.Lerp(m_AttachPoint.transform.position, m_TargetLocation.transform.position, Time.fixedDeltaTime * AttractionForce);

            // rotate attach point towards the controller
            m_AttachPoint.transform.LookAt(m_HandLocation);

            // set beam's length
            if(m_OriginPoint != null)
                m_OriginPoint.transform.position = m_HandLocation.position;

            // get object's velocity
            m_NewPos = m_AttachPoint.transform.position;
            m_ObjectToLevitateVelocity = (m_NewPos - m_OldPos) / Time.deltaTime;
            m_OldPos = m_NewPos;
            m_NewPos = m_AttachPoint.transform.position;

            // rotate attach point according to controller's 'Z' rotation
            m_AttachPoint.transform.eulerAngles = new Vector3(m_AttachPoint.transform.eulerAngles.x, m_AttachPoint.transform.eulerAngles.y, m_HandLocation.transform.eulerAngles.z * -1);
            if (!m_SnappedByDragAndDrop)
                m_IOToLevitate.GrabLogic.FollowPoint(m_AttachPoint.transform, m_DynamicLevitationPoint.transform);
            else
            {
                if (!m_CurrentDragAndDropManager.IsInDistance(m_AttachPoint.transform.position, m_SnappedTolerance))
                {
                    m_IOToLevitate.transform.position = m_AttachPoint.transform.position;
                    S_GravityManager.ChangeGravityState(m_IOToLevitate, S_GravityRequestType.UNLOCK);
                    S_GravityManager.ChangeGravityState(m_IOToLevitate, S_GravityRequestType.ACTIVATE_AND_DETACH);
                    m_SnappedByDragAndDrop = false;
                }
            }

        }

        private Vector3 GetPullPushDirection()
        {
            return m_TargetLocation.transform.position - m_HandLocation.transform.position;
        }

        private void PullPush()
        {
            CalcPullPushSpeed();

            float speedFactor = m_ActualTimePressingTrigger / TimePressingToReachMaxSpeed;

            speedFactor = Mathf.Max(0.2f, speedFactor);

            Vector3 pullPushDir = GetPullPushDirection();
            float pullPushMagnitude = AttractionForce * Time.deltaTime * speedFactor;

            // if pushing
            switch (m_LevitationState)
            {
                case S_LevitationStates.PULL:
                    m_TargetLocation.transform.position += pullPushDir * pullPushMagnitude;
                    S_EventManager.FireLevitationEvent(new S_LevitationEventArgs(this, m_IOToLevitate.gameObject, S_LevitationTypes.PULL, m_ActualHand));
                    break;
                case S_LevitationStates.PUSH:
                    if (Vector3.Distance(m_TargetLocation.transform.position, m_HandLocation.transform.position) > ClosestDistance)
                    {
                        m_TargetLocation.transform.position -= pullPushDir * pullPushMagnitude;
                        S_EventManager.FireLevitationEvent(new S_LevitationEventArgs(this, m_IOToLevitate.gameObject, S_LevitationTypes.PUSH, m_ActualHand));
                    }
                    break;
                default:
                    break;
            }
        }

        private void CalcPullPushSpeed()
        {
            if (m_LevitationState == S_LevitationStates.NEUTRAL)
            {
                m_ActualTimePressingTrigger = 0.0f;
            }

            m_ActualTimePressingTrigger += Time.deltaTime;
            m_ActualTimePressingTrigger = Mathf.Min(m_ActualTimePressingTrigger, TimePressingToReachMaxSpeed);
        }

        private void AnimateBeamTexture()
        {
            if (BeamMaterial)
                BeamMaterial.SetTextureOffset("_MainTex", new Vector2(Time.time * BeamTextureSpeed * -1, 0f));
        }

        private void UpdateBeamControlPoints()
        {
            m_BeamStartPosition = m_HandLocation.position;
            m_BeamControlPoints[0] = m_BeamStartPosition;
            m_BeamControlPoints[1] = m_BeamStartPosition;

            for (int i = 2; i < m_BeamControlPoints.Length - 2; i++)
            {
                m_BeamControlPoints[i] = PerturbateBeam(getVectorPointAtDistance(m_HandLocation.position, m_TargetLocation.transform.position, (float)(1f / (BeamNumberOfControlPoints - 3) * (i - 1))), i);
            }

            m_BeamControlPoints[m_BeamControlPoints.Length - 2] = m_AttachPoint.transform.position;
            m_BeamControlPoints[m_BeamControlPoints.Length - 1] = m_AttachPoint.transform.position;
        }

        private Vector3 PerturbateBeam(Vector3 _point, int _index)
        {
            return _point + Mathf.Sin((Time.time + _index) * BeamOffsetSpeed) * beamOffsetAmplitude * Vector3.right;
        }

        private void ResetCharge()
        {
            StopAllCoroutines();
            m_IsCharged = false;
            m_IsCharging = false;
            m_Beam.positionCount = 1;

            if (m_OriginPoint != null)
                m_OriginPoint.SetActive(false);

            GetComponent<AudioSource>().Stop();

            // notify the levitation has stopped
            if (m_IOToLevitate != null)
                S_EventManager.FireLevitationEvent(new S_LevitationEventArgs(this, m_IOToLevitate.gameObject, S_LevitationTypes.LEVITATE_STOP, m_ActualHand));

            S_GrabManager.EnableAllGrabManagers();
            UpdateBeamFeedback(false);
        }

        private void OnLevitationEvent(S_LevitationEventArgs e)
        {
            if (e.Hand != m_ActualHand && e.Hand != S_HandsEnum.BOTH)
                return;

            if (e.Type.Equals(S_LevitationTypes.LEVITATE_START))
            {
                m_IOToLevitate = S_Utils.GetIOFromGameObject(e.ObjectToLevitate);
                S_GravityManager.ChangeGravityState(m_IOToLevitate, S_GravityRequestType.UNLOCK);
                S_GravityManager.ChangeGravityState(m_IOToLevitate, S_GravityRequestType.ACTIVATE_AND_DETACH);
                m_TargetLocation.transform.position = m_IOToLevitate.transform.position;
                UpdateBeamControlPoints();
                S_Teleporter.IsTeleportAllowed = false;
                m_SnappedByDragAndDrop = false;

                m_CurrentDragAndDropManager = m_IOToLevitate.GetComponent<S_DragAndDropManager>();

                if (m_CurrentDragAndDropManager.IsSnapped)
                {
                    m_SnappedByDragAndDrop = true;
                    m_SnappedTolerance = Vector3.Distance(m_AttachPoint.transform.position, m_IOToLevitate.transform.position);
                }
            }
            else if (e.Type.Equals(S_LevitationTypes.LEVITATE_STOP))
            {
                m_Beam.enabled = false;
                Destroy(m_DynamicLevitationPoint);
                S_GravityManager.ChangeGravityState(m_IOToLevitate, S_GravityRequestType.RETURN_TO_DEFAULT);
                m_IOToLevitate = null;
                S_GrabManager.EnableAllGrabManagers();
                ClearAttachPoint();
                S_Teleporter.IsTeleportAllowed = true;
            }
        }

        private void OnControllerGrabEvent(S_ControllerGrabEventArgs e)
        {
            // else, if the I'm the concerned controller
            if ((e.ControllerObjectPair.Key.Equals(UnityEngine.XR.XRNode.LeftHand) && m_IsLeftHand) || (e.ControllerObjectPair.Key.Equals(UnityEngine.XR.XRNode.RightHand) && !m_IsLeftHand))
            {
                // and this object is in LEVITATE mode
                if (e.ControllerObjectPair.Value.GetComponent<S_InteractiveObject>().ManipulationMode == S_ManipulationModes.LEVITATE)
                {
                    m_IsControllerTrigger = e.IsGrabbing;
                    m_IOToLevitate = e.ControllerObjectPair.Value.GetComponent<S_InteractiveObject>();
                    m_HitPosition = e.HitPosition;

                    // Ensure 1 dynamic levitation point on the manager.
                    Destroy(m_DynamicLevitationPoint);
                    m_DynamicLevitationPoint = new GameObject();
                    m_DynamicLevitationPoint.transform.localScale = Vector3.zero;

                    m_DynamicLevitationPoint.transform.position = e.HitPosition;
                    m_DynamicLevitationPoint.transform.SetParent(m_IOToLevitate.transform);
                    m_DynamicLevitationPoint.transform.rotation =
                        m_IOToLevitate.transform.rotation;
                }

            }
        }

        private void OnDragAndDropEvent(S_DragAndDropEventArgs e)
        {
            if (m_IOToLevitate == null)
                return;

            S_DragAndDropManager dndManager = S_Utils.ConvertIntoGameObject(e.DropObject).GetComponent<S_DragAndDropManager>();

            if (m_IOToLevitate.gameObject != dndManager.gameObject)
                return;


            // Check if the position control is in drag and drop
            if (e.State.Equals(S_DragAndDropStates.REMOVE))
            {
                TintBeam(false);
            }
            else if (e.State.Equals(S_DragAndDropStates.DROPREADY))
            {
                if (m_IOToLevitate.DnDSnapBeforeDrop)
                {
                    m_SnappedByDragAndDrop = true;
                    m_CurrentDragAndDropManager = dndManager;
                    m_SnappedTolerance = Vector3.Distance(m_AttachPoint.transform.position, m_IOToLevitate.transform.position);
                }

                TintBeam(true);
            }
            else if (e.State.Equals(S_DragAndDropStates.DROPREADYCANCELED))
            {
                if (m_IOToLevitate.DnDSnapBeforeDrop)
                    m_SnappedByDragAndDrop = false;
            }

            if (e.State.Equals(S_DragAndDropStates.DROPREADY) && !m_DropReady)
            {
                m_DropReady = !m_DropReady;
                TintBeam(true);

            }
            else if (e.State.Equals(S_DragAndDropStates.DROPREADYCANCELED) && m_DropReady)
            {
                m_DropReady = !m_DropReady;
                TintBeam(false);
            }
            else if (e.State.Equals(S_DragAndDropStates.DROP))
            {
                ResetBeamColor();

                // Trying to hide everything 
                m_Beam.enabled = false;
                m_AttachPoint.GetComponent<Renderer>().enabled = false;
            }
        }

        private void TintBeam(bool _dropReady)
        {
            if (m_UpdateBeamColorRoutine != null)
                StopCoroutine(m_UpdateBeamColorRoutine);
            m_UpdateBeamColorRoutine = UpdateBeamFeedback(_dropReady);
            StartCoroutine(m_UpdateBeamColorRoutine);
        }

        public void ResetBeamColor()
        {
            m_Beam.startColor = DropOffStartColor;
            m_Beam.endColor = dropOffEndColor;
            m_AttachPoint.GetComponent<Renderer>().material.color = dropOffEndColor;
        }
    }
}
