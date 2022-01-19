// <copyright file="Gaze_InteractiveObjectEditor.cs" company="apelab sàrl">
// © apelab. All Rights Reserved.
//
// This source is subject to the apelab license.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2014-06-01</date>
using SpatialStories;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(S_InteractiveObject))]
    public class S_InteractiveObjectEditor : S_Editor
    {
        #region Members
        private S_InteractiveObject m_InteractiveObjectScript;

        public int DndTargetsToGenerate;
        public List<string> Dnd_DropTargetsNames { get { return m_DndDropTargetsNames; } private set { } }

        // logo image
        private Texture m_Logo;
        private Rect m_LogoRect;

        // This is used so that we go back to the previously selected
        // manipulation mode when we enable/disable the manipulation
        // Not necessary, but a nice addition
        private S_ManipulationModes m_CachedManipulationMode = S_ManipulationModes.GRAB;

        private List<string> m_DndDropTargetsNames;
        private Material m_DndTargetMaterial;
        private List<GameObject> m_SceneInteractiveObjects = new List<GameObject>();

        private readonly string[] m_ManipulationModes = { "GRAB", "POINT AND CLICK", "LEVITATE" };

        #endregion

        void OnEnable()
        {
            InitMembers();
        }

        private void InitMembers()
        {
            m_InteractiveObjectScript = (S_InteractiveObject)target;

            m_Logo = Resources.Load<Texture>("UI/Zoe_Logo_256");
            m_LogoRect = new Rect();
            m_LogoRect.x = 4;
            m_LogoRect.y = 4;
            m_LogoRect.width = 200;
            m_DndDropTargetsNames = new List<string>();
            m_DndTargetMaterial = Resources.Load<Material>("Materials/DnD_TargetMaterial");
        }

        public override void OnEditorUI()
        {
            UpdateDropTargetsNames();
            EditorGUILayout.Space();
            DisplayLogo();
            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("Interactive Object Setup");
            EditorGUILayout.Space();
            DisplayAffectedByGravity();
            DisplayManipulationMode();
            DisplayDragAndDrop();
        }

        public override void OnRuntimeUI()
        {
            UpdateDropTargetsNames();
            EditorGUILayout.Space();
            DisplayLogo();
            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("Interactive Object Setup");
            EditorGUILayout.Space();
            DisplayAffectedByGravity();
            DisplayManipulationMode();
            DisplayDragAndDrop();
        }

        // call the following function in OnInspectorUI or OnRuntimeUI
        // to display meta data for the interactive object
        bool showMetadata = false;
        private void DisplayMetadataBlock()
        {
            showMetadata = EditorGUILayout.Foldout(showMetadata, "Metadata:");
            if (showMetadata)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Creation Time:", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
                EditorGUILayout.LabelField(m_InteractiveObjectScript.Metadata.CreationTime);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Unique ID:", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
                EditorGUILayout.LabelField(m_InteractiveObjectScript.Metadata.GUID);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
        }

        private void UpdateDropTargetsNames()
        {
            m_DndDropTargetsNames.Clear();
            m_SceneInteractiveObjects.Clear();

            // rebuild them
            S_InteractiveObject[] interactiveObjects = FindObjectsOfType(typeof(S_InteractiveObject)) as S_InteractiveObject[];
            foreach (S_InteractiveObject interactiveObject in interactiveObjects)
            {
                m_SceneInteractiveObjects.Add(interactiveObject.gameObject);
                m_DndDropTargetsNames.Add(interactiveObject.gameObject.name);
            }
        }

        private void DisplayLogo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(m_Logo);
            GUILayout.EndHorizontal();
            GUILayout.Label(S_Version.VERSION, EditorStyles.boldLabel);
        }

        private void DisplayAffectedByGravity()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                m_InteractiveObjectScript.AffectedByGravity = EditorGUILayout.ToggleLeft("Affected By Gravity", m_InteractiveObjectScript.AffectedByGravity, GUILayout.MaxWidth(300));
                EditorGUILayout.Space();

                m_InteractiveObjectScript.gameObject.GetComponent<Rigidbody>().isKinematic = !m_InteractiveObjectScript.AffectedByGravity;
                m_InteractiveObjectScript.gameObject.GetComponent<Rigidbody>().useGravity = m_InteractiveObjectScript.AffectedByGravity;
            }
        }

        private void DisplayManipulationMode()
        {
            if ((m_InteractiveObjectScript.GetComponent<S_Head>() != null) ||
                (m_InteractiveObjectScript.GetComponent<S_RightHandRoot>() != null) ||
                (m_InteractiveObjectScript.GetComponent<S_LeftHandRoot>() != null))
            {
                return;
            }

            bool enableManipulation = EditorGUILayout.ToggleLeft("Enable manipulation", m_InteractiveObjectScript.EnableManipulation, GUILayout.MaxWidth(300));

            if (!enableManipulation)
            {
                if (m_InteractiveObjectScript.EnableManipulation)
                {
                    m_CachedManipulationMode = m_InteractiveObjectScript.ManipulationMode;
                }

                m_InteractiveObjectScript.ManipulationMode = S_ManipulationModes.NONE;

                return;
            }

            if (m_InteractiveObjectScript.ManipulationMode == S_ManipulationModes.NONE)
            {
                m_InteractiveObjectScript.ManipulationMode = m_CachedManipulationMode;
            }

            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            // 1º- DISPLAY THE MIN DISTANCE
            switch (m_InteractiveObjectScript.ManipulationMode)
            {
                case S_ManipulationModes.GRAB:
                    m_InteractiveObjectScript.GrabDistance = EditorGUILayout.FloatField("Min Distance [m]", m_InteractiveObjectScript.GrabDistance, GUILayout.MaxWidth(300));
                    S_Utils.EnsureFieldIsPositiveOrZero(ref m_InteractiveObjectScript.GrabDistance);
                    break;

                case S_ManipulationModes.POINT_AND_CLICK:
                    GUILayout.BeginHorizontal();
                    m_InteractiveObjectScript.TouchDistance = EditorGUILayout.FloatField(new GUIContent("Min Distance [m]"), m_InteractiveObjectScript.TouchDistance, GUILayout.MaxWidth(300));
                    S_Utils.EnsureFieldIsAboveLimit(ref m_InteractiveObjectScript.TouchDistance, 1);
                    GUILayout.EndHorizontal();
                    break;

                case S_ManipulationModes.LEVITATE:
                    m_InteractiveObjectScript.GrabDistance = EditorGUILayout.FloatField("Min Distance [m]", m_InteractiveObjectScript.GrabDistance, GUILayout.MaxWidth(300));
                    break;
            }

            // 2º- DISPLAY THE POPUP OPTIONS
            m_InteractiveObjectScript.ManipulationMode = (S_ManipulationModes)EditorGUILayout.Popup("", (int)m_InteractiveObjectScript.ManipulationMode, m_ManipulationModes, GUILayout.MaxWidth(200));

            // 3º- DISPLAY AFTER POPUP OPTIONS
            switch (m_InteractiveObjectScript.ManipulationMode)
            {
                case S_ManipulationModes.GRAB:
                    m_InteractiveObjectScript.SnapOnGrab = EditorGUILayout.ToggleLeft(new GUIContent("Snap", "Make the IO snap into a specific position. Uses the Snap Left or Snap Right positions"), m_InteractiveObjectScript.SnapOnGrab);
                    if (m_InteractiveObjectScript.SnapOnGrab)
                    {
                        m_InteractiveObjectScript.IsManipulable = false;
                    }
                    else
                    {
                        m_InteractiveObjectScript.IsManipulable = true;
                    }
                    GUILayout.BeginHorizontal();
                    m_InteractiveObjectScript.AttractionSpeed = EditorGUILayout.FloatField("Attraction Speed", m_InteractiveObjectScript.AttractionSpeed, GUILayout.MaxWidth(300));
                    S_Utils.EnsureFieldIsPositiveOrZero(ref m_InteractiveObjectScript.AttractionSpeed);
                    GUILayout.EndHorizontal();
                    break;

                case S_ManipulationModes.POINT_AND_CLICK:
                    break;

                case S_ManipulationModes.LEVITATE:
                    m_InteractiveObjectScript.SnapOnGrab = EditorGUILayout.ToggleLeft(new GUIContent("Snap", "Make the IO snap into a specific rotation. Uses the Snap Left or Snap Right rotations"), m_InteractiveObjectScript.SnapOnGrab);
                    S_Utils.EnsureFieldIsPositiveOrZero(ref m_InteractiveObjectScript.GrabDistance);
                    break;
            }

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }

        private void DisplayDragAndDrop()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            m_InteractiveObjectScript.IsDragAndDropEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Enable Drag And Drop", "Enable Drag and Drop if you want your IO to be snapped in a particular target."), m_InteractiveObjectScript.IsDragAndDropEnabled);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (m_InteractiveObjectScript.IsDragAndDropEnabled)
            {
                DisplayTargets();
                DisplayTargetGenerator();

                EditorGUILayout.Space();
                S_EditorUtils.DrawSectionTitle("Drop Requirements");
                EditorGUILayout.Space();

                DisplayAxisConstraints();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_InteractiveObjectScript.DnDMinDistance = EditorGUILayout.FloatField("Min Distance for Drop [m]", m_InteractiveObjectScript.DnDMinDistance);
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();
                S_EditorUtils.DrawSectionTitle("Drop Parameters");
                 EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                m_InteractiveObjectScript.DnDDropOnAlreadyOccupiedTargets = EditorGUILayout.ToggleLeft(new GUIContent("Allow drop on occupied Targets", "Can this object be dropped into a target that is already occupied ?"), m_InteractiveObjectScript.DnDDropOnAlreadyOccupiedTargets);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                m_InteractiveObjectScript.DnDAttached = EditorGUILayout.ToggleLeft(new GUIContent("Locked after Drop", "The IO can not be taken off its target after being released"), m_InteractiveObjectScript.DnDAttached);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                m_InteractiveObjectScript.DnDSnapOnDrop = EditorGUILayout.ToggleLeft(new GUIContent("Snap On Drop", "The IO will snap in place when released"), m_InteractiveObjectScript.DnDSnapOnDrop);
                EditorGUILayout.EndHorizontal();

                // We decided to hide the snap before drop because it was giving weird results when
                // matching with axes
                /*if (m_InteractiveObjectScript.DnDSnapOnDrop)
                {
                    EditorGUILayout.BeginHorizontal();
                    m_InteractiveObjectScript.DnDSnapBeforeDrop = EditorGUILayout.ToggleLeft(new GUIContent("Snap Before Drop", "The IO will snap in place if already in the right position (before releasing it)"), m_InteractiveObjectScript.DnDSnapBeforeDrop);
                    EditorGUILayout.EndHorizontal();
                }
                else
                    m_InteractiveObjectScript.DnDSnapBeforeDrop = false;*/
                m_InteractiveObjectScript.DnDSnapBeforeDrop = false;

                EditorGUILayout.BeginHorizontal();
                m_InteractiveObjectScript.DnDTimeToSnap = EditorGUILayout.FloatField(new GUIContent("Time To Snap", "The time it takes for the object to snap"), m_InteractiveObjectScript.DnDTimeToSnap);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DisplayTargetGenerator()
        {
            EditorGUILayout.BeginHorizontal();
            DndTargetsToGenerate = EditorGUILayout.IntField(new GUIContent("Automatic Targets", "Generates a pre-configured target with a blue transparent visual."), DndTargetsToGenerate);
            if (GUILayout.Button("GENERATE"))
            {
                GenerateDnDTargets();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DisplayAxisConstraints()
        {
            GUILayout.BeginHorizontal();
            m_InteractiveObjectScript.DnDRespectXAxis = EditorGUILayout.ToggleLeft("Match X Axis Orientation", m_InteractiveObjectScript.DnDRespectXAxis);
            if (m_InteractiveObjectScript.DnDRespectXAxis)
            {
                m_InteractiveObjectScript.DnDRespectXAxisMirrored = EditorGUILayout.ToggleLeft("Allow mirror", m_InteractiveObjectScript.DnDRespectXAxisMirrored);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            m_InteractiveObjectScript.DnDRespectYAxis = EditorGUILayout.ToggleLeft("Match Y Axis Orientation", m_InteractiveObjectScript.DnDRespectYAxis);
            if (m_InteractiveObjectScript.DnDRespectYAxis)
            {
                m_InteractiveObjectScript.DnDRespectYAxisMirrored = EditorGUILayout.ToggleLeft("Allow mirror", m_InteractiveObjectScript.DnDRespectYAxisMirrored);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            m_InteractiveObjectScript.DnDRespectZAxis = EditorGUILayout.ToggleLeft("Match Z Axis Orientation", m_InteractiveObjectScript.DnDRespectZAxis);
            if (m_InteractiveObjectScript.DnDRespectZAxis)
            {
                m_InteractiveObjectScript.DnDRespectZAxisMirrored = EditorGUILayout.ToggleLeft("Allow mirror", m_InteractiveObjectScript.DnDRespectZAxisMirrored);
            }
            GUILayout.EndHorizontal();

            if (m_InteractiveObjectScript.DnDRespectXAxis || m_InteractiveObjectScript.DnDRespectYAxis || m_InteractiveObjectScript.DnDRespectZAxis)
            {
                m_InteractiveObjectScript.DnDAngleThreshold = EditorGUILayout.Slider("Angle Threshold", m_InteractiveObjectScript.DnDAngleThreshold, 1, 100);
            }

        }

        private void DisplayTargets()
        {
			EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Drop Targets", " Use existing IO’s in the scene or create an automatic target below for your IO."));
			EditorGUILayout.EndHorizontal();

            // help message if no target is specified
            if (m_InteractiveObjectScript.DnD_Targets == null || m_InteractiveObjectScript.DnD_Targets.Count < 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Add at least one drop target to make the behaviour work.", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                // for each DnD target
                for (int i = 0; i < m_InteractiveObjectScript.DnD_Targets.Count; i++)
                {
                    if (m_InteractiveObjectScript.DnD_Targets[i] == null)
                    {
                        m_InteractiveObjectScript.DnD_Targets.RemoveAt(i);
                    }
                    else
                    {
                        // refresh DnD_Targets IOs list modification (an IO target may has been destroyed)
                        if (m_SceneInteractiveObjects.Contains(m_InteractiveObjectScript.DnD_Targets[i]))
                        {
                            // display it in a popup
                            EditorGUILayout.BeginHorizontal();

                            m_InteractiveObjectScript.DnD_Targets[i] = m_SceneInteractiveObjects[EditorGUILayout.Popup(m_SceneInteractiveObjects.IndexOf(m_InteractiveObjectScript.DnD_Targets[i]), m_DndDropTargetsNames.ToArray())];

                            // and a '-' button to remove it if needed
                            if (GUILayout.Button("-"))
                                m_InteractiveObjectScript.DnD_Targets.Remove(m_InteractiveObjectScript.DnD_Targets[i]);

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }

            // display 'add' button
            if (GUILayout.Button("+"))
            {
                // exit if there are no Interactive Object in the scene
                if (m_SceneInteractiveObjects.Count < 1)
                    return;

                // add the first Interactive Object by default
                // TODO @mike add only if doesn't exist already !
                m_InteractiveObjectScript.DnD_Targets.Add(m_SceneInteractiveObjects[0]);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    //targetConditions.InputsMap.Delete(d);
                    m_InteractiveObjectScript.DnD_Targets.Remove(m_SceneInteractiveObjects[0]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        private void GenerateDnDTargets()
        {
            GameObject instance;
            S_InteractiveObject gaze_InteractiveObject;
            for (int i = 0; i < DndTargetsToGenerate; i++)
            {
                // instantiate a target
                instance = Instantiate(m_InteractiveObjectScript.gameObject);

                // add suffix to its name
                instance.name = m_InteractiveObjectScript.gameObject.name + " (DnD Target " + i + ")";

                // get the visuals of Drop object
                S_InteractiveObjectVisuals visualsRoot = instance.GetComponentInChildren<S_InteractiveObjectVisuals>();
                Renderer[] visualsChildren = visualsRoot.gameObject.GetComponentsInChildren<Renderer>();

                // for every visual
                for (int k = 0; k < visualsChildren.Length; k++)
                {
                    // assign ghost material for each generated target
                    visualsChildren[k].material = m_DndTargetMaterial;

                    // set all colliders in this visual gameobject to isTrigger
                    Collider[] collider = visualsChildren[k].gameObject.GetComponents<Collider>();
                    if (collider != null && collider.Length > 0)
                    {
                        for (int l = 0; l < collider.Length; l++)
                        {
                            collider[l].isTrigger = true;
                        }
                    }
                }

                // get the InteractiveObject script
                gaze_InteractiveObject = instance.GetComponent<S_InteractiveObject>();

                //gaze_InteractiveObject.DnD_Targets.Clear();

                // change manipulation mode to NONE
                gaze_InteractiveObject.ManipulationMode = S_ManipulationModes.NONE;

                // deactivate DnD
                gaze_InteractiveObject.IsDragAndDropEnabled = false;

                // change gravity to none and kinematic to true
                instance.GetComponent<Rigidbody>().useGravity = false;
                instance.GetComponent<Rigidbody>().isKinematic = true;

                // add the generated targets in the list of targets for this drop object
                m_InteractiveObjectScript.DnD_Targets.Add(instance);
            }
        }
    }
}