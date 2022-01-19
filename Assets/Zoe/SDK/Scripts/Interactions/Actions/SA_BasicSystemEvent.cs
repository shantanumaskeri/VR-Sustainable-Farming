using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public class SA_BasicSystemEvent : S_AbstractAction
    {
        public string EventName;
        public List<string> EventParams = new List<string>();

        protected SA_BasicSystemEvent()
        {
        }

        protected override void ActionLogic()
        {
            BasicSystemEventController.Instance.DispatchBasicSystemEvent(EventName, EventParams.ToArray());
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            throw new System.NotImplementedException();
        }
    }
}
