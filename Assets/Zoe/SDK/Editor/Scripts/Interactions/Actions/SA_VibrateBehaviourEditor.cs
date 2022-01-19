using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_VibrateBehaviour))]
    public class SA_VibrateBehaviourEditor : S_AbstractActionEditor
    {
        private SA_VibrateBehaviour m_vibrateBehaviour;

        public override void OnEnable()
        {
            m_enableEitherHandsOption = false;
            base.OnEnable();
            m_visualHandSelector.LoadHandAssets();
            m_vibrateBehaviour = (SA_VibrateBehaviour)target;
        }

        public override void OnEditorUI()
        {
            DisplayVibrateBehaviour(true);
        }

        public override void OnRuntimeUI()
        {
            DisplayVibrateBehaviour(false);
        }

        protected override void DisplayDelay()
        {
            CustomDisplayDelay();
        }

        private void DisplayVibrateBehaviour(bool _isEdition)
        {
            DisplayDelay();

            S_EditorUtils.DrawSectionTitle("Hands to vibrate", FontStyle.Normal);

            m_HUDButtonHeight = 80;
            DisplayHandsSelectionButtons();

            if (m_abstractAction.LogoLeftHandValue)
            {
                m_vibrateBehaviour.Controller = S_HandsEnum.LEFT;
            }
            if (m_abstractAction.LogoRigthHandValue)
            {
                m_vibrateBehaviour.Controller = S_HandsEnum.RIGHT;
            }
            if (m_abstractAction.LogoBothHandsValue)
            {
                m_vibrateBehaviour.Controller = S_HandsEnum.BOTH;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            m_vibrateBehaviour.VibrationFrequency = EditorGUILayout.Slider("Frequency", m_vibrateBehaviour.VibrationFrequency, 0f, 1f);
            m_vibrateBehaviour.VibrationIntensity = EditorGUILayout.Slider("Intensity", m_vibrateBehaviour.VibrationIntensity, 0f, 1f);
            m_vibrateBehaviour.VibrationDuration = EditorGUILayout.Slider("Duration", m_vibrateBehaviour.VibrationDuration, 0f, 2f);
        }
    }
}