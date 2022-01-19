// <copyright file="Gaze_ProximityMap.cs" company="apelab sàrl">
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
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    [Serializable]
    public class S_ProximityEntry
    {
        /// <summary>
        /// The associated proximity collider.
        /// </summary>
        public S_InteractiveObject DependentGameObject;

        // list of all colliding objects
        private List<S_InteractiveObject> m_CollidingObjects;

        public List<S_InteractiveObject> CollidingObjects
        {
            get
            {
                if (m_CollidingObjects == null)
                {
                    m_CollidingObjects = new List<S_InteractiveObject>();
                }
                return m_CollidingObjects;
            }
        }

        public S_ProximityEntry()
        {
            m_CollidingObjects = new List<S_InteractiveObject>();
        }

        public void AddCollidingObject(S_InteractiveObject g)
        {
            if (!m_CollidingObjects.Contains(g))
            {
                m_CollidingObjects.Add(g);
            }
        }

        public void RemoveCollidingObject(S_InteractiveObject g)
        {
            m_CollidingObjects.Remove(g);
        }

        private bool IsColliding()
        {
            return m_CollidingObjects.Count > 0 ? true : false;
        }

        public bool IsValid(int _colliderStateIndex)
        {

            if (_colliderStateIndex.Equals((int)S_ProximityStates.ENTER) && IsColliding())
                return true;
            else if (_colliderStateIndex.Equals((int)S_ProximityStates.EXIT) && !IsColliding())
                return true;

            return false;
        }

        public void DisplayCollidingObjects()
        {
            if (m_CollidingObjects == null)
                return;
            Debug.Log("Entry " + DependentGameObject + " is colliding with :");
            foreach (S_InteractiveObject item in m_CollidingObjects)
            {
                Debug.Log(item.name);
            }
        }
    }

    [System.Serializable]
    public class S_ProximityEntryGroup
    {
        [SerializeField]
        public List<S_ProximityEntry> proximityEntries = new List<S_ProximityEntry>();

        public void AddProximityEntryToGroup(S_InteractiveObject dependentObject)
        {
            S_ProximityEntry p = new S_ProximityEntry();
            p.DependentGameObject = dependentObject;
            proximityEntries.Add(p);
        }


    }

    [System.Serializable]
    public class S_ProximityMap
    {
        public List<S_ProximityEntry> proximityEntryList;
        public List<S_ProximityEntryGroup> proximityEntryGroupList;

        public int NumValidCollidingObjects { get { return GetValidatedEntriesCount(); } }

        /// <summary>
        /// The index of the collider state. (OnEnter or OnExit)
        /// </summary>
        public int proximityStateIndex;
        public int proximityIndexByName = -1;

        /// <summary>
        /// Are all the interactive objects inside the proximity ?
        /// Used if OnExit & RequireAll specified because we need to know first that they all were inside at the same time.
        /// </summary>
        private bool isEveryoneColliding = false;

        public bool IsEveryoneColliding { get { return isEveryoneColliding; } }

        public S_ProximityMap()
        {
            proximityEntryList = new List<S_ProximityEntry>();
            proximityEntryGroupList = new List<S_ProximityEntryGroup>();
        }

        public S_ProximityEntry AddProximityEntry()
        {
            S_ProximityEntry d = new S_ProximityEntry();
            proximityEntryList.Add(d);
            return d;
        }

        public S_ProximityEntryGroup AddProximityEntryGroup()
        {
            S_ProximityEntryGroup d = new S_ProximityEntryGroup();
            proximityEntryGroupList.Add(d);
            return d;
        }

        public bool DeleteProximityEntry(S_ProximityEntry d)
        {
            return proximityEntryList.Remove(d);
        }

        public bool DeleteProximityEntryGroup(S_ProximityEntryGroup d)
        {
            return proximityEntryGroupList.Remove(d);
        }

        public void AddCollidingObjectToEntry(S_ProximityEntry _entry, S_InteractiveObject _collidingObject, bool displayCollidingObjects = false)
        {
            _entry.AddCollidingObject(_collidingObject);
            if (displayCollidingObjects)
                _entry.DisplayCollidingObjects();
        }

        public void RemoveCollidingObjectToEntry(S_ProximityEntry _entry, S_InteractiveObject _collidingObject, bool displayCollidingObjects = false)
        {
            _entry.RemoveCollidingObject(_collidingObject);
            if (displayCollidingObjects)
                _entry.DisplayCollidingObjects();
        }

        /// <summary>
        /// Returns the number of validated entries.
        /// </summary>
        /// <returns>The validated entries count.</returns>
        public int GetValidatedEntriesCount()
        {
            int count = 0;
            foreach (S_ProximityEntry p in proximityEntryList)
            {
                if (p.IsValid(proximityStateIndex))
                    count++;
            }

            foreach (S_ProximityEntryGroup g in proximityEntryGroupList)
            {
                foreach (S_ProximityEntry p in g.proximityEntries)
                {
                    if (p.IsValid(proximityStateIndex))
                    {
                        count++;
                        break;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Determines whether every Interactive Object in the list is colliding
        /// </summary>
        /// <returns><c>true</c> if everyone is colliding; otherwise, <c>false</c>.</returns>
        public void UpdateEveryoneColliding()
        {
            foreach (S_ProximityEntry p in proximityEntryList)
            {
                if (p.CollidingObjects.Count < 1)
                {
                    return;
                }
            }
            foreach (S_ProximityEntryGroup g in proximityEntryGroupList)
            {
                foreach (S_ProximityEntry p in g.proximityEntries)
                {
                    if (p.CollidingObjects.Count < 1)
                    {
                        return;
                    }
                }
            }
            isEveryoneColliding = true;
        }

        public void ResetEveryoneColliding()
        {
            isEveryoneColliding = false;
        }
    }
}