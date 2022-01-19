using System;
using System.Collections.Generic;
using UnityEngine.XR;

public class XRRig
{
    private const string m_OculusRiftSName = "Rift S";
    private const string m_OculusRiftName = "Rift";
    private const string m_OculusQuestName = "Quest";
    private const string m_HTCViveProName = "VIVE_Pro";
    private const string m_WindowsMixedRealityName = "WindowsMR";

    private event Action m_TypeResolved;
    public event Action TypeResolved
    {
        add
        {
            m_TypeResolved += value;

            if (Type != XRRigType.Undefined)
            {
                value();
            }
        }

        remove
        {
            m_TypeResolved -= value;
        }
    }

    private static XRRig m_Instance;
    public static XRRig Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new XRRig();
            }

            return m_Instance;
        }
    }

    public XRRigType Type { get; private set; }
    public InputDevice LeftController { get; private set; }
    public InputDevice RightController { get; private set; }
    public InputDevice HeadMount { get; private set; }

    private bool m_Started = false;

    private XRRig()
    {
    }

    public void StartUp()
    {
        if (m_Started)
        {
            return;
        }

        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, inputDevices);

        if (inputDevices.Count != 0)
        {
            HeadMount = inputDevices[0];

            ResolveRigType(HeadMount.name);
        }

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, inputDevices);

        if (inputDevices.Count != 0)
        {
            LeftController = inputDevices[0];
        }

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, inputDevices);

        if (inputDevices.Count != 0)
        {
            RightController = inputDevices[0];
        }

        InputDevices.deviceConnected += OnDeviceConnected;
        InputDevices.deviceDisconnected += OnDeviceDisconnected;
    }

    public void ShutDown()
    {
        if (!m_Started)
        {
            return;
        }

        InputDevices.deviceConnected -= OnDeviceConnected;
        InputDevices.deviceDisconnected -= OnDeviceDisconnected;
    }

    private void ResolveRigType(string _headMountName)
    {
        if (_headMountName.Contains(m_OculusRiftSName))
        {
            Type = XRRigType.OculusRiftS;
        }
        else if (_headMountName.Contains(m_OculusRiftName))
        {
            Type = XRRigType.OculusRift;
        }
        else if (_headMountName.Contains(m_OculusQuestName))
        {
            Type = XRRigType.OculusQuest;
        }
        else if (_headMountName.Contains(m_HTCViveProName))
        {
            Type = XRRigType.VivePro;
        }
        else if (_headMountName.Contains(m_WindowsMixedRealityName))
        {
            Type = XRRigType.WindowsMixedReality;
        }
        else
        {
            // Windows Mixed Reality through SteamVR
            // has the headset model (Acer, Samsung, etc...)
            // Therefore, no mention of WindowsMixedReality.
            // In order to avoid having a string for every single one
            // of the headsets, we simply assume any other headsets
            // is WindowsMixedReality.
            Type = XRRigType.WindowsMixedReality;
        }

        m_TypeResolved?.Invoke();
    }

    private void OnDeviceConnected(InputDevice _inputDevice)
    {
        if ((_inputDevice.characteristics & InputDeviceCharacteristics.Controller) != 0 &&
            (_inputDevice.characteristics & InputDeviceCharacteristics.Left) != 0)
        {
            LeftController = _inputDevice;

            return;
        }

        if ((_inputDevice.characteristics & InputDeviceCharacteristics.Controller) != 0 &&
            (_inputDevice.characteristics & InputDeviceCharacteristics.Right) != 0)
        {
            RightController = _inputDevice;

            return;
        }

        if ((_inputDevice.characteristics & InputDeviceCharacteristics.HeadMounted) != 0)
        {
            HeadMount = _inputDevice;

            ResolveRigType(HeadMount.name);
        }
    }

    private void OnDeviceDisconnected(InputDevice _inputDevice)
    {
        if ((_inputDevice.characteristics & InputDeviceCharacteristics.Controller) != 0 &&
            (_inputDevice.characteristics & InputDeviceCharacteristics.Left) != 0)
        {
            LeftController = _inputDevice;

            return;
        }

        if ((_inputDevice.characteristics & InputDeviceCharacteristics.Controller) != 0 &&
            (_inputDevice.characteristics & InputDeviceCharacteristics.Right) != 0)
        {
            RightController = _inputDevice;

            return;
        }

        if ((_inputDevice.characteristics & InputDeviceCharacteristics.HeadMounted) != 0)
        {
            HeadMount = _inputDevice;
        }
    }
}
