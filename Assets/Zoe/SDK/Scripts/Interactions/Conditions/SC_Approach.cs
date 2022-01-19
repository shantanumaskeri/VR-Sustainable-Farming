using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SC_Approach")]
    public class SC_Approach : S_AbstractCondition
    {
        public List<S_InteractiveObject> TargetsObjects = new List<S_InteractiveObject>();
        public float DetectionDistance;
        public S_ApproachStates ApproachMode;
        public bool ConsiderBoundaries = false;
        public bool RequireAll = false;

        private bool m_Setup = false;

        public SC_Approach()
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

        public override void Update()
        {
            if (!m_Setup)
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
                S_Proximity proximityItem = item.GetComponentInChildren<S_Proximity>();

                float distanceToCamera = -1;
                if (ConsiderBoundaries)
                {
                    Vector3 sizeProximity = proximityItem.gameObject.GetComponent<BoxCollider>().size / 2;
                    Vector3 normalToPlayer = (S_InputManager.Instance.gameObject.transform.position - proximityItem.gameObject.transform.position).normalized;
                    Vector3 closestDistanceToPlayer = proximityItem.gameObject.transform.position + (normalToPlayer * sizeProximity.magnitude);
                    distanceToCamera = Vector3.Distance(S_InputManager.Instance.gameObject.transform.position, closestDistanceToPlayer);
                }
                else
                {
                    distanceToCamera = Vector3.Distance(S_InputManager.Instance.gameObject.transform.position, proximityItem.gameObject.transform.position);
                }

                if (ApproachMode == S_ApproachStates.ENTER)
                {
                    if (RequireAll)
                    {
                        isInsideTargets = isInsideTargets && (distanceToCamera < DetectionDistance);
                    }
                    else
                    {
                        if (distanceToCamera < DetectionDistance)
                        {
                            isInsideTargets = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (RequireAll)
                    {
                        isInsideTargets = isInsideTargets && (distanceToCamera > DetectionDistance);
                    }
                    else
                    {
                        if (distanceToCamera > DetectionDistance)
                        {
                            isInsideTargets = true;
                            break;
                        }
                    }
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
        public static SC_Approach CreateApproachCondition(this S_InteractionDefinition _def, string _objectToGazeAtGUID, float _detectionDistance, S_ApproachStates _approachState, bool _useProximitySize = false)
        {
            return _def.CreateCondition<SC_Approach>(_objectToGazeAtGUID, _detectionDistance, _approachState, _useProximitySize);
        }

        public static SC_Approach CreateApproachCondition(this S_InteractionDefinition _def, List<string> _objectToGazeAtGUID, float _detectionDistance, S_ApproachStates _approachState, bool _useProximitySize = false)
        {
            return _def.CreateCondition<SC_Approach>(_objectToGazeAtGUID, _detectionDistance, _approachState, _useProximitySize);
        }
    }
}
