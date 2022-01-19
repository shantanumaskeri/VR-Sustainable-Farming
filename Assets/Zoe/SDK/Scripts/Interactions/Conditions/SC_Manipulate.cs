using Gaze;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SC_Manipulate")]
    public class SC_Manipulate : S_AbstractCondition
    {
        public const string EVENT_MANIPULATE_RELEASE_CONFIRMATION = "EVENT_MANIPULATE_RELEASE_CONFIRMATION";

        private bool m_GrabLeftValid = false;
        private bool m_GrabRightValid = false;
        private bool m_GrabStateLeftValid = false;
        private bool m_GrabStateRightValid = false;

        private S_HandsEnum m_HandReleased = S_HandsEnum.BOTH;

        public S_GrabMap GrabMap = new S_GrabMap();
        
        public SC_Manipulate(){ }

        public override void Setup()
        {
            if (!m_listenersSetUp && !m_hasBeenDestroyed)
            {
                m_listenersSetUp = true;

                S_InputManager.Instance.OnControllerGrabEvent += OnControllerGrabEvent;

                // If we are valid when setup just fire validate
                if (IsValid)
                    Validate();
            }
        }

        public override void Dispose()
        {
            if (m_listenersSetUp && !m_hasBeenDestroyed)
            {
                m_listenersSetUp = false;
                S_InputManager.Instance.OnControllerGrabEvent -= OnControllerGrabEvent;
            }
        }


        public override void OnDestroy()
        {
            m_listenersSetUp = true;
            Dispose();
            m_hasBeenDestroyed = true;
        }

        private bool IsGrabbingControllerStateValid(bool _isGrabbing, S_HandsEnum _mapHand, UnityEngine.XR.XRNode _dicoHand)
        {
            if (_mapHand.Equals(S_HandsEnum.BOTH))
            {
                // check left hand state is ok
                if (_dicoHand.Equals(UnityEngine.XR.XRNode.LeftHand))
                {
                    if (_isGrabbing)
                    {
                        m_GrabStateLeftValid = GrabMap.GrabStateLeftIndex.Equals((int)S_GrabStates.GRAB) || GrabMap.GrabStateLeftIndex.Equals((int)S_GrabStates.HOLDING);
                    }
                    else
                    {
                        m_GrabStateLeftValid = GrabMap.GrabStateLeftIndex.Equals((int)S_GrabStates.UNGRAB);
                        m_HandReleased = S_HandsEnum.LEFT;
                    }
                }

                // check right hand state is ok
                if (_dicoHand.Equals(UnityEngine.XR.XRNode.RightHand))
                {
                    if (_isGrabbing)
                    {
                        m_GrabStateRightValid = GrabMap.GrabStateRightIndex.Equals((int)S_GrabStates.GRAB) || GrabMap.GrabStateRightIndex.Equals((int)S_GrabStates.HOLDING);
                    }
                    else
                    {
                        m_GrabStateRightValid = GrabMap.GrabStateRightIndex.Equals((int)S_GrabStates.UNGRAB);
                        m_HandReleased = S_HandsEnum.RIGHT;
                    }
                }
                return m_GrabStateLeftValid || m_GrabStateRightValid;
            }
            else
            {
                int state = _mapHand.Equals(S_HandsEnum.LEFT) ? GrabMap.GrabStateLeftIndex : GrabMap.GrabStateRightIndex;

                if (_isGrabbing && state.Equals((int)S_GrabStates.GRAB) || state.Equals((int)S_GrabStates.HOLDING))
                {
                    return true;
                }

                if (!_isGrabbing && state.Equals((int)S_GrabStates.UNGRAB))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsGrabbingObjectValid(GameObject _grabbedObject, int _handIndex)
        {
            return _grabbedObject.Equals(GrabMap.GrabEntryList[0].interactiveObject.gameObject);
        }

        private void ValidateGrab(S_ControllerGrabEventArgs e)
        {
            if (IsValid) return;

            bool isValid = ValidateGrabController(e);

            if (IsDuration && (FocusDuration > 0))
            {
                if (e.IsGrabbing)
                {
                    ActivateDuration();
                }
                else
                {
                    DeactivateDuration();
                }
            }
            else
            {                
                if (isValid)
                {
                    Validate();
                }
                else
                {
                    Invalidate();
                }
            }
        }

        private bool ValidateGrabController(S_ControllerGrabEventArgs e)
        {
            UnityEngine.XR.XRNode dicoVRNode = e.ControllerObjectPair.Key;
            GameObject grabbedObject = e.ControllerObjectPair.Value;

            // get the hand VRNode from the event
            bool isGrabbingControllerLeft = e.ControllerObjectPair.Key == UnityEngine.XR.XRNode.LeftHand;
            UnityEngine.XR.XRNode eventVRNode = isGrabbingControllerLeft ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand;

            bool grabbedObjectValid = IsGrabbingObjectValid(grabbedObject, GrabMap.GrabHandsIndex);

            // if we've configured
            switch (GrabMap.GrabHandsIndex)
            {
                case (int)S_HandsEnum.BOTH:
                    bool isGrabbingControllerStateValid = IsGrabbingControllerStateValid(e.IsGrabbing, S_HandsEnum.BOTH, eventVRNode);
                    bool isGrabbingControllerInMap = IsGrabbingControllerInMap(dicoVRNode);

                    return isGrabbingControllerInMap && isGrabbingControllerStateValid && grabbedObjectValid;

                //  the LEFT hand
                case (int)S_HandsEnum.LEFT:
                    m_GrabLeftValid = IsGrabbingControllerInMap(dicoVRNode) && IsGrabbingControllerStateValid(e.IsGrabbing, S_HandsEnum.LEFT, eventVRNode) && grabbedObjectValid;
                    return m_GrabLeftValid;

                //  the RIGHT hand
                case (int)S_HandsEnum.RIGHT:
                    m_GrabRightValid = IsGrabbingControllerInMap(dicoVRNode) && IsGrabbingControllerStateValid(e.IsGrabbing, S_HandsEnum.RIGHT, eventVRNode) && grabbedObjectValid;
                    return m_GrabRightValid;
            }

            return false;
        }

        private void OnControllerGrabEvent(S_ControllerGrabEventArgs e)
        {
            ValidateGrab(e);
        }


        private bool IsGrabbingControllerInMap(UnityEngine.XR.XRNode grabbingController)
        {
            for (int i = 0; i < GrabMap.GrabEntryList.Count; i++)
            {
                if (GrabMap.GrabHandsIndex == (int)S_HandsEnum.BOTH)
                {
                    if (GrabMap.GrabEntryList[i].hand.Equals(UnityEngine.XR.XRNode.RightHand) ||
                        GrabMap.GrabEntryList[i].hand.Equals(UnityEngine.XR.XRNode.LeftHand))
                        return true;
                }
                else
                    if (GrabMap.GrabEntryList[i].hand.Equals(grabbingController))
                    return true;
            }

            return false;
        }


        public override void Reload()
        {
            // Reload all the states
            m_GrabLeftValid = false;
            m_GrabRightValid = false;
            m_GrabStateLeftValid = false;
            m_GrabStateRightValid = false;

            S_InteractiveObject interactiveObject = GrabMap.GrabEntryList[0].interactiveObject;

            // If the object is not being grabbed just invalidate
            if (!IsTheGrabbingStateHolding() || !IsTheObjectStillGrabbed(interactiveObject) || !IsTheObjectGrabbedWithTheCorrectHand(interactiveObject))
            {
                Invalidate();
            }
        }

        private bool IsTheObjectGrabbedWithTheCorrectHand(S_InteractiveObject _interactiveObject)
        {
            S_GrabManager GrabbingHand = S_GrabManager.GetGrabbingHands(_interactiveObject)[0];
            
            switch ((S_HandsEnum)GrabMap.GrabHandsIndex)
            {
                case S_HandsEnum.LEFT:
                    if (GrabbingHand.IsLeftHand)
                    {
                        return true;
                    }
                    break;
                case S_HandsEnum.RIGHT:
                    if (!GrabbingHand.IsLeftHand)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        private bool IsTheObjectStillGrabbed(S_InteractiveObject _interactiveObject)
        {
            return _interactiveObject.GrabLogic.IsBeingGrabbed;
        }

        private bool IsTheGrabbingStateHolding()
        {
            bool isTheStateHolding = true;

            // If the state is not holding invalidate
            switch ((S_HandsEnum)GrabMap.GrabHandsIndex)
            {
                case S_HandsEnum.BOTH:
                    // Check if some of them are in holding
                    if (!GrabMap.GrabStateRightIndex.Equals((int)S_GrabStates.HOLDING))
                        isTheStateHolding = false;
                    break;
                case S_HandsEnum.LEFT:
                    if (!GrabMap.GrabStateLeftIndex.Equals((int)S_GrabStates.HOLDING))
                        isTheStateHolding = false;
                    break;
                case S_HandsEnum.RIGHT:
                    if (!GrabMap.GrabStateRightIndex.Equals((int)S_GrabStates.HOLDING))
                        isTheStateHolding = false;
                    break;
            }

            return isTheStateHolding;
        }

        public override void Validate()
        {
            base.Validate();

            if (m_HandReleased == S_HandsEnum.RIGHT)
            {
                BasicSystemEventController.Instance.DelayBasicSystemEvent(EVENT_MANIPULATE_RELEASE_CONFIRMATION, 0.01f, GameObject.FindObjectOfType<S_RightHandRoot>().gameObject.transform.forward);
            } 
            else 
            if (m_HandReleased == S_HandsEnum.LEFT)
            {
                BasicSystemEventController.Instance.DelayBasicSystemEvent(EVENT_MANIPULATE_RELEASE_CONFIRMATION, 0.01f, GameObject.FindObjectOfType<S_LeftHandRoot>().gameObject.transform.forward);
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            // if there are no entry yet, create a default one
            if (GrabMap.GrabEntryList.Count < 1)
                GrabMap.AddGrabableEntry(SpatialStoriesAPI.GetInteractiveObjectWithGUID(creationData[2].ToString()).gameObject);

            // Add the grabbing hand
            GrabMap.GrabHandsIndex = (int)creationData[0];
            
            // Setup the grab state for the hand
            switch ((S_HandsEnum)GrabMap.GrabHandsIndex)
            {
                case S_HandsEnum.BOTH:
                    GrabMap.GrabStateLeftIndex = (int)creationData[1];
                    GrabMap.GrabEntryList[0].hand = UnityEngine.XR.XRNode.LeftHand;
                    GrabMap.GrabStateRightIndex = (int)creationData[1];
                    break;
                case S_HandsEnum.LEFT:
                    GrabMap.GrabStateLeftIndex = (int)creationData[1];
                    GrabMap.GrabEntryList[0].hand = UnityEngine.XR.XRNode.LeftHand;
                    break;
                case S_HandsEnum.RIGHT:
                    GrabMap.GrabStateRightIndex = (int)creationData[1];
                    GrabMap.GrabEntryList[0].hand = UnityEngine.XR.XRNode.RightHand;
                    break;
            }
        }
    }

    public partial class APIExtensions
    {
        public static SC_Manipulate CreateGrabCondition(this S_InteractionDefinition _def, S_HandsEnum _hand, S_GrabStates _state, string _interactiveObjectID)
        {
            return _def.CreateCondition<SC_Manipulate>(_hand, _state, _interactiveObjectID);
        }
    }
}
