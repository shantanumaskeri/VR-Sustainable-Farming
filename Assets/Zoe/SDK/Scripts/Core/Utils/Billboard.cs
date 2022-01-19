using UnityEngine;

/// <summary>
/// Billboard makes the transform object facing the camera
/// </summary>
public class Billboard : MonoBehaviour
{
    public float Damping = 1f;
    private Camera m_TargetCamera;
    
    void Start()
    {
        m_TargetCamera = Camera.main;
    }

    void Update()
    {
        if(m_TargetCamera == null)
            m_TargetCamera = Camera.main;


        Vector3 lookPos = m_TargetCamera.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * Damping);
    }
}