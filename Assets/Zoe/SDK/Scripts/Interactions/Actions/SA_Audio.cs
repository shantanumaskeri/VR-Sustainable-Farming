using Gaze;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SA_Audio")]
    public class SA_Audio : S_AbstractAction
    {
        public enum AUDIO_SEQUENCE { InOrder, Random }
        public enum LOOP_MODES { None, Whole_List, Loop_The_Last_Clip }
        public enum PLAYLIST_MODES { In_Order, Random }
        public enum ACTIVABLE_OPTION { NOTHING, ACTIVATE, DEACTIVATE, MUTE, PAUSE }
        public ACTIVABLE_OPTION ActionAudio = ACTIVABLE_OPTION.NOTHING;

        private int m_AudioPlayListKey;

        // Audio
        private S_AudioPlayer _gazeAudioPlayer = null;
        private S_AudioPlayer m_GazeAudioPlayer
        {
            get
            {
                if(_gazeAudioPlayer == null)
                {
                    if (TargetAudioSource == null)
                    {
                        TargetAudioSource = GetComponentInParent<S_InteractiveObject>().GetComponentInChildren<AudioSource>();
                    }

                    if (TargetAudioSource.GetComponent<S_AudioPlayer>() == null)
                    {
                        TargetAudioSource.gameObject.AddComponent<S_AudioPlayer>();
                    }

                    _gazeAudioPlayer = TargetAudioSource.GetComponent<S_AudioPlayer>();
                }
                return _gazeAudioPlayer;
            }
        }

        public S_InteractiveObject Target;
        public AudioSource TargetAudioSource;
        public bool[] ActiveTriggerStatesAudio = new bool[Enum<TriggerEventsAndStates>.Count];


        [SerializeField]
        public List<AudioClip> AudioClips = new List<AudioClip>();
        private int m_currentIndexAudio = 0;

        public LOOP_MODES LoopAudio;
        public bool LoopSingleAudio = false;
        public bool FadeInBetween;        
        public LOOP_MODES LoopAudioNew;
        public bool AudioLoopOnLast;

        public bool ShowParameters = false;
        public bool DuckingEnabled = false;
        public float FadeInTime = 1f;
        public float FadeOutTime = 1f;
        public float FadeOutDeactTime = 1f;
        public AnimationCurve FadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public AnimationCurve FadeOutDeactCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        public AnimationCurve FadeOutCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        public bool FadeInEnabled = false;
        public bool FadeOutEnabled = false;
        public bool FadeOutDeactEnabled = false;
        public float AudioVolumeMin = .2f;
        public float AudioVolumeMax = 1f;
        public float FadeSpeed = .005f;
        public bool OverlapAllow = true;
        public float OverlapTime = 0;
        public bool OverlapFade = false;
        public bool EnablePitch = false;
        public float AudioPitchValue = 2f;
        
        public SA_Audio() {}
        private bool m_Started = false;

        public PLAYLIST_MODES PlaylistModes;
        public float SimoultaneousRandomDelay = 0;

        public bool LogoPlayValue = false;
        public bool LogoPauseValue = false;
        public bool LogoStopValue = false;
        public bool LogoMuteValue = false;

        private List<int> m_indexesAudioConsume = new List<int>();
        private bool m_audioPaused = false;
        private bool m_audioMuted = false;

        // -------------------------------------------
        /* 
		 * Start
		 */
        private void Start()
        {
            if (m_Started)
                return;

            ActiveTriggerStatesAudio[0] = true;
            if (TargetAudioSource == null)
            {
                TargetAudioSource = GetComponentInParent<S_InteractiveObject>().GetComponentInChildren<AudioSource>();
            }
            m_Started = true;
        }

        // -------------------------------------------
        /* 
		 * OnEnable
		 */
        private void OnEnable()
        {
            BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);
        }

        // -------------------------------------------
        /* 
		 * OnDisable
		 */
        private void OnDisable()
        {
            BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
        }

        // -------------------------------------------
        /* 
		 * DontHaveAudioLoop
		 */
        public bool DontHaveAudioLoop()
        {
            return true;
        }

        // -------------------------------------------
        /* 
		 * UpdateSettings
		 */
        private void UpdatePlayerSettings()
        {
            if (m_GazeAudioPlayer == null) return;

            m_GazeAudioPlayer.VolumeMin = AudioVolumeMin;
            m_GazeAudioPlayer.VolumeMax = AudioVolumeMax;
            m_GazeAudioPlayer.DuckingEnabled = DuckingEnabled;
            m_GazeAudioPlayer.FadeSpeed = FadeSpeed;
            m_GazeAudioPlayer.FadeInTime = FadeInTime;
            m_GazeAudioPlayer.FadeOutTime = FadeOutTime;
            m_GazeAudioPlayer.FadeInEnabled = FadeInEnabled;
            m_GazeAudioPlayer.FadeOutEnabled = FadeOutEnabled;
            m_GazeAudioPlayer.FadeInCurve = FadeInCurve;
            m_GazeAudioPlayer.FadeOutCurve = FadeOutCurve;
            m_GazeAudioPlayer.OverlapAllow = OverlapAllow;
            m_GazeAudioPlayer.OverlapTime = OverlapTime;
            m_GazeAudioPlayer.OverlapFade = OverlapFade;
            m_GazeAudioPlayer.EnablePitch = EnablePitch;
            m_GazeAudioPlayer.PitchValue = AudioPitchValue;
        }

        // -------------------------------------------
        /* 
         * InitializeListAudioClipsToPlay
         */
        private void InitializeListAudioClipsToPlay()
        {
            m_currentIndexAudio = 0;
            m_indexesAudioConsume = Enumerable.Range(0, AudioClips.Count).ToList();
            switch (PlaylistModes)
            {
                case PLAYLIST_MODES.In_Order:
                    break;

                case PLAYLIST_MODES.Random:
                    m_indexesAudioConsume = S_Utils.Shuffle(m_indexesAudioConsume);
                    break;

                /*case PLAYLIST_MODES.Simultaneously:
                    break;*/

                default:
                    break;
            }
        }

        // -------------------------------------------
        /* 
		 * PlayCurrentIndexAudioClip
		 */
        private void PlayCurrentIndexAudioClip()
        {
            /*if (PlaylistModes == PLAYLIST_MODES.Simultaneously)
            {
                m_indexesAudioConsume.Clear();
                UpdatePlayerSettings();
                for (int i = 0; i < AudioClips.Count; i++)
                {
                    m_GazeAudioPlayer.Play(AudioClips[i]);
                }                
            }
            else*/
            {
                if (m_indexesAudioConsume.Count > 0)
                {
                    m_currentIndexAudio = m_indexesAudioConsume[0];
                    m_indexesAudioConsume.RemoveAt(0);
                    if (m_currentIndexAudio < AudioClips.Count)
                    {
                        m_GazeAudioPlayer.StopAllSounds();
                        UpdatePlayerSettings();
                        m_GazeAudioPlayer.Play(AudioClips[m_currentIndexAudio], TargetAudioSource);
                    }
                }
            }
        }

        // -------------------------------------------
        /* 
		 * ActionLogic
		 */
        protected override void ActionLogic()
        {
            if (ActionAudio == ACTIVABLE_OPTION.NOTHING)
                return;

            // If we want to activate audio just do it.
            switch (ActionAudio)
            {
                case ACTIVABLE_OPTION.ACTIVATE:
                    if (m_audioPaused)
                    {
                        m_audioPaused = false;
                        BasicSystemEventController.Instance.DispatchBasicSystemEvent(S_AudioPlayer.EVENT_AUDIOPLAYER_RESUME_ALL_SOUNDS, TargetAudioSource);
                    }
                    else
                    {
                        if (m_audioMuted)
                        {
                            m_audioMuted = false;
                            BasicSystemEventController.Instance.DispatchBasicSystemEvent(S_AudioPlayer.EVENT_AUDIOPLAYER_UNMUTE_ALL_SOUNDS, TargetAudioSource);
                        }
                        else
                        {
                            if (AudioClips.Count > 0)
                            {
                                InitializeListAudioClipsToPlay();
                                PlayCurrentIndexAudioClip();
                            }
                        }
                    }
                    break;

                case ACTIVABLE_OPTION.DEACTIVATE:
                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(S_AudioPlayer.EVENT_AUDIOPLAYER_STOP_ALL_SOUNDS, TargetAudioSource);
                    break;

                case ACTIVABLE_OPTION.PAUSE:
                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(S_AudioPlayer.EVENT_AUDIOPLAYER_PAUSE_ALL_SOUNDS, TargetAudioSource);
                    break;

                case ACTIVABLE_OPTION.MUTE:
                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(S_AudioPlayer.EVENT_AUDIOPLAYER_MUTE_ALL_SOUNDS, TargetAudioSource);
                    break;
            }
        }

        // -------------------------------------------
        /* 
		 * IsAudioClip
		 */
        private bool IsAudioClip(AudioClip _audioClip)
        {
            for (int i = 0; i < AudioClips.Count; i++)
            {
                if (AudioClips[i]== _audioClip)
                {
                    return true;
                }
            }

            return false;
        }

        // -------------------------------------------
        /* 
		 * OnBasicSystemEvent
		 */
        private void OnBasicSystemEvent(string _nameEvent, object[] _list)
        {
            if (_nameEvent == S_AudioPlayer.EVENT_AUDIOPLAYER_STOP_ALL_SOUNDS)
            {
                m_indexesAudioConsume.Clear();
            }
            if (_nameEvent == S_AudioPlayer.EVENT_AUDIOPLAYER_PAUSE_ALL_SOUNDS)
            {
                if (TargetAudioSource == (AudioSource)_list[0])
                {
                    m_audioPaused = true;
                }                    
            }
            if (_nameEvent == S_AudioPlayer.EVENT_AUDIOPLAYER_MUTE_ALL_SOUNDS)
            {
                if (TargetAudioSource == (AudioSource)_list[0])
                {
                    m_audioMuted = true;
                }
            }
            if (_nameEvent == S_AudioPlayer.EVENT_AUDIOPLAYER_AUDIO_FINISHED)
            {
                AudioSource audioSourceFinished = (AudioSource)_list[0];
                if (TargetAudioSource == audioSourceFinished)
                {
                    AudioClip audioClipFinished = (AudioClip)_list[1];
                    if (IsAudioClip(audioClipFinished))
                    {
                        switch (ActionAudio)
                        {
                            case ACTIVABLE_OPTION.ACTIVATE:
                                if (m_indexesAudioConsume.Count == 0)
                                {
                                    switch (LoopAudio)
                                    {
                                        case LOOP_MODES.Whole_List:
                                            InitializeListAudioClipsToPlay();
                                            break;

                                        case LOOP_MODES.Loop_The_Last_Clip:
                                            m_indexesAudioConsume = new List<int>();
                                            m_indexesAudioConsume.Add(AudioClips.Count - 1);
                                            break;

                                        default:
                                            return;
                                    }
                                }

                                PlayCurrentIndexAudioClip();
                                break;

                            case ACTIVABLE_OPTION.DEACTIVATE:
                                m_GazeAudioPlayer.StopAllSounds();
                                break;

                            case ACTIVABLE_OPTION.PAUSE:
                                break;

                            case ACTIVABLE_OPTION.MUTE:
                                break;
                        }
                    }
                }
            }
        }

        // -------------------------------------------
        /* 
		 * SetupUsingApi
		 */
        public override void SetupUsingApi(GameObject _interaction)
        {
            S_InteractiveObject io = SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[0].ToString());
            ActionAudio = (SA_Audio.ACTIVABLE_OPTION)creationData[1];
            if (creationData[2] is List<AudioClip>)
            {
                List<AudioClip> clipsToPlay = (List<AudioClip>)creationData[2];
                for (int i = 0; i < clipsToPlay.Count; i++)
                {
                    AudioClips.Add(clipsToPlay[i]);
                }
            }
            else
            {
                AudioClip clipToPlay = (AudioClip)creationData[2];
                AudioClips.Add(clipToPlay);
            }
            TargetAudioSource = io.GetComponentInChildren<AudioSource>();
            OverlapAllow = true;
            AudioVolumeMin = (float)creationData[3];
            AudioVolumeMax = (float)creationData[4];
            LoopSingleAudio = (bool)creationData[5];
            if (LoopSingleAudio)
            {
                LoopAudio = LOOP_MODES.Whole_List;
            }
            FadeInEnabled = (bool)creationData[6];
            FadeInTime = (float)creationData[7];
            FadeOutEnabled = (bool)creationData[8];
            FadeOutTime = (float)creationData[9];
            EnablePitch = (bool)creationData[10];
            AudioPitchValue = (float)creationData[11];
        }
    }
    
    public static partial class APIExtensions
    {
        public static SA_Audio CreateAudioAction(this S_InteractionDefinition _def, 
                                                string _ioGUID, 
                                                SA_Audio.ACTIVABLE_OPTION _option, 
                                                AudioClip _clipToPlay,
                                                float _audioVolumeMin = 0,
                                                float _audioVolumeMax = 1,
                                                bool _loop = false,
                                                bool _fadeInEnabled = false,
                                                float _fadeInTime = 0,
                                                bool _fadeOutEnabled = false,
                                                float _fadeOutTime = 0,
                                                bool _enablePitch = false,
                                                float _audioPitchValue = 1)
        {
            return _def.CreateAction<SA_Audio>(_ioGUID, _option, _clipToPlay, _audioVolumeMin, _audioVolumeMax, _loop, _fadeInEnabled, _fadeInTime, _fadeOutEnabled, _fadeOutTime, _enablePitch, _audioPitchValue);
        }


        public static SA_Audio CreateAudioAction(this S_InteractionDefinition _def,
                                                string _ioGUID,
                                                SA_Audio.ACTIVABLE_OPTION _option,
                                                List<AudioClip> _clipToPlay,
                                                float _audioVolumeMin = 0,
                                                float _audioVolumeMax = 1,
                                                bool _loop = false,
                                                bool _fadeInEnabled = false,
                                                float _fadeInTime = 0,
                                                bool _fadeOutEnabled = false,
                                                float _fadeOutTime = 0,
                                                bool _enablePitch = false,
                                                float _audioPitchValue = 1)
        {
            return _def.CreateAction<SA_Audio>(_ioGUID, _option, _clipToPlay, _audioVolumeMin, _audioVolumeMax, _loop, _fadeInEnabled, _fadeInTime, _fadeOutEnabled, _fadeOutTime, _enablePitch, _audioPitchValue);
        }
    }
}