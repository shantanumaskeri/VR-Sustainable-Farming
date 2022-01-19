using Gaze;
using System;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_Approach))]
    public class SC_ApproachEditor : S_AbstractConditionEditor
    {
        private SC_Approach m_Condition;

        public override void OnEnable()
        {
            base.OnEnable();

            m_Condition = (SC_Approach)target;
        }

        protected override void ShowAllOptions()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawEditorHint("Checks if the user approaches another object (uses the Proximity Editor)");

            S_EditorUtils.DrawSectionTitle("Approach Mode");
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            m_Condition.ApproachMode = (S_ApproachStates)EditorGUILayout.Popup((int)m_Condition.ApproachMode, Enum.GetNames(typeof(S_ApproachStates)), GUILayout.MaxWidth(100));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUIStyle alignedTextStyle = new GUIStyle("label");
            alignedTextStyle.alignment = TextAnchor.MiddleRight;
            EditorGUILayout.LabelField("Distance [m]", alignedTextStyle, GUILayout.MaxWidth(100));
            m_Condition.DetectionDistance = EditorGUILayout.FloatField("", m_Condition.DetectionDistance, GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", alignedTextStyle, GUILayout.MaxWidth(50));
            m_Condition.ConsiderBoundaries = EditorGUILayout.ToggleLeft("Calculate from the edges of the Proximity Collider", m_Condition.ConsiderBoundaries);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            S_EditorUtils.DrawSectionTitle("Target Objects");
            EditorGUILayout.Space();
            m_Condition.RequireAll = EditorGUILayout.ToggleLeft("Require All", m_Condition.RequireAll);
            S_Utils.DisplayInteractiveObjectList(m_Condition.TargetsObjects, 200, true, m_Condition.gameObject);
            EditorGUILayout.Space();

            
            EditorGUILayout.Space();

            base.ShowAllOptions();
        }

    }
}