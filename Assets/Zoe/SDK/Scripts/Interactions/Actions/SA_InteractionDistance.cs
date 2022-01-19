using UnityEngine;

namespace SpatialStories
{
    public class SA_InteractionDistance : S_AbstractAction
    {
        public float NewDistance;

        public SA_InteractionDistance(){}

        protected override void ActionLogic()
        {
            InteractiveObject.GrabDistance = NewDistance;
            InteractiveObject.TouchDistance = NewDistance;
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            NewDistance = (float)creationData[0];
        }
    }
    
    public static partial class APIExtensions
    {
        public static SA_InteractionDistance CreateManipulationModeAction(this S_InteractionDefinition _def, float _newDinstance)
        {
            return _def.CreateAction<SA_InteractionDistance>(_newDinstance);
        }
    }
}
