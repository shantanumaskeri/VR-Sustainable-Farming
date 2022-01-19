using UnityEngine;

namespace SpatialStories
{
    public class SA_ChangeMaterial : S_AbstractAction
    {
        public MeshRenderer MeshTarget;
        public Material MaterialTarget;

        public SA_ChangeMaterial(){}

        public override void SetupUsingApi(GameObject _interaction)
        {
        }

        protected override void ActionLogic()
        {
            MeshTarget.material = MaterialTarget;
        }
    }
}
