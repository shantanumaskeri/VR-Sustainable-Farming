using Gaze;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_Manipulate))]
    public class SC_ManipulateEditor : S_AbstractConditionEditor
    {
        private readonly string[] m_grabNameStates = { "Manipulate", "Release" };

        public List<GameObject> HierarchyIOs;
        private SC_Manipulate m_Condition;

        public override void OnEnable()
        {
            m_enableBothHandsOption = false;

            base.OnEnable();
            m_Condition = (SC_Manipulate)target;
            HierarchyIOs = new List<GameObject>();

            m_visualHandSelector.LoadHandAssets();
            m_HUDButtonHeight = 60;
        }

        public override void OnEditorUI()
        {
            UpdateInteractiveObjectsList();
            // if there are no grabbables in the scene, exit
            if (HierarchyIOs.Count < 1)
            {
                EditorGUILayout.HelpBox("You need at least one Interactive Object in the scene !", MessageType.Warning);
                return;
            }

            // if there are no entry yet, create a default one
            if (m_Condition.GrabMap.GrabEntryList.Count < 1)
                m_Condition.GrabMap.AddGrabableEntry(m_Condition.InteractiveObject.gameObject);
                
            DisplayHandsSelectionButtons();

            base.ShowAllOptions();
        }

        private int IndexGrabHandSelected(int _originalState)
        {
            switch ((S_GrabStates)_originalState)
            {
                case S_GrabStates.GRAB:
                    m_Condition.GrabMap.VisualGrabStateIndex = 0;
                    break;
                case S_GrabStates.UNGRAB:
                    m_Condition.GrabMap.VisualGrabStateIndex = 1;
                    break;
                case S_GrabStates.HOLDING:
                    m_Condition.GrabMap.VisualGrabStateIndex = 0;
                    break;
            }
            m_Condition.GrabMap.VisualGrabStateIndex = EditorGUILayout.Popup("", m_Condition.GrabMap.VisualGrabStateIndex, m_grabNameStates, GUILayout.MaxWidth(120));
            switch (m_Condition.GrabMap.VisualGrabStateIndex)
            {
                case 0:
                    return (int)S_GrabStates.GRAB;

                default:
                    return (int)S_GrabStates.UNGRAB;
            }
        }

        protected override void ActionAfterHandSelection()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawEditorHint("Checks if the user GRABS or LEVITATES an object (uses the Manipulate collider)");
            S_EditorUtils.DrawSectionTitle("Hand");
            if (m_Condition.LogoEitherHandValue)
            {
                m_Condition.GrabMap.GrabHandsIndex = (int)S_HandsEnum.BOTH;
            }
            if (m_Condition.LogoLeftHandValue)
            {
                m_Condition.GrabMap.GrabHandsIndex = (int)S_HandsEnum.LEFT;
            }
            if (m_Condition.LogoRigthHandValue)
            {
                m_Condition.GrabMap.GrabHandsIndex = (int)S_HandsEnum.RIGHT;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(220));
            EditorGUILayout.LabelField("Mode", GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("", GUILayout.MaxWidth(220));
            if (m_Condition.GrabMap.GrabHandsIndex.Equals((int)S_HandsEnum.LEFT))
            {
                m_Condition.GrabMap.GrabStateLeftIndex = IndexGrabHandSelected(m_Condition.GrabMap.GrabStateLeftIndex);
                m_Condition.GrabMap.GrabEntryList[0].hand = UnityEngine.XR.XRNode.LeftHand;
            }
            else if (m_Condition.GrabMap.GrabHandsIndex.Equals((int)S_HandsEnum.RIGHT))
            {
                m_Condition.GrabMap.GrabStateRightIndex = IndexGrabHandSelected(m_Condition.GrabMap.GrabStateRightIndex);
                m_Condition.GrabMap.GrabEntryList[0].hand = UnityEngine.XR.XRNode.RightHand;
            }
            else
            {
                m_Condition.GrabMap.GrabStateLeftIndex = IndexGrabHandSelected(m_Condition.GrabMap.GrabStateLeftIndex);
                m_Condition.GrabMap.GrabStateRightIndex = m_Condition.GrabMap.GrabStateLeftIndex;
                m_Condition.GrabMap.GrabEntryList[0].hand = UnityEngine.XR.XRNode.LeftHand;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Make sure Manipulation is enabled at the root of your target object.", MessageType.Info);
            EditorGUILayout.Space();

            S_EditorUtils.DrawSectionTitle("Target Object");
            EditorGUILayout.BeginHorizontal();
            var grabObject = EditorGUILayout.ObjectField(m_Condition.GrabMap.GrabEntryList[0].interactiveObject, typeof(S_InteractiveObject), true, GUILayout.MaxWidth(300));
            if (grabObject != null)
            {
                if (grabObject is S_InteractiveObject)
                    m_Condition.GrabMap.GrabEntryList[0].interactiveObject = (S_InteractiveObject)grabObject;
                else
                    m_Condition.GrabMap.GrabEntryList[0].interactiveObject = ((GameObject)grabObject).GetComponent<S_InteractiveObject>();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        /// <summary>
        /// Get all InteractiveObjects in the scene !
        /// Only executed in Editor Mode (not at runtime)
        /// </summary>
        private void UpdateInteractiveObjectsList()
        {
            // clear lists
            HierarchyIOs.Clear();

            S_InteractiveObject[] hierarchyIOsScripts = (FindObjectsOfType(typeof(S_InteractiveObject)) as S_InteractiveObject[]);
            for (int i = 0; i < hierarchyIOsScripts.Length; i++)
            {
                HierarchyIOs.Add(hierarchyIOsScripts[i].gameObject);
            }
        }

        public override void OnRuntimeUI()
        {
            OnEditorUI();
            return;

            S_HandsEnum hand = (S_HandsEnum)m_Condition.GrabMap.GrabHandsIndex;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Hand: ");
            EditorGUILayout.LabelField(hand.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type: ");
            switch (hand)
            {
                case S_HandsEnum.BOTH:
                    EditorGUILayout.LabelField(((S_GrabStates)m_Condition.GrabMap.GrabStateLeftIndex).ToString());
                    break;
                case S_HandsEnum.LEFT:
                    EditorGUILayout.LabelField(((S_GrabStates)m_Condition.GrabMap.GrabStateLeftIndex).ToString());
                    break;
                case S_HandsEnum.RIGHT:
                    EditorGUILayout.LabelField(((S_GrabStates)m_Condition.GrabMap.GrabStateRightIndex).ToString());
                    break;
            }
            EditorGUILayout.LabelField(((S_HandsEnum)m_Condition.GrabMap.GrabHandsIndex).ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Valid: ");
            EditorGUILayout.LabelField(m_Condition.IsValid.ToString());
            EditorGUILayout.EndHorizontal();

        }
    }
}
