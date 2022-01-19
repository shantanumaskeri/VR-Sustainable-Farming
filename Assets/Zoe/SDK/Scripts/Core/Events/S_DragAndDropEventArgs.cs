//-----------------------------------------------------------------------
// <copyright file="LevitationEventArgs.cs" company="apelab sàrl">
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

namespace Gaze
{
    public class S_DragAndDropEventArgs : EventArgs
    {
        /// <summary>
        /// The Drop Object
        /// </summary>
        private object m_Sender;
        public object Sender { get { return m_Sender; } }
        
        /// <summary>
        /// The object dropped on a target
        /// </summary>
        private object m_DropObject;
        public object DropObject { get { return m_DropObject; } }
        
        /// <summary>
        /// The target an object is dropped on
        /// </summary>
        private object m_DropTarget;
        public object DropTarget { get { return m_DropTarget; } }

        private S_DragAndDropStates m_State;
        public S_DragAndDropStates State { get { return m_State; } }
        
        public S_DragAndDropEventArgs(object _sender, object _dropObject, object _dropTarget, S_DragAndDropStates _state)
        {
            m_Sender = _sender;
            m_DropObject = _dropObject;
            m_DropTarget = _dropTarget;
            m_State = _state;
        }
    }
}
