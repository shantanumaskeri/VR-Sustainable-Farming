using UnityEngine;

namespace SpatialStories
{
    public class SA_IncrementCounter : S_AbstractAction
    {
        public UnityEngine.UI.Text[] txt;
        public static int Count = 0;

        public SA_IncrementCounter(){}

        public override void SetupUsingApi(GameObject _interaction)
        {
        }

        protected override void ActionLogic()
        {
            Count++;
            foreach (UnityEngine.UI.Text t in txt)
            {
                t.text = Count.ToString();
            }
        }
    }
}
