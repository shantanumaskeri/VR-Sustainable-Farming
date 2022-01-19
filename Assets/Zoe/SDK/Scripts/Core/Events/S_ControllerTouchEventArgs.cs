//-----------------------------------------------------------------------
// <copyright file="Gaze_ControllerTouchEventArgs.cs" company="apelab sàrl">
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
using Gaze;
using System;
using System.Collections.Generic;
using UnityEngine;

public class S_ControllerTouchEventArgs : EventArgs
{
    private object m_Sender;
    public object Sender { get { return m_Sender; } set { m_Sender = value; } }
    
    private KeyValuePair<UnityEngine.XR.XRNode, GameObject> m_Dico;
    public KeyValuePair<UnityEngine.XR.XRNode, GameObject> Dico { get { return m_Dico; } set { m_Dico = value; } }
    
    private S_TouchDistanceMode m_Mode;
    public S_TouchDistanceMode Mode { get { return m_Mode; } set { m_Mode = value; } }
    
    private bool m_IsTouching;
    public bool IsTouching { get { return m_IsTouching; } set { m_IsTouching = value; } }
    
    private bool m_IsTriggerPressed;
    public bool IsTriggerPressed { get { return m_IsTriggerPressed; } set { m_IsTriggerPressed = value; } }
    
    public S_ControllerTouchEventArgs(object _sender)
    {
        m_Sender = _sender;
    }

    public S_ControllerTouchEventArgs(object _sender, KeyValuePair<UnityEngine.XR.XRNode, GameObject> _dico, S_TouchDistanceMode _eventDistanceMode, bool _isTouching, bool _isTriggerPressed)
    {
        m_Sender = _sender;
        m_Dico = _dico;
        m_Mode = _eventDistanceMode;
        m_IsTouching = _isTouching;
        m_IsTriggerPressed = _isTriggerPressed;
    }
}