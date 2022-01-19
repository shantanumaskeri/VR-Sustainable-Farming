using SpatialStories;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Behaviour class to make controllers vibrate for a specific time
/// at a specific frequency and intensity.
/// N.B. This behaviour will be improved on as it is now specific to
/// Oculus
/// </summary>
public class S_VibrateBehaviour : S_AbstractBehaviour
{
    private S_HandsEnum m_Controller;

    private float m_VibrationFrequency;

    private float m_VibrationIntensity;

    /// <summary>
    /// Full constructor for VibrateControllerBehaviour
    /// </summary>
    /// <param name="_actingObject">The gameobject on which this behaviour acts</param>
    /// <param name="_controller">The controller to vibrate</param>
    /// <param name="_vibrationFrequency">The vibration frequency (N.B. ranges from 0 to 1 as specified by Oculus doc)</param>
    /// <param name="_vibrationIntensity">The vibration intensity (N.B. ranges from 0 to 1 as specified by Oculus doc)</param>
    /// <param name="_vibrationDuration">The vibration duration in seconds (N.B. ranges from 0 to 2 as specified by Oculus doc)</param>
    public S_VibrateBehaviour(S_HandsEnum _controller, float _vibrationFrequency, float _vibrationIntensity, float _vibrationDuration) : base(null, _vibrationDuration)
    {
        m_VibrationFrequency = _vibrationFrequency;
        m_VibrationIntensity = _vibrationIntensity;
        m_Controller = _controller;
    }

    protected override void Init()
    {
        switch (m_Controller)
        {
            case S_HandsEnum.LEFT:
                XRRig.Instance.LeftController.SendHapticImpulse(0, m_VibrationIntensity, m_Duration);
                break;
            case S_HandsEnum.RIGHT:
                XRRig.Instance.RightController.SendHapticImpulse(0, m_VibrationIntensity, m_Duration);
                break;
            case S_HandsEnum.BOTH:
                XRRig.Instance.LeftController.SendHapticImpulse(0, m_VibrationIntensity, m_Duration);
                XRRig.Instance.RightController.SendHapticImpulse(0, m_VibrationIntensity, m_Duration);
                break;
        }
    }

    protected override void OnFinish()
    {
    }

    protected override void Update()
    {
    }
}
