// <copyright file="Gaze_GrabEventArgs.cs" company="apelab sàrl">
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

namespace Gaze
{
    public class S_GrabEventArgs : EventArgs
    {
        private object m_Sender;
        public object Sender { get { return m_Sender; } }

        private S_GrabManager m_GrabManager;
        public S_GrabManager GrabManager { get { return m_GrabManager; } }
        
        private S_InteractiveObject m_InteractiveObject;
        public S_InteractiveObject InteractiveObject { get { return m_InteractiveObject; } }
        
        private float m_TimeToGrab;
        public float TimeToGrab { get { return m_TimeToGrab; } }
        
        /// <summary>
        /// Arguments for animation events related.     
        /// </summary>
        /// <param name="_sender">Is the sender's object.</param>
        public S_GrabEventArgs(object _sender, S_GrabManager _grabManager, S_InteractiveObject _interactiveObject, float _timeToGrab = 0, bool _forceEnable = false)
        {
            // If the grab manaber is disabled we can force the enable of it
            if (_forceEnable)
                _grabManager.enabled = true;
            m_Sender = _sender;
            m_GrabManager = _grabManager;
            m_InteractiveObject = _interactiveObject;
            m_TimeToGrab = _timeToGrab;
        }
    }
}
