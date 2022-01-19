using Gaze;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SA_Physics")]
    public class SA_Physics : S_AbstractAction
    {
        public List<S_InteractiveObject> TargetsToActivate = new List<S_InteractiveObject>();

        public bool AffectedByGravity = false;
        public bool EnableCollisions = false;

        public float Mass = 1;
        public bool RandomMass = false;
        public float MinMass = 0;
        public float MaxMass = 0;

        protected override void ActionLogic()
        {
            for (int i = 0; i < TargetsToActivate.Count; i++)
            {
                S_InteractiveObject targetPhysics = TargetsToActivate[i];
                if (targetPhysics != null)
                {
                    // APPLY SELECTED GRAVITY
                    if (targetPhysics.ActualGravityState == S_GravityState.LOCKED)
                        S_GravityManager.ChangeGravityState(targetPhysics, S_GravityRequestType.UNLOCK, false);

                    if (AffectedByGravity)
                        S_GravityManager.ChangeGravityState(targetPhysics, S_GravityRequestType.ACTIVATE_AND_DETACH, false);
                    else
                        S_GravityManager.ChangeGravityState(targetPhysics, S_GravityRequestType.DEACTIVATE_AND_ATTACH, false);

                    S_GravityManager.ChangeGravityState(targetPhysics, S_GravityRequestType.SET_AS_DEFAULT, true);

                    // ENABLE COLLIDERS INSIDE IO
                    Collider[] ioColliders = targetPhysics.GetComponentsInChildren<Collider>();
                    if (ioColliders != null)
                    {
                        for (int j = 0; j < ioColliders.Length; j++)
                        {
                            if (ioColliders[j] != null)
                            {
                                ioColliders[j].enabled = EnableCollisions;
                            }
                        }
                    }

                    if (RandomMass)
                    {
                        float randomMass = UnityEngine.Random.Range(MinMass, MaxMass);
                        S_GravityManager.SetMass(targetPhysics, randomMass);
                    }
                    else
                    {
                        S_GravityManager.SetMass(targetPhysics, Mass);
                    }                    
                }
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            if (creationData[0] is List<string>)
            {
                List<string> targetsToRotate = (List<string>)creationData[0];
                for (int i = 0; i < targetsToRotate.Count; i++)
                {
                    TargetsToActivate.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(targetsToRotate[i]));
                }
            }
            else
            {
                TargetsToActivate.Add(SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[0].ToString()));
            }
            AffectedByGravity = (bool)creationData[1];
            EnableCollisions = (bool)creationData[1];
        }
    }

    public static partial class APIExtensions
    {
        public static SA_Physics CreatePhysicsBehaviour(this S_InteractionDefinition _def, string _toPhysicIOs, bool _affectedByGravity, bool _enabledCollisions)
        {
            return _def.CreateAction<SA_Physics>(_toPhysicIOs, _affectedByGravity, _enabledCollisions);
        }
        public static SA_Physics CreatePhysicsBehaviour(this S_InteractionDefinition _def, List<string> _toColorGUID, bool _affectedByGravity, bool _enabledCollisions)
        {
            return _def.CreateAction<SA_Physics>(_toColorGUID, _affectedByGravity, _enabledCollisions);
        }
    }
}