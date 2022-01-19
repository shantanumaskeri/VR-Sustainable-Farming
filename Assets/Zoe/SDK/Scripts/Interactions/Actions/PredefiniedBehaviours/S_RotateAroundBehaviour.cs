using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[SelectionBase]
public class S_RotateAroundBehaviour : S_AbstractBehaviour
{
    public List<GameObject> TargetsObject = new List<GameObject>();
    public Transform PivotObject;
    public float Duration;
    public Vector3 TargetRotation = Vector3.zero;
    private Vector3 m_TargetRotation;

    private Vector3 m_CurrentRotationIncrement;

    private Vector3 m_PreviousPivotPosition;
    private List<Vector3> m_InitialDisplacements;

    public S_RotateAroundBehaviour(List<GameObject> _targets, GameObject _pivot, float _duration, Vector3 _targetRotation, bool _loop) : base(_targets, _duration)
    {
        TargetsObject = _targets;
        PivotObject = _pivot.transform;
        Duration = _duration;
        TargetRotation = _targetRotation;
        m_TargetRotation = TargetRotation;
        Loop = _loop;
    }

    public override void Start()
    {
        Init();
        S_BehaviourManager.Instance.AddBehaviour(this, false);
    }

    public void ResetCurrentRotation()
    {
        m_repeated = 0;
    }

    public override void Reset()
    {
        base.Reset();

        for (int i = 0; i < m_Targets.Count; i++)
        {
            if (BackAndForth)
            {
                m_TargetRotation = -m_TargetRotation;
            }
            else
            {
                if (Loop)
                {
                    m_Targets[i].transform.position = PivotObject.position + m_InitialDisplacements[i];
                }
            }
        }
    }

    protected override void Init()
    {
        if (m_InitialDisplacements == null)
        {
            m_InitialDisplacements = new List<Vector3>();
            m_TargetRotation = TargetRotation;
        }

        Set(TargetsObject, Duration);
        m_CurrentRotationIncrement = Vector3.zero;

        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_InitialDisplacements.Add(m_Targets[i].transform.position - PivotObject.position);
        }

        if (m_PreviousPivotPosition == Vector3.zero)
        {
            m_PreviousPivotPosition = PivotObject.position;
        }
    }

    protected override void Update()
    {
        Vector3 pivotDisplacement = PivotObject.position - m_PreviousPivotPosition;
        m_PreviousPivotPosition = PivotObject.position;

        for (int i = 0; i < m_Targets.Count; i++)
        {
            // It is important that we apply the translation first
            // otherwise the rotation will be wrong
            Vector3 localRotationBeforeRotateAround = Targets[i].transform.localEulerAngles;
            m_Targets[i].transform.position += pivotDisplacement;

            if (Mathf.Abs(m_TargetRotation.x) > 0)
            {
                float angle = Time.deltaTime * (m_TargetRotation.x / Duration);
                m_CurrentRotationIncrement.x += angle;
                m_Targets[i].transform.RotateAround(PivotObject.position, Vector3.right, angle);
            }

            if (Mathf.Abs(m_TargetRotation.y) > 0)
            {
                float angle = Time.deltaTime * (m_TargetRotation.y / Duration);
                m_CurrentRotationIncrement.y += angle;
                m_Targets[i].transform.RotateAround(PivotObject.position, Vector3.up, angle);
            }

            if (Mathf.Abs(m_TargetRotation.z) > 0)
            {
                float angle = Time.deltaTime * (m_TargetRotation.z / Duration);
                m_CurrentRotationIncrement.z += angle;
                m_Targets[i].transform.RotateAround(PivotObject.position, Vector3.forward, angle);
            }

            // For some reason, Transform.RotateAround makes the rotating transform
            // face the pivot point, for now we simply force back the initial rotation.
            Targets[i].transform.localEulerAngles = localRotationBeforeRotateAround;
        }
    }

    protected override void OnFinish()
    {
        Vector3 pivotDisplacement = PivotObject.position - m_PreviousPivotPosition;
        m_PreviousPivotPosition = PivotObject.position;

        for (int i = 0; i < m_Targets.Count; i++)
        {
            // It is important that we apply the translation first
            // otherwise the rotation will be wrong
            m_Targets[i].transform.position += pivotDisplacement;

            if (Mathf.Abs(m_TargetRotation.x) > 0)
            {
                float angle = m_TargetRotation.x - m_CurrentRotationIncrement.x;
                m_Targets[i].transform.RotateAround(PivotObject.position, Vector3.right, angle);
            }

            if (Mathf.Abs(m_TargetRotation.y) > 0)
            {
                float angle = m_TargetRotation.y - m_CurrentRotationIncrement.y;
                m_Targets[i].transform.RotateAround(PivotObject.position, Vector3.up, angle);
            }

            if (Mathf.Abs(m_TargetRotation.z) > 0)
            {
                float angle = m_TargetRotation.z - m_CurrentRotationIncrement.z;
                m_Targets[i].transform.RotateAround(PivotObject.position, Vector3.forward, angle);
            }
        }
    }
}
