using SpatialStories;
using UnityEngine;

namespace SpatialStories
{
    /// <summary>
    /// This manager is the only class that can change a gravity state of an IO.
    /// </summary>
    public static class S_GravityManager
    {
        /// <summary>
        /// If set to true the gravity manager will begin to log gravity inconcistencies
        /// </summary>
        public static bool SHOW_GRAVITY_WARNINGS = false;

        public static void ChangeGravityState(S_InteractiveObject _IO, S_GravityRequestType operation, bool _notifyGrabLogic = true)
        {
            switch (operation)
            {
                case S_GravityRequestType.ACTIVATE:
                    SetGravity(_IO, true);
                    break;
                case S_GravityRequestType.DEACTIVATE:
                    SetGravity(_IO, false);
                    break;
                case S_GravityRequestType.DEACTIVATE_AND_ATTACH:
                    SetGravityAndAttachIO(_IO, false);
                    break;
                case S_GravityRequestType.ACTIVATE_AND_DETACH:
                    SetGravityAndAttachIO(_IO, true);
                    break;
                case S_GravityRequestType.LOCK:
                    _IO.LockGravity();
                    break;
                case S_GravityRequestType.UNLOCK:
                    _IO.UnlockGravity();
                    break;
                case S_GravityRequestType.RETURN_TO_DEFAULT:
                    ReturnToDefaultState(_IO);
                    break;
                case S_GravityRequestType.SET_AS_DEFAULT:
                    SetActualGravityStateAsDefault(_IO);
                    break;
            }

            // Usefull if the object is being grabbed
            if (_notifyGrabLogic)
                _IO.GrabLogic.GravityChanged();
        }

        /// <summary>
        /// Sets the actual gravity state of an object as the default state to return to when
        /// the object is released.
        /// </summary>
        /// <param name="_IO"></param>
        private static void SetActualGravityStateAsDefault(S_InteractiveObject _IO)
        {
            if (_IO.IsGravityLocked())
            {
                if (SHOW_GRAVITY_WARNINGS)
                    Debug.LogWarning(string.Format("GravityManager -> Interactive object: {0} has ben requested to change its gravity but the object is LOCKED", _IO.name));
                return;
            }
            else
            {
                _IO.SetActualGravityStateAsDefault();
            }
        }

        private static void ReturnToDefaultState(S_InteractiveObject _IO)
        {
            if (_IO.IsGravityLocked())
            {
                if (SHOW_GRAVITY_WARNINGS)
                    Debug.LogWarning(string.Format("GravityManager -> Interactive object: {0} has ben requested to change its gravity but the object is LOCKED", _IO.name));
                return;
            }
            else
            {
                _IO.ReturnToInitialGravityState();
            }
        }

        /// <summary>
        /// Sets the gravity of a game object and attach to it.
        /// </summary>
        /// <param name="_IO">The io where the changed where be performed</param>
        /// <param name="_hasGravity">True = Gravity and not kinematic, False = No Gravity and kinematic</param>
        private static void SetGravityAndAttachIO(S_InteractiveObject _IO, bool _hasGravity)
        {
            if (_IO)
            {
                if (_IO.IsGravityLocked())
                {
                    if (SHOW_GRAVITY_WARNINGS)
                        Debug.LogWarning(string.Format("GravityManager -> Interactive object: {0} has ben requested to change its gravity but the object is LOCKED", _IO.name));
                    return;
                }
                _IO.SetGravityAndAttach(_hasGravity);
            }
        }

        /// <summary>
        /// Sets Only the gravity to true or false to attach please call
        /// SetGravityAndAttachIO.
        /// </summary>
        /// <param name="_IO"></param>
        /// <param name="_hasGravity"></param>
        private static void SetGravity(S_InteractiveObject _IO, bool _hasGravity)
        {
            if (_IO)
            {
                if (_IO.IsGravityLocked())
                {
                    if (SHOW_GRAVITY_WARNINGS)
                        Debug.LogWarning(string.Format("GravityManager -> Interactive object: {0} has ben requested to set its gravity to {1} but the object is LOCKED", _IO.name, _hasGravity));
                    return;
                }
                _IO.SetGravity(_hasGravity);
            }
        }

        /// <summary>
        /// Sets the mass of the IO
        /// </summary>
        /// <param name="_IO"></param>
        /// <param name="_hasGravity"></param>
        public static void SetMass(S_InteractiveObject _IO, float _mass)
        {
            if (_IO)
            {
                _IO.SetMass(_mass);
            }
        }

        public static void AddRigidBodyToIO(S_InteractiveObject _IO)
        {
            if (_IO.GetComponent<Rigidbody>() == null)
            {
                _IO.gameObject.AddComponent<Rigidbody>();
                SetGravity(_IO, true);
            }
        }
    }
}
