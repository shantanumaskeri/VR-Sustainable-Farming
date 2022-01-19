// <copyright file="Gaze_Proximity.cs" company="apelab sàrl">
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gaze;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpatialStories
{
    [ExecuteInEditMode]
    public class S_Proximity : MonoBehaviour
    {
        public static List<S_InteractiveObject> HierarchyRigProximities
        {
            get
            {
                if (m_HierarchyRigProximities == null)
                    m_HierarchyRigProximities = new List<S_InteractiveObject>();
                UpdateRigProximitiesList();
                return m_HierarchyRigProximities;
            }
        }


        [HideInInspector]
        public S_InteractiveObject IOScript;

        public bool IsDebug = false;
        private static List<S_InteractiveObject> m_HierarchyRigProximities;

        private bool m_ProximityFlag = false;
        private GameObject m_OtherGameObject;
        private S_ProximityEventArgs m_ProximityEventArgs;
        private int m_ProximityLayerIndex;
        private int m_ProximityLayerMask;

        private Vector3 m_lastPositionGrabOrigin;
        private Vector3 m_lastForwardGrabOrigin;

        private static void UpdateRigProximitiesList()
        {
            m_HierarchyRigProximities.Clear();
            S_InputManager rigRoot = (S_InputManager)FindObjectOfType(typeof(S_InputManager));
            if (rigRoot)
            {
                S_Proximity[] rigProximities = rigRoot.GetComponentsInChildren<S_Proximity>();
                for (int i = 0; i < rigProximities.Length; i++)
                {
                    S_InteractiveObject io = rigProximities[i].GetComponentInParent<S_InteractiveObject>();
                    if (io && S_TransformUtils.FindGameObjectInChilds(rigRoot.gameObject, io.gameObject))
                    {
                        m_HierarchyRigProximities.Add(io);
                    }
                }
            }
        }



        private void Awake()
        {
            // otherwise the event is fired too early
            IOScript = GetComponentInParent<S_InteractiveObject>();
            m_ProximityEventArgs = new S_ProximityEventArgs(IOScript);
            m_ProximityLayerIndex = LayerMask.NameToLayer(S_UserLayers.PROXIMITY);
            m_ProximityLayerMask = LayerMask.GetMask(S_UserLayers.PROXIMITY);
            // check if sdk layers existy
            gameObject.layer = m_ProximityLayerIndex;
        }

        void Start()
        {
            StartCoroutine(NotifyAtStart());
        }

#if ANDROID && !(UNITY_EDITOR || UNITY_STANDALONE)
        private bool m_ColliderHitOnPress;
        private void Update()
        {
            if (Input.touchCount != 1)
            {
                return;
            }

            Touch touch = Input.touches[0];

            // CHECK ME: This piece of code makes sure that the raycast for the proximity collider
            // does not launch if we are over UI elements.
            // It is very surprising that there is no real easy way to do this... Especially without
            // having to call RaycastAll a second time (first time happens at the beginning of every frame
            // in Input modules)
            // A possible solution would be to somehow re-use the result from that first pass...
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = touch.position }, raycastResults);
            if (raycastResults.Count > 0 &&
                raycastResults[0].module is GraphicRaycaster)
            {
                return;
            }

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out raycastHit, Mathf.Infinity, m_ProximityLayerMask))
                {
                    if (raycastHit.collider == GetComponent<Collider>())
                    {
                        S_EventManager.FireInteractiveObjectTouched(IOScript);

                        m_ColliderHitOnPress = true;
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out raycastHit, Mathf.Infinity, m_ProximityLayerMask))
                {
                    if (m_ColliderHitOnPress && raycastHit.collider == GetComponent<Collider>())
                    {
                        S_EventManager.FireInteractiveObjectUntouched(IOScript);
                    }
                }
            }
        }
#endif
        private IEnumerator NotifyAtStart()
        {
            yield return new WaitForEndOfFrame();
            if (m_ProximityFlag)
            {
                m_ProximityEventArgs.Other = m_OtherGameObject.GetComponentInParent<S_InteractiveObject>();
                m_ProximityEventArgs.IsInProximity = true;
                S_EventManager.FireProximityEvent(m_ProximityEventArgs);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<S_Proximity>() != null)
            {
                if (IsDebug)
                    Debug.Log("Gaze_Proximity (" + transform.parent.name + ") OnTriggerEnter with " + other.GetComponentInParent<S_InteractiveObject>().name);

                m_OtherGameObject = other.gameObject;

                m_ProximityEventArgs.Other = m_OtherGameObject.GetComponentInParent<S_InteractiveObject>();
                m_ProximityEventArgs.IsInProximity = true;
                S_EventManager.FireProximityEvent(m_ProximityEventArgs);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<S_Proximity>() != null)
            {
                if (IsDebug)
                    Debug.Log("Gaze_Proximity (" + transform.parent.name + ") OnTriggerExit with " + other.GetComponentInParent<S_InteractiveObject>().name);
                m_ProximityFlag = false;
                m_OtherGameObject = other.gameObject;

                m_ProximityEventArgs.Other = m_OtherGameObject.GetComponentInParent<S_InteractiveObject>();
                m_ProximityEventArgs.IsInProximity = false;
                S_EventManager.FireProximityEvent(m_ProximityEventArgs);
            }
        }
    }
}