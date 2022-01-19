using UnityEngine;

public class CameraFollowingCapsuleCollider : MonoBehaviour
{
    public Camera Camera;

    private CapsuleCollider m_CapsuleCollider;

    private void Awake()
    {
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera == null || m_CapsuleCollider == null)
            return;
        Vector3 local = transform.InverseTransformPoint(Camera.transform.position);

        // we reset the y component to zero so that the collider always stays upright
        local.y = m_CapsuleCollider.center.y;
        m_CapsuleCollider.center = local;
    }
}
