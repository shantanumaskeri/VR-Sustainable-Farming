using Gaze;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_Drop))]
    public class SC_DropEditor : SC_CollisionEditor
    {
        private readonly string[] m_dropIndexesName = { "Drop", "Ready To Drop", "Cancel Drop", "Remove" };

        private SC_Drop m_DropCondition;
        private List<string> m_DndDropTargetsNames = new List<string>();
        private List<GameObject> m_DragAndDropGameObjects = new List<GameObject>();

        public override void OnEnable()
        {
            base.OnEnable();

            m_DropCondition = (SC_Drop)target;
        }

        protected override void ActionTypeToTriggerTheCondition()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawEditorHint("Checks if an object has been dropped into a drop target.", false);
            EditorGUILayout.HelpBox("Make sure Drag&Drop is enabled at the root of your dropped object", MessageType.Info);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            S_EditorUtils.DrawSectionTitle("Drop Object");
            m_DropCondition.SourceObject = (S_InteractiveObject)EditorGUILayout.ObjectField(m_DropCondition.SourceObject, typeof(S_InteractiveObject), true, GUILayout.MaxWidth(200));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            S_EditorUtils.DrawSectionTitle("Mode", FontStyle.Normal);
            if (m_DropCondition.IndexDropEvent == -1)
            {
                switch (m_DropCondition.DropEvent)
                {
                    case S_DragAndDropStates.DROP:
                        m_DropCondition.IndexDropEvent = 0;
                        break;
                    case S_DragAndDropStates.DROPREADY:
                        m_DropCondition.IndexDropEvent = 1;
                        break;
                    case S_DragAndDropStates.DROPREADYCANCELED:
                        m_DropCondition.IndexDropEvent = 2;
                        break;

                    case S_DragAndDropStates.PICKUP:
                    case S_DragAndDropStates.REMOVE:
                        m_DropCondition.IndexDropEvent = 3;
                        break;

                    case S_DragAndDropStates.INIT:
                        m_DropCondition.IndexDropEvent = 0;
                        break;
                }
            }
            m_DropCondition.IndexDropEvent = EditorGUILayout.Popup("", m_DropCondition.IndexDropEvent, m_dropIndexesName, GUILayout.MaxWidth(100));
            switch (m_DropCondition.IndexDropEvent)
            {
                case 0:
                    m_DropCondition.DropEvent = S_DragAndDropStates.DROP;
                    break;

                case 1:
                    m_DropCondition.DropEvent = S_DragAndDropStates.DROPREADY;
                    break;

                case 2:
                    m_DropCondition.DropEvent = S_DragAndDropStates.DROPREADYCANCELED;
                    break;

                case 3:
                    m_DropCondition.DropEvent = S_DragAndDropStates.REMOVE;
                    break;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            DisplayAvailableDragAndDropForObject();
        }

        private void DisplayAvailableDragAndDropForObject()
        {
            UpdateDropTargetsNames();

            EditorGUILayout.LabelField(new GUIContent("Target (s)", " Use existing IO’s in the scene or create an automatic target below for your IO."));

            // help message if no target is specified
            if (m_DropCondition.TargetsObjects == null)
            {
                m_DropCondition.TargetsObjects = new List<GameObject>();
            }
            // for each DnD target
            for (int i = 0; i < m_DropCondition.TargetsObjects.Count; i++)
            {
                if (m_DropCondition.TargetsObjects[i] == null)
                {
                    m_DropCondition.TargetsObjects.RemoveAt(i);
                }
                else
                {
                    if (m_DragAndDropGameObjects.Contains(m_DropCondition.TargetsObjects[i]))
                    {
                        EditorGUILayout.BeginHorizontal();

                        m_DropCondition.TargetsObjects[i] = m_DragAndDropGameObjects[EditorGUILayout.Popup(m_DragAndDropGameObjects.IndexOf(m_DropCondition.TargetsObjects[i]), m_DndDropTargetsNames.ToArray(), GUILayout.MaxWidth(200))];

                        // and a '-' button to remove it if needed
                        if (GUILayout.Button("-", GUILayout.MaxWidth(50)))
                            m_DropCondition.TargetsObjects.Remove(m_DropCondition.TargetsObjects[i]);

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            // display 'add' button
            if (GUILayout.Button("+", GUILayout.MaxWidth(200)))
            {
                if (m_DragAndDropGameObjects.Count == 0)
                    return;

                m_DropCondition.TargetsObjects.Add(m_DragAndDropGameObjects[0]);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.MaxWidth(200)))
                {
                    m_DropCondition.TargetsObjects.Remove(m_DragAndDropGameObjects[0]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        private void UpdateDropTargetsNames()
        {
            m_DndDropTargetsNames.Clear();
            m_DragAndDropGameObjects.Clear();

            // rebuild them
            S_InteractiveObject[] interactiveObjects = FindObjectsOfType(typeof(S_InteractiveObject)) as S_InteractiveObject[];
            foreach (S_InteractiveObject interactiveObject in interactiveObjects)
            {
                if (!((interactiveObject.GetComponentInParent<S_InputManager>() != null)
                    || (interactiveObject.GetComponentInParent<S_InteractiveObject>() == m_DropCondition.SourceObject)))
                {
                    m_DragAndDropGameObjects.Add(interactiveObject.gameObject);
                    m_DndDropTargetsNames.Add(interactiveObject.gameObject.name);
                }
            }
        }

        protected override void DisplayTargetObjects()
        {
            m_Condition.ProximityMap.proximityEntryList.Clear();

            if (m_DropCondition.SourceObject != null)
            {
                m_Condition.ProximityMap.AddProximityEntry();
                m_Condition.ProximityMap.proximityEntryList[0].DependentGameObject = m_DropCondition.SourceObject;
            }
            else
            {
                return;
            }

            for (int i = 0; i < m_DropCondition.TargetsObjects.Count; i++)
            {
                if (m_DropCondition.TargetsObjects[i] != null)
                {
                    m_Condition.ProximityMap.AddProximityEntry();
                    m_Condition.ProximityMap.proximityEntryList[i + 1].DependentGameObject = m_DropCondition.TargetsObjects[i].GetComponentInParent<S_InteractiveObject>();
                }
            }
        }

        public override void OnEditorUI()
        {
            ActionTypeToTriggerTheCondition();
            DisplayTargetObjects();
        }
    }
}
