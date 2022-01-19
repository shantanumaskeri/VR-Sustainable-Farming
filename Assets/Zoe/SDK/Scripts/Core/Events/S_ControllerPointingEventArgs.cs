// <copyright file="Gaze_ControllerPointingEventArgs.cs" company="apelab sàrl">
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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public class S_ControllerPointingEventArgs : EventArgs
    {
        private object m_Sender;
        public object Sender { get { return m_Sender; } set { m_Sender = value; } }

        private KeyValuePair<UnityEngine.XR.XRNode, GameObject> m_KeyValue;
        public KeyValuePair<UnityEngine.XR.XRNode, GameObject> KeyValue { get { return m_KeyValue; } set { m_KeyValue = value; } }
        
        private KeyValuePair<UnityEngine.XR.XRNode, GameObject> m_Dico;
        public KeyValuePair<UnityEngine.XR.XRNode, GameObject> Dico { get { return m_Dico; } set { m_Dico = value; } }
        
        private bool m_IsPointed;
        public bool IsPointed { get { return m_IsPointed; } set { m_IsPointed = value; } }
        
        public S_ControllerPointingEventArgs() { }

        /// <summary>
        /// Arguments for animation events related.     
        /// </summary>
        /// <param name="_sender">Is the sender's object.</param>
        public S_ControllerPointingEventArgs(object _sender, KeyValuePair<UnityEngine.XR.XRNode, GameObject> _dico, bool _isPointed)
        {
            m_Sender = _sender;
            m_Dico = _dico;
            m_IsPointed = _isPointed;
        }
    }
}
