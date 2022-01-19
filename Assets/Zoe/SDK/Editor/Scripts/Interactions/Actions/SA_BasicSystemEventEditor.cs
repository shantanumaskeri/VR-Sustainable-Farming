using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_BasicSystemEvent))]
    public class SA_BasicSystemEventEditor : S_AbstractActionEditor
    {
        private SA_BasicSystemEvent m_basicSystemEvent;

        public override void OnEnable()
        {
            base.OnEnable();
            m_basicSystemEvent = (SA_BasicSystemEvent)target;
        }

        public override void OnEditorUI()
        {
            base.OnEditorUI();
            if (DisplayBasicSystemEvent())
            {
                EditorUtility.SetDirty(m_basicSystemEvent);
            }
        }

        public override void OnRuntimeUI()
        {
            base.OnEditorUI();
            DisplayBasicSystemEvent();
        }

        private bool DisplayBasicSystemEvent()
        {
            bool output = false;

            S_EditorUtils.DrawSectionTitle("");
            m_basicSystemEvent.EventName = EditorGUILayout.TextField(new GUIContent("Event Name"), m_basicSystemEvent.EventName, GUILayout.Width(500));

            // LIST PARAMS
            for (int i = 0; i < m_basicSystemEvent.EventParams.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                m_basicSystemEvent.EventParams[i] = EditorGUILayout.TextField(new GUIContent(i.ToString()), m_basicSystemEvent.EventParams[i], GUILayout.Width(400));

                // and a '-' button to remove it if needed
                if (GUILayout.Button("-", GUILayout.Width(100)))
                {
                    output = true;
                    m_basicSystemEvent.EventParams.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            // "ADD" BUTTON
            if (GUILayout.Button("+", GUILayout.Width(400)))
            {
                output = true;
                m_basicSystemEvent.EventParams.Add("");

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    m_basicSystemEvent.EventParams.RemoveAt(m_basicSystemEvent.EventParams.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
            }

            return output;
        }
    }
}