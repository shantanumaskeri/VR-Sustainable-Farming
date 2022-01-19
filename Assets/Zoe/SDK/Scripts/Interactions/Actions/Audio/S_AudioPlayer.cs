using System.Collections;
using UnityEngine;

namespace SpatialStories
{
    [RequireComponent(typeof(AudioSource))]
    public class S_AudioPlayer : MonoBehaviour
    {
        public const string EVENT_AUDIOPLAYER_AUDIO_STARTED     = "EVENT_AUDIOPLAYER_AUDIO_STARTED";
        public const string EVENT_AUDIOPLAYER_AUDIO_STOPPED     = "EVENT_AUDIOPLAYER_AUDIO_STOPPED";
        public const string EVENT_AUDIOPLAYER_AUDIO_FINISHED    = "EVENT_AUDIOPLAYER_AUDIO_FINISHED";
        public const string EVENT_AUDIOPLAYER_AUDIO_PAUSED      = "EVENT_AUDIOPLAYER_AUDIO_PAUSED";
        public const string EVENT_AUDIOPLAYER_AUDIO_MUTED       = "EVENT_AUDIOPLAYER_AUDIO_MUTED";

        public const string EVENT_AUDIOPLAYER_STOP_ALL_SOUNDS   = "EVENT_AUDIOPLAYER_STOP_ALL_SOUNDS";
        public const string EVENT_AUDIOPLAYER_PAUSE_ALL_SOUNDS  = "EVENT_AUDIOPLAYER_PAUSE_ALL_SOUNDS";
        public const string EVENT_AUDIOPLAYER_RESUME_ALL_SOUNDS = "EVENT_AUDIOPLAYER_RESUME_ALL_SOUNDS";
        public const string EVENT_AUDIOPLAYER_MUTE_ALL_SOUNDS   = "EVENT_AUDIOPLAYER_MUTE_ALL_SOUNDS";
        public const string EVENT_AUDIOPLAYER_UNMUTE_ALL_SOUNDS = "EVENT_AUDIOPLAYER_UNMUTE_ALL_SOUNDS";
        
        private AudioSource[] m_audioSources;
        private AudioSource m_currentAudioSource;
        private AudioClip m_currentAudioClip;

        private float m_volumeMin;
        private float m_volumeMax;

        private SA_Audio.LOOP_MODES m_loopAudio;
        private SA_Audio.AUDIO_SEQUENCE m_sequence;
        private bool m_fadeInBetween = false;
        private bool m_ducking = true;

        private float m_fadeOutTime = 0f;
        private float m_fadeInTime = 0f;
        private float m_fadeSpeed = 0f;
        private bool m_fadeInEnabled = false;
        private bool m_fadeOutEnabled = false;

        private AnimationCurve m_fadeInCurve;
        private AnimationCurve m_fadeOutCurve;

        private bool m_enablePitch = false;
        private float m_pitchValue = 2;

        private bool m_forceStop = false;
        private bool m_overlapAllow = false;
        private float m_overlapTime = 0;
        private bool m_overlapFade = false;

        private bool m_reportFinishedSound = true;

        private float m_timer = 0;
        private bool m_fadeOutPerformed = false;

        private bool m_isMutedSound = false;
        private float m_backUpVolume = 0;

        // -------------------------------------------
        // GETTERS/SETTERS
        // -------------------------------------------
        public bool FadeInBetween
        {
            get { return m_fadeInBetween; }
            set { m_fadeInBetween = value; }
        }
        public float VolumeMin
        {
            get { return m_volumeMin; }
            set { m_volumeMin = value; }
        }
        public float VolumeMax
        {
            get { return m_volumeMax; }
            set { m_volumeMax = value; }
        }
        public bool DuckingEnabled
        {
            get { return m_ducking; }
            set { m_ducking = value; }
        }
        public float FadeSpeed
        {
            get { return m_fadeSpeed; }
            set { m_fadeSpeed = value; }
        }
        public float FadeInTime
        {
            get { return m_fadeInTime; }
            set { m_fadeInTime = value; }
        }
        public float FadeOutTime
        {
            get { return m_fadeOutTime; }
            set { m_fadeOutTime = value; }
        }
        public bool FadeInEnabled
        {
            get { return m_fadeInEnabled; }
            set { m_fadeInEnabled = value; }
        }
        public bool FadeOutEnabled
        {
            get { return m_fadeOutEnabled; }
            set { m_fadeOutEnabled = value; }
        }
        public AnimationCurve FadeInCurve
        {
            get { return m_fadeInCurve; }
            set { m_fadeInCurve = value; }
        }
        public AnimationCurve FadeOutCurve
        {
            get { return m_fadeOutCurve; }
            set { m_fadeOutCurve = value; }
        }
        public bool OverlapAllow
        {
            get { return m_overlapAllow; }
            set { m_overlapAllow = value; }
        }
        public float OverlapTime
        {
            get { return m_overlapTime; }
            set { m_overlapTime = value; }
        }
        public bool OverlapFade
        {
            get { return m_overlapFade; }
            set { m_overlapFade = value; }
        }
        public bool EnablePitch
        {
            get { return m_enablePitch; }
            set { m_enablePitch = value; }
        }
        public float PitchValue
        {
            get { return m_pitchValue; }
            set { m_pitchValue = value; }
        }

        // -------------------------------------------
        /* 
		 * Start
		 */
        private void Start()
        {
            m_audioSources = GetComponentsInChildren<AudioSource>();

            BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);
        }

        // -------------------------------------------
        /* 
		 * OnDestroy
		 */
        private void OnDestroy()
        {
            BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
        }

        // -------------------------------------------
        /* 
		 * StopAllSounds
		 */
        public void StopAllSounds()
        {
            if (m_currentAudioSource != null) m_currentAudioSource.Stop();

            if (m_audioSources != null)
            {
                for (int i = 0; i < m_audioSources.Length; i++)
                {
                    if (m_audioSources[i] != null) m_audioSources[i].Stop();
                }
            }
        }

        // -------------------------------------------
        /* 
		 * IsAudioSource
		 */
        public bool IsAudioSource(AudioSource _audioSource)
        {
            if (m_currentAudioSource == _audioSource) return true;

            if (m_audioSources != null)
            {
                for (int i = 0; i < m_audioSources.Length; i++)
                {
                    if (m_audioSources[i] == _audioSource) return true;
                }
            }
            return false;
        }

        // -------------------------------------------
        /* 
		 * Play
		 */
        public void Play(AudioClip _sound, AudioSource _targetaudioSource = null)
        {
            if (_sound != null)
            {
                m_currentAudioClip = _sound;

                // SET AUDIO SOURCE
                if (_targetaudioSource != null)
                {
                    m_currentAudioSource = _targetaudioSource;
                }
                else
                {
                    if (m_audioSources.Length > 0)
                    {
                        m_currentAudioSource = m_audioSources[0];
                    }
                }
                m_reportFinishedSound = true;
                m_currentAudioSource.clip = m_currentAudioClip;
                m_currentAudioSource.volume = m_volumeMax;
                if (m_isMutedSound) m_currentAudioSource.volume = 0;
                if (EnablePitch) m_currentAudioSource.pitch = PitchValue;
                if (FadeInEnabled) StartCoroutine(FadeIn(m_currentAudioSource, FadeInTime));
                m_timer = 0;
                m_fadeOutPerformed = false;
                m_currentAudioSource.PlayOneShot(m_currentAudioClip);
            }
        }

        // -------------------------------------------
        /* 
		 * FadeOut
		 */
        public IEnumerator FadeOut(AudioSource _audioSource, float _fadeTime)
        {
            float startVolume = _audioSource.volume;
            while (_audioSource.volume > 0)
            {
                _audioSource.volume -= startVolume * Time.deltaTime / _fadeTime;
                yield return null;
            }
        }

        // -------------------------------------------
        /* 
		 * FadeIn
		 */
        public IEnumerator FadeIn(AudioSource _audioSource, float _fadeTime)
        {
            _audioSource.volume = 0f;
            while (_audioSource.volume < m_volumeMax)
            {
                _audioSource.volume += m_volumeMax *(Time.deltaTime / _fadeTime);
                yield return null;
            }
        }

        // -------------------------------------------
        /* 
		 * OnBasicSystemEvent
		 */
        private void OnBasicSystemEvent(string _nameEvent, object[] _list)
        {
            if (m_currentAudioSource == null) return;

            if (_nameEvent == EVENT_AUDIOPLAYER_STOP_ALL_SOUNDS)
            {
                if (IsAudioSource((AudioSource)_list[0]))
                {
                    m_reportFinishedSound = false;
                    StopAllSounds();
                }
            }
            if (_nameEvent == EVENT_AUDIOPLAYER_PAUSE_ALL_SOUNDS)
            {
                if (IsAudioSource((AudioSource)_list[0]))
                {
                    m_reportFinishedSound = false;
                    m_currentAudioSource.Pause();
                }
            }
            if (_nameEvent == EVENT_AUDIOPLAYER_RESUME_ALL_SOUNDS)
            {
                if (IsAudioSource((AudioSource)_list[0]))
                {
                    m_reportFinishedSound = true;
                    m_currentAudioSource.UnPause();
                }
            }
            if (_nameEvent == EVENT_AUDIOPLAYER_MUTE_ALL_SOUNDS)
            {
                if (IsAudioSource((AudioSource)_list[0]))
                {
                    m_isMutedSound = true;
                    m_backUpVolume = m_currentAudioSource.volume;
                    m_currentAudioSource.volume = 0;
                }
            }
            if (_nameEvent == EVENT_AUDIOPLAYER_UNMUTE_ALL_SOUNDS)
            {
                if (IsAudioSource((AudioSource)_list[0]))
                {
                    m_isMutedSound = false;
                    m_currentAudioSource.volume = m_backUpVolume;
                }
            }
        }

        // -------------------------------------------
        /* 
		 * Update
		 */
        private void Update()
        {
            if (m_currentAudioClip != null)
            {
                if (m_currentAudioSource != null)
                {
                    if (m_currentAudioSource.isPlaying)
                    {
                        m_timer += Time.deltaTime;
                        if (FadeOutEnabled)
                        {
                            if (!m_fadeOutPerformed)
                            {
                                if ((m_currentAudioClip.length - m_timer) < FadeOutTime)
                                {
                                    m_fadeOutPerformed = true;
                                    StartCoroutine(FadeOut(m_currentAudioSource, FadeOutTime));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_reportFinishedSound)
                        {
                            AudioClip audioFinishedReference = m_currentAudioClip;
                            m_currentAudioSource.volume = 1;
                            m_currentAudioClip = null;
                            m_reportFinishedSound = false;
                            BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_AUDIOPLAYER_AUDIO_FINISHED, m_currentAudioSource, audioFinishedReference);
                        }                        
                    }
                }
            }
        }
    }
}