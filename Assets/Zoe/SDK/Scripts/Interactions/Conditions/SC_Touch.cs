using UnityEngine;
using System;
using System.Collections.Generic;
using Gaze;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SC_Touch")]
    public class SC_Touch : SC_Collision
    {
        public int TouchIndexAction = 0;

        public override void Setup()
        {
            base.Setup();
            S_EventManager.OnIODestroyed += OnIODestroyed;
        }

        public override void Dispose()
        {
            base.Dispose();
            S_EventManager.OnIODestroyed -= OnIODestroyed;
        }

        public override void Validate()
        {
            m_CollisionsOccuringCount = 0;
            base.Validate();
        }

        protected override bool ConsiderRigsToValidateRequireAll(int _validatedEntriesCount)
        {
            if (TotalElementsToCollide == CountAllCollidingObjectEntryList() + ProximityMap.proximityEntryGroupList.Count)
            {
                return _validatedEntriesCount == ProximityMap.proximityEntryList.Count + ProximityMap.proximityEntryGroupList.Count;
            }
            else
            {
                return false;
            }
        }

        public int GetProximityGroupIndexFromHandsEnum(S_HandsEnum _handCombination)
        {
            UpdateRigSets(S_Proximity.HierarchyRigProximities);

            switch (_handCombination)
            {
                case S_HandsEnum.LEFT:
                    for (int i = 0; i < ProximityRigGroups.Count; i++)
                    {
                        List<S_InteractiveObject> rigGroup = ProximityRigGroups[i];

                        if (rigGroup.Count != 1)
                        {
                            continue;
                        }

                        if (rigGroup[0].GetComponent<S_LeftHandRoot>() == null)
                        {
                            Debug.Log("Not left hand");
                            continue;
                        }

                        return i;
                    }

                    break;
                case S_HandsEnum.RIGHT:
                    for (int i = 0; i < ProximityRigGroups.Count; i++)
                    {
                        List<S_InteractiveObject> rigGroup = ProximityRigGroups[i];

                        if (rigGroup.Count != 1)
                        {
                            continue;
                        }

                        if (rigGroup[0].GetComponent<S_RightHandRoot>() == null)
                        {
                            Debug.Log("Not right hand");
                            continue;
                        }

                        return i;
                    }
                    break;
                case S_HandsEnum.BOTH:
                    for (int i = 0; i < ProximityRigGroups.Count; i++)
                    {
                        List<S_InteractiveObject> rigGroup = ProximityRigGroups[i];

                        if (rigGroup.Count != 2)
                        {
                            continue;
                        }

                        if (rigGroup[0].GetComponent<S_LeftHandRoot>() == null &&
                            rigGroup[1].GetComponent<S_LeftHandRoot>() == null)
                        {
                            continue;
                        }

                        if (rigGroup[0].GetComponent<S_RightHandRoot>() == null &&
                            rigGroup[1].GetComponent<S_RightHandRoot>() == null)
                        {
                            continue;
                        }

                        return i;
                    }
                    break;
            }

            return -1;
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

        protected void OnIODestroyed(S_IODestroyEventArgs _e)
        {
            if (S_Proximity.HierarchyRigProximities.Contains(_e.IO))
            {
                ProximityGroupIndex = 0;
                UpdateRigSets(S_Proximity.HierarchyRigProximities);
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            S_ProximityStates state = (S_ProximityStates)creationData[1];
            bool requireAll = (bool)creationData[3];

            // Require all
            RequireAllProximities = requireAll;

            // State (OnEnter onExit..)
            ProximityMap.proximityStateIndex = (int)state;

            S_HandsEnum handCombination = (S_HandsEnum)creationData[2];
            ProximityGroupIndex = GetProximityGroupIndexFromHandsEnum(handCombination);

            if (ProximityGroupIndex == -1)
            {
                throw new IndexOutOfRangeException(string.Format("Could not find a rig that match the hand combination {0}", handCombination));
            }

            if (handCombination == S_HandsEnum.BOTH && requireAll)
            {
                TotalElementsToCollide = 3;
            }
            else
            {
                TotalElementsToCollide = 2;
            }

            S_ProximityEntry proximityEntry = ProximityMap.AddProximityEntry();
            proximityEntry.DependentGameObject = SpatialStoriesAPI.GetInteractiveObjectWithGUID((string)creationData[0]);

            S_ProximityEntryGroup proximityEntryGroup = ProximityMap.AddProximityEntryGroup();
            List<S_InteractiveObject> rigGroup = ProximityRigGroups[ProximityGroupIndex];
            for (int i = 0; i < rigGroup.Count; i++)
            {
                proximityEntryGroup.AddProximityEntryToGroup(rigGroup[i]);
            }
        }
    }

    public partial class APIExtensions
    {
        public static SC_Touch CreateTouchCondition(this S_InteractionDefinition _def, string _collidingObjectGUID, S_ProximityStates _state, S_HandsEnum _handCombination, bool _requireAll)
        {
            return _def.CreateCondition<SC_Touch>(_collidingObjectGUID, _state, _handCombination, _requireAll);
        }
    }
}
