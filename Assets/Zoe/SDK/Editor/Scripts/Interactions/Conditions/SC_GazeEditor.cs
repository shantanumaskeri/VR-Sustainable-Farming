using Gaze;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_Gaze))]
    public class SC_GazeEditor : S_AbstractConditionEditor
    {
        private readonly string[] m_gazeIndexesName = { "Gaze At", "Gaze Away" };

        private SerializedProperty m_IsBeingGazed;
        private SerializedProperty m_InteractiveObject;
        private SerializedProperty m_GazeHoverState;
        private SerializedProperty m_TimeToStay;
        private SC_Gaze m_Condition;

        public override void OnEnable()
        {
            base.OnEnable();

            // Setup the SerializedProperties
            m_InteractiveObject = serializedObject.FindProperty("TargetObject");
            m_IsBeingGazed = serializedObject.FindProperty("IsBeingGazed");
            m_GazeHoverState = serializedObject.FindProperty("GazeHoverState");
            m_TimeToStay = serializedObject.FindProperty("TimeToStay");
            m_Condition = (SC_Gaze)target;
        }

        protected override void ShowAllOptions()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawEditorHint("Checks if the user looks at an object (uses the Gaze Collider)");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            S_EditorUtils.DrawSectionTitle("Target Object");
            m_Condition.TargetObject = (S_InteractiveObject)EditorGUILayout.ObjectField(m_Condition.TargetObject, typeof(S_InteractiveObject), true, GUILayout.MaxWidth(200));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            S_EditorUtils.DrawSectionTitle("Mode");
            if (m_Condition.IndexGazeHoverState == -1)
            {
                switch (m_Condition.GazeHoverState)
                {
                    case S_HoverStates.GAZE_IN:
                    case S_HoverStates.GAZE:
                        m_Condition.IndexGazeHoverState = 0;
                        break;

                    case S_HoverStates.GAZE_OUT:
                        m_Condition.IndexGazeHoverState = 1;
                        break;
                }
            }
            m_Condition.IndexGazeHoverState = EditorGUILayout.Popup("", m_Condition.IndexGazeHoverState, m_gazeIndexesName, GUILayout.MaxWidth(100));
            switch (m_Condition.IndexGazeHoverState)
            {
                case 0:
                    m_Condition.GazeHoverState = S_HoverStates.GAZE_IN;
                    break;

                default:
                    m_Condition.GazeHoverState = S_HoverStates.GAZE_OUT;
                    break;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            base.ShowAllOptions();

            if (m_InteractiveObject.objectReferenceValue == null)
                m_InteractiveObject.objectReferenceValue = m_Condition.GetComponentInParent<S_InteractiveObject>();
        }

        public override void OnRuntimeUI()
        {
            EditorGUILayout.BeginHorizontal();
            if ((m_GazeHoverState.enumValueIndex == (int)S_HoverStates.GAZE_IN)
                || (m_GazeHoverState.enumValueIndex == (int)S_HoverStates.GAZE))
            {
                if (m_IsBeingGazed.boolValue)
                {
                    RenderSatisfiedLabel("Gazed:");
                    RenderSatisfiedLabel("Gazed");
                }
                else
                {
                    RenderNonSatisfiedLabel("Gazed:");
                    RenderNonSatisfiedLabel("Ungazed");
                }
            }
            else
            {
                if (m_IsBeingGazed.boolValue)
                {
                    RenderSatisfiedLabel("Gazed:");
                    RenderSatisfiedLabel("Ungazed");
                }
                else
                {
                    RenderNonSatisfiedLabel("Gazed:");
                    RenderNonSatisfiedLabel("Gazed");
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
