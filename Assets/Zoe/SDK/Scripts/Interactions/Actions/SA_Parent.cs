using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SA_Parent")]
    public class SA_Parent : S_AbstractAction
    {
        public bool Parented = true;
        public Transform ParentObject;
        public GameObject ObjectToParent;

       

        public SA_Parent() { }

        private void Start()
        {
           
        }

        public void OnDestroy()
        {
            
        }

        protected override void ActionLogic()
        {
        
            if(Parented=true)
                {

                    ObjectToParent.transform.SetParent(ParentObject);
                }
            else
                {
                    ObjectToParent.transform.SetParent(null);
                }
            }

        public override void SetupUsingApi(GameObject _interaction)
        {
            
        }
    }

    public static partial class APIExtensions
    {
        
    }
}