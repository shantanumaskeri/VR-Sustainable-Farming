//-----------------------------------------------------------------------
// <copyright file="Gaze_ControllerGrabEventArgs.cs" company="apelab sàrl">
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
using System;
using System.Collections.Generic;
using UnityEngine;

public class S_ControllerGrabEventArgs : EventArgs
{
    private object m_Sender;
    public object Sender { get { return m_Sender; } }

    private bool m_IsGrabbing;
    public bool IsGrabbing { get { return m_IsGrabbing; } }

    private Vector3 m_HitPosition;
    public Vector3 HitPosition { get { return m_HitPosition; } }

    private KeyValuePair<UnityEngine.XR.XRNode, GameObject> m_ControllerObjectPair;
    public KeyValuePair<UnityEngine.XR.XRNode, GameObject> ControllerObjectPair { get { return m_ControllerObjectPair; } }

    public S_ControllerGrabEventArgs(object _sender, KeyValuePair<UnityEngine.XR.XRNode, GameObject> _dico, bool _isGrabbing)
    {
        m_Sender = _sender;
        m_ControllerObjectPair = _dico;
        m_IsGrabbing = _isGrabbing;
    }

    public S_ControllerGrabEventArgs(object _sender, KeyValuePair<UnityEngine.XR.XRNode, GameObject> _dico, bool _isGrabbing, Vector3 _hitPosition)
    {
        m_Sender = _sender;
        m_ControllerObjectPair = _dico;
        m_IsGrabbing = _isGrabbing;
        m_HitPosition = _hitPosition;
    }
}