using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_MicrophonePlay))]
    public class SA_MicrophonePlayEditor : S_AbstractActionEditor
    {
        private SA_MicrophonePlay m_recordMicrophone;

        public override void OnEnable()
        {
            base.OnEnable();
            m_recordMicrophone = (SA_MicrophonePlay)target;
        }

        public override void OnEditorUI()
        {
            DisplayRecordAudio(true);
        }

        public override void OnRuntimeUI()
        {
            DisplayRecordAudio(false);
        }

        private void DisplayRecordAudio(bool _isEdition)
        {
            EditorGUILayout.BeginHorizontal();
            S_EditorUtils.DrawSectionTitle("IO Audio Source", 200);
            m_recordMicrophone.AudioSource = (S_InteractiveObject)EditorGUILayout.ObjectField("", m_recordMicrophone.AudioSource, typeof(S_InteractiveObject), true, GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();

            m_recordMicrophone.AudioName = EditorGUILayout.TextField(new GUIContent("File To Load Audio"), m_recordMicrophone.AudioName, GUILayout.Width(500));
        }
    }
}