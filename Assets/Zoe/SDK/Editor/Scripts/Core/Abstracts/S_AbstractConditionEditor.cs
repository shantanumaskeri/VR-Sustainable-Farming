using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    public class S_AbstractConditionEditor : S_Editor
    {
        protected S_AbstractCondition m_abstractCondition;

        private readonly string[] m_focusLossModes = { "Reset count", "Keep progress", "Decrease progressively" };

        private Texture2D m_iconCondition;

        protected S_VisualHandSelector m_visualHandSelector;

        protected bool m_enableEitherHandsOption = true;
        protected bool m_enableBothHandsOption = true;

        protected int m_HUDButtonHeight = 0;

        // -------------------------------------------
        /* 
		 * OnEnable
		 */
        public virtual void OnEnable()
        {
            m_abstractCondition = (S_AbstractCondition)target;

            m_iconCondition = (Texture2D)Resources.Load<Texture2D>("UI/Interaction/custom_condition");

            m_visualHandSelector = new S_VisualHandSelector();
            m_visualHandSelector.Initialize(m_abstractCondition.LogoEitherHandValue, m_abstractCondition.LogoLeftHandValue, m_abstractCondition.LogoRigthHandValue, m_abstractCondition.LogoBothHandsValue, m_enableEitherHandsOption, m_enableBothHandsOption);
        }

        // -------------------------------------------
        /* 
		 * ShowDuration
		 */
        private void ShowDuration()
        {
            m_abstractCondition.IsDuration = EditorGUILayout.ToggleLeft(" Minimum duration required", m_abstractCondition.IsDuration);
            EditorGUI.indentLevel++;
            S_EditorUtils.DrawEditorHint("How long this condition needs to remain true");

            // DURATION
            if (m_abstractCondition.IsDuration)
            {
                GUILayout.BeginHorizontal();
                m_abstractCondition.FocusDuration = EditorGUILayout.FloatField("Duration [s]", m_abstractCondition.FocusDuration, GUILayout.MaxWidth(400));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                m_abstractCondition.FocusLossModeIndex = EditorGUILayout.Popup("If condition turns false", m_abstractCondition.FocusLossModeIndex, m_focusLossModes, GUILayout.MaxWidth(400));
                if (m_abstractCondition.FocusLossModeIndex.Equals((int)S_FocusLossMode.DECREASE_PROGRESSIVELY))
                {
                    m_abstractCondition.FocusLossSpeed = EditorGUILayout.FloatField("Speed Factor", m_abstractCondition.FocusLossSpeed);
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
        }

        // -------------------------------------------
        /* 
		 * ShowAllOptions
		 */
        protected virtual void ShowAllOptions()
        {
            if (m_abstractCondition == null) return;
            ShowDuration();
        }

        // -------------------------------------------
        /* 
		 * RenderDefaultLabel
		 */
        public static void RenderDefaultLabel(string text)
        {
            EditorGUILayout.LabelField(text);
        }

        // -------------------------------------------
        /* 
		 * RenderNonSatisfiedLabel
		 */
        public static void RenderNonSatisfiedLabel(string text)
        {
            GUI.color = Color.gray;
            EditorGUILayout.LabelField(text);
            GUI.color = Color.white;
        }

        // -------------------------------------------
        /* 
		 * RenderSatisfiedLabel
		 */
        public static void RenderSatisfiedLabel(string text)
        {
            EditorGUILayout.LabelField(text, EditorStyles.whiteLabel);
        }

        // -------------------------------------------
        /* 
		 * DisplayHandsSelectionButtons
		 */
        protected virtual void DisplayHandsSelectionButtons()
        {
            m_visualHandSelector.DisplayHandsSelectionButtons(m_HUDButtonHeight);

            m_abstractCondition.SetHands(m_visualHandSelector.LogoEitherHandValue, m_visualHandSelector.LogoLeftHandValue, m_visualHandSelector.LogoRigthHandValue, m_visualHandSelector.LogoBothHandsValue);

            ActionAfterHandSelection();
        }

        // -------------------------------------------
        /* 
		 * ActionAfterHandSelection
		 */
        protected virtual void ActionAfterHandSelection()
        {

        }

        // -------------------------------------------
        /* 
		 * OnEditorUI
		 */
        public override void OnEditorUI()
        {
            ShowAllOptions();
        }

        // -------------------------------------------
        /* 
		 * OnRuntimeUI
		 */
        public override void OnRuntimeUI()
        {
            ShowAllOptions();
        }
    }

}