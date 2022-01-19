using System;
using UnityEngine;

public class S_HandController : MonoBehaviour
{
    public event Action<GameObject> ControllerVisualsInstantiated;

    [SerializeField]
    private Transform ControllerRoot;
    [SerializeField]
    private GameObject m_OculusTouchControllerButtonLayoutPrefab;
    [SerializeField]
    private GameObject m_ViveProControllerButtonLayoutPrefab;
    [SerializeField]
    private GameObject m_ViveFocusPlusControllerButtonLayoutPrefab;
    [SerializeField]
    private GameObject m_WindowsMixedRealityControllerButtonLayoutPrefab;

    public GameObject Controller { get; private set; }

    public bool IsLeftHand;

    private void OnEnable()
    {
        XRRig.Instance.TypeResolved += OnXRRigTypeResolved;
    }

    private void OnDisable()
    {
        XRRig.Instance.TypeResolved -= OnXRRigTypeResolved;
    }

    private void OnXRRigTypeResolved()
    {
        switch (XRRig.Instance.Type)
        {
            case XRRigType.OculusRiftS:
            case XRRigType.OculusQuest:
            case XRRigType.OculusRift:
                Controller = Instantiate(m_OculusTouchControllerButtonLayoutPrefab, ControllerRoot);
                break;
            case XRRigType.VivePro:
                Controller = Instantiate(m_ViveProControllerButtonLayoutPrefab, ControllerRoot);
                break;
            case XRRigType.ViveFocusPlus:
                Controller = Instantiate(m_ViveFocusPlusControllerButtonLayoutPrefab, ControllerRoot);
                break;
            case XRRigType.WindowsMixedReality:
                Controller = Instantiate(m_WindowsMixedRealityControllerButtonLayoutPrefab, ControllerRoot);
                break;
            default:
                break;
        }

        if (Controller == null)
        {
            return;
        }

        ControllerVisualsInstantiated?.Invoke(Controller);
    }
}