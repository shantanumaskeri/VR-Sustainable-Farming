using SpatialStories;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(S_Teleporter))]
    public class S_TeleporterEditor : S_Editor
    {
        private S_Teleporter m_Teleporter;
        private SerializedProperty m_Hotspots;

        private void OnEnable()
        {
            m_Teleporter = (S_Teleporter)target;
            m_Hotspots = serializedObject.FindProperty("HotSpots");
        }

        public void DisplayHotspots()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Hotspots, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        public override void OnEditorUI()
        {
            /*
            // DO NOT DELETE UNTIL UI INSPECTOR IMPLEMENTATION OF S_InputManagerEditor HAS BEEN FULLY APPROVED
            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("ORIENTATION");
            S_EditorUtils.DrawEditorHint("Will the user be reoriented after the teleport?");
            m_Teleporter.OrientOnTeleport = EditorGUILayout.Toggle("Orient on Teleport", m_Teleporter.OrientOnTeleport);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("VISUAL PROPERTIES");
            S_EditorUtils.DrawEditorHint("Customize the visual aspect of the teleport");
            m_Teleporter.GyroPrefab = (GameObject)EditorGUILayout.ObjectField("Target Destination", m_Teleporter.GyroPrefab, typeof(GameObject), true);
            m_Teleporter.GoodDestinationColor = EditorGUILayout.ColorField(new GUIContent("Allowed Destination", "The color that the teleport stream will have when the destination is valid"), m_Teleporter.GoodDestinationColor);
            m_Teleporter.BadDestinationColor = EditorGUILayout.ColorField(new GUIContent("Not Allowed Destination", "The color that the teleport stream will have when the destination is not valid"), m_Teleporter.BadDestinationColor);
            m_Teleporter.LineWidth = EditorGUILayout.FloatField(new GUIContent("Line width", "With of the line in metters"), m_Teleporter.LineWidth);
            m_Teleporter.LineMaterial = (Material)EditorGUILayout.ObjectField("Line Material", m_Teleporter.LineMaterial, typeof(Material), true);

            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("CONSTRAINTS");
            S_EditorUtils.DrawEditorHint("Conditions that the user will need to meet to teleport");
            m_Teleporter.MaxTeleportDistance = EditorGUILayout.FloatField(new GUIContent("Max Teleport Distance", "Ho far in metters the user can teleport"), m_Teleporter.MaxTeleportDistance);
            m_Teleporter.MaxSlope = EditorGUILayout.FloatField(new GUIContent("Max Slope", "Max difference in heigth where the user can teleport"), m_Teleporter.MaxSlope);
            
            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("HOTSPOTS");
            S_EditorUtils.DrawEditorHint("Use them to make the user teleports in precise places");
            DisplayHotspots();
            m_Teleporter.MinHotspotDistance = EditorGUILayout.FloatField(new GUIContent("Minimum detection distance", "Distance to use a teleport hotspot"), m_Teleporter.MinHotspotDistance);

            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("ERROR PREVENTION");
            S_EditorUtils.DrawEditorHint("Use this parameters to avoid user input errors");
            m_Teleporter.HoldTimeToAppear = EditorGUILayout.FloatField(new GUIContent("Hold Duration To Activate", "Time that the user needs to be holding the teleport button in order to make it appear"), m_Teleporter.HoldTimeToAppear);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Input Sensitivity", "Tolerance in the joystic in order to avoid false inputs, where 0 is VERY sensitive"));
            m_Teleporter.InptuThreshold = EditorGUILayout.Slider(m_Teleporter.InptuThreshold, 0.0f, 1.0f);
            EditorGUILayout.EndHorizontal(); 
            m_Teleporter.Cooldown = EditorGUILayout.FloatField(new GUIContent("Cooldown", "Time that the user needs to wait between teleports"), m_Teleporter.Cooldown);
            */
        }

        public override void OnRuntimeUI()
        {
            // DO NOT DELETE UNTIL UI INSPECTOR IMPLEMENTATION OF S_InputManagerEditor HAS BEEN FULLY APPROVED
            /*
            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("ORIENTATION");
            S_EditorUtils.DrawEditorHint("Will the user be reoriented after the teleport?");
            m_Teleporter.OrientOnTeleport = EditorGUILayout.Toggle("Orient on Teleport", m_Teleporter.OrientOnTeleport);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("VISUAL PROPERTIES");
            S_EditorUtils.DrawEditorHint("Customize the visual aspect of the teleport");
            m_Teleporter.GyroPrefab = (GameObject)EditorGUILayout.ObjectField("Target Destination", m_Teleporter.GyroPrefab, typeof(GameObject), true);
            m_Teleporter.GoodDestinationColor = EditorGUILayout.ColorField(new GUIContent("Allowed Destination", "The color that the teleport stream will have when the destination is valid"), m_Teleporter.GoodDestinationColor);
            m_Teleporter.BadDestinationColor = EditorGUILayout.ColorField(new GUIContent("Not Allowed Destination", "The color that the teleport stream will have when the destination is not valid"), m_Teleporter.BadDestinationColor);
            m_Teleporter.LineWidth = EditorGUILayout.FloatField(new GUIContent("Line width", "With of the line in metters"), m_Teleporter.LineWidth);
            m_Teleporter.LineMaterial = (Material)EditorGUILayout.ObjectField("Line Material", m_Teleporter.LineMaterial, typeof(Material), true);

            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("CONSTRAINTS");
            S_EditorUtils.DrawEditorHint("Conditions that the user will need to meet to teleport");
            m_Teleporter.MaxTeleportDistance = EditorGUILayout.FloatField(new GUIContent("Max Teleport Distance", "Ho far in metters the user can teleport"), m_Teleporter.MaxTeleportDistance);
            m_Teleporter.MaxSlope = EditorGUILayout.FloatField(new GUIContent("Max Slope", "Max difference in heigth where the user can teleport"), m_Teleporter.MaxSlope);
            
            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("HOTSPOTS");
            S_EditorUtils.DrawEditorHint("Use them to make the user teleports in precise places");
            DisplayHotspots();
            m_Teleporter.MinHotspotDistance = EditorGUILayout.FloatField(new GUIContent("Minimum detection distance", "Distance to use a teleport hotspot"), m_Teleporter.MinHotspotDistance);

            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("ERROR PREVENTION");
            S_EditorUtils.DrawEditorHint("Use this parameters to avoid user input errors");
            m_Teleporter.HoldTimeToAppear = EditorGUILayout.FloatField(new GUIContent("Hold Duration To Activate", "Time that the user needs to be holding the teleport button in order to make it appear"), m_Teleporter.HoldTimeToAppear);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Input Sensitivity", "Tolerance in the joystic in order to avoid false inputs, where 0 is VERY sensitive"));
            m_Teleporter.InptuThreshold = EditorGUILayout.Slider(m_Teleporter.InptuThreshold, 0.0f, 1.0f);
            EditorGUILayout.EndHorizontal(); 
            m_Teleporter.Cooldown = EditorGUILayout.FloatField(new GUIContent("Cooldown", "Time that the user needs to wait between teleports"), m_Teleporter.Cooldown);
            */
        }
    }
}
