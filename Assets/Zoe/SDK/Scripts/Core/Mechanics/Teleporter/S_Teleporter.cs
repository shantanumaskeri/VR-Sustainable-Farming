//========= Copyright 2016, Sam Tague, All rights reserved. ===========
//
// Attach to either or both tracked controller objects in SteamVR camera rig
//
//=====================================================================

// <copyright file="Gaze_Teleporter.cs" company="apelab sàrl">
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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SpatialStories
{
public class S_Teleporter : MonoBehaviour
    {
        private const int PARABOLA_PRECISION = 450;

        #region StaticMembers
        private static bool m__IsteleportAllowed = true;
        public static bool IsTeleportAllowed { get { return m__IsteleportAllowed; } set { m__IsteleportAllowed = value; } }
        internal static float PlayerHeightBeforeTeleport;
        #endregion StaticMembers

        public S_HandsEnum TeleportingHand;
        #region PublicMembers
        public float HoldTimeToAppear = 0.2f;
        public GameObject GyroPrefab;
        public bool OrientOnTeleport = true;
        public float InptuThreshold = .5f;
        public float MaxTeleportDistance = 5f;
        public float MaxSlope = 10f;
        public float Cooldown = 1.0f;
        public float MatScale = 5;
        public Color GoodDestinationColor = new Color(0, 0.6f, 1f, 0.2f);
        public Color BadDestinationColor = new Color(0.8f, 0, 0, 0.2f);
        public float LineWidth = 0.05f;
        public Material LineMaterial;

        public List<Transform> HotSpots = new List<Transform>();
        public float MinHotspotDistance = 1f;
        #endregion PublicMembers

        #region InternalMembers
        internal GameObject GyroInstance;
        internal float Angle;
        internal LineRenderer OwnLineRenderer;
        internal float LastTeleportTime;
        internal bool ISGoodSpot;
        internal bool IsTeleportActive;
        internal Ray TeleportRay;
        internal RaycastHit[] Hits;
        internal GameObject Cam;
        internal Vector2 AxisValue;
        internal Transform TeleportOrigin;
        #endregion InternalMembers

        private bool m_TeleportingRight = false;
        private bool m_TeleportingLeft = false;
        #region PrivateMembers
        private Transform m_TeleportDirection;
        private Vector3 m_FinalHitLocation = new Vector3();
        private Vector3 m_FinalHitNormal = new Vector3();
        private GameObject m_FinalHitGameObject;
        private float m_TimeHoldingButton = 0;
        private S_TeleportEventArgs m_GazeTeleportEventArgs;
        // stores the current TeleportMode while teleport is enables
        private S_TeleportMode m_LastTeleportMode;
        private LayerMask m_allowedLayers;
        #endregion PrivateMembers

        void Awake()
        {
            IsTeleportAllowed = true;
            m_allowedLayers = LayerMask.GetMask(S_UserLayers.TELEPORT);
            TeleportingHand = S_InputManager.Instance.TeleportJoystickSelected;
        }

        void OnEnable()
        {
            if (TeleportingHand == S_HandsEnum.LEFT || TeleportingHand == S_HandsEnum.BOTH)
            {
                S_InputManager.Instance.LeftPrimary2DAxisEvent += OnLeftPrimary2DAxisEvent;
                S_InputManager.Instance.LeftPrimary2DAxisReleaseEvent += OnPrimary2DAxisReleaseEvent;
            }

            if (TeleportingHand == S_HandsEnum.RIGHT || TeleportingHand == S_HandsEnum.BOTH)
            {
                S_InputManager.Instance.RightPrimary2DAxisEvent += OnRightPrimary2DAxisEvent;
                S_InputManager.Instance.RightPrimary2DAxisReleaseEvent += OnPrimary2DAxisReleaseEvent;
            }
        }

        void OnDisable()
        {
            if (TeleportingHand == S_HandsEnum.LEFT || TeleportingHand == S_HandsEnum.BOTH)
            {
                S_InputManager.Instance.LeftPrimary2DAxisEvent -= OnLeftPrimary2DAxisEvent;
                S_InputManager.Instance.LeftPrimary2DAxisReleaseEvent -= OnPrimary2DAxisReleaseEvent;
            }

            if (TeleportingHand == S_HandsEnum.RIGHT || TeleportingHand == S_HandsEnum.BOTH)
            {
                S_InputManager.Instance.RightPrimary2DAxisEvent -= OnRightPrimary2DAxisEvent;
                S_InputManager.Instance.RightPrimary2DAxisReleaseEvent -= OnPrimary2DAxisReleaseEvent;
            }
        }

        private void OnDestroy()
        {
            Destroy(GyroInstance);

            if (OwnLineRenderer != null)
                Destroy(OwnLineRenderer.gameObject);

            Destroy(c_line1);
            Destroy(c_line2);
            Destroy(c_lineParent);
        }

        GameObject c_lineParent;
        GameObject c_line1;
        GameObject c_line2;
        void Start()
        {
            OrientOnTeleport = S_InputManager.Instance.OrientOnTeleport;
            MaxSlope = S_InputManager.Instance.MaxTeleportHeight;
            MaxTeleportDistance = S_InputManager.Instance.MaxTeleportDistance;
            HoldTimeToAppear = S_InputManager.Instance.HoldDuration;
            Cooldown = S_InputManager.Instance.Cooldown;
            GoodDestinationColor = S_InputManager.Instance.ColorAllowedDestination;
            BadDestinationColor = S_InputManager.Instance.ColorRestrictedDestination;
            GyroPrefab = S_InputManager.Instance.TeleportTarget;
            InptuThreshold = S_InputManager.Instance.InputSensitivity;
            LineWidth = S_InputManager.Instance.LineWidth;
            LineMaterial = S_InputManager.Instance.LineMaterial;

            InstanciateGyroPrefab();

            S_Camera gazeCamera = Camera.main.transform.gameObject.GetComponentInChildren<S_Camera>();
            if (gazeCamera.IsReconfiguiringNeeded)
                gazeCamera.ReconfigureCamera();

            Cam = gazeCamera.gameObject;

            LastTeleportTime = -Cooldown;

            c_lineParent = new GameObject("Line");
            c_lineParent.transform.localScale = transform.localScale;
            c_line1 = new GameObject("Line1");

            c_line1.transform.SetParent(c_lineParent.transform);
            OwnLineRenderer = c_line1.AddComponent<LineRenderer>();
            c_line2 = new GameObject("Line2");
            c_line2.transform.SetParent(c_lineParent.transform);
            OwnLineRenderer.startWidth = LineWidth * transform.localScale.magnitude;
            OwnLineRenderer.endWidth = LineWidth * transform.localScale.magnitude;
            OwnLineRenderer.material = LineMaterial;
            OwnLineRenderer.SetPosition(0, Vector3.zero);
            OwnLineRenderer.SetPosition(1, Vector3.zero);

            if (PlayerHeightBeforeTeleport == 0)
                PlayerHeightBeforeTeleport = GetPlayerHeight();

            m_GazeTeleportEventArgs = new S_TeleportEventArgs(this);
        }

        void Update()
        {
            if (IsTeleportActive)
            {
                ComputeParabola();
            }
        }

        public void InstanciateGyroPrefab()
        {
            if (GyroInstance != null)
                Destroy(GyroInstance);

            GyroInstance = Instantiate(GyroPrefab);

            if (OrientOnTeleport)
                GyroInstance.transform.Find("GyroSprite").GetComponent<SpriteRenderer>().enabled = true;
            else
                GyroInstance.transform.Find("GyroSpriteNoRotation").GetComponent<SpriteRenderer>().enabled = true;
        }

        private void Gyro(Vector3 _pos, Vector3 _normal)
        {
            GyroInstance.transform.position = _pos;
            GyroInstance.transform.localEulerAngles = new Vector3(_normal.x, Angle, _normal.z);
        }

        private void MoveToTarget(Vector3 _position)
        {
            // Get the correct offset between the camera position and the origin of the "user space"
            Vector3 offset = Cam.transform.position - transform.position;

            // Then teleport the player to the destination having in account this offset
            transform.position = new Vector3(_position.x - offset.x, _position.y + PlayerHeightBeforeTeleport, _position.z - offset.z);
        }

        public float GetPlayerHeight()
        {
            TeleportRay = new Ray(transform.position, Vector3.down);

            // raycast on chosen layers only
            Hits = Physics.RaycastAll(TeleportRay, 4f, m_allowedLayers.value);
            if (Hits.Length <= 0)
                return GetDifferenceBetweenClosestPlaneAndPlayer();
            Hits = SortArray(Hits);
            return Hits[0].distance;
        }

        private float GetDifferenceBetweenClosestPlaneAndPlayer()
        {
            float heightToreturn = 1.6f;

            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            List<GameObject> allTeleportZones = new List<GameObject>();

            foreach (GameObject go in allGameObjects)
            {
                if (go.layer == m_allowedLayers)
                    allTeleportZones.Add(go);
            }

            if (allTeleportZones.Count == 0)
                return heightToreturn;

            float possibleHeight = float.MaxValue;
            foreach (GameObject spot in allTeleportZones)
            {
                float distanceBetweenGroundAndCamera = transform.position.y - spot.transform.position.y;
                if (distanceBetweenGroundAndCamera < possibleHeight && distanceBetweenGroundAndCamera > 0)
                    possibleHeight = distanceBetweenGroundAndCamera;
            }
            return possibleHeight;
        }

        private static RaycastHit[] SortArray(RaycastHit[] array)
        {
            int length = array.Length;

            RaycastHit temp = array[0];

            for (int i = 0; i < length; i++)
            {
                for (int j = i + 1; j < length; j++)
                {
                    if (array[i].distance < array[j].distance)
                    {
                        temp = array[i];

                        array[i] = array[j];

                        array[j] = temp;
                    }
                }
            }

            return array;
        }

        /// <summary>
        /// Rotates the camera aligned with the Gyro UI arrow's direction when teleport occurs.
        /// </summary>
        private void RotateCamera()
        {
            // Set the rotation of the camera rig correctly
            transform.forward = GyroInstance.transform.forward;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles - new Vector3(0, Cam.transform.localRotation.eulerAngles.y, 0));
        }

        internal void ComputeParabola()
        {
            m_TimeHoldingButton += Time.deltaTime;

            // Ensure that we don't show teleport until the button hold time has passed
            if (m_TimeHoldingButton < HoldTimeToAppear)
            {
                if (OwnLineRenderer.enabled)
                    OwnLineRenderer.enabled = false;
                if (GyroInstance.activeSelf)
                    GyroInstance.SetActive(false);
                return;
            }
            else
            {
                if (!OwnLineRenderer.enabled)
                    OwnLineRenderer.enabled = true;

                if (!GyroInstance.activeSelf)
                    GyroInstance.SetActive(true);
            }

            //	Line renderer position storage (two because line renderer texture will stretch if one is used)
            List<Vector3> positions1 = new List<Vector3>();

            //	first Vector3 positions array will be used for the curve and the second line renderer is used for the straight down after the curve
            float totalDistance1 = 0;

            //	Variables need for curve
            Quaternion currentRotation = m_TeleportDirection.rotation;
            Vector3 currentPosition = TeleportOrigin.position;
            Vector3 lastPostion;
            positions1.Add(currentPosition);

            lastPostion = TeleportOrigin.position - m_TeleportDirection.forward;
            Vector3 currentDirection = m_TeleportDirection.forward;
            Vector3 downForward = new Vector3(m_TeleportDirection.forward.x * 0.01f, -1, m_TeleportDirection.forward.z * 0.01f);
            RaycastHit hit = new RaycastHit();
            m_FinalHitLocation = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int step = 0; step < PARABOLA_PRECISION; step++)
            {
                Quaternion downRotation = Quaternion.LookRotation(downForward);
                currentRotation = Quaternion.RotateTowards(currentRotation, downRotation, 1f);

                Ray newRay = new Ray(currentPosition, currentPosition - lastPostion);

                float length = (MaxTeleportDistance * 0.01f) * transform.localScale.magnitude;
                if (currentRotation == downRotation)
                {
                    length = (MaxTeleportDistance * MatScale) * transform.localScale.magnitude;
                    positions1.Add(currentPosition);
                }

                float raycastLength = length * 1.1f;

                //	Check if we hit something
                bool hitSomething = Physics.Raycast(newRay, out hit, raycastLength, m_allowedLayers);

                // don't allow to teleport to negative normals (we don't want to be stuck under floors)
                if (hit.normal.y > 0)
                {
                    if (!CloseToHotSpot(hit.point) && hitSomething)
                    {
                        m_FinalHitLocation = hit.point;
                        m_FinalHitNormal = hit.normal;
                        m_FinalHitGameObject = hit.collider.gameObject;
                    }

                    totalDistance1 += (currentPosition - m_FinalHitLocation).magnitude;
                    positions1.Add(m_FinalHitLocation);

                    Gyro(m_FinalHitLocation, m_FinalHitNormal);

                    break;
                }

                //	Convert the rotation to a forward vector and apply to our current position
                currentDirection = currentRotation * Vector3.forward;
                lastPostion = currentPosition;
                currentPosition += currentDirection * length;

                totalDistance1 += length;
                positions1.Add(currentPosition);

                if (currentRotation == downRotation)
                    break;
            }

            //	Decide using the current teleport rule whether this is a good teleporting spot or not
            ISGoodSpot = IsGoodSpot(m_FinalHitLocation);

            //	Update line, teleport highlight and room highlight based on it being a good spot or bad
            if (ISGoodSpot)
            {
                OwnLineRenderer.enabled = true;
                GyroInstance.SetActive(true);

                OwnLineRenderer.material.color = GoodDestinationColor;

                // If we need to redirect the line
                if (CloseToHotSpot(hit.point))
                {
                    // Remove the 30 percent of the points
                    int pointsToRemove = Mathf.FloorToInt(positions1.Count * 0.80f);
                    positions1.RemoveRange(pointsToRemove, positions1.Count - pointsToRemove);

                    positions1.Add(m_FinalHitLocation);

                    // Create the second curve by using the points array
                    MakeSmoothCurve(positions1, 1f);

                    // Assing the new curve to the positions
                    positions1 = curvedPoints;
                }

                OwnLineRenderer.positionCount = positions1.Count;
                OwnLineRenderer.SetPositions(positions1.ToArray());
            }
            else
            {
                GyroInstance.SetActive(false);

                OwnLineRenderer.material.color = BadDestinationColor;

                OwnLineRenderer.positionCount = positions1.Count;
                OwnLineRenderer.SetPositions(positions1.ToArray());
            }
        }

        private bool CloseToHotSpot(Vector3 _hit)
        {
            for (int i = 0; i < HotSpots.Count; i++)
            {
                if (Vector3.Distance(HotSpots[i].position, _hit) < MinHotspotDistance)
                {
                    m_FinalHitLocation = HotSpots[i].position;
                    return true;
                }
            }
            return false;
        }

        private void CheckSpotChange()
        {
            if (m_LastTeleportMode.Equals(m_GazeTeleportEventArgs.Mode))
                return;

            m_LastTeleportMode = m_GazeTeleportEventArgs.Mode;
            S_EventManager.FireTeleportEvent(m_GazeTeleportEventArgs);
        }

        //	Overide and change to expand on what is a good landing spot
        virtual public bool IsGoodSpot(Vector3 _pos)
        {
            if (_pos == null)
            {
                m_GazeTeleportEventArgs.Mode = S_TeleportMode.BAD_DESTINATION;
                CheckSpotChange();
                return false;
            }

            //check if height delta is ok
            if (Mathf.Abs(GetComponentInParent<S_InputManager>().transform.position.y - _pos.y) > MaxSlope)
            {
                m_GazeTeleportEventArgs.Mode = S_TeleportMode.BAD_DESTINATION;
                CheckSpotChange();
                return false;
            }

            m_GazeTeleportEventArgs.Mode = S_TeleportMode.GOOD_DESTINATION;
            CheckSpotChange();

            return true;
        }

        virtual public void EnableTeleport()
        {
            if (GyroInstance == null)
                return;

            IsTeleportActive = true;

            // fire event
            m_GazeTeleportEventArgs.Mode = S_TeleportMode.ACTIVATED;
            S_EventManager.FireTeleportEvent(m_GazeTeleportEventArgs);
        }

        virtual public void DisableTeleport()
        {
            m_TimeHoldingButton = 0;
            if (GyroInstance == null)
                return;

            GyroInstance.SetActive(false);
            IsTeleportActive = false;
            if (OwnLineRenderer != null)
                OwnLineRenderer.enabled = false;
        }

        virtual public void Teleport()
        {
            Teleport(GyroInstance.transform.position);
        }

        public void SetTeleportRotation(Quaternion _rotation)
        {
            GyroInstance.transform.rotation = _rotation;
        }

        virtual public void Teleport(Vector3 _position)
        {
            if (m_TimeHoldingButton < HoldTimeToAppear)
                return;

            if ((Time.time - LastTeleportTime) < Cooldown)
                return;

            bool isGoodSpoot = ISGoodSpot;

            if (IsTeleportActive && isGoodSpoot)
            {
                if (OrientOnTeleport)
                    RotateCamera();

                // If the user haven't specified a position we are going to choose the gyro position
                MoveToTarget(_position);
                LastTeleportTime = Time.time;

                // fire event
                m_GazeTeleportEventArgs.Mode = S_TeleportMode.TELEPORT;
                m_GazeTeleportEventArgs.TargetObject = m_FinalHitGameObject;
                S_EventManager.FireTeleportEvent(m_GazeTeleportEventArgs);
            }
        }

        public void MoveToGyro()
        {
            if (OrientOnTeleport)
                RotateCamera();

            MoveToTarget(GyroInstance.transform.position);
        }

        static List<Vector3> curvedPoints = new List<Vector3>();
        static Vector3 lastPointInCurve = Vector3.zero;
        public bool debug = false;
        private bool isScriptEnabled;
        static List<Vector3> points;

        public static Vector3[] MakeSmoothCurve(List<Vector3> _arrayToCurve, float _smoothness)
        {

            if (Vector3.Distance(_arrayToCurve[0], lastPointInCurve) > 0.001f)
            {
                curvedPoints.Clear();
                lastPointInCurve = _arrayToCurve[0];
                int pointsLength = 0;
                int curvedLength = 0;

                if (_smoothness < 1.0f) _smoothness = 1.0f;

                pointsLength = _arrayToCurve.Count;

                curvedLength = (pointsLength * Mathf.RoundToInt(_smoothness)) - 1;

                // Don't create the array all the time
                if (curvedPoints == null)
                    curvedPoints = new List<Vector3>(curvedLength);

                float t = 0.0f;
                for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
                {
                    t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

                    points = new List<Vector3>(_arrayToCurve);

                    for (int j = pointsLength - 1; j > 0; j--)
                    {
                        for (int i = 0; i < j; i++)
                        {
                            points[i] = (1 - t) * points[i] + t * points[i + 1];
                        }
                    }

                    curvedPoints.Add(points[0]);
                }
                return (curvedPoints.ToArray());
            }
            else
            {
                return curvedPoints.ToArray();
            }
        }

        private void OnRightPrimary2DAxisEvent(S_InputEventArgs _event)
        {
            if (m_TeleportingLeft)
            {
                return;
            }

            m_TeleportingRight = true;

            TeleportOrigin = GetComponentInChildren<S_RightHandRoot>().GetComponentInChildren<S_GrabPositionController>().transform;
            m_TeleportDirection = GetComponentInChildren<S_RightHandRoot>().transform;

            OnStickLeftAxisEvent(_event.AxisValue);
        }

        private void OnLeftPrimary2DAxisEvent(S_InputEventArgs _event)
        {
            if (m_TeleportingRight)
            {
                return;
            }

            m_TeleportingLeft = true;

            TeleportOrigin = GetComponentInChildren<S_LeftHandRoot>().GetComponentInChildren<S_GrabPositionController>().transform;
            m_TeleportDirection = GetComponentInChildren<S_LeftHandRoot>().transform;

            OnStickLeftAxisEvent(_event.AxisValue);
        }

        private void OnStickLeftAxisEvent(Vector2 _axisValue)
        {
            if (_axisValue.magnitude > InptuThreshold)
            {
                AxisValue = _axisValue;
            }

            ActivateTeleport();
        }

        private void ActivateTeleport()
        {
            //If the teleport is not allowed we need to deactivate it and return
            if (!IsTeleportAllowed)
            {
                DisableTeleport();

                return;
            }

            if (!IsTeleportActive)
            {
                EnableTeleport();
            }

            if (ISGoodSpot)
            {
                if (GyroInstance == null)
                    InstanciateGyroPrefab();

                if (OrientOnTeleport)
                {
                    Angle = Mathf.Atan2(AxisValue.x, AxisValue.y) * Mathf.Rad2Deg;

                    // angle take hand's rotation into account
                    Angle += TeleportOrigin.eulerAngles.y;
                }
                else
                {
                    Angle = TeleportOrigin.eulerAngles.y;
                }
            }
            else
            {
                if (GyroInstance)
                    GyroInstance.SetActive(false);
            }
        }

        private void OnPrimary2DAxisReleaseEvent(S_InputEventArgs _event)
        {
            CompleteTeleportAction();
        }

        private void CompleteTeleportAction()
        {
            if (ISGoodSpot)
            {
                Teleport();
            }

            m_TeleportingRight = false;
            m_TeleportingLeft = false;

            DisableTeleport();
        }
    }
}

