using UnityEngine;

public static class AudioClipExtensions
{
    public static AudioClip Trim(this AudioClip _audioClip, float _trimLengthSeconds)
    {
        if (_audioClip.length <= _trimLengthSeconds)
        {
            return _audioClip;
        }

        int samplePosition = Mathf.Min((int)(_trimLengthSeconds * _audioClip.samples / _audioClip.length), _audioClip.samples);

        float[] data = new float[samplePosition * _audioClip.channels];
        _audioClip.GetData(data, 0);

        AudioClip audioClip = AudioClip.Create(_audioClip.name,
            samplePosition,
            _audioClip.channels,
            _audioClip.frequency,
            false);

        audioClip.SetData(data, 0);

        return audioClip;
    }

    public static void Amplify(this AudioClip _audioClip, float _amplificationRatio)
    {
        if (_amplificationRatio <= 0.0f)
        {
            return;
        }

        float[] data = new float[_audioClip.samples * _audioClip.channels];
        _audioClip.GetData(data, 0);

        for (int i = 0; i < data.Length; i++)
        {
            data[i] *= _amplificationRatio;
        }

        _audioClip.SetData(data, 0);
    }
}
