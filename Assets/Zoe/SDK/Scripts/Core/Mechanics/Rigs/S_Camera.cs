// <copyright file="Gaze_InputManager.cs" company="apelab sàrl">
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
using SpatialStories;
using UnityEngine;

/// <summary>
/// Gaze camera.
/// Changes the parenting structure to allow Proximity and other children to follow the Camera.
/// </summary>
public class S_Camera : MonoBehaviour
{

    private bool m_IsReconfigurationNeeded = true;
    public bool IsReconfiguiringNeeded { get { return m_IsReconfigurationNeeded; } }

    private void Awake()
    {
        //OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
    }

    void Start()
    {
        if (m_IsReconfigurationNeeded)
            ReconfigureCamera();
    }

    public void ReconfigureCamera()
    {
        if (m_IsReconfigurationNeeded == false)
            return;

        S_Head head = GetComponentInParent<S_Head>();
        if(head != null)
        {
            Transform cameraIO = head.transform;
            Transform rootIO = GetComponentInParent<S_InputManager>().transform;

            transform.position = rootIO.localPosition;
            transform.rotation = rootIO.localRotation;
            transform.parent = cameraIO.parent.transform;
            cameraIO.parent = transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            m_IsReconfigurationNeeded = false;
        }

    }
}