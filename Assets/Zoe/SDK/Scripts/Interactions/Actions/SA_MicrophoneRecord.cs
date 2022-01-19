using System;
using System.IO;
using UnityEngine;

namespace SpatialStories
{
    public class SA_MicrophoneRecord : S_AbstractAction
    {
        public const string EVENT_MICROPHONE_RECORD_START_RECORDING = "EVENT_MICROPHONE_RECORD_START_RECORDING";
        public const string EVENT_MICROPHONE_RECORD_STOP_RECORDING = "EVENT_MICROPHONE_RECORD_STOP_RECORDING";
        public const string EVENT_MICROPHONE_RECORD_PLAYBACK = "EVENT_MICROPHONE_RECORD_PLAYBACK";
        public const string EVENT_MICROPHONE_START_FRESH = "EVENT_MICROPHONE_START_FRESH";

        public S_InteractiveObject AudioSource;
        public string AudioName = "audio_recorded.dat";
        public int TotalTimeRecord = 4;
        public bool ActivatePlaybackEcho = true;

        private float m_timerAudio = -1;

        public SA_MicrophoneRecord(){}

        private void Start()
        {
            if (AudioSource == null)
            {
                AudioSource = this.gameObject.GetComponentInParent<S_InteractiveObject>();
            }
            if (AudioSource.GetComponentInChildren<AudioSource>() == null)
            {
                throw new Exception("There is no AudioSource attached to the target object");
            }
            SoundsController.Instance.Initialize(false, AudioSource.GetComponentInChildren<AudioSource>().transform);
            BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);
        }

        private void OnDestroy()
        {
            BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
        }

        private void OnBasicSystemEvent(string _nameEvent, object[] _list)
        {
            if ((_nameEvent == EVENT_MICROPHONE_RECORD_START_RECORDING) || 
                (_nameEvent == EVENT_MICROPHONE_RECORD_STOP_RECORDING) || 
                (_nameEvent == EVENT_MICROPHONE_RECORD_PLAYBACK))
            {
                Debug.Log((string)_list[1]);
            }
            if (_nameEvent == SoundsController.EVENT_SOUNDSCONTROLLER_AUDIO_DATA)
            {
                float[] floatData = (float[])_list[0];
                byte[] uncompressedData = new byte[floatData.Length * 4];
                Buffer.BlockCopy(floatData, 0, uncompressedData, 0, uncompressedData.Length);
                FileStream stream = new FileStream(Application.persistentDataPath + "/" + AudioName, FileMode.Create);
                stream.Write(uncompressedData, 0, uncompressedData.Length);
                stream.Close();
                if (ActivatePlaybackEcho)
                {
                    BasicSystemEventController.Instance.DelayBasicSystemEvent(EVENT_MICROPHONE_RECORD_PLAYBACK, 0.2f, this.gameObject, "Playing echo for recorded sound");
                    SoundsController.Instance.PlayRecordedSound((float[])_list[0]);
                    BasicSystemEventController.Instance.DelayBasicSystemEvent(EVENT_MICROPHONE_START_FRESH, TotalTimeRecord, this.gameObject, "Point and click to record");
                }
            }
        }

        protected override void ActionLogic()
        {
            BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_MICROPHONE_RECORD_START_RECORDING, this.gameObject, "Now recording microphone...");
            m_timerAudio = TotalTimeRecord;
            SoundsController.Instance.StartMicrophoneRecording(TotalTimeRecord);
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            AudioSource = SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[0].ToString()).GetComponentInChildren<S_InteractiveObject>();
            AudioName = creationData[1].ToString();
            TotalTimeRecord = (int)creationData[2];
            ActivatePlaybackEcho = (bool)creationData[3];
        }

        void Update()
        {
            if (m_timerAudio > 0)
            {
                m_timerAudio -= Time.deltaTime;
                if (m_timerAudio <= 0)
                {
                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_MICROPHONE_RECORD_STOP_RECORDING, this.gameObject, "Recording finished");
                    SoundsController.Instance.StopMicrophoneRecording();
                }
            }
        }
    }

    public static partial class APIExtensions
    {
        public static SA_MicrophoneRecord CreateRecordAudio(this S_InteractionDefinition _def, string _IOAudioSource, string _filename, int _totalTime, bool _playEcho)
        {
            return _def.CreateAction<SA_MicrophoneRecord>(_IOAudioSource, _filename, _totalTime, _playEcho);
        }
    }
}
