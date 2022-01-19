using UnityEngine;
using UnityEngine.XR;

namespace SpatialStories
{
    public class S_VibrateController : MonoBehaviour
    {
        private float m_Intensity;

        private float m_Frequency;

        private float m_DurationSec;

        public void SetControllerVibration(S_HandsEnum _handToVibrate, float _vibrationFrequency, float _vibrationIntensity, float _vibrationTimeSec)
        {
            switch (_handToVibrate)
            {
                case S_HandsEnum.LEFT:
                    XRRig.Instance.LeftController.SendHapticImpulse(0, _vibrationIntensity, _vibrationTimeSec);
                    break;
                case S_HandsEnum.RIGHT:
                    XRRig.Instance.RightController.SendHapticImpulse(0, _vibrationIntensity, _vibrationTimeSec);
                    break;
                case S_HandsEnum.BOTH:
                    XRRig.Instance.LeftController.SendHapticImpulse(0, _vibrationIntensity, _vibrationTimeSec);
                    XRRig.Instance.RightController.SendHapticImpulse(0, _vibrationIntensity, _vibrationTimeSec);
                    break;
            }

            m_Intensity = _vibrationIntensity;
            m_Frequency = _vibrationFrequency;
            m_DurationSec = _vibrationTimeSec;
        }
    }
}
