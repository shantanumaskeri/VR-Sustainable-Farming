using UnityEngine;

namespace SpatialStories
{
    public class SA_ManipulationAbility : S_AbstractAction
    {
        public S_ManipulationModes NewManipulationMode;

        protected override void ActionLogic()
        {
            // InteractiveObject.ManipulationModeIndex;
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
        }
    }
}
