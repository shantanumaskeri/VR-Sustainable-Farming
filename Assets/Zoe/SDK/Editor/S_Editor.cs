using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpatialStories
{
    public abstract class S_Editor : Editor
    {
        public abstract void OnEditorUI();
        public abstract void OnRuntimeUI();

        private bool m_IsDestroyRequired = false;
        
        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "Changes");
            EditorGUI.BeginChangeCheck();

            if (m_IsDestroyRequired)
                DestroyImmediate(target);

            serializedObject.Update();

            if (Application.isPlaying)
                OnRuntimeUI();
            else
                OnEditorUI();

            serializedObject.ApplyModifiedProperties();

            if (!Application.isPlaying && EditorGUI.EndChangeCheck())
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorUtility.SetDirty(target);
            }
        }

        protected void CleanDestroy()
        {
            m_IsDestroyRequired = true;
        }
    }
}