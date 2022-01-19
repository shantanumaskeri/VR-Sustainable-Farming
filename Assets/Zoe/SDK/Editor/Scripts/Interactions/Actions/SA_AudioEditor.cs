using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SA_Audio))]
    public class SA_AudioEditor : S_AbstractActionEditor
    {
        private List<AudioSource> m_HierarchyAudioSources;
        private List<string> m_HierarchyAudioSourceNames;

        private SA_Audio m_ActionsScript;
        
        private bool m_ShowOtherAudioOptions = false;
        private bool m_HasBeenActivate = false;

        private Texture2D m_logoPlayIdle;
        private Texture2D m_logoPlayPressed;

        private Texture2D m_logoPauseIdle;
        private Texture2D m_logoPausePressed;

        private Texture2D m_logoStopIdle;
        private Texture2D m_logoStopPressed;

        private Texture2D m_logoMuteIdle;
        private Texture2D m_logoMutePressed;

        public override void OnEnable()
        {
            base.OnEnable();

            m_ActionsScript = (SA_Audio)target;
            if (m_ActionsScript.Target == null)
            {
                m_ActionsScript.Target = m_ActionsScript.GetComponentInParent<S_InteractiveObject>();
            }

            m_HierarchyAudioSources = new List<AudioSource>();
            m_HierarchyAudioSourceNames = new List<string>();
            FindAudioSourcesInHierarchy();

            m_logoPlayIdle = (Texture2D)Resources.Load<Texture2D>("UI/Audio/SoundPlay_Idle");
            m_logoPlayPressed = (Texture2D)Resources.Load<Texture2D>("UI/Audio/SoundPlay_Pressed");

            m_logoPauseIdle = (Texture2D)Resources.Load<Texture2D>("UI/Audio/SoundPause_Idle");
            m_logoPausePressed = (Texture2D)Resources.Load<Texture2D>("UI/Audio/SoundPause_Pressed");

            m_logoStopIdle = (Texture2D)Resources.Load<Texture2D>("UI/Audio/SoundStop_Idle");
            m_logoStopPressed = (Texture2D)Resources.Load<Texture2D>("UI/Audio/SoundStop_Pressed");

            m_logoMuteIdle = (Texture2D)Resources.Load<Texture2D>("UI/Audio/SoundMute_Idle");
            m_logoMutePressed = (Texture2D)Resources.Load<Texture2D>("UI/Audio/SoundMute_Pressed");
        }

        public override void OnEditorUI()
        {
            FindAudioSourcesInHierarchy();
            ShowAudioOptions();
        }

        private void ResetAllButtons()
        {
            m_ActionsScript.LogoPlayValue = false;
            m_ActionsScript.LogoPauseValue = false;
            m_ActionsScript.LogoStopValue = false;
            m_ActionsScript.LogoMuteValue = false;
        }

        private void DisplayImageButtons(int _yIni)
        {
            S_EditorUtils.DrawSectionTitle("Action", FontStyle.Normal);

            int xIni = 10;
            int yIni = _yIni;
            int sizeButton = 40;
            string nameActionSound = "";

            // PLAY BUTTON
            if (GUI.Button(new Rect(xIni, yIni, sizeButton, sizeButton), (!m_ActionsScript.LogoPlayValue ? m_logoPlayIdle : m_logoPlayPressed)))
            {
                bool tmpPlayValue = m_ActionsScript.LogoPlayValue;
                ResetAllButtons();
                m_ActionsScript.LogoPlayValue = !tmpPlayValue;
                if (m_ActionsScript.LogoPlayValue)
                {
                    Debug.Log("PLAY IS PRESSED");
                }
                else
                {
                    Debug.Log("PLAY IS IDLE");
                }
            }
            xIni += sizeButton;
            // STOP BUTTON
            if (GUI.Button(new Rect(xIni, yIni, sizeButton, sizeButton), (!m_ActionsScript.LogoStopValue ? m_logoStopIdle : m_logoStopPressed)))
            {
                bool tmpStopValue = m_ActionsScript.LogoStopValue;
                ResetAllButtons();
                m_ActionsScript.LogoStopValue = !tmpStopValue;
                if (m_ActionsScript.LogoStopValue)
                {
                    Debug.Log("STOP IS PRESSED");
                }
                else
                {
                    Debug.Log("STOP IS IDLE");
                }
            }
            xIni += sizeButton;
            // PAUSE BUTTON
            if (GUI.Button(new Rect(xIni, yIni, sizeButton, sizeButton), (!m_ActionsScript.LogoPauseValue ? m_logoPauseIdle : m_logoPausePressed)))
            {
                bool tmpPauseValue = m_ActionsScript.LogoPauseValue;
                ResetAllButtons();
                m_ActionsScript.LogoPauseValue = !tmpPauseValue;
                if (m_ActionsScript.LogoPauseValue)
                {
                    Debug.Log("PAUSE IS PRESSED");
                }
                else
                {
                    Debug.Log("PAUSE IS IDLE");
                }
            }
            xIni += sizeButton;
            // MUTE BUTTON
            if (GUI.Button(new Rect(xIni, yIni, sizeButton, sizeButton), (!m_ActionsScript.LogoMuteValue ? m_logoMuteIdle : m_logoMutePressed)))
            {
                bool tmpMuteValue = m_ActionsScript.LogoMuteValue;
                ResetAllButtons();
                m_ActionsScript.LogoMuteValue = !tmpMuteValue;
                if (m_ActionsScript.LogoMuteValue)
                {
                    Debug.Log("MUTE IS PRESSED");
                }
                else
                {
                    Debug.Log("MUTE IS IDLE");
                }
            }
            xIni += sizeButton;

            // AUDIO SOURCE
            m_ActionsScript.ActionAudio = SA_Audio.ACTIVABLE_OPTION.NOTHING;
            nameActionSound = "";
            if (m_ActionsScript.LogoPlayValue)
            {
                m_ActionsScript.ActionAudio = SA_Audio.ACTIVABLE_OPTION.ACTIVATE;
                nameActionSound = "PLAY";
            }
            if (m_ActionsScript.LogoPauseValue)
            {
                m_ActionsScript.ActionAudio = SA_Audio.ACTIVABLE_OPTION.PAUSE;
                nameActionSound = "PAUSE";
            }
            if (m_ActionsScript.LogoStopValue)
            {
                m_ActionsScript.ActionAudio = SA_Audio.ACTIVABLE_OPTION.DEACTIVATE;
                nameActionSound = "STOP";
            }
            if (m_ActionsScript.LogoMuteValue)
            {
                m_ActionsScript.ActionAudio = SA_Audio.ACTIVABLE_OPTION.MUTE;
                nameActionSound = "MUTE";
            }

            GUI.Label(new Rect(xIni + 10, yIni + (sizeButton/3), 100, sizeButton), nameActionSound);
        }

        protected override void DisplayDelay()
        {
            CustomDisplayDelay();
        }

        public void ShowAudioOptions()
        {
            DisplayDelay();

            int yIni = 50;
            if (m_abstractAction.IsDelayed) yIni = 80;
            if (m_abstractAction.IsDelayed && m_abstractAction.Random) yIni = 120;

            if (m_ActionsScript.ActionAudio == SA_Audio.ACTIVABLE_OPTION.NOTHING)
            {
                m_ActionsScript.LogoPlayValue = true;
                m_ActionsScript.ActionAudio = SA_Audio.ACTIVABLE_OPTION.ACTIVATE;
            }

            DisplayImageButtons(yIni);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // TARGET
            EditorGUILayout.BeginHorizontal();
            S_EditorUtils.DrawSectionTitle("Audio Source", 100, FontStyle.Normal);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (m_HierarchyAudioSources.Count > 0)
            {
                m_ActionsScript.TargetAudioSource = m_HierarchyAudioSources[0];
            }
            m_ActionsScript.TargetAudioSource = (AudioSource)EditorGUILayout.ObjectField("", m_ActionsScript.TargetAudioSource, typeof(AudioSource), true, GUILayout.Width(300));
            if (m_ActionsScript.TargetAudioSource != null)
            {
                m_ActionsScript.Target = m_ActionsScript.TargetAudioSource.GetComponentInParent<S_InteractiveObject>();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (m_ActionsScript.ActionAudio != SA_Audio.ACTIVABLE_OPTION.NOTHING)
            {
                // AUDIO CLIPS
                if (m_ActionsScript.ActionAudio == SA_Audio.ACTIVABLE_OPTION.ACTIVATE)
                {
                    if (!m_HasBeenActivate)
                    {
                        m_ActionsScript.OverlapAllow = true;
                        m_HasBeenActivate = true;
                    }
    
                    DisplayAudioBlock();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    S_EditorUtils.DrawSectionTitle("Audio Parameters", FontStyle.Normal);
                    EditorGUILayout.Space();

                    DisplayAudioVolume();

                    DisplayAudioLoopSetUp();

                    DisplayPlaylistSetUp();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    S_EditorUtils.DrawSectionTitle("Effects", FontStyle.Normal);
                    EditorGUILayout.Space();

                    DisplayAudioFadeSetUp();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    /*
                    if (m_ActionsScript.AudioClips.Count > 1)
                    {
                        S_EditorUtils.DrawSectionTitle("Reload Options");
                        S_EditorUtils.DrawEditorHint("Audio parameters when the interaction is reloaded.");

                        DisplayAudioReloadSetUp();
                    }
                    */
                }
            }
        }

        private void DisplayAudioBlock()
        {
            S_EditorUtils.DrawSectionTitle("Audio Clip(s)", FontStyle.Normal);
            EditorGUILayout.Space();
            S_Utils.InitAudioClip(m_ActionsScript.AudioClips);
            S_Utils.DisplayAudioClipList(m_ActionsScript.AudioClips);
        }

        private void DisplayAudioLoopSetUp()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            if (m_ActionsScript.AudioClips.Count > 1)
            {
                m_ActionsScript.LoopAudio = (SA_Audio.LOOP_MODES)EditorGUILayout.EnumPopup("Loop", m_ActionsScript.LoopAudio);
            }
            else
            {
                m_ActionsScript.LoopSingleAudio = EditorGUILayout.ToggleLeft("Loop", m_ActionsScript.LoopSingleAudio);
                if (m_ActionsScript.LoopSingleAudio)
                {
                    m_ActionsScript.LoopAudio = SA_Audio.LOOP_MODES.Whole_List;
                }
                else
                {
                    m_ActionsScript.LoopAudio = SA_Audio.LOOP_MODES.None;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        private void DisplayPlaylistSetUp()
        {
            EditorGUI.indentLevel++;
            if (m_ActionsScript.AudioClips.Count > 1)
            {
                EditorGUILayout.BeginHorizontal();
                m_ActionsScript.PlaylistModes = (SA_Audio.PLAYLIST_MODES)EditorGUILayout.EnumPopup("Playlist", m_ActionsScript.PlaylistModes);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        private void DisplayAudioPitch()
        {
            m_ActionsScript.EnablePitch = EditorGUILayout.ToggleLeft(new GUIContent("Change pitch", "Change the pitch of this interaction only"), m_ActionsScript.EnablePitch);
            if (m_ActionsScript.EnablePitch)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Pitch", GUILayout.MaxWidth(100));
                m_ActionsScript.AudioPitchValue = EditorGUILayout.Slider(m_ActionsScript.AudioPitchValue, 0.5f, 3);
                S_Utils.EnsureFieldIsPositiveOrZero(ref m_ActionsScript.AudioPitchValue);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DisplayAudioFadeSetUp()
        {
            EditorGUI.indentLevel++;
            // FADE IN
            GUILayout.BeginHorizontal();
            m_ActionsScript.FadeInEnabled = EditorGUILayout.ToggleLeft("Fade In", m_ActionsScript.FadeInEnabled);
            if (m_ActionsScript.FadeInEnabled)
            {
                m_ActionsScript.FadeInTime = EditorGUILayout.FloatField("", m_ActionsScript.FadeInTime, GUILayout.Width(100));
                S_Utils.EnsureFieldIsPositiveOrZero(ref m_ActionsScript.FadeInTime);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();

                // DEVELOPER ESTEBAN: WE CAN RESTORE THE CURVE BEHAVIOUR FOR A CUSTOM FADE WHEN THERE IS ENOUGH TIME
                // m_ActionsScript.FadeInCurve = EditorGUILayout.CurveField(m_ActionsScript.FadeInCurve, Color.green, new Rect(0, 0, m_ActionsScript.FadeInTime, 1), GUILayout.Width(400));
            }
            GUILayout.EndHorizontal();
            if (m_ActionsScript.DontHaveAudioLoop())
            {
                GUILayout.BeginHorizontal();
                m_ActionsScript.FadeOutEnabled = EditorGUILayout.ToggleLeft("Fade Out (at the end)", m_ActionsScript.FadeOutEnabled);
                if (m_ActionsScript.FadeOutEnabled)
                {
                    m_ActionsScript.FadeOutTime = EditorGUILayout.FloatField("", m_ActionsScript.FadeOutTime, GUILayout.Width(100));
                    S_Utils.EnsureFieldIsPositiveOrZero(ref m_ActionsScript.FadeOutTime);
                    EditorGUILayout.LabelField("[s]");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();

                    // DEVELOPER ESTEBAN: WE CAN RESTORE THE CURVE BEHAVIOUR FOR A CUSTOM FADE WHEN THERE IS ENOUGH TIME
                    // m_ActionsScript.FadeOutCurve = EditorGUILayout.CurveField(m_ActionsScript.FadeOutCurve, Color.green, new Rect(0, 0, m_ActionsScript.FadeOutTime, 1), GUILayout.Width(400));
                }
                GUILayout.EndHorizontal();

                DisplayAudioPitch();
            }
            EditorGUI.indentLevel--;
        }

        private void DisplayAudioReloadSetUp()
        {
            m_ActionsScript.OverlapAllow = EditorGUILayout.ToggleLeft(new GUIContent("Cumulate audios", "Cumulates audios launched with this interaction when reloaded."), m_ActionsScript.OverlapAllow);

            if (m_ActionsScript.OverlapAllow)
            {
                float minTimeSound = 1000000;
                for (int i = 0; i < m_ActionsScript.AudioClips.Count; i++)
                {
                    if (m_ActionsScript.AudioClips[i] != null)
                    {
                        if (minTimeSound > m_ActionsScript.AudioClips[i].length)
                        {
                            minTimeSound = m_ActionsScript.AudioClips[i].length;
                        }
                    }
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Time Transition");
                m_ActionsScript.OverlapTime = EditorGUILayout.Slider(m_ActionsScript.OverlapTime, 0, (int)minTimeSound);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                m_ActionsScript.OverlapFade = EditorGUILayout.ToggleLeft("Fade out/Fade In", m_ActionsScript.OverlapFade);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void FindAudioSourcesInHierarchy()
        {
            if (m_ActionsScript.Target == null)
            {
                Debug.LogError("SA_Audio does not have set up any target IO");
            }
            else
            {
                AudioSource[] asrc = m_ActionsScript.Target.GetComponentInParent<S_InteractiveObject>().GetComponentsInChildren<AudioSource>();

                m_HierarchyAudioSources.Clear();
                m_HierarchyAudioSourceNames.Clear();

                foreach (AudioSource a in asrc)
                {
                    m_HierarchyAudioSources.Add(a);
                    m_HierarchyAudioSourceNames.Add(a.gameObject.name);
                }
            }
        }


        private void DisplayAudioDuckingBlock()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Volume Min.");
            m_ActionsScript.AudioVolumeMin = EditorGUILayout.Slider(m_ActionsScript.AudioVolumeMin, 0, 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Volume Max.");
            m_ActionsScript.AudioVolumeMax = EditorGUILayout.Slider(m_ActionsScript.AudioVolumeMax, 0, 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fader Speed");
            m_ActionsScript.FadeSpeed = EditorGUILayout.Slider(m_ActionsScript.FadeSpeed, 0, 1);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        private void DisplayAudioVolume()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Volume",GUILayout.MaxWidth(100));
            m_ActionsScript.AudioVolumeMax = EditorGUILayout.Slider(m_ActionsScript.AudioVolumeMax, 0, 1);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        public static string[] AudioTriggerEvensAndStatesHints =
        {
            "",
            "Choose the clips you want to launch when the interaction is reloaded",
            "",
            "Choose the clips you want to launch when the interaction in it's Active ",
            "Choose the clips you want to launch when the interaction in it's After State"
        };

        public override void OnRuntimeUI()
        {
            EditorGUILayout.LabelField("Runtime UI not implemented yet :(");
        }
    }
}
