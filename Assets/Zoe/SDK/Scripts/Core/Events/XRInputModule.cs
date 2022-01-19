using System;
using System.Collections.Generic;
using SpatialStories;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

/// <summary>
/// XRInputModule replaces the PointerInputModule and StandaloneInputModule to make it
/// possible to see controllers as pointers like a mouse. Most of the code has been
/// either taken from PointerInputModule, BaseInputModule or StandaloneInputModule.
/// Some other implementation actually extend PointerInputModule rather than BaseInputModule,
/// it was decided not to do that because PointerInputModule starts to refer a lot to mouse
/// states and other mouse button states, and we wanted the code to be clear of these concepts
/// here.
/// </summary>
public class XRInputModule : BaseInputModule
{
    [SerializeField]
    private S_HandsEnum m_HandPointer = S_HandsEnum.RIGHT;

    public Transform leftControllerTransform;
    public Transform rightControllerTransform;

    [Header("Haptics when hovering")]
    public float vibrationDurationSec;
    [Range(0.0f, 1.0f)]
    public float vibrationIntensity;
    [Range(0.0f, 1.0f)]
    public float vibrationFrequency;

    private InputFeatureUsage<bool> m_ClickButton = CommonUsages.triggerButton;

    private WorldPointerData leftControllerPointerData;
    private WorldPointerData rightControllerPointerData;

    // This is needed to avoid sending pointer enter and exit events more than once
    // since we have more than one pointer
    private Dictionary<GameObject, int> m_NumberOfPointersByGameObject = new Dictionary<GameObject, int>();

    protected override void Awake()
    {
        base.Awake();

        if ((m_HandPointer == S_HandsEnum.LEFT || m_HandPointer == S_HandsEnum.BOTH) &&
            leftControllerTransform == null)
        {
            throw new NullReferenceException("Transform for the left controller is null");
        }

        if ((m_HandPointer == S_HandsEnum.RIGHT || m_HandPointer == S_HandsEnum.BOTH) &&
            rightControllerTransform == null)
        {
            throw new NullReferenceException("Transform for the right controller is null");
        }
    }

    private bool m_RightClickButtonPressed;
    private bool m_LeftClickButtonPressed;
    public override void Process()
    {
        SendUpdateEventToSelectedObject();

        if (m_HandPointer == S_HandsEnum.LEFT || m_HandPointer == S_HandsEnum.BOTH)
        {
            InitializeWorldPointerData(ref leftControllerPointerData, leftControllerTransform);

            ProcessPointerEnterAndExit(leftControllerPointerData);
            ProcessPointerUpAndDown(leftControllerPointerData, XRRig.Instance.LeftController, ref m_LeftClickButtonPressed);
            ProcessPointerDrag(leftControllerPointerData);
        }

        if (m_HandPointer == S_HandsEnum.RIGHT || m_HandPointer == S_HandsEnum.BOTH)
        {
            InitializeWorldPointerData(ref rightControllerPointerData, rightControllerTransform);

            ProcessPointerEnterAndExit(rightControllerPointerData);
            ProcessPointerUpAndDown(rightControllerPointerData, XRRig.Instance.RightController, ref m_RightClickButtonPressed);
            ProcessPointerDrag(rightControllerPointerData);
        }
    }

    protected bool SendUpdateEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            return false;
        }

        BaseEventData data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }

    private void InitializeWorldPointerData(ref WorldPointerData _worldPointerData, Transform _pointerTransform)
    {
        if (_pointerTransform == null)
        {
            return;
        }

        if (_worldPointerData == null)
        {
            _worldPointerData = new WorldPointerData(eventSystem);
        }

        _worldPointerData.Reset();

        _worldPointerData.button = PointerEventData.InputButton.Left;
        _worldPointerData.worldSpaceRay = new Ray(_pointerTransform.position, _pointerTransform.forward);

        eventSystem.RaycastAll(_worldPointerData, m_RaycastResultCache);
        RaycastResult raycast = FindFirstRaycast(m_RaycastResultCache);

        _worldPointerData.pointerCurrentRaycast = raycast;

        // In the case of drag, we set the position a little differently because it can
        // happen that the pointer will move outside the actual object. In this particular
        // case, pointerDrag won't be null and we can use that to compute a better position.
        // N.B. This works only for UI elements retrieved with WorldGraphicRaycaster for now
        if (_worldPointerData.pointerDrag != null)
        {
            RectTransform rectTransform = _worldPointerData.pointerDrag.GetComponent<RectTransform>();
            Vector2 screenPosition;
            if (rectTransform != null &&
                WorldGraphicRaycaster.GetScreenSpaceHitPointOnPlaneFromRectangle(rectTransform, _worldPointerData.worldSpaceRay, _worldPointerData.pressEventCamera, out screenPosition))
            {
                _worldPointerData.delta = screenPosition - _worldPointerData.position;
                _worldPointerData.position = screenPosition;
            }
            else
            {
                // FIX ME: Find a way to compute a position that can be used by 3D objects to move
                _worldPointerData.delta = raycast.screenPosition - _worldPointerData.position;
                _worldPointerData.position = raycast.screenPosition;
            }
        }
        else
        {
            _worldPointerData.delta = raycast.screenPosition - _worldPointerData.position;
            _worldPointerData.position = raycast.screenPosition;
        }
    }

    private void ProcessPointerEnterAndExit(PointerEventData _pointerEventData)
    {
        GameObject newEnterTarget = _pointerEventData.pointerCurrentRaycast.gameObject;

        HandlePointerExitAndEnter(_pointerEventData, newEnterTarget);
    }

    /// <summary>
    /// This function replaces HandlePointerExitAndEnter, as implied by the
    /// subtle new keyword, in order to add the behaviour for several pointers
    /// pointing at the same gameobject
    /// </summary>
    /// <param name="_pointerEventData">The current pointer event data</param>
    /// <param name="_newEnterTarget">The gameobject under the pointer, can be null if nothing is under
    /// the pointer</param>
    private new void HandlePointerExitAndEnter(PointerEventData _pointerEventData, GameObject _newEnterTarget)
    {
        // if we have no target / pointerEnter has been deleted
        // just send exit events to anything we are tracking
        // then exit
        if (_newEnterTarget == null || _pointerEventData.pointerEnter == null)
        {
            for (int i = 0; i < _pointerEventData.hovered.Count; ++i)
            {
                // This can happen when we unparent/reparent stuff, such as a world canvas
                // suddenly being parented to some elements. In that case, the parents
                // will not appear in the m_NumberOfPointersByGameObject dictionary.
                // In order to keep a flow that makes sense, meaning every pointer exit
                // event should have been preceded by a pointer enter, we just skip these gameobjects
                if (!m_NumberOfPointersByGameObject.ContainsKey(_pointerEventData.hovered[i]))
                {
                    continue;
                }

                m_NumberOfPointersByGameObject[_pointerEventData.hovered[i]]--;

                if (m_NumberOfPointersByGameObject[_pointerEventData.hovered[i]] == 0)
                {
                    ExecuteEvents.Execute(_pointerEventData.hovered[i], _pointerEventData, ExecuteEvents.pointerExitHandler);
                    m_NumberOfPointersByGameObject.Remove(_pointerEventData.hovered[i]);
                }
            }

            _pointerEventData.hovered.Clear();

            if (_newEnterTarget == null)
            {
                _pointerEventData.pointerEnter = null;
                return;
            }
        }

        // if we have not changed hover target
        if (_pointerEventData.pointerEnter == _newEnterTarget && _newEnterTarget)
        {
            return;
        }

        GameObject commonRoot = FindCommonRoot(_pointerEventData.pointerEnter, _newEnterTarget);

        // and we already an entered object from last time
        if (_pointerEventData.pointerEnter != null)
        {
            // send exit handler call to all elements in the chain
            // until we reach the new target, or null!
            Transform t = _pointerEventData.pointerEnter.transform;

            while (t != null)
            {
                // if we reach the common root break out!
                if (commonRoot != null && commonRoot.transform == t)
                    break;

                // This can happen when we unparent/reparent stuff, such as a world canvas
                // suddenly being parented to some elements. In that case, the parents
                // will not appear in the m_NumberOfPointersByGameObject dictionary.
                // In order to keep a flow that makes sense, meaning every pointer exit
                // event should have been preceded by a pointer enter, we just skip these gameobjects
                if (!m_NumberOfPointersByGameObject.ContainsKey(t.gameObject))
                {
                    t = t.parent;

                    continue;
                }

                m_NumberOfPointersByGameObject[t.gameObject]--;

                if (m_NumberOfPointersByGameObject[t.gameObject] == 0)
                {
                    ExecuteEvents.Execute(t.gameObject, _pointerEventData, ExecuteEvents.pointerExitHandler);
                    m_NumberOfPointersByGameObject.Remove(t.gameObject);
                }


                _pointerEventData.hovered.Remove(t.gameObject);
                t = t.parent;
            }
        }

        // now issue the enter call up to but not including the common root
        _pointerEventData.pointerEnter = _newEnterTarget;
        if (_newEnterTarget != null)
        {
            Transform t = _newEnterTarget.transform;

            while (t != null && t.gameObject != commonRoot)
            {
                _pointerEventData.hovered.Add(t.gameObject);

                if (!m_NumberOfPointersByGameObject.ContainsKey(t.gameObject))
                {
                    m_NumberOfPointersByGameObject.Add(t.gameObject, 0);
                }

                m_NumberOfPointersByGameObject[t.gameObject]++;

                if (m_NumberOfPointersByGameObject[t.gameObject] == 1)
                {
                    // if we found a pointer enter handler, that means we are probably hovering a Selectable
                    // so vibrate the controller
                    // FIX ME: Right hand is hardcoded here but we should find a way to retrieve which pointer
                    // it is
                    if (ExecuteEvents.Execute(t.gameObject, _pointerEventData, ExecuteEvents.pointerEnterHandler))
                    {
                        S_HapticsManager.SetControllerVibration(S_HandsEnum.RIGHT, vibrationFrequency, vibrationIntensity, vibrationDurationSec);
                    }
                }

                t = t.parent;
            }
        }
    }

    /// <summary>
    /// This function has been entirely taken from StandaloneInputModule and has
    /// only been slightly altered to use an Oculus controller
    /// </summary>
    /// <param name="_pointerEventData">The current pointer event data</param>
    /// <param name="_controller">The controller for which we are processing the events</param>
    /// <param name="_previousClickButtonPressedState">The previous click button pressed state for this controller</param>
    private void ProcessPointerUpAndDown(PointerEventData _pointerEventData, InputDevice _controller, ref bool _previousClickButtonPressedState)
    {
        PointerEventData.FramePressState pressedState = StateForClickButton(_controller, ref _previousClickButtonPressedState);

        GameObject currentHoveredGameObject = _pointerEventData.pointerCurrentRaycast.gameObject;

        if (pressedState == PointerEventData.FramePressState.Pressed || pressedState == PointerEventData.FramePressState.PressedAndReleased)
        {
            _pointerEventData.eligibleForClick = true;
            _pointerEventData.delta = Vector2.zero;
            _pointerEventData.dragging = false;
            _pointerEventData.useDragThreshold = true;
            _pointerEventData.pressPosition = _pointerEventData.position;
            _pointerEventData.pointerPressRaycast = _pointerEventData.pointerCurrentRaycast;

            DeselectIfSelectionChanged(currentHoveredGameObject, _pointerEventData);

            // search for the control that will receive the press
            // if we can't find a press handler set the press
            // handler to be what would receive a click.
            GameObject newPressed = ExecuteEvents.ExecuteHierarchy(currentHoveredGameObject, _pointerEventData, ExecuteEvents.pointerDownHandler);

            // didnt find a press handler... search for a click handler
            if (newPressed == null)
            {
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentHoveredGameObject);
            }

            float time = Time.unscaledTime;

            if (newPressed == _pointerEventData.lastPress)
            {
                float diffTime = time - _pointerEventData.clickTime;
                if (diffTime < 0.3f)
                    ++_pointerEventData.clickCount;
                else
                    _pointerEventData.clickCount = 1;

                _pointerEventData.clickTime = time;
            }
            else
            {
                _pointerEventData.clickCount = 1;
            }

            _pointerEventData.pointerPress = newPressed;
            _pointerEventData.rawPointerPress = currentHoveredGameObject;

            _pointerEventData.clickTime = time;

            // Save the drag handler as well
            _pointerEventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentHoveredGameObject);

            if (_pointerEventData.pointerDrag != null)
            {
                ExecuteEvents.Execute(_pointerEventData.pointerDrag, _pointerEventData, ExecuteEvents.initializePotentialDrag);
            }
        }

        // PointerUp notification
        if (pressedState == PointerEventData.FramePressState.Released || pressedState == PointerEventData.FramePressState.PressedAndReleased)
        {
            ExecuteEvents.Execute(_pointerEventData.pointerPress, _pointerEventData, ExecuteEvents.pointerUpHandler);

            GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentHoveredGameObject);

            // PointerClick and Drop events
            if (_pointerEventData.pointerPress == pointerUpHandler && _pointerEventData.eligibleForClick)
            {
                ExecuteEvents.Execute(_pointerEventData.pointerPress, _pointerEventData, ExecuteEvents.pointerClickHandler);
            }
            else if (_pointerEventData.pointerDrag != null && _pointerEventData.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentHoveredGameObject, _pointerEventData, ExecuteEvents.dropHandler);
            }

            _pointerEventData.eligibleForClick = false;
            _pointerEventData.pointerPress = null;
            _pointerEventData.rawPointerPress = null;

            if (_pointerEventData.pointerDrag != null && _pointerEventData.dragging)
            {
                ExecuteEvents.Execute(_pointerEventData.pointerDrag, _pointerEventData, ExecuteEvents.endDragHandler);
            }

            _pointerEventData.dragging = false;
            _pointerEventData.pointerDrag = null;

            // redo pointer enter / exit to refresh state
            // so that if we moused over something that ignored it before
            // due to having pressed on something else
            // it now gets it.
            if (currentHoveredGameObject != _pointerEventData.pointerEnter)
            {
                HandlePointerExitAndEnter(_pointerEventData, null);
                HandlePointerExitAndEnter(_pointerEventData, currentHoveredGameObject);
            }
        }
    }

    /// <summary>
    /// This function has been entirely copied and not altered from PointerInputModule
    /// </summary>
    /// <returns>Whether we should interpret the slight movement as a drag or not</returns>
    private static bool ShouldStartDrag(Vector2 _pointerPressPosition, Vector2 _currentPointerPosition, float _dragThreshold, bool _useDragThreshold)
    {
        if (!_useDragThreshold)
        {
            return true;
        }

        return (_pointerPressPosition - _currentPointerPosition).sqrMagnitude >= _dragThreshold * _dragThreshold;
    }

    /// <summary>
    /// This function has been entirely copied and not altered from PointerInputModule.
    /// Process the drag for the current frame with the given pointer event.
    /// </summary>
    protected virtual void ProcessPointerDrag(PointerEventData _pointerEventData)
    {
        if (!_pointerEventData.IsPointerMoving() ||
            Cursor.lockState == CursorLockMode.Locked ||
            _pointerEventData.pointerDrag == null)
        {
            return;
        }

        if (!_pointerEventData.dragging
            && ShouldStartDrag(_pointerEventData.pressPosition, _pointerEventData.position, eventSystem.pixelDragThreshold, _pointerEventData.useDragThreshold))
        {
            ExecuteEvents.Execute(_pointerEventData.pointerDrag, _pointerEventData, ExecuteEvents.beginDragHandler);
            _pointerEventData.dragging = true;
        }

        // Drag notification
        if (_pointerEventData.dragging)
        {
            // Before doing drag we should cancel any pointer down state
            // And clear selection!
            if (_pointerEventData.pointerPress != _pointerEventData.pointerDrag)
            {
                ExecuteEvents.Execute(_pointerEventData.pointerPress, _pointerEventData, ExecuteEvents.pointerUpHandler);

                _pointerEventData.eligibleForClick = false;
                _pointerEventData.pointerPress = null;
                _pointerEventData.rawPointerPress = null;
            }

            ExecuteEvents.Execute(_pointerEventData.pointerDrag, _pointerEventData, ExecuteEvents.dragHandler);
        }
    }

    /// <summary>
    /// This function has been entirely copied and not altered from PointerInputModule.
    /// Deselect the current selected GameObject if the currently pointed-at GameObject is different.
    /// </summary>
    /// <param name="_currentHoverGameObject">The GameObject the pointer is currently over.</param>
    /// <param name="_baseEventData">Current event data.</param>
    protected void DeselectIfSelectionChanged(GameObject _currentHoverGameObject, BaseEventData _baseEventData)
    {
        // Selection tracking
        GameObject selectHandlerGameObject = ExecuteEvents.GetEventHandler<ISelectHandler>(_currentHoverGameObject);

        // if we have clicked something new, deselect the old thing
        // leave 'selection handling' up to the press event though.
        if (selectHandlerGameObject != eventSystem.currentSelectedGameObject)
        {
            eventSystem.SetSelectedGameObject(null, _baseEventData);
        }
    }

    /// <summary>
    /// Given an XR.InputDevice return the current state of the clickButton for the frame.
    /// This function has been taken from PointerInputModule and sligtly changes it
    /// to use an XR.InputDeice
    /// </summary>
    /// <param name="_controller">The XR.InputDevice, in other term, the controller, on which to check</param>
    /// <param name="_previousClickButtonPressedState">The previous pressed state for the provided input device</param>
    protected PointerEventData.FramePressState StateForClickButton(InputDevice _controller, ref bool _previousClickButtonPressedState)
    {
        bool pressedThisFrame = false;
        _controller.TryGetFeatureValue(m_ClickButton, out pressedThisFrame);

        bool pressed = false;
        if (pressedThisFrame && !_previousClickButtonPressedState)
        {
            pressed = true;
        }

        bool released = false;
        if (!pressedThisFrame && _previousClickButtonPressedState)
        {
            released = true;
        }

        _previousClickButtonPressedState = pressedThisFrame;

        // The following condition looks impossible to achieve but for the sake of consistency
        // with that was done on Unity's side, I decided to keep it
        if (pressed && released)
        {
            return PointerEventData.FramePressState.PressedAndReleased;
        }

        if (pressed)
        {
            return PointerEventData.FramePressState.Pressed;
        }

        if (released)
        {
            return PointerEventData.FramePressState.Released;
        }

        return PointerEventData.FramePressState.NotChanged;
    }
};
