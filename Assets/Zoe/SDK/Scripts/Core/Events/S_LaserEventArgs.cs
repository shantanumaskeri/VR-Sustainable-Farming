// <copyright file="Gaze_LaserEventArgs.cs" company="apelab sàrl">
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

public class S_LaserEventArgs
{
    private object m_Sender;
    public object Sender { get { return m_Sender; } set { m_Sender = value; } }

    private Vector3 m_StartPosition;
    public Vector3 StartPosition { get { return m_StartPosition; } set { m_StartPosition = value; } }

    private Vector3 m_EndPosition;
    public Vector3 EndPosition { get { return m_EndPosition; } set { m_EndPosition = value; } }

    private RaycastHit[] m_LaserHits;
    public RaycastHit[] LaserHits { get { return m_LaserHits; } set { m_LaserHits = value; } }

    public S_LaserEventArgs() { }

    public S_LaserEventArgs(object _sender, Vector3 _startPosition, Vector3 _endPosition, RaycastHit[] _laserHits = null)
    {
        m_Sender = _sender;
        m_StartPosition = _startPosition;
        m_EndPosition = _endPosition;
        m_LaserHits = _laserHits;
    }
}
