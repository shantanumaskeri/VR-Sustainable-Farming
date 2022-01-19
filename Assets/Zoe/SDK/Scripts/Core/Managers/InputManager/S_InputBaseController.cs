using UnityEngine;

namespace SpatialStories
{
    public abstract class S_InputBaseController : MonoBehaviour
    {
        protected S_InputManager m_InputManager;

        public virtual S_InputBaseController Initialitzation(S_InputManager _inputManager)
        {
            m_InputManager = _inputManager;
            return this;
        }

        public abstract Vector2 LeftAxisStick();
        public abstract Vector2 RightAxisStick();

        public abstract void Logic();
        public abstract bool CheckIfControllerConnected();
    }
}
