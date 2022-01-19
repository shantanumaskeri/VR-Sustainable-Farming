using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_LoadScene))]
    [CanEditMultipleObjects]
    public class SA_LoadSceneEditor : S_AbstractActionEditor
    {
        private SA_LoadScene m_scene;

        public override void OnEnable()
        {
            base.OnEnable();
            m_scene = (SA_LoadScene)target;
        }

        public override void OnEditorUI()
        {
            DisplaySceneLoad(true);
        }

        public override void OnRuntimeUI()
        {
            DisplaySceneLoad(true);
        }

        protected override void DisplayDelay()
        {
            CustomDisplayDelay();
        }

        private void DisplaySceneLoad(bool _isEdition)
        {
            DisplayDelay();


            m_scene.SceneName = EditorGUILayout.TextField(new GUIContent("Scene to Load"), m_scene.SceneName, GUILayout.Width(500));
#if PHOTON_INSTALLED
            m_scene.Networked = EditorGUILayout.Toggle(new GUIContent("Networked transition"), m_scene.Networked);
#endif

        }
    }
}