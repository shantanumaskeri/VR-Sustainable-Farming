using UnityEngine;

namespace SpatialStories
{
    public class S_DisplayTextfieldInfo : MonoBehaviour
    {
        public GameObject[] OriginEvents;
        public string[] EventsToDisplayInfo;
        public string[] Labels;
        public TextMesh DisplayInfo;

        private void Start()
        {
            BasicSystemEventController.Instance.BasicSystemEvent += new BasicSystemEventHandler(OnBasicSystemEvent);
        }

        private void OnDestroy()
        {
            BasicSystemEventController.Instance.BasicSystemEvent -= OnBasicSystemEvent;
        }

        private void OnBasicSystemEvent(string _nameEvent, object[] _list)
        {
            bool checkEvents = false;
            if (OriginEvents == null)
            {
                checkEvents = true;
            }
            else
            {
                if (_list.Length > 0)
                {
                    if (_list[0] is GameObject)
                    {
                        foreach (GameObject item in OriginEvents)
                        {
                            if (item == (GameObject)_list[0])
                            {
                                checkEvents = true;
                            }
                        }
                    }
                }
            }

            if (checkEvents)
            {
                for (int i = 0; i < EventsToDisplayInfo.Length; i++)
                {
                    if (EventsToDisplayInfo[i].IndexOf(_nameEvent) != -1)
                    {
                        string info = "";
                        if (i < Labels.Length) info = Labels[i];
                        for (int j = 1; j < _list.Length; j++)
                        {
                            System.Type typeData = _list[j].GetType();
                            if (typeData == typeof(int))
                            {
                                info += " " + ((int)_list[j]).ToString();
                            }
                            if (typeData == typeof(float))
                            {
                                info += " " + ((float)_list[j]).ToString();
                            }
                            if (typeData == typeof(double))
                            {
                                info += " " + ((double)_list[j]).ToString();
                            }
                            if (typeData == typeof(string))
                            {
                                info += " " + ((string)_list[j]).ToString();
                            }
                            if (typeData == typeof(Vector3))
                            {
                                info += " " + ((Vector3)_list[j]).ToString();
                            }
                            if (typeData == typeof(Transform))
                            {
                                info += " " + ((Transform)_list[j]).name;
                            }
                            if (typeData == typeof(GameObject))
                            {
                                info += " " + ((GameObject)_list[j]).name;
                            }
                        }
                        DisplayInfo.text = info;
                    }
                }
            }
        }
    }

}