using UnityEngine;

namespace SpatialStories
{

    /******************************************
	 * 
	 * PassengerObject
	 * 
	 * @author Esteban Gallardo
	 */
    public class PassengerObject : MonoBehaviour
	{
        public GameObject MainObject;

        private bool m_isAlive = true;

        private void Update()
        {
            if (MainObject != null)
            {
                transform.position = MainObject.transform.position;
            }
            else
            {
                if (m_isAlive)
                {
                    m_isAlive = false;
                    GameObject.Destroy(this.gameObject, 3);
                }                
            }            
        }
    }
}