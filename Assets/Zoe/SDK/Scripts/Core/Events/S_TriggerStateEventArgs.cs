// <copyright file="Gaze_TriggerEventArgs.cs" company="apelab sàrl">
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

namespace Gaze
{
	public class S_TriggerStateEventArgs : EventArgs
	{
		public object Sender { get; private set; }
		public S_TriggerState TriggerState { get; private set; }
        
		/// <summary>
		/// Initializes a new instance of the <see cref="S_TriggerStateEventArgs"/> class.
		/// <param name="_sender">the sender of the event.</param>
		/// <param name="_triggerState">the current State of the sender.</param>
		public S_TriggerStateEventArgs (object _sender, S_TriggerState _triggerState)
		{
			Sender = _sender;
			TriggerState = _triggerState;
		}
	}
}