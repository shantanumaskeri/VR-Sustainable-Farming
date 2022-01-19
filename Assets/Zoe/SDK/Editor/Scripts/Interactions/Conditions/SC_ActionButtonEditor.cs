using Gaze;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_ActionButton))]
    public class SC_ActionButtonEditor : S_AbstractConditionEditor
    {
        protected SC_ActionButton m_ActionButton;
        protected string[] m_platformsNames;
        protected string[] m_viveInputNames;
        protected string[] m_oculusInputNames;
        protected string[] m_inputsNames;
        protected string[] m_gearVrInputNames;

        public override void OnEnable()
        {
            m_ActionButton = (SC_ActionButton)target;

            m_platformsNames = Enum.GetNames(typeof(S_Controllers));
            m_oculusInputNames = Enum.GetNames(typeof(S_OculusInputTypes));
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
            EditorGUILayout.BeginHorizontal();

            // INPUTS LIST
            if (m_ActionButton.InputsMap.InputsEntries.Count < 1)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Add at least one input or deactivate this condition if not needed.", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                m_ActionButton.RequireAllInputs = EditorGUILayout.ToggleLeft("Require all", m_ActionButton.RequireAllInputs);
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < m_ActionButton.InputsMap.InputsEntries.Count; i++)
                {
                    DisplayInputEntry(m_ActionButton.InputsMap.InputsEntries[i]);
                }
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
        }

        private void DisplayInputEntry(S_InputsMapEntry _inputEntry)
        {
            S_Controllers lastSelectedPlatform = _inputEntry.UISelectedPlatform;
            int lastSelectedSpecificInput = _inputEntry.UIControllerSpecificInput;
            bool somethingHasChanged = false;

            // display the entry
            EditorGUILayout.BeginHorizontal();

            // Display the platform
            _inputEntry.UISelectedPlatform = S_Controllers.OCULUS_TOUCH;

            // If the user changes the platform restart the input index to avoid index out of bound exeptions
            if (lastSelectedPlatform != _inputEntry.UISelectedPlatform)
            {
                _inputEntry.UIControllerSpecificInput = 0;
                somethingHasChanged = true;
            }

            // Display the correct input list for the selected platform
            switch (_inputEntry.UISelectedPlatform)
            {
                case S_Controllers.OCULUS_TOUCH:
                    _inputEntry.UIControllerSpecificInput = EditorGUILayout.Popup(_inputEntry.UIControllerSpecificInput, m_oculusInputNames);
                    break;
            }

            somethingHasChanged = lastSelectedSpecificInput != _inputEntry.UIControllerSpecificInput || somethingHasChanged;

            if (somethingHasChanged)
            {
                _inputEntry.InputType = S_PlatformSpecificToGenericInputMapper.ToGenericInput(_inputEntry.UISelectedPlatform, _inputEntry.UIControllerSpecificInput);
            }

            if (GUILayout.Button("-"))
                m_ActionButton.InputsMap.Delete(_inputEntry);

            EditorGUILayout.EndHorizontal();
        }
    }
}