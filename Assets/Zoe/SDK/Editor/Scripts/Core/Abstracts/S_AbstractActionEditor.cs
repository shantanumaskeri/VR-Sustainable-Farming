using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    /******************************************
	 * 
	 * S_AbstractActionEditor
	 * 
	 * Show the common properties in the Unity Editor UI for CA
	 * 
	 * @author Esteban Gallardo
	 */
    public class S_AbstractActionEditor : S_Editor
    {
        protected S_AbstractAction m_abstractAction;
        protected S_VisualHandSelector m_visualHandSelector;

        private Texture2D m_iconAction;

        protected bool m_enableEitherHandsOption = true;
        protected bool m_enableBothHandsOption = true;

        protected int m_HUDButtonHeight = 0;

        // -------------------------------------------
        /* 
		 * OnEnable
		 */
        public virtual void OnEnable()
        {
            m_abstractAction = (S_AbstractAction)target;
            m_iconAction = (Texture2D)Resources.Load<Texture2D>("UI/Interaction/custom_action");

            m_visualHandSelector = new S_VisualHandSelector();
            m_visualHandSelector.Initialize(m_abstractAction.LogoEitherHandValue, m_abstractAction.LogoLeftHandValue, m_abstractAction.LogoRigthHandValue, m_abstractAction.LogoBothHandsValue, m_enableEitherHandsOption, m_enableBothHandsOption);
        }

        // -------------------------------------------
        /* 
         * ShowAllOptions
         */
        protected virtual void ShowAllOptions()
        {
            if (m_abstractAction == null) return;

            m_abstractAction.ShowOptions = EditorGUILayout.ToggleLeft("Show Options", m_abstractAction.ShowOptions);
            if (m_abstractAction.ShowOptions)
            {
                DisplayDelay();
            }
        }

        // -------------------------------------------
        /* 
		 * DisplayDelay
		 */
        protected virtual void DisplayDelay()
        {
            EditorGUILayout.Space();
            m_abstractAction.IsDelayed = EditorGUILayout.ToggleLeft("Delayed", m_abstractAction.IsDelayed);
            if (m_abstractAction.IsDelayed)
            {
                m_abstractAction.Random = EditorGUILayout.ToggleLeft("Random", m_abstractAction.Random);
                if (!m_abstractAction.Random)
                {
                    GUILayout.BeginHorizontal();
                    S_EditorUtils.DrawSectionTitle("Delay");
                    m_abstractAction.Delay = EditorGUILayout.FloatField(m_abstractAction.Delay);
                    S_Utils.EnsureFieldIsPositiveOrZero(ref m_abstractAction.Delay);
                    EditorGUILayout.LabelField("[s]");
                    GUILayout.EndHorizontal();
                }
                else
                if (m_abstractAction.Random)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Min", GUILayout.MaxWidth(50));
                    m_abstractAction.Min = EditorGUILayout.FloatField(m_abstractAction.Min);
                    S_Utils.EnsureFieldIsPositiveOrZero(ref m_abstractAction.Max);
                    EditorGUILayout.LabelField("[s]");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Max", GUILayout.MaxWidth(50));
                    m_abstractAction.Max = EditorGUILayout.FloatField(m_abstractAction.Max);
                    S_Utils.EnsureFieldIsPositiveOrZero(ref m_abstractAction.Max);
                    EditorGUILayout.LabelField("[s]");
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
            }
        }

        // -------------------------------------------
        /* 
		 * CustomDisplayDelay
		 */
        protected void CustomDisplayDelay()
        {
            EditorGUILayout.Space();
            m_abstractAction.IsDelayed = true;
            if (m_abstractAction.IsDelayed)
            {
                GUILayout.BeginHorizontal();
                S_EditorUtils.DrawSectionTitle("Delay", 85, FontStyle.Normal);
                if (m_abstractAction.Random)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        m_abstractAction.Delay = 0;
                        m_abstractAction.Delay = EditorGUILayout.FloatField(m_abstractAction.Delay);
                    }
                }
                else
                {
                    m_abstractAction.Delay = EditorGUILayout.FloatField(m_abstractAction.Delay);
                }
                S_Utils.EnsureFieldIsPositiveOrZero(ref m_abstractAction.Delay);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel += 6;
                m_abstractAction.Random = EditorGUILayout.ToggleLeft("Random", m_abstractAction.Random);
                if (m_abstractAction.Random)
                {
                    GUILayout.BeginHorizontal(GUILayout.MaxWidth(180));
                    m_abstractAction.Min = EditorGUILayout.FloatField("Min [s]", m_abstractAction.Min);
                    S_Utils.EnsureFieldIsPositiveOrZero(ref m_abstractAction.Max);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(GUILayout.MaxWidth(180));
                    m_abstractAction.Max = EditorGUILayout.FloatField("Max [s]", m_abstractAction.Max);
                    S_Utils.EnsureFieldIsPositiveOrZero(ref m_abstractAction.Max);
                    GUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel -= 6;
            }
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

        // -------------------------------------------
        /* 
		 * DisplayHandsSelectionButtons
		 */
        protected virtual void DisplayHandsSelectionButtons()
        {
            m_visualHandSelector.DisplayHandsSelectionButtons(m_HUDButtonHeight);

            m_abstractAction.SetHands(m_visualHandSelector.LogoEitherHandValue, m_visualHandSelector.LogoLeftHandValue, m_visualHandSelector.LogoRigthHandValue, m_visualHandSelector.LogoBothHandsValue);
        }
    }

}