// <copyright file="Gaze_CollisionEventArgs.cs" company="apelab sàrl">
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
using System;

namespace Gaze
{
	public class S_CollisionEventArgs : EventArgs
	{
		private object m_Sender;
		public object Sender { get { return m_Sender; } set { m_Sender = value; } }
        
		private Collision m_CollisionInfo;
		public Collision CollisionInfo { get { return m_CollisionInfo; } set { m_CollisionInfo = value; } }
        
		private int m_CollisionState;
		public int CollisionState { get { return m_CollisionState; } set { m_CollisionState = value; } }
        
		/// <summary>
		/// Arguments for animation events related.     
		/// </summary>
		/// <param name="_sender">Is the sender's object.</param>
		public S_CollisionEventArgs ()
		{
		}

		/// <summary>
		/// Arguments for animation events related.     
		/// </summary>
		/// <param name="_sender">Is the sender's object.</param>
		public S_CollisionEventArgs (object _sender, Collision _collisionInfo, int _collisionState)
		{
			m_Sender = _sender;
			m_CollisionInfo = _collisionInfo;
			m_CollisionState = _collisionState;
		}
	}
}
