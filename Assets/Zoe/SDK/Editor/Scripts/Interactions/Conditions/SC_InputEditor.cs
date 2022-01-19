using Gaze;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_Input))]
    public class SC_InputEditor : S_AbstractConditionEditor
    {
        protected SC_Input m_ActionButton;
        protected string[] m_platformsNames;
        protected string[] m_viveInputNames;
        protected string[] m_oculusInputNames;
        protected string[] m_inputsNames;
        protected string[] m_gearVrInputNames;

        protected string[] m_handNames;
        protected string[] m_baseInputNames;
        protected string[] m_directionNames;
        protected string[] m_stickDirectionNames;

        public S_SingleHandsEnum HandSelected;
        public S_BaseInputTypes BaseInputType;
        public S_DirectionTypes DirectionType;

        List<S_Controllers> m_ControllerTypesInCondition = new List<S_Controllers>();

        public override void OnEnable()
        {
            base.OnEnable();

            m_ActionButton = (SC_Input)target;

            m_platformsNames = Enum.GetNames(typeof(S_Controllers));
            m_oculusInputNames = Enum.GetNames(typeof(S_OculusInputTypes));

            string[] handNames = Enum.GetNames(typeof(S_SingleHandsEnum));
            m_handNames = new string[2];
            for (int i = 0; i < 2; i++)
            {
                m_handNames[i] = handNames[i];
            }
            m_baseInputNames = Enum.GetNames(typeof(S_BaseInputTypes));
            m_directionNames = new string[] { Enum.GetName(typeof(S_DirectionTypes), S_DirectionTypes.PRESSED), Enum.GetName(typeof(S_DirectionTypes), S_DirectionTypes.RELEASED) };
            m_stickDirectionNames = Enum.GetNames(typeof(S_StickDirections));
        }

        public override void OnEditorUI()
        {
            RenderInputs();
        }

        public override void OnRuntimeUI()
        {
            RenderInputs();
        }

        protected virtual void RenderInputs()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawEditorHint("Checks if the user makes an input with the controllers.");

            EditorGUILayout.BeginHorizontal();

            // INPUTS LIST
            if (m_ActionButton.InputsMap.InputsEntries.Count < 1)
            {
                m_ActionButton.InputsMap.Add();
            }

            if (m_ControllerTypesInCondition.Count > 1)
            {
                EditorGUI.BeginDisabledGroup(true);
                if (m_ActionButton.RequireAllInputs)
                {
                    m_ActionButton.RequireAllInputs = false;
                }
            }

            m_ActionButton.RequireAllInputs = EditorGUILayout.ToggleLeft("Require all", m_ActionButton.RequireAllInputs);

            if (m_ControllerTypesInCondition.Count > 1)
            {
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndHorizontal();

            m_ControllerTypesInCondition.Clear();
            for (int i = 0; i < m_ActionButton.InputsMap.InputsEntries.Count; i++)
            {
                S_InputsMapEntry entry = m_ActionButton.InputsMap.InputsEntries[i];

                if (!m_ControllerTypesInCondition.Contains(entry.UISelectedPlatform))
                {
                    m_ControllerTypesInCondition.Add(entry.UISelectedPlatform);
                }

                DisplayInputEntry(entry);
            }

            // "ADD" BUTTON
            if (GUILayout.Button("+"))
            {
                S_InputsMapEntry d = m_ActionButton.InputsMap.Add();

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("-"))
                {
                    m_ActionButton.InputsMap.Delete(d);
                }
                EditorGUILayout.EndHorizontal();
            }

            ShowAllOptions();
        }

        private void DisplayInputEntry(S_InputsMapEntry _inputEntry)
        {
            S_Controllers lastSelectedPlatform = _inputEntry.UISelectedPlatform;
            S_SingleHandsEnum lastUIHandSelected = _inputEntry.UIHandSelected;
            S_BaseInputTypes lastUIBaseInputType = _inputEntry.UIBaseInputType;
            S_DirectionTypes lastUIDirectionType = _inputEntry.UIDirectionType;
            S_StickDirections lastUIStickDirections = _inputEntry.UIStickDirection;
            bool somethingHasChanged = false;

            // display the entry
            EditorGUILayout.BeginHorizontal();

            S_EditorLayoutUtils.ControllerInputSelectionField(ref _inputEntry.UISelectedPlatform, ref _inputEntry.UIHandSelected, ref _inputEntry.UIBaseInputType, ref _inputEntry.UIDirectionType, ref _inputEntry.UIStickDirection);

            // If the user changes the platform restart the input index to avoid index out of bound exeptions
            if (lastSelectedPlatform != _inputEntry.UISelectedPlatform)
            {
                _inputEntry.UIControllerSpecificInput = 0;
                somethingHasChanged = true;
            }

            if ((lastUIHandSelected != _inputEntry.UIHandSelected) ||
                (lastUIBaseInputType != _inputEntry.UIBaseInputType) ||
                (lastUIDirectionType != _inputEntry.UIDirectionType) ||
                (lastUIStickDirections != _inputEntry.UIStickDirection) ||
                somethingHasChanged)
            {
                _inputEntry.Set(_inputEntry.UIHandSelected, _inputEntry.UIBaseInputType, _inputEntry.UIDirectionType, _inputEntry.UIStickDirection);
                // Debug.LogError("_inputEntry.InputType=" + _inputEntry.InputType.ToString());
            }

            if (GUILayout.Button("-"))
                m_ActionButton.InputsMap.Delete(_inputEntry);

            EditorGUILayout.EndHorizontal();
        }
    }
}
