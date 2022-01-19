using SpatialStories;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[SelectionBase]
public class S_ScalingBehaviour : S_AbstractBehaviour
{
    public List<GameObject> TargetsObject;
    public float Duration;

    public List<Vector3> InitialScale = new List<Vector3>();
    public List<Vector3> TargetScale = new List<Vector3>();
    public S_ScaleMode ScalingMode;
    public int IndexScalingMode;

    public Vector3 TargetScaleReference;

    private bool m_isFirtRun = true;
    private Vector3 m_InitialTargetScaleReference = Vector3.zero;
    private Vector3 m_backUpTargetScaleReference = Vector3.zero;
    private List<Vector3> m_backUpInitialScale = null;

    public S_ScalingBehaviour(List<GameObject> _targets, float _duration, Vector3 _targetScale, S_ScaleMode _scaleMode = S_ScaleMode.SCALE_ABSOLUTE) : base(_targets, _duration)
    {
        TargetsObject = _targets;
        Duration = _duration;
        TargetScaleReference = _targetScale;
        ScalingMode = _scaleMode;
        m_isFirtRun = true;
    }

    public S_ScalingBehaviour(List<GameObject> _targets, float _duration, float _scaleFactor, S_ScaleMode _scaleMode = S_ScaleMode.SCALE_ABSOLUTE) : base(_targets, _duration)
    {
        TargetsObject = _targets;
        Duration = _duration;
        for (int i = 0; i < TargetsObject.Count; i++)
        {
            TargetScaleReference = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);
        }
        ScalingMode = _scaleMode;
        m_isFirtRun = true;
    }

    public void ResetCurrentScale()
    {
        m_isFirtRun = true;
        m_repeated = 0;
        if (!m_backUpTargetScaleReference.Equals(Vector3.zero))
        {
            TargetScaleReference = m_backUpTargetScaleReference;
        }
        if (m_backUpInitialScale != null)
        {
            if (m_backUpInitialScale.Count > 0)
            {
                if (m_Targets != null)
                {
                    for (int i = 0; i < m_Targets.Count; i++)
                    {
                        m_Targets[i].transform.localScale = m_backUpInitialScale[i];
                    }
                }
            }
        }
    }

    public override void Reset()
    {
        base.Reset();

        if (!m_isFirtRun)
        {
            for (int i = 0; i < m_Targets.Count; i++)
            {
                if (BackAndForth)
                {
                    if (TargetScaleReference.Equals(InitialScale[i]))
                    {
                        TargetScaleReference = m_InitialTargetScaleReference;
                    }
                    else
                    {
                        TargetScaleReference = InitialScale[i];
                    }
                    break;
                }
                else
                {
                    if (Loop)
                    {
                        m_Targets[i].transform.localScale = InitialScale[i];
                        m_isFirtRun = true;
                    }
                }
            }
        }
    }

    protected override void Init()
    {
        Set(TargetsObject, Duration);
        InitialScale = new List<Vector3>();
        for (int i = 0; i < m_Targets.Count; i++)
        {
            InitialScale.Add(m_Targets[i].transform.localScale);
        }

        if (!m_isFirtRun)
        {
            m_InitialTargetScaleReference = TargetScaleReference;
        }

        if (m_backUpInitialScale == null)
        {
            m_backUpTargetScaleReference = TargetScaleReference;
            m_backUpInitialScale = new List<Vector3>();
            for (int i = 0; i < m_Targets.Count; i++)
            {
                m_backUpInitialScale.Add(m_Targets[i].transform.localScale);
            }
        }

        TargetScale = new List<Vector3>();
        for (int i = 0; i < m_Targets.Count; i++)
        {
            if (TargetScaleReference.Equals(Vector3.zero))
            {
                TargetScale.Add(m_Targets[i].transform.localScale);
            }
            else
            {
                TargetScale.Add(TargetScaleReference);
            }
        }

        if (m_isFirtRun &&
            (ScalingMode == S_ScaleMode.SCALE_FACTOR) && (!TargetScaleReference.Equals(Vector3.zero)))
        {
            for (int i = 0; i < m_Targets.Count; i++)
            {
                TargetScale[i] = new Vector3(InitialScale[i].x * TargetScaleReference.x,
                                            InitialScale[i].y * TargetScaleReference.y,
                                            InitialScale[i].z * TargetScaleReference.z);
            }
        }

        m_isFirtRun = false;
    }

    protected override void Update()
    {
        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_Targets[i].transform.localScale = Vector3.Lerp(InitialScale[i], TargetScale[i], m_CompletionRatio);
        }
    }

    protected override void OnFinish()
    {
        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_Targets[i].transform.localScale = TargetScale[i];
        }
    }
}
