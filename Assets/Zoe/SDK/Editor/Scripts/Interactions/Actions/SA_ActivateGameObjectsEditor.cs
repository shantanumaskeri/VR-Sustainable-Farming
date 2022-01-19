using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_ActivateGameObjects))]
    [CanEditMultipleObjects]
    public class SA_ActivateGameObjectsEditor : S_AbstractActionEditor
    {
        private SA_ActivateGameObjects m_enableObjects;

        public override void OnEnable()
        {
            base.OnEnable();
            m_enableObjects = (SA_ActivateGameObjects)target;
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

            S_EditorUtils.DrawSectionTitle("GameObjects to Activate", FontStyle.Normal);

            S_Utils.DisplayTransformList(m_enableObjects.TargetsToActivate, 300, true);

            S_EditorUtils.DrawSectionTitle("GameObjects to Deactivate", FontStyle.Normal);

            S_Utils.DisplayTransformList(m_enableObjects.TargetsToDeactivate, 300, true);
        }
    }
}