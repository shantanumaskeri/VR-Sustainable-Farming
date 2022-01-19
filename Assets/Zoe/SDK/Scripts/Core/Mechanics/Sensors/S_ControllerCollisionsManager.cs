// <copyright file="S_ControllerCollisionsManager.cs" company="apelab sàrl">
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
using UnityEngine;

namespace SpatialStories
{
    public class S_ControllerCollisionsManager : MonoBehaviour
    {
        public float colliderSize = 1f;

        [HideInInspector]
        public S_GrabManager GrabManager;

        private Transform m_ControllerTransform;

        void Start()
        {
            GetComponent<BoxCollider>().size = new Vector3(colliderSize, colliderSize, colliderSize);
            GrabManager = S_Utils.GetIOFromGameObject(gameObject).GetComponentInChildren<S_GrabManager>();
        }

        void Update()
        {
            ProximityFollow();
        }

        /// <summary>
        /// Makes the Proximity GameObject follow the Hand Model to keep detecting occuring at Hand's location.
        /// </summary>
        private void ProximityFollow()
        {
            // Find the hand model within the IO children to make the Proximity follow it
            if (GetComponentInParent<S_InteractiveObject>() != null && GetComponentInParent<S_InteractiveObject>().GetComponentInChildren<S_HandController>() != null)
            {
                m_ControllerTransform = GetComponentInParent<S_InteractiveObject>().GetComponentInChildren<S_HandController>().transform;
                if (m_ControllerTransform != null)
                {
                    transform.position = m_ControllerTransform.position;
                    transform.rotation = m_ControllerTransform.rotation;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            // notify manager
            // Debug.LogError("OnTriggerEnter::THIS["+ this.gameObject.name + "]["+ other.gameObject.name + "]++++++++++++++++++++++++");
            S_InputManager.Instance.FireControllerCollisionEvent(new S_ControllerCollisionEventArgs(this.gameObject, other.gameObject, S_CollisionTypes.COLLIDER_ENTER, GrabManager));
        }

        void OnTriggerExit(Collider other)
        {
            // notify manager
            // Debug.LogError("OnTriggerExit--------------------------");
            S_InputManager.Instance.FireControllerCollisionEvent(new S_ControllerCollisionEventArgs(this.gameObject, other.gameObject, S_CollisionTypes.COLLIDER_EXIT, GrabManager));
        }
    }
}
