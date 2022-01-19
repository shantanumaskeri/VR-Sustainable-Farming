// <copyright file="Gaze_CameraRaycaster.cs" company="apelab sàrl">
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
using UnityEngine;

namespace SpatialStories
{
    /// <summary>
    /// This script notifies Event Manager when an object is gazed or no longer gazed at.
    /// Version 2.0 : now cast on all objects (RaycastAll).
    /// </summary>
    public class S_CameraRaycaster : MonoBehaviour
    {
        // delay at start of the scene before casting a ray
        public float RayLength = 500f;
        public bool IsDebugModeActive = false;
        public Camera GazeCamera;
        public bool Zoom = false;
        public float ZoomFOV = 10f;
        public float ZoomSpeed = 4;
        public float UpdateInterval = .2f;
        public List<GameObject> PreviousGazedObjects;
        public bool ForceClearPathToObject = false;

        private Ray m_Ray;
        private List<RaycastHit> m_Hits;
        private List<GameObject> m_CurrentGazedObjects;
        private Collider m_LastZoomCollider;
        private bool m_CurrentZoom = false;
        private float m_FovDefault;
        private float m_FovFactor = 1;
        private float m_ZoomSpeedFactor = 1;
        private float m_DezoomSpeedFactor = 1;
        private float m_ZoomTime = 0;
        private AnimationCurve m_ZoomCurve;
        private S_GazeEventArgs m_SGazeEventArgs;
        private float m_LastUpdateTime;
        private int m_GazeRaycastLayer;
        
        public virtual void OnEnable()
        {
            S_EventManager.OnZoomEvent += OnZoomEvent;
        }

        public virtual void OnDisable()
        {
            S_EventManager.OnZoomEvent -= OnZoomEvent;
        }

        void Awake()
        {
            PreviousGazedObjects = new List<GameObject>();
            m_CurrentGazedObjects = new List<GameObject>();
            Cursor.visible = false;
        }

        void Start()
        {
            findCamera();
            m_Hits = new List<RaycastHit>();
            m_SGazeEventArgs = new S_GazeEventArgs();
            m_LastUpdateTime = Time.time;
            m_Ray = new Ray();
            gameObject.layer = LayerMask.NameToLayer(S_UserLayers.GAZE);
            m_GazeRaycastLayer = 1 << gameObject.layer;

        }

        private void findCamera()
        {
            foreach (Camera cam in GetComponentsInChildren<Camera>())
            {
                if (cam.isActiveAndEnabled)
                {
                    GazeCamera = cam;
                    m_FovDefault = GazeCamera.fieldOfView;
                    break;
                }
            }
        }

        public void SetCamera(Camera cam)
        {
            GazeCamera = cam;
            m_FovDefault = GazeCamera.fieldOfView;
        }

        void FixedUpdate()
        {
            if (Time.time > m_LastUpdateTime + UpdateInterval)
            {

                // clear current gazed objects list
                m_CurrentGazedObjects.Clear();

                // cast a ray
                //ray = new Ray(gazeCamera.transform.position, gazeCamera.transform.forward);
                m_Ray.origin = GazeCamera.transform.position;
                m_Ray.direction = GazeCamera.transform.forward;
                m_Hits.Clear();
                m_Hits.AddRange(Physics.RaycastAll(m_Ray, RayLength, m_GazeRaycastLayer));

                // GET THE DISTANCE TO THE TARGET
                float minimalDistanceCurrentCollision = 100000000000000f;
                if (ForceClearPathToObject)
                {
                    RaycastHit raycastInfo = new RaycastHit();
                    if (S_RaycastUtils.GetRaycastHitInfoByRayWithMask(m_Ray.origin, m_Ray.direction, ref raycastInfo, S_UserLayers.GAZE))
                    {
                        minimalDistanceCurrentCollision = Vector3.Distance(m_Ray.origin, raycastInfo.point);
                        // Debug.LogError("MINIMUM DISTANCE[" + raycastInfo.collider.gameObject.transform.root.name + "][" + (int)minimalDistanceCurrentCollision + "]");
                    }
                }

                if (IsDebugModeActive)
                    Debug.DrawRay(GazeCamera.transform.position, GazeCamera.transform.forward * RayLength, Color.red);

                // if camera's ray hits something
                if (m_Hits != null && m_Hits.Count > 0)
                {
                    // construct new current gazed objects list
                    foreach (RaycastHit h in m_Hits)
                    {
                        // add it to the current gazed objects list
                        m_CurrentGazedObjects.Add(h.collider.gameObject);

                        // if it wasn't in the previous gazed objects list
                        if (!PreviousGazedObjects.Contains(h.collider.gameObject))
                        {
                            S_InteractiveObject collidedIO = h.collider.GetComponentInParent<S_InteractiveObject>();
                            if (collidedIO != null)
                            {
                                // Debug.LogError("CHECKING IO COLLIDED[" + collidedIO + "]");
                                float distanceGazedObject = Vector3.Distance(h.point, m_Ray.origin);
                                if ((distanceGazedObject <= minimalDistanceCurrentCollision) || (!ForceClearPathToObject))
                                {
                                    // notify every listener with the new current gazed object
                                    // Debug.LogError("MINIMUM DISTANCE[" + minimalDistanceCurrentCollision + "]::CURRENT DISTANCE TO IO COLLIDED["+ collidedIO + "][" + distanceGazedObject + "]");
                                    m_SGazeEventArgs.Sender = this;
                                    m_SGazeEventArgs.GazedObject = h.collider.GetComponentInParent<S_InteractiveObject>();
                                    m_SGazeEventArgs.HoverState = S_HoverStates.GAZE_IN;
                                    S_EventManager.FireGazeEvent(m_SGazeEventArgs);
                                }
                            }
                        }
                    }

                    // if there were any previous gazed object
                    if (PreviousGazedObjects.Count > 0)
                    {

                        // check for each one of them
                        foreach (GameObject p in PreviousGazedObjects)
                        {

                            // if they don't exist anymore in the current gazed objects
                            if (!m_CurrentGazedObjects.Contains(p))
                            {
                                // notify every listener this previous gazed object is no longer gazed at
                                m_SGazeEventArgs.Sender = this;
                                m_SGazeEventArgs.GazedObject = p.GetComponentInParent<S_InteractiveObject>();
                                m_SGazeEventArgs.HoverState = S_HoverStates.GAZE_OUT;
                                S_EventManager.FireGazeEvent(m_SGazeEventArgs);
                            }
                        }
                    }

                    // update previous gazed objects list with the new ones
                    PreviousGazedObjects = new List<GameObject>(m_CurrentGazedObjects);

                    // if camera's ray hits nothing
                }
                else //TODO:(apelab) [Refactoring] Check if this is necessary
                {

                    // and the there were previous gazed objects
                    if (PreviousGazedObjects.Count > 0)
                    {
                        // for each one of the previous gazed object
                        foreach (GameObject p in PreviousGazedObjects)
                        {
                            // notify every listener its no longer gazed at
                            m_SGazeEventArgs.Sender = this;
                            m_SGazeEventArgs.GazedObject = p.GetComponentInParent<S_InteractiveObject>();
                            m_SGazeEventArgs.HoverState = S_HoverStates.GAZE_OUT;
                            S_EventManager.FireGazeEvent(m_SGazeEventArgs);
                        }

                        // clear the previous gazed objects list
                        PreviousGazedObjects.Clear();
                    }
                }

                // zoom logic
                if (Zoom)
                {
                    // if no object is gazed or the last zoomed object is no longer gazed
                    if (m_CurrentGazedObjects.Count == 0 || (m_LastZoomCollider && !m_CurrentGazedObjects.Contains(m_LastZoomCollider.gameObject)))
                    {
                        //					if (currentGazedObjects.Count == 0)
                        if (m_CurrentZoom)
                        {
                            m_ZoomTime = 0;
                            m_CurrentZoom = false;
                        }

                        // zoom out
                        if (GazeCamera.fieldOfView < m_FovDefault)
                        {
                            GazeCamera.fieldOfView = m_FovDefault - ZoomFOV * m_FovFactor + ZoomFOV * m_FovFactor * m_ZoomCurve.Evaluate(m_ZoomTime * ZoomSpeed * m_DezoomSpeedFactor);
                            GazeCamera.fieldOfView = Mathf.Min(GazeCamera.fieldOfView, m_FovDefault);
                        }
                    }
                    else
                    {
                        // if a zoomable object is gazed
                        if (m_CurrentZoom)
                        {
                            // zoom in
                            if (GazeCamera.fieldOfView > m_FovDefault - ZoomFOV * m_FovFactor)
                            {
                                GazeCamera.fieldOfView = m_FovDefault - ZoomFOV * m_FovFactor * m_ZoomCurve.Evaluate(m_ZoomTime * ZoomSpeed * m_ZoomSpeedFactor);
                                GazeCamera.fieldOfView = Mathf.Max(GazeCamera.fieldOfView, m_FovDefault - ZoomFOV * m_FovFactor);
                            }
                        }
                    }

                    m_ZoomTime += Time.deltaTime;
                }


                // update last time value
                m_LastUpdateTime = Time.time;
            }
        }

        public bool IsInteractiveObjectBeingGazed(S_InteractiveObject _io)
        {
            if (PreviousGazedObjects.Count < 1)
                return false;

            return PreviousGazedObjects.Contains(_io.GetComponentInChildren<S_Gaze>().gameObject);
        }

        private void OnZoomEvent(S_ZoomEventArgs e)
        {
            if (Zoom)
            {
                if (!m_CurrentZoom)
                {
                    m_ZoomTime = 0;
                    m_CurrentZoom = true;
                }

                m_LastZoomCollider = e.Collider;
                m_FovFactor = e.FovFactor;
                m_ZoomSpeedFactor = e.ZoomSpeedFactor;

                if (e.DezoomMode.Equals(S_DezoomMode.CUSTOM))
                {
                    m_DezoomSpeedFactor = e.DezoomSpeedFactor;
                }
                else
                {
                    m_DezoomSpeedFactor = m_ZoomSpeedFactor;
                }

                if (e.ZoomCurve != null)
                {
                    m_ZoomCurve = e.ZoomCurve;
                }
                else
                {
                    m_ZoomCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
                }
            }
        }
    }
}
