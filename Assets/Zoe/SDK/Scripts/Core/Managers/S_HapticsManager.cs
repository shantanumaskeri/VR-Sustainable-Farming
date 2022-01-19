using UnityEngine;

namespace SpatialStories
{
    public static class S_HapticsManager
    {
        private static bool m_Initialized = false;

        private static GameObject m_HapticsManagerSingletonGameObject;
        private static S_VibrateController m_RightHandVibrateController;
        private static S_VibrateController m_LeftHandVibrateController;

        public static void StartUp()
        {
            if (m_Initialized)
            {
                return;
            }

            m_HapticsManagerSingletonGameObject = new GameObject("HapticsManager");
            Object.DontDestroyOnLoad(m_HapticsManagerSingletonGameObject);

            m_RightHandVibrateController = m_HapticsManagerSingletonGameObject.AddComponent<S_VibrateController>();
            m_LeftHandVibrateController = m_HapticsManagerSingletonGameObject.AddComponent<S_VibrateController>();

            m_Initialized = true;
        }

        public static void ShutDown()
        {
            if (!m_Initialized)
            {
                return;
            }

            Object.Destroy(m_HapticsManagerSingletonGameObject);

            m_Initialized = false;
        }

        public static void SetControllerVibration(S_HandsEnum _handToVibrate, float _vibrationFrequency, float _vibrationIntensity, float _vibrationTimeSec)
        {
            switch (_handToVibrate)
            {
                case S_HandsEnum.LEFT:
                    m_LeftHandVibrateController.SetControllerVibration(S_HandsEnum.LEFT, _vibrationFrequency, _vibrationIntensity, _vibrationTimeSec);
                    break;
                case S_HandsEnum.RIGHT:
                    m_RightHandVibrateController.SetControllerVibration(S_HandsEnum.RIGHT, _vibrationFrequency, _vibrationIntensity, _vibrationTimeSec);
                    break;
                case S_HandsEnum.BOTH:
                    m_LeftHandVibrateController.SetControllerVibration(S_HandsEnum.LEFT, _vibrationFrequency, _vibrationIntensity, _vibrationTimeSec);
                    m_RightHandVibrateController.SetControllerVibration(S_HandsEnum.RIGHT, _vibrationFrequency, _vibrationIntensity, _vibrationTimeSec);
                    break;
            }
        }
    }
}
