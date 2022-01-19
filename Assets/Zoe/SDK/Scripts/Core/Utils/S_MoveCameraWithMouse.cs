using System;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_EDITOR
using UnityEngine.XR;
#endif

namespace SpatialStories
{
    /******************************************
     * 
     * S_MoveCameraWithMouse
     * 
     * Use the mouse to move the camera
     * 
     * @author Esteban Gallardo
     */
    public class S_MoveCameraWithMouse : MonoBehaviour
    {
        private enum RotationAxes { None = 0, MouseXAndY = 1, MouseX = 2, MouseY = 3, Controller = 4 }

        public const float SPECTATOR_SPEED = 2;

        // ----------------------------------------------
        // PUBLIC VARIABLES
        // ----------------------------------------------	
        public Transform Target;
        public Transform Camera;

        public float SensitivityX = 7F;
        public float SensitivityY = 7F;
        public float MinimumY = -60F;
        public float MaximumY = 60F;

        public float Speed = 4;

        public bool EnableInteraction = true;
        

        // ----------------------------------------------
        // PRIVATE VARIABLES
        // ----------------------------------------------	
        public float m_rotationY = 0F;
        private bool m_enableGyroscope = true;

        public bool EnableGyroscope
        {
            get { return m_enableGyroscope; }
            set {
                bool enableGyroscope = value;
                if (m_enableGyroscope && !enableGyroscope)
                {
                    if (Target != null)
                    {
                        if (m_parentCamera != null)
                        { 
                            m_parentCamera.transform.Rotate(Vector3.right, -90);
                            Target.transform.parent = null;
                            GameObject.Destroy(m_parentCamera);
                            m_parentCamera = null;
                        }
                    }
                }
                m_enableGyroscope = enableGyroscope;
            }
        }

        // -------------------------------------------
        /* 
		 * Start
		 */
        void Start()
        {
#if UNITY_EDITOR
            Camera = this.transform;
#endif
        }

        // -------------------------------------------
        /* 
		 * RotateCamera
		 */
        private void RotateCamera()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                RotationAxes axes = RotationAxes.None;

                if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0))
                {
                    axes = RotationAxes.MouseXAndY;
                }

                if ((axes != RotationAxes.Controller) && (axes != RotationAxes.None))
                {
                    if (axes == RotationAxes.MouseXAndY)
                    {
                        float rotationX = Camera.localEulerAngles.y + Input.GetAxis("Mouse X") * SensitivityX;

                        m_rotationY += Input.GetAxis("Mouse Y") * SensitivityY;
                        m_rotationY = Mathf.Clamp(m_rotationY, MinimumY, MaximumY);

                        Camera.localEulerAngles = new Vector3(-m_rotationY, rotationX, 0);
                    }
                    else if (axes == RotationAxes.MouseX)
                    {
                        Camera.Rotate(0, Input.GetAxis("Mouse X") * SensitivityX, 0);
                    }
                    else
                    {
                        m_rotationY += Input.GetAxis("Mouse Y") * SensitivityY;
                        m_rotationY = Mathf.Clamp(m_rotationY, MinimumY, MaximumY);

                        Camera.localEulerAngles = new Vector3(-m_rotationY, transform.localEulerAngles.y, 0);
                    }
                }
            }
        }

        // -------------------------------------------
        /* 
         * MoveCamera
         */
        private bool EnabledTeleporting()
        {
            S_Teleporter[] teleporterInstances = GameObject.FindObjectsOfType<S_Teleporter>();
            int enabledTeleporter = 0;
            for (int i = 0; i < teleporterInstances.Length; i++)
            {
                if (teleporterInstances[i].enabled)
                {
                    enabledTeleporter++;
                }
            }
            return (enabledTeleporter > 0);
        }

        // -------------------------------------------
        /* 
         * MoveCamera
         */
        private void MoveCamera()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Vector3 forward = Input.GetAxis("Vertical") * Camera.forward * Speed * Time.deltaTime;
                Vector3 lateral = Input.GetAxis("Horizontal") * Camera.right * Speed * Time.deltaTime;

                Vector3 increment = forward + lateral;
                Target.transform.position += increment;
            }
#endif
        }

        private GameObject m_parentCamera;

        // -------------------------------------------
        /* 
		 * Update
		 */
        void Update()
        {
            if (EnableInteraction)
            {
                if (Target != null)
                {
#if UNITY_EDITOR
                    if (EnableGyroscope)
                    {
                        MoveCamera();
                        RotateCamera();
                    }
#endif
                }
            }

#if !UNITY_EDITOR
            if (!EnableGyroscope ||
                XRDevice.isPresent)
            {
                return;
            }

            if (m_parentCamera == null)
            {
                Input.gyro.enabled = true;
                m_parentCamera = new GameObject();
                Vector3 initPosition = Target.transform.position;
                Target.transform.localPosition = Vector3.zero;
                Target.transform.parent = m_parentCamera.transform;
                m_parentCamera.transform.Rotate(Vector3.right, 90);
                m_parentCamera.transform.position = initPosition;
            }

            if (Target != null)
            {
                Quaternion rotFix = new Quaternion(Input.gyro.attitude.x, Input.gyro.attitude.y, -Input.gyro.attitude.z, -Input.gyro.attitude.w);
                Target.transform.localRotation = rotFix;

                if (Input.GetMouseButton(0))
                {
                    m_parentCamera.transform.position += Target.transform.forward.normalized * SPECTATOR_SPEED * Time.deltaTime;
                }
            }
#endif

        }
    }
}
