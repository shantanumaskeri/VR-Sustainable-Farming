using System;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_Teleport))]
    public class SC_TeleportEditorEditor : S_AbstractConditionEditor
    {
        private SC_Teleport m_teleport;

        public override void OnEnable()
        {
            base.OnEnable();

            m_teleport = (SC_Teleport)target;
        }

        protected override void ShowAllOptions()
        {
            base.ShowAllOptions();

            DisplayCollisionObjects();
        }

        private void DisplayCollisionObjects()
        {
            S_EditorUtils.DrawSectionTitle("Target Object (s)", FontStyle.Normal);

            S_Utils.DisplayInteractiveObjectList(m_teleport.TargetObjects);
        }
    }
}