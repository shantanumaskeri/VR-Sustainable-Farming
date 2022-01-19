// <copyright file="Gaze_Proximity.cs" company="apelab sàrl">
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
using Gaze;

namespace SpatialStories
{
	public class S_Manipulation : MonoBehaviour
    {
		public bool IsDebugActive = false;

		void OnTriggerEnter (Collider other)
		{
			if (IsDebugActive)
                UnityEngine.Debug.Log ("Gaze_Handle (" + transform.parent.name + ") OnTriggerEnter with " + other.name);

			S_EventManager.FireHandleEvent (new S_HandleEventArgs (this, true, other));
		}

		void OnTriggerExit (Collider other)
		{
			if (IsDebugActive)
                UnityEngine.Debug.Log ("Gaze_Handle (" + transform.parent.name + ") OnTriggerExit with " + other.name);

			S_EventManager.FireHandleEvent (new S_HandleEventArgs (this, false, other));
		}
	}
}
