using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpatialStories
{
    public class S_LoadNextScene : MonoBehaviour
    {
        public string[] EventsToEnableLoad;

        public bool EnableLoadOnActionUp = false;
        public string NextSceneToLoad;

        private List<string> m_eventsToBeTriggered = new List<string>();
        private S_Controllers m_controllerType;

        void Awake()
        {
            S_InputManager.Instance.RightTriggerButtonReleaseEvent += OnInputUpEvent;
            S_InputManager.Instance.LeftTriggerButtonReleaseEvent += OnInputUpEvent;
            S_InputManager.Instance.RightGripButtonPressEvent += OnInputUpEvent;
            S_InputManager.Instance.RightGripButtonReleaseEvent += OnInputUpEvent;
            S_InputManager.Instance.LeftGripButtonPressEvent += OnInputUpEvent;
            S_InputManager.Instance.LeftGripButtonReleaseEvent += OnInputUpEvent;
        }

        private void Start()
        {            
            BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);

            if (EventsToEnableLoad.Length > 0)
            {
                m_eventsToBeTriggered = new List<string>();
                EnableLoadOnActionUp = false;
                for (int i = 0; i < EventsToEnableLoad.Length; i++)
                {
                    m_eventsToBeTriggered.Add(EventsToEnableLoad[i]);
                }
            }
        }

        private void OnDestroy()
        {
            S_InputManager.Instance.RightTriggerButtonReleaseEvent -= OnInputUpEvent;
            S_InputManager.Instance.LeftTriggerButtonReleaseEvent -= OnInputUpEvent;
            S_InputManager.Instance.RightGripButtonPressEvent -= OnInputUpEvent;
            S_InputManager.Instance.RightGripButtonReleaseEvent -= OnInputUpEvent;
            S_InputManager.Instance.LeftGripButtonPressEvent -= OnInputUpEvent;
            S_InputManager.Instance.LeftGripButtonReleaseEvent -= OnInputUpEvent;

            BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
        }

        private void OnBasicSystemEvent(string _nameEvent, object[] _list)
        {
            if (m_eventsToBeTriggered.IndexOf(_nameEvent) != -1)
            {
                Debug.Log("REMOVED EVENT["+ _nameEvent + "]");
                m_eventsToBeTriggered.Remove(_nameEvent);
                if (m_eventsToBeTriggered.Count == 0)
                {
                    Debug.Log("ALL EVENTS COMPLETED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    EnableLoadOnActionUp = true;
                }
            }
        }

        private void OnInputUpEvent(S_InputEventArgs _event)
        {
            if (EnableLoadOnActionUp)
            {
                if ((_event.InputType == S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_RELEASE) 
                    || (_event.InputType == S_InputTypes.LEFT_GRIP_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.LEFT_TRIGGER_BUTTON_RELEASE) || (_event.InputType == S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_RELEASE))
                {
                    SceneManager.LoadScene(NextSceneToLoad);
                }
            }
        }
    }

}