using System;
using System.IO;
using UnityEngine;

namespace SpatialStories
{
    public class SA_MicrophonePlay : S_AbstractAction
    {
        public S_InteractiveObject AudioSource;
        public string AudioName = "audio_recorded.dat";

        public SA_MicrophonePlay(){}

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
        }

        protected override void ActionLogic()
        {
            string filename = Application.persistentDataPath + "/" + AudioName;
            FileStream stream = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(stream);
            long numBytes = new FileInfo(filename).Length;
            byte[] buff = br.ReadBytes((int)numBytes);
            float[] floatAudioData = new float[buff.Length / 4];
            for (int k = 0; k < buff.Length; k += 4)
            {
                floatAudioData[(int)(k / 4)] = BitConverter.ToSingle(buff, k);
            }
            stream.Close();
            SoundsController.Instance.PlayRecordedSound(floatAudioData);
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            AudioSource = SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[0].ToString()).GetComponentInChildren<S_InteractiveObject>();
            AudioName = creationData[1].ToString();
        }
    }

    public static partial class APIExtensions
    {
        public static SA_MicrophonePlay CreateMicrophonePlay(this S_InteractionDefinition _def, string _IOAudioSource, string _filename)
        {
            return _def.CreateAction<SA_MicrophonePlay>(_IOAudioSource, _filename);
        }
    }
}
