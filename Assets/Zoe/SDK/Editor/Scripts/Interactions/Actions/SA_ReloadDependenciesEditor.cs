using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_ReloadDependencies))]
    public class SA_ReloadDependenciesEditor : S_AbstractActionEditor
    {
        public static Dictionary<int, SA_ReloadDependenciesEditor> Editors = new Dictionary<int, SA_ReloadDependenciesEditor>();

        private SerializedProperty m_RequireAllProp;
        private SerializedProperty m_AreDependenciesValidProp;
        private S_Interaction m_Interaction;
        private SA_ReloadDependencies m_reloadDependencies;

        public override void OnEnable()
        {
            m_reloadDependencies = ((SA_ReloadDependencies)target);
            m_Interaction = m_reloadDependencies.GetComponent<S_Interaction>();
            m_RequireAllProp = serializedObject.FindProperty("RequireAll");
            m_AreDependenciesValidProp = serializedObject.FindProperty("AreDependenciesValid");
            Editors.Add(m_reloadDependencies.GetInstanceID(), this);
        }

        private void OnDisable()
        {
            Editors.Remove(m_reloadDependencies.GetInstanceID());
        }

        public void ShowInspector()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_RequireAllProp, new GUIContent("RequireAll:"));
            ShowDependencyList();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnEditorUI()
        {
            ShowInspector();
        }

        private void ShowDependencyList()
        {
            // If dependencies is null just create an empty list
            if (m_reloadDependencies.Dependencies == null)
            {
                m_reloadDependencies.Dependencies = new S_Interaction[0];
            }

            for (int i = 0; i < m_reloadDependencies.Dependencies.Length; i++)
            {
                GUILayout.BeginHorizontal();
                m_reloadDependencies.Dependencies[i] = (S_Interaction)EditorGUILayout.ObjectField(m_reloadDependencies.Dependencies[i], typeof(S_Interaction), true);
                if (GUILayout.Button("-"))
                {
                    m_reloadDependencies.Dependencies = S_EditorUtils.RemoveElementFromArrayByIndex<S_Interaction>(m_reloadDependencies.Dependencies, i);
                }
                GUILayout.EndHorizontal();
            }

            // Add dependency button
            if (GUILayout.Button("+"))
            {
                S_Interaction[] newList = new S_Interaction[m_reloadDependencies.Dependencies.Length + 1];
                m_reloadDependencies.Dependencies.CopyTo(newList, 0);
                m_reloadDependencies.Dependencies = newList;
            }
        }

        public override void OnRuntimeUI()
        {
            EditorGUILayout.PropertyField(m_AreDependenciesValidProp, new GUIContent("AreDependenciesValid:"));
        }
    }
}