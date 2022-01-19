// <copyright file="Gaze_InputEventArgs.cs" company="apelab sàrl">
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

public class S_InputEventArgs
{
    private object m_Sender;
    public object Sender { get { return m_Sender; } set { m_Sender = value; } }

    private UnityEngine.XR.XRNode? m_VrNode;
    public UnityEngine.XR.XRNode? VrNode { get { return m_VrNode; } set { m_VrNode = value; } }

    private S_InputTypes m_InputType;
    public S_InputTypes InputType { get { return m_InputType; } set { m_InputType = value; } }

    private float m_InputValue;
    public float InputValue { get { return m_InputValue; } set { m_InputValue = value; } }

    private Vector2 m_AxisValue;
    public Vector2 AxisValue { get { return m_AxisValue; } set { m_AxisValue = value; } }
    
    public S_InputEventArgs(object _sender)
    {
        this.m_Sender = _sender;
    }

    public S_InputEventArgs(object _sender, S_InputTypes _inputType)
    {
        this.m_Sender = _sender;
        this.m_InputType = _inputType;
    }

    public S_InputEventArgs(object _sender, UnityEngine.XR.XRNode _vrNode, S_InputTypes _inputType) : this(_sender, _inputType)
    {
        this.m_VrNode = _vrNode;
    }

    public S_InputEventArgs(object _sender, UnityEngine.XR.XRNode _vrNode, S_InputTypes _inputType, float _inputValue) : this(_sender, _vrNode, _inputType)
    {
        this.m_InputValue = _inputValue;
    }

    public S_InputEventArgs(object _sender, UnityEngine.XR.XRNode _vrNode, S_InputTypes _inputType, Vector2 _axisValue) : this(_sender, _vrNode, _inputType)
    {
        this.m_AxisValue = _axisValue;
    }

    public S_InputEventArgs Clone()
    {
        S_InputEventArgs output = new S_InputEventArgs(Sender, VrNode.Equals(UnityEngine.XR.XRNode.RightHand)? UnityEngine.XR.XRNode.RightHand: UnityEngine.XR.XRNode.LeftHand, InputType, AxisValue);
        return output;
    }
}
