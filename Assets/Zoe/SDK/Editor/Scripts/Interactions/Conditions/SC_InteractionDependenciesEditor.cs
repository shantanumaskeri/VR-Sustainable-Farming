using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SC_InteractionDependencies))]
    public class SC_InteractionDependenciesEditor : S_Editor
    {
        public static Dictionary<int, SC_InteractionDependenciesEditor> Editors = new Dictionary<int, SC_InteractionDependenciesEditor>();

        private SerializedProperty m_RequireAllProp;
        private SerializedProperty m_AreDependenciesValidProp;
        private S_Interaction m_Interaction;
        private SC_InteractionDependencies m_Deps;
        
        private void OnEnable()
        {
            m_Deps = ((SC_InteractionDependencies)target);
            m_Interaction = m_Deps.GetComponent<S_Interaction>();
            m_RequireAllProp = serializedObject.FindProperty("RequireAll");
            m_AreDependenciesValidProp = serializedObject.FindProperty("AreDependenciesValid");
            Editors.Add(m_Deps.GetInstanceID(), this);
        }

        private void OnDisable()
        {
            Editors.Remove(m_Deps.GetInstanceID());
        }

        public void ShowInspector()
        {
            serializedObject.Update();
            if (CountDependencies() < 2)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    m_Deps.RequireAll = EditorGUILayout.ToggleLeft("Require All", m_Deps.RequireAll);
                }
            }
            else
            {
                m_Deps.RequireAll = EditorGUILayout.ToggleLeft("Require All", m_Deps.RequireAll);
            }
            ShowDependencyList();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnEditorUI()
        {
            if (!IsReferenced())
            {
                CleanDestroy();
            }
        }

        private void ShowDependencyList()
        {
            // If dependencies is null just create an empty list
            if (m_Deps.Dependencies == null)
            {
                m_Deps.Dependencies = new S_Interaction[0];
            }

            for (int i = 0; i < m_Deps.Dependencies.Length; i++)
            {
                GUILayout.BeginHorizontal();
                m_Deps.Dependencies[i] = (S_Interaction)EditorGUILayout.ObjectField(m_Deps.Dependencies[i], typeof(S_Interaction), true, GUILayout.Width(300));
                if (m_Deps.Dependencies.Length > 1)
                {
                    if (GUILayout.Button("-", GUILayout.Width(100)))
                    {
                        m_Deps.Dependencies = S_EditorUtils.RemoveElementFromArrayByIndex<S_Interaction>(m_Deps.Dependencies, i);
                    }
                }
                GUILayout.EndHorizontal();
            }

            // Add dependency button
            if (GUILayout.Button("+", GUILayout.Width(200)))
            {
                AddEmptyDependency();
            }
        }

        public int CountDependencies()
        {
            if (m_Deps.Dependencies == null)
            {
                return 0;
            }
            else
            {
                return m_Deps.Dependencies.Length;
            }
        }

        public void AddEmptyDependency()
        {
            // If dependencies is null just create an empty list
            if (m_Deps.Dependencies == null)
            {
                m_Deps.Dependencies = new S_Interaction[0];
            }

            S_Interaction[] newList = new S_Interaction[m_Deps.Dependencies.Length + 1];
            m_Deps.Dependencies.CopyTo(newList, 0);
            m_Deps.Dependencies = newList;
        }

        /// <summary>
        /// Ensures that deps is refereced on editor mode
        /// </summary>
        private bool IsReferenced()
        {
            return m_Interaction.ActivationDependencies == m_Deps ||
                m_Interaction.DeactivationDependencies == m_Deps ||
                m_Interaction.InteractionDependencies == m_Deps;
        }

        public override void OnRuntimeUI()
        {
            EditorGUILayout.PropertyField(m_AreDependenciesValidProp, new GUIContent("AreDependenciesValid:"));
        }
    }
}
