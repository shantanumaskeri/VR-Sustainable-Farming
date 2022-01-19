// <copyright file="Gaze_HandleEventArgs.cs" company="apelab sàrl">
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
	public class S_HandleEventArgs : EventArgs
	{
		private object m_Sender;
		public object Sender { get { return m_Sender; } }

        private bool m_IsColliding;
		public bool IsColliding { get { return m_IsColliding; } }
        
		private Collider m_Other;
		public Collider Other  { get { return m_Other; } }
        
		/// <summary>
		/// Initializes a new instance of the <see cref="Gaze.S_HandleEventArgs"/> class.
		/// </summary>
		/// <param name="_sender">Sender.</param>
		/// <param name="_isColliding">If set to <c>true</c> is colliding.</param>
		public S_HandleEventArgs (object _sender, bool _isColliding, Collider _other)
		{
			m_Sender = _sender;
			m_IsColliding = _isColliding;
			m_Other = _other;
		}
	}
}