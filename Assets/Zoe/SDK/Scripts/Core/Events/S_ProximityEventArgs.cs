// <copyright file="Gaze_ProximityEventArgs.cs" company="apelab sàrl">
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
using SpatialStories;
using System;
using UnityEngine;

namespace Gaze
{
    public class S_ProximityEventArgs : EventArgs
    {
        private object m_Sender;
        public object Sender { get { return m_Sender; } }

        private S_InteractiveObject m_Other;
        public S_InteractiveObject Other
        {
            get { return m_Other; }
            set { m_Other = value; }
        }

        private bool m_IsInProximity;
        public bool IsInProximity
        {
            get { return m_IsInProximity; }
            set { m_IsInProximity = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="S_ProximityEventArgs"/> class.
        /// </summary>
        /// <param name="_sender">The GameObject that fires the event. The Interactive Object (Root)</param>
        /// <param name="_other">The colliding GameObject</param>
        /// <param name="_isInProximity">true means the camera entered the proximity zone, false means exits the zone</param>
        public S_ProximityEventArgs(object _sender, S_InteractiveObject _other, bool _isInProximity)
        {
            m_Sender = _sender;
            m_Other = _other;
            m_IsInProximity = _isInProximity;
        }

        public S_ProximityEventArgs(object _sender)
        {
            m_Sender = _sender;
        }
    }
}
