using System;
using System.Collections.Generic;
using Gaze;
using UnityEngine;

namespace SpatialStories
{
    public class SC_ActionButton : S_AbstractCondition
    {
        public bool RequireAllInputs = false;
        public S_InputsMap InputsMap = new S_InputsMap();

        public SC_ActionButton()
        {
        }

        public override void Setup()
        {
            if (!m_listenersSetUp && !m_hasBeenDestroyed)
            {
                m_listenersSetUp = true;
                S_InputManager.Instance.RightGripButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.RightGripButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);
                S_InputManager.Instance.LeftGripButtonPressEvent += new S_InputManager.InputEvent(OnInputDownEvent);
                S_InputManager.Instance.LeftGripButtonReleaseEvent += new S_InputManager.InputEvent(OnInputUpEvent);
            }
        }

        public override void Dispose()
        {
            if (m_listenersSetUp && !m_hasBeenDestroyed)
            {
                m_listenersSetUp = false;
                S_InputManager.Instance.RightGripButtonReleaseEvent -= OnInputUpEvent;
                S_InputManager.Instance.RightGripButtonPressEvent -= OnInputDownEvent;
                S_InputManager.Instance.LeftGripButtonReleaseEvent -= OnInputUpEvent;
                S_InputManager.Instance.LeftGripButtonPressEvent -= OnInputDownEvent;
            }
        }

        public override void OnDestroy()
        {
            m_listenersSetUp = true;
            Dispose();
            m_hasBeenDestroyed = true;
        }

        protected void OnInputDownEvent(S_InputEventArgs _event)
        {
            if (this == null) return;
            if (IsValid) return;

            foreach (S_InputsMapEntry input in InputsMap.InputsEntries)
            {
                if (input.InputType == _event.InputType)
                {
                    Validate();
                }
            }
        }

        protected void OnInputUpEvent(S_InputEventArgs _event)
        {
            if (IsValid) return;

            foreach (S_InputsMapEntry input in InputsMap.InputsEntries)
            {
                if (input.InputType == _event.InputType)
                {
                    Validate();
                }
            }
        }

        public override void Reload()
        {
            Invalidate();
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
        }

    }

    /// <summary>
    /// Wrapper for the API in order to create a collision condition
    /// </summary>
    public static partial class APIExtensions
    {
        public static SC_ActionButton CreateActionButtonCondition(this S_InteractionDefinition _def, string _objectToCollideAtGUID)
        {
            return _def.CreateCondition<SC_ActionButton>(_objectToCollideAtGUID);
        }
    }
}