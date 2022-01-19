using Gaze;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_ActionReleased))]
    public class SC_ActionReleasedEditor : SC_ActionButtonEditor
    {
        private SC_ActionReleased m_ActionReleased;

        public override void OnEnable()
        {
            base.OnEnable();

            m_ActionReleased = (SC_ActionReleased)target;
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
            m_ActionReleased.LeftHand = EditorGUILayout.ToggleLeft("Enable Left Hand", m_ActionReleased.LeftHand);
            m_ActionReleased.RightHand = EditorGUILayout.ToggleLeft("Enable Right Hand", m_ActionReleased.RightHand);

            base.RenderInputs();
        }
    }
}