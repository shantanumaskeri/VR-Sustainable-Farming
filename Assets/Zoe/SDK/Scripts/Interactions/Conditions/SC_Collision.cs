using UnityEngine;
using System;
using System.Collections.Generic;
using Gaze;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SC_Collision")]
    public class SC_Collision : S_AbstractCondition
    {
        protected int m_EntryInGroupIndex;
        public S_ProximityMap ProximityMap = new S_ProximityMap();

        /// <summary>
        /// All the proximities in the list are required in order to be validated.
        /// </summary>
        public bool RequireAllProximities;
        protected List<GameObject> m_CollidingProximities;
        public List<GameObject> CollidingProximities { get { return m_CollidingProximities; } }
        protected int m_CollisionsOccuringCount = 0;
        protected int m_CollisionsLocalCount = 0;

        [HideInInspector]
        public int TotalElementsToCollide = 0;
        [HideInInspector]
        public int InternalProximityGroupIndex = 0;

        /// <summary>
        /// Whether this Interactive Object's Proximity is colliding.
        /// </summary>
        public bool IsInProximity;

        [SerializeField]
        public int ProximityGroupIndex = 0;
        [SerializeField]
        public List<string> RigCombinations = new List<string>();
        [SerializeField]
        public List<List<S_InteractiveObject>> ProximityRigGroups = new List<List<S_InteractiveObject>>();

        public override void Setup()
        {
            S_EventManager.OnProximityEvent += OnProximityEvent;
            m_CollidingProximities = new List<GameObject>();
        }


        public override void Dispose()
        {
            if (m_CollidingProximities != null)
            {
                m_CollidingProximities.Clear();
            }            
            S_EventManager.OnProximityEvent -= OnProximityEvent;
        }

        protected virtual void OnProximityEvent(S_ProximityEventArgs e)
        {
            if (e.Sender.Equals(InteractiveObject))
                IsInProximity = e.IsInProximity;

            bool valid = HandleProximity(e);

            if ((!ProximityMap.proximityStateIndex.Equals((int)S_ProximityStates.INSIDE))
                && !((ProximityMap.proximityStateIndex.Equals((int)S_ProximityStates.ENTER)) && (IsDuration && (FocusDuration > 0))))
            {
                if (valid)
                    Validate();
            }
        }

        protected void ResetProximitiesCondition()
        {
            Invalidate();
            ProximityMap.ResetEveryoneColliding();
        }

        protected int CountAllCollidingObjectEntryList()
        {
            int counter = 0;
            for (int i = 0; i < ProximityMap.proximityEntryList.Count; i++)
            {
                counter += ProximityMap.proximityEntryList[i].CollidingObjects.Count;
            }
            return counter;
        }

        protected virtual bool ConsiderRigsToValidateRequireAll(int _validatedEntriesCount)
        {
            return _validatedEntriesCount == ProximityMap.proximityEntryList.Count;
        }

        /// <summary>
        /// Checks the proximities conditions validity.
        /// </summary>
        /// <returns><c>true</c>, if proximities was checked, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        protected bool HandleProximity(S_ProximityEventArgs e)
        {
            bool validity = false;

            // get colliding objects
            S_InteractiveObject sender = ((S_InteractiveObject)e.Sender).Proximity.IOScript;
            S_InteractiveObject other = ((S_InteractiveObject)e.Other).Proximity.IOScript;

            // make sure the collision concerns two objects in the list of proximities (-1 if NOT)
            int otherIndex = IsCollidingObjectsInList(other, sender);

            //Debug.LogError("["+ this.gameObject.transform.root.name + "]::otherIndex = " + otherIndex + "::other="+ other.name + "::sender="+ sender.name);
            if (otherIndex != -1)
            {
                Debug.Log("COLLISION::other = "+ other.name + "::sender = "+ sender.name);
                // OnEnter and tested only if validation is not already true (to avoid multiple collision to toggle the proximitiesValidated flag
                if (e.IsInProximity && !IsValid)
                {
                    // update number of collision in the list occuring
                    m_CollisionsOccuringCount++;
                    // if the sender is normal entry, add colliding object to it
                    if (otherIndex > -1)
                    {
                        ProximityMap.AddCollidingObjectToEntry(ProximityMap.proximityEntryList[otherIndex], sender.Proximity.IOScript);
                    }
                    // if the sender is an entryGroup (then otherIndex starts at -2 and goes down instead of going up), add colliding object to the entry of the group that triggered the event
                    else
                    {
                        ProximityMap.AddCollidingObjectToEntry(ProximityMap.proximityEntryGroupList[-otherIndex - 2].proximityEntries[m_EntryInGroupIndex], sender.Proximity.IOScript);
                    }

                    if (ProximityMap.proximityStateIndex.Equals((int)S_ProximityStates.ENTER) ||
                        ProximityMap.proximityStateIndex.Equals((int)S_ProximityStates.INSIDE))
                    {
                        // update number of local collisions
                        m_CollisionsLocalCount++;

                        // get number of valid entries
                        int validatedEntriesCount = ProximityMap.GetValidatedEntriesCount();
                        // OnEnter + RequireAll
                        if (RequireAllProximities)
                        {
                            return ConsiderRigsToValidateRequireAll(validatedEntriesCount);
                        }
                        // OnEnter + NOT RequireAll
                        if (!RequireAllProximities)
                        {
                            return validatedEntriesCount >= 2;
                        }
                    }
                }

                // OnExit
                else if (!e.IsInProximity)
                {
                    // update number of collision in the list occuring
                    m_CollisionsOccuringCount--;
                    // update everyoneIsColliding tag before removing an element
                    ProximityMap.UpdateEveryoneColliding();
                    if (otherIndex > -1)
                    {
                        // update number of local collisions
                        m_CollisionsLocalCount--;

                        // // if the sender is normal entry, remove colliding object to it
                        ProximityMap.RemoveCollidingObjectToEntry(ProximityMap.proximityEntryList[otherIndex], sender.Proximity.IOScript);
                    }
                    else
                    {
                        // if the sender is an entryGroup (then otherIndex starts at -2 and goes down instead of going up), remove colliding object to the entry of the group that triggered the event
                        ProximityMap.RemoveCollidingObjectToEntry(ProximityMap.proximityEntryGroupList[-otherIndex - 2].proximityEntries[m_EntryInGroupIndex], sender.Proximity.IOScript);
                    }

                    // if proximity condition is EXIT
                    if (ProximityMap.proximityStateIndex.Equals((int)S_ProximityStates.EXIT))
                    {
                        if (RequireAllProximities)
                        {
                            // every entry was colliding before the exit
                            if (ProximityMap.IsEveryoneColliding)
                                validity = true;
                            else
                                validity = false;
                            // OnExit + NOT RequireAll
                        }
                        else
                        {
                            ProximityMap.ResetEveryoneColliding();
                            validity = true;
                        }
                        // if proximity condition is ENTER
                    }
                    else
                    {
                        // if proximities was validated
                        if (IsValid)
                        {
                            // and if require all
                            if (RequireAllProximities)
                            {
                                return false;
                            }
                            else
                            {
                                // check there's a valid collision left in the list...
                                if (m_CollisionsOccuringCount > 0)
                                    validity = true;
                                else
                                    validity = false;
                            }
                        }
                    }
                }

            }
            return validity;
        }

        /// <summary>
        /// Returns the selected hands to check
        /// </summary>
        /// <returns>The colliding's (_other) index within list.
        protected S_HandsEnum GetHands()
        {
            if (LogoEitherHandValue) return S_HandsEnum.BOTH;
            if (LogoLeftHandValue) return S_HandsEnum.LEFT;
            if (LogoRigthHandValue) return S_HandsEnum.RIGHT;
            if (LogoBothHandsValue) return S_HandsEnum.BOTH;

            return S_HandsEnum.BOTH;
        }

        /// <summary>
        /// Check if both colliding objects are in the list.
        /// </summary>
        /// <returns>The colliding's (_other) index within list.
        /// If one of the object is not in the list, return -1</returns>
        /// <param name="_other">Other.</param>
        /// <param name="_sender">Sender.</param>
        protected int IsCollidingObjectsInList(S_InteractiveObject _other, S_InteractiveObject _sender, bool _showLog = false)
        {
            int found = 0;
            int foundSameGroup = 0;
            int otherIndex = -1;
            int tmpIndex = -1;
            int entryInGroupIndex = -1;

            // if there is EntryGroups in the ProximityMap
            if (ProximityMap.proximityEntryGroupList.Count > 0)
            {
                // then we iterate through each one of them
                for (int i = 0; i < ProximityMap.proximityEntryGroupList.Count; i++)
                {
                    List<S_ProximityEntry> groupList = ProximityMap.proximityEntryGroupList[i].proximityEntries;
                    for (int j = 0; j < groupList.Count; j++)
                    {
                        if (groupList[j].DependentGameObject != null)
                        {
                            if (groupList[j].DependentGameObject.Equals(_other))
                            {
                                // TODO : check if assigning tmpIndex to the index of the group is ok
                                entryInGroupIndex = j;
                                tmpIndex = -i - 2;
                                found++;
                                foundSameGroup++;

                            }
                            if (groupList[j].DependentGameObject.Equals(_sender))
                            {
                                found++;
                                foundSameGroup++;
                            }
                        }

                        // if sender and other are in the same group, invalidate the collision
                        if (foundSameGroup == 2)
                        {
                            if (_showLog)
                            {
                                Debug.LogError("SC_Proximity::IN THE SAME GROUP");
                            }
                            return -1;
                        }

                    }
                }
            }

            for (int i = 0; i < ProximityMap.proximityEntryList.Count; i++)
            {
                if (ProximityMap.proximityEntryList[i].DependentGameObject != null)
                {
                    if (ProximityMap.proximityEntryList[i].DependentGameObject.Equals(_other))
                    {
                        tmpIndex = i;
                        found++;
                    }
                    if (ProximityMap.proximityEntryList[i].DependentGameObject.Equals(_sender))
                    {
                        found++;
                    }
                }
                if (found == 2)
                {
                    otherIndex = tmpIndex;
                    this.m_EntryInGroupIndex = entryInGroupIndex;
                    break;
                }
            }

            if (_showLog)
            {
                if (otherIndex == -1)
                {
                    // RIGS
                    Debug.LogError("ProximityMap.proximityEntryGroupList.Count = " + ProximityMap.proximityEntryGroupList.Count);
                    for (int i = 0; i < ProximityMap.proximityEntryGroupList.Count; i++)
                    {
                        for (int j = 0; j < ProximityMap.proximityEntryGroupList[i].proximityEntries.Count; j++)
                        {
                            S_ProximityEntry entry = ProximityMap.proximityEntryGroupList[i].proximityEntries[j];
                            Debug.LogError("+++++GROUP ENTRY[" + j + "]=" + entry.DependentGameObject.name);
                        }
                    }
                    // COLLIDERS
                    Debug.LogError("ProximityMap.proximityEntryList.Count = " + ProximityMap.proximityEntryList.Count);
                    for (int i = 0; i < ProximityMap.proximityEntryList.Count; i++)
                    {
                        Debug.LogError("+++++SINGLE ENTRY[" + i + "]=" + ProximityMap.proximityEntryList[i].DependentGameObject.name);
                    }
                }
            }

            return otherIndex;
        }

        /// <summary>
        /// Check if the object is in the list
        /// </summary>
        /// <returns>true or false
        /// If one of the object is not in the list, return -1</returns>
        /// <param name="_object">_object.</param>
        protected bool IsCollidingObjectInList(S_InteractiveObject _object)
        {
            // if there is EntryGroups in the ProximityMap
            if (ProximityMap.proximityEntryGroupList.Count > 0)
            {
                // then we iterate through each one of them
                for (int i = 0; i < ProximityMap.proximityEntryGroupList.Count; i++)
                {
                    List<S_ProximityEntry> groupList = ProximityMap.proximityEntryGroupList[i].proximityEntries;
                    for (int j = 0; j < groupList.Count; j++)
                    {
                        if (groupList[j].DependentGameObject.Equals(_object))
                        {
                            return true;
                        }
                    }
                }
            }

            for (int i = 0; i < ProximityMap.proximityEntryList.Count; i++)
            {
                if (ProximityMap.proximityEntryList[i].DependentGameObject.Equals(_object))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Update the rigs
        /// </summary>
        public void UpdateRigSets(List<S_InteractiveObject> hierarchyRigProximities)
        {
            RigCombinations.Clear();
            ProximityRigGroups.Clear();
            if (hierarchyRigProximities.Count > 1)
            {
                int rigProxNum = hierarchyRigProximities.Count;
                if (rigProxNum > 1)
                {
                    // ++ OR COMBINATIONS ++
                    // get all the subsets of the hierarchyRigProximities list
                    int subsetNum = 1 << rigProxNum;
                    for (int set = 1; set < subsetNum; set++)
                    {
                        // building each subset
                        string s = "  ||  ";
                        List<S_InteractiveObject> l = new List<S_InteractiveObject>();
                        int count = 0;
                        // for each value of  hierarchyRigProximity, check if the value should be in the subset
                        for (int j = 0; j < rigProxNum; j++)
                        {
                            if ((set & (1 << j)) > 0)
                            {
                                s += hierarchyRigProximities[j].gameObject.name + "  ||  ";
                                l.Add(hierarchyRigProximities[j]);
                                count++;
                            }
                        }
                        RigCombinations.Add(s);
                        ProximityRigGroups.Add(l);
                    }
                }
            }
        }

        public override void Reload()
        {
            if (ProximityMap.proximityStateIndex.Equals((int)S_ProximityStates.INSIDE))
            {
                if (m_CollisionsLocalCount == 0)
                {
                    ResetProximitiesCondition();
                }
            }
            else
            {
                ResetProximitiesCondition();
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            S_ProximityStates state = (S_ProximityStates)creationData[0];
            bool requireAll = (bool)creationData[1];
            List<string> _proximityObjectsIDS = (List<string>)creationData[2];

            // Require all
            RequireAllProximities = requireAll;

            // State (OnEnter onExit..)
            ProximityMap.proximityStateIndex = (int)state;

            // Add the game objects to the proximity list
            for (int i = 0; i < _proximityObjectsIDS.Count; i++)
            {
                S_InteractiveObject io = SpatialStoriesAPI.GetInteractiveObjectWithGUID(_proximityObjectsIDS[i]);
                S_ProximityEntry proxEntry = ProximityMap.AddProximityEntry();
                proxEntry.DependentGameObject = io;
            }
        }

        public override void Update()
        {
            base.Update();

            bool isInside = ProximityMap.proximityStateIndex.Equals((int)S_ProximityStates.INSIDE);
            bool isEnter = ProximityMap.proximityStateIndex.Equals((int)S_ProximityStates.ENTER);

            if (isInside || isEnter)
            {
                if (!IsValid)
                {
                    if (IsDuration && (FocusDuration > 0))
                    {
                        if (m_CollisionsOccuringCount > 0)
                        {
                            if (!m_durationActivated)
                            {
                                ActivateDuration();
                            }
                        }
                        else
                        {
                            DeactivateDuration();
                        }
                    }
                }
            }
            /*
            if (m_CollisionsLocalCount > 0)
            {
                // RIGS
                Debug.LogError("ProximityMap.proximityEntryGroupList.Count = " + ProximityMap.proximityEntryGroupList.Count);
                for (int i = 0; i < ProximityMap.proximityEntryGroupList.Count; i++)
                {
                    for (int j = 0; j < ProximityMap.proximityEntryGroupList[i].proximityEntries.Count; j++)
                    {
                        S_ProximityEntry entry = ProximityMap.proximityEntryGroupList[i].proximityEntries[j];
                        Debug.LogError("+++++GROUP ENTRY[" + j + "]=" + entry.DependentGameObject.name);
                    }
                }
                // COLLIDERS
                Debug.LogError("ProximityMap.proximityEntryList.Count = " + ProximityMap.proximityEntryList.Count);
                for (int i = 0; i < ProximityMap.proximityEntryList.Count; i++)
                {
                    Debug.LogError("+++++SINGLE ENTRY[" + i + "]=" + ProximityMap.proximityEntryList[i].DependentGameObject.name);                    
                }                
            }
            */
        }
    }

    public partial class APIExtensions
    {
        public static SC_Collision CreateProximityCondition(this S_InteractionDefinition _def, S_ProximityStates _state, bool _requireAll, List<string> _gameObjectsIDs)
        {
            int minimunNumber = 2;
            if(_gameObjectsIDs.Count < minimunNumber)
            {
                Debug.LogError(String.Format("SpatialStories API> too few gameobjects added to proximity condition on interaction {0}.", _def.Name));
            }
            return _def.CreateCondition<SC_Collision>(_state, _requireAll, _gameObjectsIDs);
        }
    }
}
