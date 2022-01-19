using Gaze;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_PointAndClick))]
    public class SC_PointAndClickEditor : SC_CollisionEditor
    {
        private readonly string[] m_pointAndClickIndexesName = { "Point At", "Released" };
        private readonly string[] m_pointIndexesName = { "Point At", "Point Away" };

        protected SC_PointAndClick m_pointAndClickCondition;

        public override void OnEnable()
        {
            base.OnEnable();
            m_pointAndClickCondition = (SC_PointAndClick)target;
            m_HUDButtonHeight = 100;
            m_enableDisplayHands = true;
        }

        // This function has been copied and pasted from SC_TouchEditor which allows
        // us to bypass the logic to display a warning when there is less than two
        // targets, which does not make sense for either SC_Touch and SC_PointAndClick
        protected override void DisplayTargetObjects()
        {
            // ++++ TARGETS OBJECTS ++++
            S_EditorUtils.DrawSectionTitle("Target Objects");

            // IN CASE IT'S EMPTY INITIALIZE WITH A NEW EMPTY INSTANCE
            if (m_Condition.ProximityMap.proximityEntryList.Count == 0)
            {
                m_Condition.ProximityMap.AddProximityEntry();
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

        protected override void ActionTypeToTriggerTheCondition()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawEditorHint("Checks if the user points (+ trigger) at an object (uses the Hand Hover collider)");

            EditorGUILayout.Space();
            m_pointAndClickCondition.EnableWithTrigger = EditorGUILayout.ToggleLeft(new GUIContent("Point + trigger", "Checks if the user presses the trigger while pointing"), m_pointAndClickCondition.EnableWithTrigger);

            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("Hand");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(220));
            EditorGUILayout.LabelField("Mode", GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (m_pointAndClickCondition.EnableWithTrigger)
            {
                m_enableBothHandsOption = false;
                m_pointAndClickCondition.RequireBothHands = false;
                m_visualHandSelector.EnableBothHandsOption = false;

                // display the OnEnter OR OnExit condition for the list
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.MaxWidth(220));
                m_pointAndClickCondition.PointAndClickIndexAction = EditorGUILayout.Popup("", m_pointAndClickCondition.PointAndClickIndexAction, m_pointAndClickIndexesName, GUILayout.MaxWidth(100));
                switch (m_pointAndClickCondition.PointAndClickIndexAction)
                {
                    case 0:
                        m_pointAndClickCondition.TouchState = (int)S_TouchStates.OnTouch;
                        break;

                    default:
                        m_pointAndClickCondition.TouchState = (int)S_TouchStates.OnUntouch;
                        break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
            else
            {
                m_visualHandSelector.EnableBothHandsOption = true;
                m_enableBothHandsOption = true;

                if (m_abstractCondition.LogoBothHandsValue)
                {
                    m_pointAndClickCondition.RequireBothHands = true;
                }
                else
                {
                    m_pointAndClickCondition.RequireBothHands = false;
                }


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.MaxWidth(220));
                m_pointAndClickCondition.HoverStateIndexAction = EditorGUILayout.Popup("", m_pointAndClickCondition.HoverStateIndexAction, m_pointIndexesName, GUILayout.MaxWidth(100));
                switch (m_pointAndClickCondition.HoverStateIndexAction)
                {
                    case 0:
                        m_pointAndClickCondition.HoverState = S_ProximityStates.ENTER;
                        break;

                    default:
                        m_pointAndClickCondition.HoverState = S_ProximityStates.EXIT;
                        break;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Make sure POINT AND CLICK is enabled as Manipulation mode for your target object(s).", MessageType.Info);
        }
    }
}
