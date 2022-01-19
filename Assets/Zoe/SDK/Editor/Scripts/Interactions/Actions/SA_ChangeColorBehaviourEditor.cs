using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_ChangeColorBehaviour))]
    public class SA_ChangeColorBehaviourEditor : S_AbstractActionEditor
    {
        private SA_ChangeColorBehaviour m_changeColorBehaviour;

        public override void OnEnable()
        {
            base.OnEnable();
            m_changeColorBehaviour = (SA_ChangeColorBehaviour)target;
        }

        public override void OnEditorUI()
        {
            base.OnEditorUI();
            DisplayChangeColorObjects();
        }

        public override void OnRuntimeUI()
        {
            base.OnEditorUI();
            DisplayChangeColorObjects();
        }

        protected override void ShowAllOptions()
        {
            CustomDisplayDelay();
        }

        private void DisplayChangeColorObjects()
        {
            S_EditorUtils.DrawSectionTitle("Target Object (s)", FontStyle.Normal);

            if (m_changeColorBehaviour.ChangeColorhaviour.Count == 0)
            {
                m_changeColorBehaviour.ChangeColorhaviour.Add(null);
            }

            // TARGET LIST
            for (int i = 0; i < m_changeColorBehaviour.ChangeColorhaviour.Count; i++)
            {
                EditorGUILayout.BeginVertical();
                S_ChangeColorBehaviour behaviour = m_changeColorBehaviour.ChangeColorhaviour[i];
                if (behaviour != null)
                {
                    if (behaviour.TargetsObject == null)
                    {
                        behaviour.TargetsObject = new List<GameObject>();
                    }
                    S_Utils.DisplayGameObjectList(behaviour.TargetsObject, 200, true);
                    EditorGUILayout.Space();
                    behaviour.TargetMaterial = ((Material)EditorGUILayout.ObjectField("New Material", behaviour.TargetMaterial, typeof(Material), true));
                    if (behaviour.TargetMaterial != null) behaviour.TargetColor = behaviour.TargetMaterial.color;
                    behaviour.Duration = EditorGUILayout.FloatField("Time[s]", behaviour.Duration);
                    behaviour.Loop = EditorGUILayout.ToggleLeft("Loop", behaviour.Loop);
                    behaviour.BackAndForth = EditorGUILayout.ToggleLeft("Back&Forth", behaviour.BackAndForth);

                    if (m_changeColorBehaviour.ChangeColorhaviour.Count > 1)
                    {
                        if (GUILayout.Button("-", GUILayout.MaxWidth(110)))
                        {
                            m_changeColorBehaviour.ChangeColorhaviour.RemoveAt(i);
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

            // "ADD" BUTTON
            if (GUILayout.Button("+"))
            {
                m_changeColorBehaviour.ChangeColorhaviour.Add(null);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    m_changeColorBehaviour.ChangeColorhaviour.RemoveAt(m_changeColorBehaviour.ChangeColorhaviour.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}