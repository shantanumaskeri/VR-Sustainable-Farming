using Gaze;
using UnityEngine;

namespace SpatialStories
{
    public class SA_ManipulationMode : S_AbstractAction
    {
        public S_ManipulationModes NewMode;

        public SA_ManipulationMode(){}

        protected override void ActionLogic()
        {
            InteractiveObject.EnableManipulationMode(NewMode);
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            NewMode = (S_ManipulationModes)creationData[0];
        }

    }
    
    public static partial class APIExtensions
    {
        public static SA_ManipulationMode CreateManipulationModeAction(this S_InteractionDefinition _def, S_ManipulationModes _newMode)
        {
            return _def.CreateAction<SA_ManipulationMode>(_newMode);
        }
    }
}
