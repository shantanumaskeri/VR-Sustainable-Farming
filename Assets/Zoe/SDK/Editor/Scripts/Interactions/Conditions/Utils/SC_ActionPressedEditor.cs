using Gaze;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_ActionPressed))]
    public class SC_ActionPressedEditor : SC_ActionButtonEditor
    {
        private SC_ActionPressed m_ActionPressed;

        public override void OnEnable()
        {
            base.OnEnable();

            m_ActionPressed = (SC_ActionPressed)target;
        }

        public override void OnEditorUI()
        {
            RenderInputs();
        }

        public override void OnRuntimeUI()
        {
            RenderInputs();
        }

        protected override void RenderInputs()
        {
            m_ActionPressed.LeftHand = EditorGUILayout.ToggleLeft("Enable Left Hand", m_ActionPressed.LeftHand);
            m_ActionPressed.RightHand = EditorGUILayout.ToggleLeft("Enable Right Hand", m_ActionPressed.RightHand);

            base.RenderInputs();
        }
    }
}