using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[SelectionBase]
public class S_RotateBehaviour : S_AbstractBehaviour
{
    public const string EVENT_ROTATEBEHAVIOUR_REQUEST_CURRENT_ROTATION = "EVENT_ROTATEBEHAVIOUR_REQUEST_CURRENT_ROTATION";
    public const string EVENT_ROTATEBEHAVIOUR_UPDATE_CURRENT_ROTATION = "EVENT_ROTATEBEHAVIOUR_UPDATE_CURRENT_ROTATION";

    public List<GameObject> TargetsObject = new List<GameObject>();
    public float Duration;
    public Vector3 TargetRotation = Vector3.zero;
    
    private List<Vector3> m_InitialRotation = new List<Vector3>();
    private Vector3 m_TargetRotation;

    private bool m_Initialized = false;

    private bool m_isListening = false;

    public S_RotateBehaviour(List<GameObject> _targets, float _duration, Vector3 _targetRotation, bool _loop, bool _backAndForth) : base(_targets, _duration)
    {
        TargetsObject = _targets;
        Duration = _duration;
        TargetRotation = _targetRotation;
        m_TargetRotation = TargetRotation;
        Loop = _loop;
        BackAndForth = _backAndForth;
    }

    public void ResetCurrentRotation()
    {
        m_repeated = 0;
    }

    public override void Start()
    {
        Init();
        S_BehaviourManager.Instance.AddBehaviour(this, false);
    }

    public override void Reset()
    {
        base.Reset();

        if (m_InitialRotation != null)
        {
            if (m_InitialRotation.Count > 0)
            {
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
                            m_Targets[i].transform.localEulerAngles = m_InitialRotation[i];
                        }
                    }
                }
            }
        }
    }

    protected override void Init()
    {
        Set(TargetsObject, Duration);

        if (m_InitialRotation == null)
        {
            m_InitialRotation = new List<Vector3>();
            m_TargetRotation = TargetRotation;
        }

        m_InitialRotation.Clear();

        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_InitialRotation.Add(m_Targets[i].transform.localEulerAngles);
        }
    }

    protected override void Update()
    {
        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_Targets[i].transform.Rotate(Time.deltaTime * (m_TargetRotation.x / Duration),
                Time.deltaTime * (m_TargetRotation.y / Duration),
                Time.deltaTime * (m_TargetRotation.z / Duration),
                Space.Self);
        }
    }

    protected override void OnFinish()
    {
        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_Targets[i].transform.localEulerAngles = m_InitialRotation[i];
            m_Targets[i].transform.Rotate(m_TargetRotation.x, m_TargetRotation.y, m_TargetRotation.z, Space.Self);
        }
    }
}
