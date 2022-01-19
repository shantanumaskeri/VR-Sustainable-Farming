// <copyright file="Gaze_GrabMap.cs" company="apelab sàrl">
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace SpatialStories
{
    [System.Serializable]
    public class S_GrabEntry
    {
        // hand associated
        public UnityEngine.XR.XRNode hand;

        /// <summary>
        /// The associated proximity collider.
        /// </summary>
        public S_InteractiveObject interactiveObject;
    }

    [System.Serializable]
    public class S_GrabMap
    {

        [SerializeField]
        public List<S_GrabEntry> GrabEntryList;

        /// <summary>
        /// The index of the hand's state. (LEFT, RIGHT or BOTH)
        /// </summary>
        public int GrabHandsIndex;

        /// <summary>
        /// The state (GRAB or UNGRAB)
        /// </summary>
        public int GrabStateLeftIndex, GrabStateRightIndex;
        public int VisualGrabStateIndex;

        public S_GrabMap()
        {
            GrabEntryList = new List<S_GrabEntry>();
        }

        public S_GrabEntry AddGrabableEntry()
        {
            S_GrabEntry d = new S_GrabEntry();
            GrabEntryList.Add(d);
            d.hand = UnityEngine.XR.XRNode.LeftHand;
            return d;
        }

        public S_GrabEntry AddGrabableEntry(GameObject _interactiveObject)
        {
            S_GrabEntry d = AddGrabableEntry();
            d.interactiveObject = _interactiveObject.GetComponent<S_InteractiveObject>();
            return d;
        }

        public bool DeleteGrabableEntry(S_GrabEntry d)
        {
            return GrabEntryList.Remove(d);
        }

        public void ClearGrabEntries()
        {
            GrabEntryList.Clear();
        }
    }
}