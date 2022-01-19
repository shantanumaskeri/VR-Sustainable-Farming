using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_Animation))]
    public class SA_AnimationEditor : S_AbstractActionEditor
    {
        private SA_Animation m_ActionsScript;
        private List<string> m_SelectedAnimatorTriggers;

        private SerializedProperty m_SerializedTargetAnimator;

        private readonly string[] m_animationModes = { "Disable", "Play" };
        private readonly string[] m_animatorModes = { "Nothing", "Animator Controller" };

        public override void OnEnable()
        {
            base.OnEnable();

            m_SerializedTargetAnimator = serializedObject.FindProperty("_targetAnimator");

            m_ActionsScript = (SA_Animation)target;
            m_SelectedAnimatorTriggers = new List<string>();

            if (m_ActionsScript.Target == null)
            {
                m_ActionsScript.Target = m_ActionsScript.GetComponentInParent<S_InteractiveObject>();
            }
        }

        public override void OnEditorUI()
        {
            ShowAnimationOptions();
        }

        public override void OnRuntimeUI()
        {
            EditorGUILayout.LabelField("No Runtime UI implemented yet :(");
        }

        protected override void DisplayDelay()
        {
            CustomDisplayDelay();
        }

        public void ShowAnimationOptions()
        {
            DisplayDelay();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            S_EditorUtils.DrawSectionTitle("Action", 85, FontStyle.Normal);

            m_ActionsScript.AnimationIndexAction = EditorGUILayout.Popup("", m_ActionsScript.AnimationIndexAction, m_animationModes, GUILayout.MaxWidth(200));
            switch (m_ActionsScript.AnimationIndexAction)
            {
                case 0:
                    m_ActionsScript.AnimationAction = SA_Animation.ANIMATION_ACTION.DEACTIVATE_ANIMATOR;
                    break;

                default:
                    m_ActionsScript.AnimationAction = SA_Animation.ANIMATION_ACTION.LAUNCH_ANIMATION;
                    break;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            S_EditorUtils.DrawSectionTitle("Animator", 85, FontStyle.Normal);
            m_SerializedTargetAnimator.objectReferenceValue = EditorGUILayout.ObjectField(m_SerializedTargetAnimator.objectReferenceValue, typeof(Animator), true, GUILayout.MaxWidth(200));
            EditorGUILayout.EndHorizontal();

            m_ActionsScript.SelectedAnimator = (Animator)m_SerializedTargetAnimator.objectReferenceValue;

            if (m_ActionsScript.AnimationAction == SA_Animation.ANIMATION_ACTION.LAUNCH_ANIMATION)
            {
                DisplayAnimatorTriggers(0);
                EditorGUILayout.Space();
            }
        }

        private void DisplayAnimatorTriggers(int k)
        {
            if (m_ActionsScript.AnimationOption == SA_Animation.ANIMATION_OPTION.MECANIM && FindAnimatorTriggers())
            {
                if (!m_SelectedAnimatorTriggers.Contains(m_ActionsScript.AnimatorTriggers[k]))
                {
                    m_ActionsScript.AnimatorTriggers[k] = m_SelectedAnimatorTriggers[0];
                }

                EditorGUI.indentLevel += 2;
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                S_EditorUtils.DrawSectionTitle("Trigger", 85, FontStyle.Normal);
                m_ActionsScript.AnimatorTriggers[k] = m_SelectedAnimatorTriggers[EditorGUILayout.Popup(m_SelectedAnimatorTriggers.IndexOf(m_ActionsScript.AnimatorTriggers[k]), m_SelectedAnimatorTriggers.ToArray(), GUILayout.MaxWidth(200))];
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel -= 2;
            }
        }
        private bool FindAnimatorTriggers()
        {
            m_SelectedAnimatorTriggers = S_AnimationUtils.FindAnimatorTriggers(m_ActionsScript.SelectedAnimator);
            return m_SelectedAnimatorTriggers.Count > 0;
        }
    }
}
