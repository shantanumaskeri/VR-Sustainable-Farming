using Gaze;
using System;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_Proximity))]
    public class SC_ProximityEditor : S_AbstractConditionEditor
    {
        private const float MimimumDetectionDistance = 0.01f;

        private SC_Proximity m_Condition;

        public override void OnEnable()
        {
            base.OnEnable();

            m_Condition = (SC_Proximity)target;
        }

        protected override void ShowAllOptions()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawEditorHint("Checks if objects are in proximity from one another (uses the Proximity colliders).");

            S_EditorUtils.DrawSectionTitle("Proximity Mode");
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            m_Condition.ApproachMode = (S_ApproachStates)EditorGUILayout.Popup((int)m_Condition.ApproachMode, Enum.GetNames(typeof(S_ApproachStates)), GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            /*EditorGUILayout.BeginVertical();*/
            EditorGUILayout.BeginHorizontal();
            GUIStyle alignedTextStyle = new GUIStyle("label");
            alignedTextStyle.alignment = TextAnchor.MiddleLeft;
            EditorGUILayout.LabelField("Max Distance [m]", alignedTextStyle, GUILayout.MaxWidth(120));
            m_Condition.DetectionDistance = Mathf.Max(EditorGUILayout.FloatField("", m_Condition.DetectionDistance, GUILayout.MaxWidth(50)), MimimumDetectionDistance);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            S_EditorUtils.DrawEditorHint("Distance is calculated from the center of the Proximity colliders.");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            m_Condition.ConsiderBoundaries = EditorGUILayout.ToggleLeft("Check the distance between the edges of the Proximity colliders", m_Condition.ConsiderBoundaries);
            EditorGUILayout.EndHorizontal();
            /*EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();*/
            EditorGUILayout.Space();

            S_EditorUtils.DrawSectionTitle("Objects In Proximity");
            if (m_Condition.NumberOfInstantiateObjects() < 2)
            {
                S_EditorUtils.DrawEditorHint("Define at least two objects", false);
            }
            else
            {
                if (m_Condition.NumberOfInstantiateObjects() < 3)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        m_Condition.RequireAll = EditorGUILayout.ToggleLeft("Require All", m_Condition.RequireAll);
                    }
                }
                else
                {
                    m_Condition.RequireAll = EditorGUILayout.ToggleLeft("Require All", m_Condition.RequireAll);
                }
            }
            EditorGUILayout.Space();
            S_Utils.DisplayInteractiveObjectList(m_Condition.TargetsObjects, 200, true, m_Condition.gameObject);
            EditorGUILayout.Space();

            base.ShowAllOptions();
        }

    }
}
