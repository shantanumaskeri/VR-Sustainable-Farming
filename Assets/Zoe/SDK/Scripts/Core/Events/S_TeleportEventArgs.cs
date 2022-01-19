//-----------------------------------------------------------------------
// <copyright file="Gaze_TeleportEventArgs.cs" company="apelab sàrl">
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
using UnityEngine;

namespace Gaze
{
    public class S_TeleportEventArgs : EventArgs
    {

        public object Sender;
        

        private S_TeleportMode m_Mode;
        public S_TeleportMode Mode { get { return m_Mode; } set { m_Mode = value; } }

        private GameObject m_targetObject;
        public GameObject TargetObject { get { return m_targetObject; } set { m_targetObject = value; }  }

        public S_TeleportEventArgs(object _sender)
        {
            Sender = _sender;
        }

        public S_TeleportEventArgs(object _sender, S_TeleportMode _mode, GameObject _targetTeleport = null)
        {
            Sender = _sender;
            m_Mode = _mode;
            m_targetObject = _targetTeleport;
        }
    }
}