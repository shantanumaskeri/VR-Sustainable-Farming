using UnityEngine;

namespace SpatialStories
{
    public class SA_Colliders : S_AbstractAction
    {
        public enum Options { ACTIVATE, DEACTIVATE }
        public Options Option;

        public SA_Colliders() { }

        protected override void ActionLogic()
        {
            Collider[] AllColliders = InteractiveObject.GetComponentsInChildren<Collider>();
            bool isEnabled = Option == Options.ACTIVATE;

            foreach (Collider collider in AllColliders)
                collider.enabled = isEnabled;
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            Option = (bool)creationData[0] ? Options.ACTIVATE : Options.DEACTIVATE;
        }
    }

    public static partial class APIExtensions
    {
        public static SA_Colliders CreateColliders(this S_InteractionDefinition _def, bool _activate)
        {
            return _def.CreateAction<SA_Colliders>(_activate);
        }
    }
}
