using SpatialStories;
using UnityEngine;

namespace SpatialStories
{
    /// <summary>
    /// Custom action that allows to make the controllers vibrate
    /// N.B. This behaviour will be improved on as it is now specific to
    /// Oculus
    /// </summary>
        [AddComponentMenu("Zoe/SA_Vibrate")]
    public class SA_VibrateBehaviour : S_AbstractAction
    {
        public S_HandsEnum Controller;

        [Range(0.0f, 1.0f)]
        public float VibrationFrequency;

        [Range(0.0f, 1.0f)]
        public float VibrationIntensity;

        [Range(0.0f, 2.0f)]
        public float VibrationDuration;

        private S_VibrateBehaviour m_vibrateControllerBehaviour;

        private void Awake()
        {
            m_vibrateControllerBehaviour = new S_VibrateBehaviour(Controller, VibrationFrequency, VibrationIntensity, VibrationDuration);
        }

        protected override void ActionLogic()
        {
            m_vibrateControllerBehaviour.Reset();
            m_vibrateControllerBehaviour.Start();
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            Controller = (S_HandsEnum)creationData[0];
            VibrationFrequency = (float)creationData[1];
            VibrationIntensity = (float)creationData[2];
            VibrationDuration = (float)creationData[3];
        }
    }

    public static partial class APIExtensions
    {
        public static SA_VibrateBehaviour CreateVibrationBehaviour(this S_InteractionDefinition _def, S_HandsEnum _hands, float _vibrationFrequency, float _vibrationIntensity, float _vibrationDuration)
        {
            return _def.CreateAction<SA_VibrateBehaviour>(_hands, _vibrationFrequency, _vibrationIntensity, _vibrationDuration);
        }
    }
}
