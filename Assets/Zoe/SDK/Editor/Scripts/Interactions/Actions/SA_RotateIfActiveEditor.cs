using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_RotateIfActive))]
    public class SA_RotateIfActiveEditor : S_AbstractActionEditor
    {
        private SA_RotateIfActive m_rotateIfActive;

        public override void OnEnable()
        {
            base.OnEnable();
            m_rotateIfActive = (SA_RotateIfActive)target;
        }

        public override void OnEditorUI()
        {
            base.OnEditorUI();
            DisplayRotationObjects();
        }

        public override void OnRuntimeUI()
        {
            base.OnEditorUI();
            DisplayRotationObjects();
        }

        private void DisplayRotationObjects()
        {
            S_EditorUtils.DrawSectionTitle("Target Object (s)", FontStyle.Normal);

            // TARGET LIST
            S_Utils.DisplayTransformList(m_rotateIfActive.Target);

            m_rotateIfActive.IncrementRotation = EditorGUILayout.Vector3Field("Rotation Increment", m_rotateIfActive.IncrementRotation);
        }
    }
}