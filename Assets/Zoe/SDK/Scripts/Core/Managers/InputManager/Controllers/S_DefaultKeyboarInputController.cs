using UnityEngine;

namespace SpatialStories
{
    /******************************************
     * 
     * S_DefaultKeyboardInputController
     * 
     * Class that manages the input from keyboard (used on Unity Editor mode)
     * 
     * @author Esteban Gallardo
     */
    public class S_DefaultKeyboardInputController : S_InputBaseController
    {
        // ----------------------------------------------
        // SINGLETON
        // ----------------------------------------------	
        private static S_DefaultKeyboardInputController _instance;

        public static S_DefaultKeyboardInputController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType(typeof(S_DefaultKeyboardInputController)) as S_DefaultKeyboardInputController;
                    if (!_instance)
                    {
                        GameObject container = new GameObject();
#if DONT_DESTROY_ON_LOAD
                        DontDestroyOnLoad(container);
#endif
                        container.name = "S_DefaultKeyboardInputController";
                        _instance = container.AddComponent(typeof(S_DefaultKeyboardInputController)) as S_DefaultKeyboardInputController;
                    }
                }
                return _instance;
            }
        }

        // ----------------------------------------------
        // PRIVATE MEMBERS
        // ----------------------------------------------	

        // -------------------------------------------
        /* 
		 * Initialitzation
		 */
        public override S_InputBaseController Initialitzation(S_InputManager _inputManager)
        {
            base.Initialitzation(_inputManager);

            return this;
        }

        // -------------------------------------------
        /* 
		 * CheckIfControllerConnected
		 */
        public override bool CheckIfControllerConnected()
        {
            // TODO (Giuseppe)
            return true;
        }

        // -------------------------------------------
        /* 
		 * Update
		 */
        public override void Logic()
        {

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                S_InputManager.Instance.FireRightTriggerButtonPressEvent(new S_InputEventArgs(this.gameObject, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_TRIGGER_BUTTON_PRESS));
                S_InputManager.Instance.FireRightGripButtonPressEvent(new S_InputEventArgs(this.gameObject, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_GRIP_BUTTON_PRESS));

                S_InputManager.Instance.IsHandRightDown = true;
                S_InputManager.Instance.FireRightGripButtonPressEvent(new S_InputEventArgs(this.gameObject, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_GRIP_BUTTON_PRESS));
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                S_InputManager.Instance.IsIndexRightDown = false;
                S_InputManager.Instance.FireRightTriggerButtonReleaseEvent(new S_InputEventArgs(this.gameObject, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE));

                S_InputManager.Instance.FireRightGripButtonPressEvent(new S_InputEventArgs(this.gameObject, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE));

                S_InputManager.Instance.IsHandRightDown = false;
                S_InputManager.Instance.FireRightGripButtonReleaseEvent(new S_InputEventArgs(this.gameObject, UnityEngine.XR.XRNode.RightHand, S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE));
            }
#endif

        }

        public override Vector2 LeftAxisStick()
        {
            return Vector2.zero;
        }

        public override Vector2 RightAxisStick()
        {
            return Vector2.zero;
        }
    }
}


