using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_ApplyForce))]
    public class SA_ApplyForceEditor : S_AbstractActionEditor
    {
        private SA_ApplyForce m_forceAction;

        public override void OnEnable()
        {
            base.OnEnable();
            m_forceAction = (SA_ApplyForce)target;
        }

        public override void OnEditorUI()
        {
            DisplayApplyForce(true);
        }

        public override void OnRuntimeUI()
        {
            DisplayApplyForce(false);
        }

        protected override void DisplayDelay()
        {
            CustomDisplayDelay();
        }

        private void DisplayApplyForce(bool _isEdition)
        {
            DisplayDelay();

            S_EditorUtils.DrawSectionTitle("Target Object", FontStyle.Normal);
            m_forceAction.Target = (Rigidbody)EditorGUILayout.ObjectField("", m_forceAction.Target, typeof(Rigidbody), true, GUILayout.MaxWidth(200));
            EditorGUILayout.Space();

            m_forceAction.HandImpulse = EditorGUILayout.ToggleLeft("Hand Impulse", m_forceAction.HandImpulse);
            if (!m_forceAction.HandImpulse)
            {
                S_EditorUtils.DrawSectionTitle("Destination", FontStyle.Normal);
                m_forceAction.Destination = (GameObject)EditorGUILayout.ObjectField("", m_forceAction.Destination, typeof(GameObject), true, GUILayout.MaxWidth(200));
            }
            EditorGUILayout.Space();
            m_forceAction.Force = EditorGUILayout.FloatField("Force Applied", m_forceAction.Force, GUILayout.MinWidth(150), GUILayout.MaxWidth(250));
        }
    }
}