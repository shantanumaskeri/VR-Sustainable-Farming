using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SC_Proximity")]
    public class SC_Proximity : S_AbstractCondition
    {
        public List<S_InteractiveObject> TargetsObjects = new List<S_InteractiveObject>();
        public float DetectionDistance;
        public S_ApproachStates ApproachMode;
        public bool ConsiderBoundaries = false;
        public bool RequireAll = false;

        private bool m_Setup = false;

        public SC_Proximity()
        {
        }

        public override void Dispose()
        {
            m_Setup = false;
        }

        public override void Reload()
        {
            Invalidate();
        }

        public override void Setup()
        {
            m_Setup = true;
        }

        public int NumberOfInstantiateObjects()
        {
            int counter = 0;
            foreach (S_InteractiveObject item in TargetsObjects)
            {
                if (item != null)
                {
                    counter++;
                }
            }
            return counter;
        }

        private bool CheckDistanceInsideOtherObjects(S_InteractiveObject _targetObject, bool _considerBoundaries, float _distance, bool _requireAll, S_ApproachStates _approachMode)
        {
            S_Proximity proximityTarget = _targetObject.GetComponentInChildren<S_Proximity>();
            if (proximityTarget == null) return false;

            bool everyoneIsInRange = true;
            
            foreach (S_InteractiveObject item in TargetsObjects)
            {
                if (_targetObject != item)
                {                    
                    S_Proximity proximityItem = item.GetComponentInChildren<S_Proximity>();
                    if (proximityItem == null)
                    {
                        if (_requireAll)
                        {
                            return false;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    Collider targetProximityCollider = proximityTarget.GetComponent<Collider>();
                    Collider itemProximityCollider = proximityItem.GetComponent<Collider>();

                    if (_considerBoundaries)
                    {
                        Vector3 closestItemPosition = itemProximityCollider.ClosestPoint(targetProximityCollider.bounds.center);
                        Vector3 closestTargetPosition = targetProximityCollider.ClosestPoint(itemProximityCollider.bounds.center);

                        if (_requireAll)
                        {
                            if (_approachMode == S_ApproachStates.ENTER)
                            {
                                everyoneIsInRange = everyoneIsInRange && (Vector3.Distance(closestTargetPosition, closestItemPosition) <= _distance);
                            }
                            else
                            {
                                everyoneIsInRange = everyoneIsInRange && (Vector3.Distance(closestTargetPosition, closestItemPosition) > _distance);
                            }
                        }
                        else
                        {
                            if (_approachMode == S_ApproachStates.ENTER)
                            {
                                return (Vector3.Distance(closestTargetPosition, closestItemPosition) <= _distance);
                            }
                            else
                            {
                                return (Vector3.Distance(closestTargetPosition, closestItemPosition) > _distance);
                            }
                        }
                    }
                    else
                    {
                        Vector3 targetPosition = targetProximityCollider.bounds.center;
                        Vector3 itemPosition = itemProximityCollider.bounds.center;

                        if (_requireAll)
                        {
                            if (_approachMode == S_ApproachStates.ENTER)
                            {
                                everyoneIsInRange = everyoneIsInRange && (Vector3.Distance(targetPosition, itemPosition) <= _distance);
                            }
                            else
                            {
                                everyoneIsInRange = everyoneIsInRange && (Vector3.Distance(targetPosition, itemPosition) > _distance);
                            }
                        }
                        else
                        {
                            if (_approachMode == S_ApproachStates.ENTER)
                            {
                                return (Vector3.Distance(targetPosition, itemPosition) <= _distance);
                            }
                            else
                            {
                                return (Vector3.Distance(targetPosition, itemPosition) > _distance);
                            }
                        }
                    }
                }
            }

            return everyoneIsInRange;
        }

        public override void Update()
        {
            if (!m_Setup ||
                IsValid)
            {
                return;
            }

            base.Update();

            bool isInsideTargets = true;
            if (!RequireAll)
            {
                isInsideTargets = false;
            }
            foreach (S_InteractiveObject item in TargetsObjects)
            {
                if (RequireAll)
                {
                    isInsideTargets = isInsideTargets && CheckDistanceInsideOtherObjects(item, ConsiderBoundaries, DetectionDistance, RequireAll, ApproachMode);
                }
                else
                {
                    isInsideTargets = isInsideTargets || CheckDistanceInsideOtherObjects(item, ConsiderBoundaries, DetectionDistance, RequireAll, ApproachMode);
                }                
            }

            if (isInsideTargets)
            {
                if (ApproachMode == S_ApproachStates.ENTER)
                {
                    if (!IsValid)
                    {
                        if (IsDuration && (FocusDuration > 0))
                        {
                            if (!m_durationActivated)
                            {
                                ActivateDuration();
                            }
                        }
                        else
                        {
                            Validate();
                        }
                    }
                }
                else
                {
                    if (ApproachMode == S_ApproachStates.EXIT)
                    {
                        if (!IsValid)
                        {
                            Validate();
                        }
                    }
                    else
                    {
                        if (IsDuration && (FocusDuration > 0))
                        {
                            DeactivateDuration();
                        }
                    }
                }
            }
            else
            {
                if (ApproachMode == S_ApproachStates.ENTER)
                {
                    if (IsDuration && (FocusDuration > 0))
                    {
                        if (m_durationActivated)
                        {
                            DeactivateDuration();
                        }
                    }
                }
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            if (creationData[0] is List<string>)
            {
                List<string> listTargets = (List<string>)creationData[0];
                for (int i = 0; i < listTargets.Count; i++)
                {
                    TargetsObjects.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(listTargets[i]));
                }
            }
            else
            {
                TargetsObjects.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[0].ToString()));
            }
            DetectionDistance = (float)creationData[1];
            ApproachMode = (S_ApproachStates)creationData[2];
            ConsiderBoundaries = (bool)creationData[3];
        }
    }

    /// <summary>
    /// Wrapper for the API in order to create a gaze condition
    /// </summary>
    public static partial class APIExtensions
    {
        public static SC_Proximity CreateProximityCondition(this S_InteractionDefinition _def, string _objectToGazeAtGUID, float _detectionDistance, S_ApproachStates _approachState, bool _useProximitySize = false)
        {
            return _def.CreateCondition<SC_Proximity>(_objectToGazeAtGUID, _detectionDistance, _approachState, _useProximitySize);
        }

        public static SC_Proximity CreateProximityCondition(this S_InteractionDefinition _def, List<string> _objectToGazeAtGUID, float _detectionDistance, S_ApproachStates _approachState, bool _useProximitySize = false)
        {
            return _def.CreateCondition<SC_Proximity>(_objectToGazeAtGUID, _detectionDistance, _approachState, _useProximitySize);
        }
    }
}
