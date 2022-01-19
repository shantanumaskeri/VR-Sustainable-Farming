using Gaze;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_Collision))]
    public class SC_CollisionEditor : S_AbstractConditionEditor
    {
        private readonly string[] m_collisionNameStates = { "Enter", "Exit" };

        protected SC_Collision m_Condition;
        protected List<S_InteractiveObject> m_HierarchyProximities;
        protected List<S_InteractiveObject> m_HierarchyIOsScripts;
        protected bool m_enableDisplayHands = false;

        private SerializedProperty m_RequireAllSerialized;

        public override void OnEnable()
        {
            base.OnEnable();

            m_RequireAllSerialized = serializedObject.FindProperty("RequireAllProximities");

            m_visualHandSelector.LoadHandAssets();

            m_Condition = (SC_Collision)target;
            if (m_Condition != null)
            {
                if (m_Condition.ProximityMap.proximityEntryGroupList.Count < 1 && S_Proximity.HierarchyRigProximities.Count > 1)
                {
                    m_Condition.ProximityMap.AddProximityEntryGroup();
                }
                m_HierarchyIOsScripts = new List<S_InteractiveObject>();
                m_HierarchyProximities = new List<S_InteractiveObject>();
            }
        }

        protected virtual void ActionTypeToTriggerTheCondition()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawEditorHint("Checks if two objects enter/exit collision (uses the Proximity collider)");
            if (m_Condition != null)
            {
                S_EditorUtils.DrawSectionTitle("Collision Mode");
                EditorGUILayout.Space();

                // display the OnEnter OR OnExit condition for the list
                EditorGUILayout.BeginHorizontal();
                if (m_Condition.ProximityMap.proximityIndexByName == -1)
                {
                    switch ((S_ProximityStates)m_Condition.ProximityMap.proximityStateIndex)
                    {
                        case S_ProximityStates.ENTER:
                            m_Condition.ProximityMap.proximityIndexByName = 0;
                            break;

                        case S_ProximityStates.EXIT:
                            m_Condition.ProximityMap.proximityIndexByName = 1;
                            break;

                        default:
                            break;
                    }
                }
                m_Condition.ProximityMap.proximityIndexByName = EditorGUILayout.Popup("", m_Condition.ProximityMap.proximityIndexByName, m_collisionNameStates, GUILayout.MaxWidth(150));
                switch (m_Condition.ProximityMap.proximityIndexByName)
                {
                    case 0:
                        m_Condition.ProximityMap.proximityStateIndex = (int)S_ProximityStates.ENTER;
                        break;

                    case 1:
                        m_Condition.ProximityMap.proximityStateIndex = (int)S_ProximityStates.EXIT;
                        break;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        private int GetNumberInstantiatedObjects()
        {
            int counterObjects = 0;
            for (int i = 0; i < m_Condition.ProximityMap.proximityEntryList.Count; i++)
            {
                if (m_Condition.ProximityMap.proximityEntryList[i].DependentGameObject != null)
                {
                    counterObjects++;
                }
            }
            return counterObjects;
        }

        protected override void DisplayHandsSelectionButtons()
        {
            if (m_enableDisplayHands)
            {
                base.DisplayHandsSelectionButtons();
            }
        }

        protected virtual void DisplayTargetObjects()
        {
            // ++++ TARGETS OBJECTS ++++
            S_EditorUtils.DrawSectionTitle("Target Objects");

            // IN CASE IT'S EMPTY INITIALIZE WITH A NEW EMPTY INSTANCE
            if (m_Condition.ProximityMap.proximityEntryList.Count == 0)
            {
                m_Condition.ProximityMap.AddProximityEntry();
            }

            if (m_Condition.ProximityMap.proximityEntryList.Count < 2)
            {
                EditorGUILayout.HelpBox("Add at least two objects.", MessageType.Warning);
            }

            if (m_Condition.ProximityMap.proximityEntryList.Count >= 3)
            {
                m_RequireAllSerialized.boolValue = EditorGUILayout.ToggleLeft("Require all", m_RequireAllSerialized.boolValue);
            }

            for (int i = 0; i < m_Condition.ProximityMap.proximityEntryList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300));

                // Set the default collider for the gaze
                if (m_Condition.ProximityMap.proximityEntryList[i].DependentGameObject == null)
                {
                    m_Condition.ProximityMap.proximityEntryList[i].DependentGameObject = m_Condition.GetComponentInParent<S_InteractiveObject>();
                }

                var proximityObject = EditorGUILayout.ObjectField(m_Condition.ProximityMap.proximityEntryList[i].DependentGameObject, typeof(S_InteractiveObject), true);

                if (proximityObject != null)
                    m_Condition.ProximityMap.proximityEntryList[i].DependentGameObject = (S_InteractiveObject)proximityObject;

                if (m_Condition.ProximityMap.proximityEntryList.Count > 1)
                {
                    if (GUILayout.Button("-"))
                    {
                        m_Condition.ProximityMap.DeleteProximityEntry(m_Condition.ProximityMap.proximityEntryList[i]);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // display 'add' button
            if (GUILayout.Button("+", GUILayout.MaxWidth(300)))
            {
                S_ProximityEntry d = m_Condition.ProximityMap.AddProximityEntry();
                EditorGUILayout.BeginHorizontal();

                // assign the first interactive object in the hierarchy list by default (0)
                if (m_HierarchyProximities != null && m_HierarchyProximities.Count > 0)
                {
                    if (d.DependentGameObject == null)
                    {
                        d.DependentGameObject = m_Condition.GetComponentInParent<S_InteractiveObject>();
                    }

                    var proximityObject = EditorGUILayout.ObjectField(d.DependentGameObject, typeof(S_InteractiveObject), true);

                    if (proximityObject != null)
                        d.DependentGameObject = (S_InteractiveObject)proximityObject;

                    if (GUILayout.Button("-", GUILayout.MaxWidth(300)))
                    {
                        m_Condition.ProximityMap.DeleteProximityEntry(d);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public override void OnEditorUI()
        {
            if (m_Condition != null)
            {
                m_HierarchyIOsScripts = (FindObjectsOfType(typeof(S_InteractiveObject)) as S_InteractiveObject[]).ToList();

                m_HierarchyProximities.Clear();
                for (int i = 0; i < m_HierarchyIOsScripts.Count; i++)
                {
                    UpdateProximitiesList(m_HierarchyIOsScripts[i].gameObject);
                }

                ActionTypeToTriggerTheCondition();

                if (m_HierarchyProximities.Count < 2)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("This condition requires a Camera(IO) in your scene", MessageType.Warning);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    if (m_Condition.ProximityMap.proximityEntryList.Count > 0)
                    {
                        // NOTE don't use foreach to avoid InvalidOperationException
                        for (int i = 0; i < m_Condition.ProximityMap.proximityEntryList.Count; i++)
                        {
                            // delete from the list the gazables no more in the hierarchy
                            if (!m_HierarchyProximities.Contains(m_Condition.ProximityMap.proximityEntryList[i].DependentGameObject))
                            {
                                m_Condition.ProximityMap.DeleteProximityEntry(m_Condition.ProximityMap.proximityEntryList[i]);
                            }
                        }
                    }

                    EditorGUILayout.Space();

                    DisplayTargetObjects();

                    EditorGUILayout.Space();

                    // update the list of all possible rig groups
                    m_Condition.UpdateRigSets(S_Proximity.HierarchyRigProximities);

                    // This is a HOTFIX for solving editor problems that appear when user deletes every element of rig except one and there is a proximity rig group set up somewhere
                    // will need to be changed if custom proximity groups (other than rig group) are implemented
                    if (S_Proximity.HierarchyRigProximities.Count < 2)
                    {
                        if (m_Condition.ProximityMap.proximityEntryGroupList.Count > 0)
                            m_Condition.ProximityMap.proximityEntryGroupList.Clear();
                    }
                    else
                    {
                        DisplayHandsSelectionButtons();
                    }
                }

                base.OnEditorUI();

                EditorGUILayout.Space();
            }
        }

        // -------------------------------------------
        /*
		 * ActionAfterHandSelection
		 */
        protected override void ActionAfterHandSelection()
        {
            m_Condition.ProximityGroupIndex = 0;
            if (m_Condition.LogoEitherHandValue)
            {
                m_Condition.ProximityGroupIndex = 5;
                m_Condition.TotalElementsToCollide = 1;
                m_Condition.RequireAllProximities = false;
            }
            if (m_Condition.LogoLeftHandValue)
            {
                m_Condition.ProximityGroupIndex = 3;
                m_Condition.TotalElementsToCollide = 1;
                m_Condition.RequireAllProximities = false;
            }
            if (m_Condition.LogoRigthHandValue)
            {
                m_Condition.ProximityGroupIndex = 1;
                m_Condition.TotalElementsToCollide = 1;
                m_Condition.RequireAllProximities = false;
            }
            if (m_Condition.LogoBothHandsValue)
            {
                m_Condition.ProximityGroupIndex = 5;
                m_Condition.TotalElementsToCollide = 3;
                m_Condition.RequireAllProximities = true;
            }

            // SET THE RIG TO COLLIDE
            if (m_Condition.ProximityGroupIndex != 0)
            {
                for (int i = 0; i < m_Condition.ProximityMap.proximityEntryGroupList.Count; i++)
                {
                    if (m_Condition.ProximityGroupIndex >= m_Condition.ProximityRigGroups.Count)
                    {
                        m_Condition.ProximityGroupIndex = 0;
                    }

                    m_Condition.ProximityMap.proximityEntryGroupList[i].proximityEntries.Clear();
                    for (int j = 0; j < m_Condition.ProximityRigGroups[m_Condition.ProximityGroupIndex].Count; j++)
                    {
                        S_InteractiveObject rigToCollide = m_Condition.ProximityRigGroups[m_Condition.ProximityGroupIndex][j];
                        m_Condition.ProximityMap.proximityEntryGroupList[i].AddProximityEntryToGroup(rigToCollide);
                    }
                }
            }
        }

        protected void UpdateProximitiesList(GameObject g)
        {
            S_Proximity prox = g.GetComponentInChildrenBFS<S_Proximity>();
            if (prox != null)
            {
                S_InteractiveObject proximity = S_Utils.GetIOFromGameObject(prox.gameObject);
                m_HierarchyProximities.Add(proximity);
            }
        }

        public override void OnRuntimeUI()
        {
            if (m_Condition != null)
            {
                base.OnRuntimeUI();

                EditorGUILayout.BeginHorizontal();
                if (m_Condition.IsValid)
                {
                    RenderSatisfiedLabel("Proximities Validated:");
                    RenderSatisfiedLabel("True");
                }
                else
                {
                    RenderNonSatisfiedLabel("Proximities Validated:");
                    RenderNonSatisfiedLabel("False");
                }
                EditorGUILayout.EndHorizontal();

                string objectsInProximity = "";

                foreach (S_ProximityEntry pe in m_Condition.ProximityMap.proximityEntryList)
                {
                    //TODO @apelab modify this method because it displays multiple times the same object
                    for (int i = 0; i < pe.CollidingObjects.Count; i++)
                    {
                        objectsInProximity = string.Concat(objectsInProximity, ", ", pe.CollidingObjects[i].name);
                    }
                }
                if (objectsInProximity.Length > 0)
                {
                    RenderSatisfiedLabel("Objects in proximity:");
                    RenderSatisfiedLabel(objectsInProximity.Substring(1, objectsInProximity.Length - 1));

                }
                else
                {
                    RenderNonSatisfiedLabel("Objects in proximity:");
                    RenderNonSatisfiedLabel("---");
                }
                EditorGUILayout.Space();
            }
        }


    }

}
