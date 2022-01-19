using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_Physics))]
    public class SA_PhysicsEditor : S_AbstractActionEditor
    {
        private SA_Physics m_actionPhysics;

        public override void OnEnable()
        {
            base.OnEnable();
            m_actionPhysics = (SA_Physics)target;
        }

        public override void OnEditorUI()
        {
            DisplayPhysicObjects(true);
        }

        public override void OnRuntimeUI()
        {
            DisplayPhysicObjects(false);
        }

        protected override void DisplayDelay()
        {
            CustomDisplayDelay();
        }

        private void DisplayPhysicObjects(bool _isEdition)
        {
            DisplayDelay();

            S_EditorUtils.DrawSectionTitle("Target Object (s)", FontStyle.Normal);

            S_Utils.DisplayInteractiveObjectList(m_actionPhysics.TargetsToActivate, 300, true, m_actionPhysics.InteractiveObject.gameObject);

            EditorGUILayout.Space();

            m_actionPhysics.AffectedByGravity = EditorGUILayout.ToggleLeft("Affected by gravity", m_actionPhysics.AffectedByGravity);
            m_actionPhysics.EnableCollisions = EditorGUILayout.ToggleLeft("Enable Collisions", m_actionPhysics.EnableCollisions);

            m_actionPhysics.Mass = EditorGUILayout.FloatField("Change mass [kg]", m_actionPhysics.Mass);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(160));
            EditorGUILayout.BeginVertical();
            m_actionPhysics.RandomMass = EditorGUILayout.ToggleLeft("Random", m_actionPhysics.RandomMass);
            if (m_actionPhysics.RandomMass)
            {
                GUILayout.BeginHorizontal(GUILayout.MaxWidth(180));
                m_actionPhysics.MinMass = EditorGUILayout.FloatField("Min [s]", m_actionPhysics.MinMass);
                S_Utils.EnsureFieldIsPositiveOrZero(ref m_actionPhysics.MinMass);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.MaxWidth(180));
                m_actionPhysics.MaxMass = EditorGUILayout.FloatField("Max [s]", m_actionPhysics.MaxMass);
                S_Utils.EnsureFieldIsAboveLimit(ref m_actionPhysics.MaxMass, m_actionPhysics.MinMass);
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}