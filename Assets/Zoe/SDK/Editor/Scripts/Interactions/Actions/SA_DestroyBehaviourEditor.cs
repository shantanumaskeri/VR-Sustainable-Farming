using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_DestroyBehaviour))]
    [CanEditMultipleObjects]
    public class SA_DestroyBehaviourEditor : S_AbstractActionEditor
    {
        private SA_DestroyBehaviour m_enableObjects;

        public override void OnEnable()
        {
            base.OnEnable();
            m_enableObjects = (SA_DestroyBehaviour)target;
        }

        public override void OnEditorUI()
        {
            DisplayEnableObjects(true);
        }

        public override void OnRuntimeUI()
        {
            DisplayEnableObjects(false);
        }

        protected override void DisplayDelay()
        {
            CustomDisplayDelay();
        }

        private void DisplayEnableObjects(bool _isEdition)
        {
            DisplayDelay();

            S_EditorUtils.DrawSectionTitle("GameObjects to Destroy");

            S_Utils.DisplayTransformList(m_enableObjects.TargetsToDestroy, 300, true);
        }
    }
}