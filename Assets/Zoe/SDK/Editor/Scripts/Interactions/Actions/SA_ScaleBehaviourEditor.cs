using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_ScaleBehaviour))]
    public class SA_ScaleBehaviourEditor : S_AbstractActionEditor
    {
        private SA_ScaleBehaviour m_scaleBehaviour;
        private readonly string[] m_scaleModes = { "Scale Factor", "Absolute Scale" };

        public override void OnEnable()
        {
            base.OnEnable();
            m_scaleBehaviour = (SA_ScaleBehaviour)target;
            m_abstractAction.ShowOptions = true;
        }

        protected override void ShowAllOptions()
        {
            CustomDisplayDelay();
        }

        public override void OnEditorUI()
        {
            base.OnEditorUI();
            DisplayScaleObjects();
        }

        public override void OnRuntimeUI()
        {
            base.OnEditorUI();
            DisplayScaleObjects();
        }

        private void DisplayScaleObjects()
        {
            S_EditorUtils.DrawSectionTitle("Object(s) to scale", FontStyle.Normal);
            EditorGUILayout.Space();

            if (m_scaleBehaviour.ScaleBehaviour.Count == 0)
            {
                m_scaleBehaviour.ScaleBehaviour.Add(null);
            }            

            // TARGET LIST
            for (int i = 0; i < m_scaleBehaviour.ScaleBehaviour.Count; i++)
            {
                EditorGUILayout.BeginVertical();
                S_ScalingBehaviour behaviour = m_scaleBehaviour.ScaleBehaviour[i];
                if (behaviour != null)
                {
                    if (behaviour.TargetsObject == null)
                    {
                        behaviour.TargetsObject = new List<GameObject>();
                    }
                    S_Utils.DisplayGameObjectList(behaviour.TargetsObject, 250, true);
                    EditorGUILayout.Space();
                    S_EditorUtils.DrawSectionTitle("Scale Mode", FontStyle.Normal);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    behaviour.IndexScalingMode = EditorGUILayout.Popup("", behaviour.IndexScalingMode, m_scaleModes, GUILayout.MaxWidth(150));
                    switch (behaviour.IndexScalingMode)
                    {
                        case 0:
                            behaviour.ScalingMode = S_ScaleMode.SCALE_FACTOR;
                            S_EditorUtils.DrawEditorHint("Current scale will be multiplied by these values");
                            break;

                        default:
                            behaviour.ScalingMode = S_ScaleMode.SCALE_ABSOLUTE;
                            S_EditorUtils.DrawEditorHint("Current scale will be modified to reach the specified values");
                            break;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    behaviour.TargetScaleReference = EditorGUILayout.Vector3Field("", behaviour.TargetScaleReference, GUILayout.MaxWidth(200));
                    EditorGUILayout.Space();
                    behaviour.Duration = EditorGUILayout.FloatField("Time[s]", behaviour.Duration);
                    behaviour.BackAndForth = EditorGUILayout.ToggleLeft("Back&Forth", behaviour.BackAndForth);
                    behaviour.Loop = EditorGUILayout.ToggleLeft("Loop", behaviour.Loop);
                    EditorGUILayout.Space();
                }

                if (m_scaleBehaviour.ScaleBehaviour.Count > 1)
                {
                    if (GUILayout.Button("-", GUILayout.Width(150)))
                    {
                        m_scaleBehaviour.ScaleBehaviour.RemoveAt(i);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            // "ADD" BUTTON
            if (GUILayout.Button("+", GUILayout.Width(200)))
            {
                m_scaleBehaviour.ScaleBehaviour.Add(null);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.MaxWidth(150)))
                {
                    m_scaleBehaviour.ScaleBehaviour.RemoveAt(m_scaleBehaviour.ScaleBehaviour.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}