using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(S_Interaction))]
    public class S_InteractionEditor : S_Editor
    {
        // Editor UI
        private SerializedProperty m_DelayProp;
        private SerializedProperty m_ReloadSubjectProp;
        private SerializedProperty m_ReloadLimitProp;
        private SerializedProperty m_ReloadMaxProp;
        private SerializedProperty m_DelayOptions;
        private SerializedProperty m_CustomCondition;
        private SerializedProperty m_CustomAction;
        private S_Interaction m_Interaction;

        private static GUIStyle m_NotActiveStyle;
        private static GUIStyle m_NotCompletedStyle;
        private static GUIStyle m_CompleteStyle;

        private string[] m_focusLossModes;

        private Texture2D m_iconInteraction;

        private readonly string[] m_repetitionModes = { "Check Conditions Only", "Check Conditions & dependencies" };

        private readonly string[] m_customConditionModes = { "ADD CONDITION", "Gaze", "Touch", "Collision", "Manipulate", "Point and Click", "Drop", "Inputs", "Approach", "Proximity" };
        private readonly string[] m_customActionModes = { "ADD ACTION", "Activate", "Rotate", "Scale", "Translate", "Animation", "Audio", "Physics", "Change Material", "Vibrate", "Rotate Around" };

        private const int CC_INDEX_GAZE = 1;
        private const int CC_INDEX_TOUCH = 2;
        private const int CC_INDEX_COLLISION = 3;
        private const int CC_INDEX_GRAB = 4;
        private const int CC_INDEX_POINT = 5;
        private const int CC_INDEX_DROP = 6;
        private const int CC_INDEX_INPUTS = 7;
        private const int CC_INDEX_APPROACH = 8;
        private const int CC_INDEX_PROXIMITY = 9;

        private const int CA_INDEX_ACTIVATE = 1;
        private const int CA_INDEX_ROTATE = 2;
        private const int CA_INDEX_SCALE = 3;
        private const int CA_INDEX_TRANSLATE = 4;
        private const int CA_INDEX_ANIMATION = 5;
        private const int CA_INDEX_AUDIO = 6;
        private const int CA_INDEX_GRAVITY = 7;
        private const int CA_INDEX_CHANGE_COLOR = 8;
        private const int CA_INDEX_VIBRATE = 9;
        private const int CA_INDEX_ROTATEAROUND = 10;
        
        private const int CA_INDEX_DESTROY = 11;
        private const int CA_INDEX_PLAY_RECORD = 12;
        private const int CA_INDEX_RECORD_SOUND = 13;

        private void OnEnable()
        {
            SetupStyles();
            m_Interaction = (S_Interaction)target;
            m_DelayProp = serializedObject.FindProperty("Delay");
            m_ReloadSubjectProp = serializedObject.FindProperty("ReloadSubject");
            m_ReloadLimitProp = serializedObject.FindProperty("ReloadLimit");
            m_ReloadMaxProp = serializedObject.FindProperty("ReloadMax");
            m_DelayOptions = serializedObject.FindProperty("DelayOptions");
            m_CustomCondition = serializedObject.FindProperty("CustomCondition");
            m_CustomAction = serializedObject.FindProperty("CustomAction");

            m_focusLossModes = Enum.GetNames(typeof(S_FocusLossMode));

            m_iconInteraction = (Texture2D)Resources.Load<Texture2D>("UI/Interaction/icon_interaction");
        }
        
        private void SetupStyles()
        {
            if (m_NotActiveStyle != null)
                return;
            
            m_NotActiveStyle = new GUIStyle();
            m_NotActiveStyle.fixedWidth = 250.0f;
            m_NotActiveStyle.alignment = TextAnchor.MiddleCenter;
            m_NotActiveStyle.fixedHeight = 50f;
            m_NotActiveStyle.normal.textColor = Color.white;
            m_NotActiveStyle.fontStyle = FontStyle.Bold;
            m_NotActiveStyle.normal.background = S_EditorUtils.MakeTex(250, 50, new Color(0.7f, 0.7f, 0.7f));
            m_NotActiveStyle.margin = new RectOffset(1, 1, 3, 3);
            m_NotCompletedStyle = new GUIStyle(m_NotActiveStyle);
            m_NotCompletedStyle.normal.background = S_EditorUtils.MakeTex(250, 50, new Color(0.91f, 0.17f, 0.22f));
            m_NotCompletedStyle.border = new RectOffset(1, 1, 1, 1);
            
            m_CompleteStyle = new GUIStyle(m_NotCompletedStyle);
            m_CompleteStyle.normal.background = S_EditorUtils.MakeTex(250, 50, new Color(0.56f, 0.93f, 0.56f));
        }

        public override void OnEditorUI()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawSectionTitle("Interaction Setup");
            S_EditorUtils.DrawEditorHint("Here you can set when the interaction will happen");

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // DEPENDENCIES
            m_Interaction.InteractionDependencies = ShowDependencies(m_Interaction.InteractionDependencies, "Dependencies", "Create chain reactions by setting other interactions this one is dependent on.");
            if (m_Interaction.InteractionDependencies != null)
            {
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.PropertyField(m_DelayProp, new GUIContent("Delay[s]:"));

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // ESTEBAN COMMENT: Please don't remove because it could be restored
            // ShowDuration();

            // REPEAT
            ShowRepeatOptions("Repeat", "Number of times you want this interaction to repeat");
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // DEACTIVATE
            m_Interaction.DeactivationDependencies = ShowDependencies(m_Interaction.DeactivationDependencies, "Deactivate", "Deactivate this interaction if the following interactions are triggered:");
            if (m_Interaction.DeactivationDependencies != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_DelayOptions, new GUIContent("On Deactivate"));
            }

            if (m_Interaction.DeactivationDependencies != null)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                m_Interaction.ActivationDependencies = ShowDependencies(m_Interaction.ActivationDependencies, "Reactivate", "Reactivate this interaction if the following interactions are triggered:");
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            S_EditorUtils.DrawEditorHint("Now define your interaction by adding conditions and actions:");
            EditorGUILayout.Space();
            AddCustomComponents();
            EditorGUILayout.Space();

            PopulateComponents();
        }

        private void InitializeIfNull()
        {
            if (m_Interaction.IndexCustomCondition == -1)
            {
                switch (m_Interaction.CustomCondition)
                {
                    case S_CustomCondition.Gaze: m_Interaction.IndexCustomCondition = CC_INDEX_GAZE; break;
                    case S_CustomCondition.Touch: m_Interaction.IndexCustomCondition = CC_INDEX_TOUCH; break;
                    case S_CustomCondition.Collision: m_Interaction.IndexCustomCondition = CC_INDEX_COLLISION; break;
                    case S_CustomCondition.Manipulate: m_Interaction.IndexCustomCondition = CC_INDEX_GRAB; break;
                    case S_CustomCondition.Point: m_Interaction.IndexCustomCondition = CC_INDEX_POINT; break;
                    case S_CustomCondition.Drop: m_Interaction.IndexCustomCondition = CC_INDEX_DROP; break;
                    case S_CustomCondition.ActionButton: m_Interaction.IndexCustomCondition = CC_INDEX_INPUTS; break;
                    case S_CustomCondition.Approach: m_Interaction.IndexCustomCondition = CC_INDEX_APPROACH; break;
                    case S_CustomCondition.Proximity: m_Interaction.IndexCustomCondition = CC_INDEX_PROXIMITY; break;

                    default: m_Interaction.IndexCustomCondition = 0; break;
                }
            }
            if (m_Interaction.IndexCustomAction == -1)
            {
                switch (m_Interaction.CustomAction)
                {
                    case S_CustomAction.Activate: m_Interaction.IndexCustomAction = CA_INDEX_ACTIVATE; break;
                    case S_CustomAction.Rotate: m_Interaction.IndexCustomAction = CA_INDEX_ROTATE; break;
                    case S_CustomAction.Scale: m_Interaction.IndexCustomAction = CA_INDEX_SCALE; break;
                    case S_CustomAction.Translate: m_Interaction.IndexCustomAction = CA_INDEX_TRANSLATE; break;
                    case S_CustomAction.Animation: m_Interaction.IndexCustomAction = CA_INDEX_ANIMATION; break;
                    case S_CustomAction.Audio: m_Interaction.IndexCustomAction = CA_INDEX_AUDIO; break;
                    case S_CustomAction.Physics: m_Interaction.IndexCustomAction = CA_INDEX_GRAVITY; break;
                    case S_CustomAction.Change_Color: m_Interaction.IndexCustomAction = CA_INDEX_CHANGE_COLOR; break;
                    case S_CustomAction.Vibrate: m_Interaction.IndexCustomAction = CA_INDEX_VIBRATE; break;
                    case S_CustomAction.RotateAround: m_Interaction.IndexCustomAction = CA_INDEX_ROTATEAROUND; break;

                    case S_CustomAction.Destroy: m_Interaction.IndexCustomAction = CA_INDEX_DESTROY; break;
                    case S_CustomAction.Microphone_Play: m_Interaction.IndexCustomAction = CA_INDEX_PLAY_RECORD; break;
                    case S_CustomAction.Microphone_Record: m_Interaction.IndexCustomAction = CA_INDEX_RECORD_SOUND; break;

                    default: m_Interaction.IndexCustomAction = 0; break;
                }
            }
        }

        private void TranslateIndexToCustom()
        {
            switch (m_Interaction.IndexCustomCondition)
            {
                case CC_INDEX_GAZE: m_Interaction.CustomCondition = S_CustomCondition.Gaze; break;
                case CC_INDEX_TOUCH: m_Interaction.CustomCondition = S_CustomCondition.Touch; break;
                case CC_INDEX_COLLISION: m_Interaction.CustomCondition = S_CustomCondition.Collision; break;
                case CC_INDEX_GRAB: m_Interaction.CustomCondition = S_CustomCondition.Manipulate; break;
                case CC_INDEX_POINT: m_Interaction.CustomCondition = S_CustomCondition.Point; break;
                case CC_INDEX_DROP: m_Interaction.CustomCondition = S_CustomCondition.Drop; break;
                case CC_INDEX_INPUTS: m_Interaction.CustomCondition = S_CustomCondition.ActionButton; break;
                case CC_INDEX_APPROACH: m_Interaction.CustomCondition = S_CustomCondition.Approach; break;
                case CC_INDEX_PROXIMITY: m_Interaction.CustomCondition = S_CustomCondition.Proximity; break;
            }

            switch (m_Interaction.IndexCustomAction)
            {
                case CA_INDEX_ACTIVATE: m_Interaction.CustomAction = S_CustomAction.Activate; break;
                case CA_INDEX_ROTATE: m_Interaction.CustomAction = S_CustomAction.Rotate; break;
                case CA_INDEX_SCALE: m_Interaction.CustomAction = S_CustomAction.Scale; break;
                case CA_INDEX_TRANSLATE: m_Interaction.CustomAction = S_CustomAction.Translate; break;
                case CA_INDEX_ANIMATION: m_Interaction.CustomAction = S_CustomAction.Animation; break;
                case CA_INDEX_AUDIO: m_Interaction.CustomAction = S_CustomAction.Audio; break;
                case CA_INDEX_GRAVITY: m_Interaction.CustomAction = S_CustomAction.Physics; break;
                case CA_INDEX_CHANGE_COLOR: m_Interaction.CustomAction = S_CustomAction.Change_Color; break;
                case CA_INDEX_VIBRATE: m_Interaction.CustomAction = S_CustomAction.Vibrate; break;
                case CA_INDEX_ROTATEAROUND: m_Interaction.CustomAction = S_CustomAction.RotateAround; break;

                case CA_INDEX_DESTROY: m_Interaction.CustomAction = S_CustomAction.Destroy; break;
                case CA_INDEX_PLAY_RECORD: m_Interaction.CustomAction = S_CustomAction.Microphone_Play; break;
                case CA_INDEX_RECORD_SOUND: m_Interaction.CustomAction = S_CustomAction.Microphone_Record; break;
            }
        }

        private void AddCustomComponents()
        {
            InitializeIfNull();

            GUILayout.BeginHorizontal();

            m_Interaction.IndexCustomCondition = EditorGUILayout.Popup("", m_Interaction.IndexCustomCondition, m_customConditionModes);
            m_Interaction.IndexCustomAction = EditorGUILayout.Popup("", m_Interaction.IndexCustomAction, m_customActionModes);

            TranslateIndexToCustom();

            bool allowDuplicates = true;

            if (m_Interaction.CustomCondition != 0)
            {
                switch (m_Interaction.CustomCondition)
                {
                    case S_CustomCondition.Gaze:
                        if ((m_Interaction.gameObject.GetComponent<SC_Gaze>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SC_Gaze>();
                        break;
                    case S_CustomCondition.Touch:
                        if ((m_Interaction.gameObject.GetComponent<SC_Touch>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SC_Touch>();
                        break;
                    case S_CustomCondition.Collision:
                        if ((m_Interaction.gameObject.GetComponent<SC_Collision>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SC_Collision>();
                        break;
                    case S_CustomCondition.Manipulate:
                        if ((m_Interaction.gameObject.GetComponent<SC_Manipulate>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SC_Manipulate>();
                        break;
                    case S_CustomCondition.Point:
                        if ((m_Interaction.gameObject.GetComponent<SC_PointAndClick>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SC_PointAndClick>();
                        break;
                    case S_CustomCondition.Drop:
                        if ((m_Interaction.gameObject.GetComponent<SC_Drop>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SC_Drop>();
                        break;
                    case S_CustomCondition.ActionButton:
                        if ((m_Interaction.gameObject.GetComponent<SC_Input>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SC_Input>();
                        break;
                    case S_CustomCondition.Approach:
                        if ((m_Interaction.gameObject.GetComponent<SC_Approach>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SC_Approach>();
                        break;
                    case S_CustomCondition.Proximity:
                        if ((m_Interaction.gameObject.GetComponent<SC_Proximity>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SC_Proximity>();
                        break;
                }
                m_Interaction.CustomCondition = 0;
                m_Interaction.IndexCustomCondition = 0;
            }
            if (m_Interaction.CustomAction != 0)
            {
                switch (m_Interaction.CustomAction)
                {
                    case S_CustomAction.Activate:
                        if ((m_Interaction.gameObject.GetComponent<SA_ActivateGameObjects>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_ActivateGameObjects>();
                        break;
                    case S_CustomAction.Rotate:
                        if ((m_Interaction.gameObject.GetComponent<SA_RotateBehaviour>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_RotateBehaviour>();
                        break;
                    case S_CustomAction.Scale:
                        if ((m_Interaction.gameObject.GetComponent<SA_ScaleBehaviour>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_ScaleBehaviour>();
                        break;
                    case S_CustomAction.Translate:
                        if ((m_Interaction.gameObject.GetComponent<SA_TranslateBehaviour>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_TranslateBehaviour>();
                        break;
                    case S_CustomAction.Animation:
                        if ((m_Interaction.gameObject.GetComponent<SA_Animation>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_Animation>();
                        break;
                    case S_CustomAction.Audio:
                        if ((m_Interaction.gameObject.GetComponent<SA_Audio>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_Audio>();
                        break;
                    case S_CustomAction.Destroy:
                        if ((m_Interaction.gameObject.GetComponent<SA_DestroyBehaviour>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_DestroyBehaviour>();
                        break;
                    case S_CustomAction.Physics:
                        if ((m_Interaction.gameObject.GetComponent<SA_Physics>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_Physics>();
                        break;
                    case S_CustomAction.Change_Color:
                        if ((m_Interaction.gameObject.GetComponent<SA_ChangeColorBehaviour>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_ChangeColorBehaviour>();
                        break;
                    case S_CustomAction.Microphone_Play:
                        if ((m_Interaction.gameObject.GetComponent<SA_MicrophonePlay>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_MicrophonePlay>();
                        break;
                    case S_CustomAction.Microphone_Record:
                        if ((m_Interaction.gameObject.GetComponent<SA_MicrophoneRecord>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_MicrophoneRecord>();
                        break;
                    case S_CustomAction.Vibrate:
                        if ((m_Interaction.gameObject.GetComponent<SA_VibrateBehaviour>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_VibrateBehaviour>();
                        break;
                    case S_CustomAction.RotateAround:
                        if ((m_Interaction.gameObject.GetComponent<SA_VibrateBehaviour>() == null) || allowDuplicates) m_Interaction.gameObject.AddComponent<SA_RotateAroundBehaviour>();
                        break;
                }
                m_Interaction.CustomAction = 0;
                m_Interaction.IndexCustomAction = 0;
            }

            GUILayout.EndHorizontal();
        }

        private SC_InteractionDependencies ShowDependencies(SC_InteractionDependencies _deps, string _title, string _description = "")
        {
            bool hasDepenencies = DependencyIsNotNull(_deps);
            hasDepenencies = EditorGUILayout.ToggleLeft(" " + _title, hasDepenencies);
            if (_description.Length > 0) S_EditorUtils.DrawEditorHint(_description);
            EditorGUI.indentLevel++;
            if (_deps != null && SC_InteractionDependenciesEditor.Editors.ContainsKey(_deps.GetInstanceID()))
            {
                if (hasDepenencies)
                {
                    if (SC_InteractionDependenciesEditor.Editors[_deps.GetInstanceID()].CountDependencies() == 0)
                    {
                        SC_InteractionDependenciesEditor.Editors[_deps.GetInstanceID()].AddEmptyDependency();
                    }                    
                }
                SC_InteractionDependenciesEditor.Editors[_deps.GetInstanceID()].ShowInspector();
                if (!hasDepenencies)
                {
                    SC_InteractionDependenciesEditor.Editors.Remove(_deps.GetInstanceID());
                    DestroyImmediate(_deps);
                }
            }
            else
            {
                if (hasDepenencies)
                {
                    _deps = m_Interaction.gameObject.AddComponent<SC_InteractionDependencies>();
                }
            }
            EditorGUI.indentLevel--;
            return _deps;
        }

        private bool DependencyIsNotNull(SC_InteractionDependencies _deps)
        {
            return _deps != null;
        }

        private void ShowReloadOptions()
        {
            EditorGUILayout.PropertyField(m_ReloadSubjectProp, new GUIContent("ReloadSubject:"));
            if (m_ReloadSubjectProp.enumValueIndex != (int)S_ReloadSubject.NONE)
                S_EditorUtils.ShowIndentedBlock(ShowReloadBlock, 1);
        }

        private void ShowRepeatOptions(string _title, string _description = "")
        {
            m_Interaction.EnableRepeat = EditorGUILayout.ToggleLeft(" " + _title, m_Interaction.EnableRepeat);
            if (_description.Length > 0) S_EditorUtils.DrawEditorHint(_description);
            if (!m_Interaction.EnableRepeat)
            {
                m_Interaction.IsInfiniteRepeat = false;
                m_Interaction.ReloadLimit = S_ReloadLimit.FINITE;
                m_Interaction.ReloadMax = 0;
                m_Interaction.ReloadSubject = S_ReloadSubject.NONE;
            }
            else
            { 
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                if (m_Interaction.IsInfiniteRepeat)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        m_Interaction.ReloadMax = EditorGUILayout.IntField("Time x", m_Interaction.ReloadMax);
                    }
                }
                else
                {
                    m_Interaction.ReloadMax = EditorGUILayout.IntField("Time x", m_Interaction.ReloadMax);
                }
                m_Interaction.IsInfiniteRepeat = EditorGUILayout.ToggleLeft(" Infinite", m_Interaction.IsInfiniteRepeat);
                EditorGUILayout.EndHorizontal();

                if (m_Interaction.IsInfiniteRepeat || (m_Interaction.ReloadMax>0))
                {
                    if (m_Interaction.IndexRepeatOptions == -1)
                    {
                        switch (m_Interaction.ReloadSubject)
                        {
                            case S_ReloadSubject.CONDITIONS:
                                m_Interaction.IndexRepeatOptions = 0;
                                break;

                            case S_ReloadSubject.CONDITIONS_AND_DEPENDENCIES:
                                m_Interaction.IndexRepeatOptions = 1;
                                break;
                        }
                    }

                    m_Interaction.IndexRepeatOptions = EditorGUILayout.Popup(" On Repeat", m_Interaction.IndexRepeatOptions, m_repetitionModes, GUILayout.MaxWidth(350));
                    switch (m_Interaction.IndexRepeatOptions)
                    {
                        case 0:
                            m_Interaction.ReloadSubject = S_ReloadSubject.CONDITIONS;
                            break;

                        default:
                            m_Interaction.ReloadSubject = S_ReloadSubject.CONDITIONS_AND_DEPENDENCIES;
                            break;
                    }
                }
                else
                {
                    m_Interaction.ReloadSubject = S_ReloadSubject.NONE;
                }

                if (m_Interaction.ReloadSubject != S_ReloadSubject.NONE)
                {
                    m_Interaction.DelayReload = EditorGUILayout.FloatField("Repeat Delay [s]", m_Interaction.DelayReload, GUILayout.MaxWidth(200));
                }

                if (m_Interaction.IsInfiniteRepeat)
                {
                    m_Interaction.ReloadLimit = S_ReloadLimit.INFINITE;
                }
                else
                {
                    m_Interaction.ReloadLimit = S_ReloadLimit.FINITE;
                }

                EditorGUI.indentLevel--;
            }
        }        

        // -------------------------------------------
        /* 
		 * ShowDuration
		 */
        private void ShowDuration()
        {
            m_Interaction.IsDuration = EditorGUILayout.ToggleLeft("Enable Duration", m_Interaction.IsDuration);

            // DURATION
            if (m_Interaction.IsDuration)
            {
                EditorGUILayout.Space();
                S_EditorUtils.DrawSectionTitle("Duration");
                S_EditorUtils.DrawEditorHint("How much time interaction remain validated.");
                S_EditorUtils.DrawEditorHint("(Usefull when you need to perform an action for a period of time)");
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();
                m_Interaction.FocusDuration = EditorGUILayout.FloatField("Duration [s]", m_Interaction.FocusDuration);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                m_Interaction.FocusLossModeIndex = EditorGUILayout.Popup("Loss Mode", m_Interaction.FocusLossModeIndex, m_focusLossModes);
                if (m_Interaction.FocusLossModeIndex.Equals((int)S_FocusLossMode.DECREASE_PROGRESSIVELY))
                {
                    m_Interaction.FocusLossSpeed = EditorGUILayout.FloatField("Speed Factor", m_Interaction.FocusLossSpeed);
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
        }

        private void ShowReloadBlock()
        {
            EditorGUILayout.PropertyField(m_ReloadLimitProp, new GUIContent("ReloadLimit:"));
            m_Interaction.DelayReload = EditorGUILayout.FloatField("Delay Reload [s]", m_Interaction.DelayReload);
            if (m_Interaction.ReloadLimit == S_ReloadLimit.FINITE)
            {
                EditorGUILayout.PropertyField(m_ReloadMaxProp, new GUIContent("ReloadMax:"));
            }
        }

        public override void OnRuntimeUI()
        {
            RuntimeDependencies();
            RuntimeConditions();
            RuntimeDelay();
            RuntimeValidation();
            RuntimeReload();
            RuntimeDebugTools();
            DisplayMetadataBlock();
        }

        private void RuntimeDependencies()
        {
            GUIStyle style = m_NotActiveStyle;
            if (DependencyIsNotNull(m_Interaction.InteractionDependencies))
            {
                style = m_Interaction.InteractionDependencies.AreDependenciesValid ? m_CompleteStyle : m_NotCompletedStyle;
            }
            ShowStatusRectangle("Dependencies", style);
        }

        private void RuntimeConditions()
        {
            GUIStyle style = m_NotActiveStyle;
            if (m_Interaction.HasConditions())
            {
                style = m_Interaction.InteractionStatus.AreConditionsValid ? m_CompleteStyle : m_Interaction.Delay > 0 && m_Interaction.InteractionStatus.HasDelayStarted ? m_CompleteStyle : m_NotCompletedStyle;
            }
            ShowStatusRectangle("Conditions", style);
        }

        private void RuntimeDelay()
        {
            GUIStyle style = m_NotActiveStyle;
            string delayText = "Delay";
            if (m_Interaction.Delay > 0)
            {
                style = m_Interaction.InteractionStatus.IsDelayCompleted ? m_CompleteStyle : m_NotCompletedStyle;
                delayText = string.Format("Delay {0} [s]", m_Interaction.Delay);
            }
            ShowStatusRectangle(delayText, style);
        }

        private void RuntimeValidation()
        {
            GUIStyle style = m_NotActiveStyle = m_Interaction.InteractionStatus.Validated ? m_CompleteStyle : m_NotCompletedStyle;
            ShowStatusRectangle("Validated", style);
        }
        
        private void RuntimeReload()
        {
            string reloadSubject = string.Empty;
            switch (m_Interaction.ReloadSubject)
            {
                case S_ReloadSubject.NONE:
                    return;
                case S_ReloadSubject.CONDITIONS:
                    reloadSubject = "Conditions";
                    break;
                case S_ReloadSubject.CONDITIONS_AND_DEPENDENCIES:
                    reloadSubject = "Conditions & Dependencies";
                    break;
            }

            S_EditorUtils.DrawSectionTitle("Reload Information: ");
            EditorGUILayout.LabelField(string.Concat("Reload Subject: ", reloadSubject));
            EditorGUILayout.LabelField(string.Concat("Times Reloaded: ", m_Interaction.ReloadCount));
            m_Interaction.DelayReload = EditorGUILayout.FloatField("Delay Reload [s]", m_Interaction.DelayReload);
            if (m_Interaction.ReloadLimit == S_ReloadLimit.FINITE)
            {
                EditorGUILayout.LabelField(string.Concat("Reload Limit: ", m_Interaction.ReloadLimit));
            }
        }

        private void RuntimeDebugTools()
        {
            S_EditorUtils.DrawSectionTitle("Debug Information: ");
            EditorGUILayout.LabelField(string.Concat("Times validated: ", m_Interaction.InteractionStatus.ValidationsCounter));
            if (GUILayout.Button("Force Validation"))
            {
                m_Interaction.ForceValidation();
            }
        }

        private void ShowStatusRectangle(string _name, GUIStyle _style)
        {
            GUILayout.Box(_name, _style);
        }

        bool showMetadata = false;
        private void DisplayMetadataBlock()
        {
            showMetadata = EditorGUILayout.Foldout(showMetadata, "Metadata:");
            if (showMetadata)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Creation Time:", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
                EditorGUILayout.LabelField(m_Interaction.Metadata.CreationTime);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Unique ID:", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
                EditorGUILayout.LabelField(m_Interaction.Metadata.GUID);
                EditorGUILayout.EndHorizontal();
            }
        }

        private bool m_reorderedComponents = false;

        private void PopulateComponents()
        {
            var targetGo = (serializedObject.targetObject as Component).gameObject;
            var comps = targetGo.GetComponents<Component>();
            List<Component> actions = new List<Component>();
            List<Component> conditions = new List<Component>();
            Component interaction = null;
            if (comps.Length > 0)
            {
                if (!m_reorderedComponents)
                {
                    m_reorderedComponents = true;
                    foreach (var comp in comps)
                    {
                        if (typeof(S_AbstractAction).IsAssignableFrom(comp.GetType()))
                        {
                            actions.Add(comp);
                        }
                        if (typeof(S_AbstractCondition).IsAssignableFrom(comp.GetType()))
                        {
                            conditions.Add(comp);
                        }
                        if (typeof(S_Interaction).IsAssignableFrom(comp.GetType()))
                        {
                            interaction = comp;
                        }
                    }

                    if (interaction != null)
                    {
                        UnityEditorInternal.ComponentUtility.MoveComponentUp(interaction);
                    }
                    for (int i = 0; i < conditions.Count; i++)
                    {
                        for (int j = 0; j < 10; j++) UnityEditorInternal.ComponentUtility.MoveComponentDown(conditions[i]);
                    }
                    for (int i = 0; i < actions.Count; i++)
                    {
                        for (int j = 0; j < 10; j++) UnityEditorInternal.ComponentUtility.MoveComponentDown(actions[i]);
                    }
  
                }
            }
        }
    }
}
