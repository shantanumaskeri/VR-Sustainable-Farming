using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_TranslateBehaviour))]
    public class SA_TranslateBehaviourEditor : S_AbstractActionEditor
    {
        private SA_TranslateBehaviour m_translateBehaviours;

        public override void OnEnable()
        {
            base.OnEnable();
            m_translateBehaviours = (SA_TranslateBehaviour)target;
        }

        protected override void ShowAllOptions()
        {
            CustomDisplayDelay();
        }

        public override void OnEditorUI()
        {
            base.OnEditorUI();
            DisplayTranslateObjects();
        }

        public override void OnRuntimeUI()
        {
            base.OnEditorUI();
            DisplayTranslateObjects();
        }

        private void DisplayTranslateObjects()
        {
            S_EditorUtils.DrawSectionTitle("Objects To Move");

            if (m_translateBehaviours != null)
            {
                if (m_translateBehaviours.TranslateBehaviours != null)
                {
                    if (m_translateBehaviours.TranslateBehaviours.Count == 0)
                    {
                        m_translateBehaviours.TranslateBehaviours.Add(null);
                    }
                }
            }

            // TARGET LIST
            for (int i = 0; i < m_translateBehaviours.TranslateBehaviours.Count; i++)
            {
                S_TranslateBehaviour behaviour = m_translateBehaviours.TranslateBehaviours[i];
                if (behaviour != null)
                {
                    EditorGUILayout.BeginVertical();

                    // ++OBJECT TO MOVE++
                    if (behaviour.TargetsObject == null)
                    {
                        behaviour.TargetsObject = new List<GameObject>();
                    }
                    S_Utils.DisplayGameObjectList(behaviour.TargetsObject, 300, true);
                    EditorGUILayout.Space();

                    // ++TARGETS++
                    S_EditorUtils.DrawSectionTitle("Destination (Transform)");
                    if (behaviour.TargetsPosition != null)
                    {
                        for (int j = 0; j < behaviour.TargetsPosition.Count; j++)
                        {
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.BeginHorizontal();
                            behaviour.TargetsPosition[j] = ((Transform)EditorGUILayout.ObjectField("", behaviour.TargetsPosition[j], typeof(Transform), true, GUILayout.Width(300)));

                            // REMOVE: TARGET DESTINATION
                            if (GUILayout.Button("-", GUILayout.Width(100)))
                            {
                                behaviour.TargetsPosition.RemoveAt(j);
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    // ADD: TARGET DESTINATION
                    if (GUILayout.Button("+", GUILayout.Width(200)))
                    {
                        if (behaviour.TargetsPosition == null)
                        {
                            behaviour.TargetsPosition = new List<Transform>();
                        }

                        behaviour.TargetsPosition.Add(null);

                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("-"))
                        {
                            behaviour.TargetsPosition.RemoveAt(behaviour.TargetsPosition.Count - 1);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.Space();

                    // ++PROPERTIES++
                    behaviour.InterpolateRotation = EditorGUILayout.ToggleLeft("Consider Rotation", behaviour.InterpolateRotation);
                    if (!behaviour.InterpolateRotation)
                    {
                        behaviour.ParabolicHeight = EditorGUILayout.FloatField("Parabola Height", behaviour.ParabolicHeight);
                        behaviour.Flips = EditorGUILayout.IntField("Number of flips", behaviour.Flips);
                    }
                    behaviour.Offset = EditorGUILayout.FloatField("Offset [m]", behaviour.Offset);
                    behaviour.Duration = EditorGUILayout.FloatField("Duration [s]", behaviour.Duration);

                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}
