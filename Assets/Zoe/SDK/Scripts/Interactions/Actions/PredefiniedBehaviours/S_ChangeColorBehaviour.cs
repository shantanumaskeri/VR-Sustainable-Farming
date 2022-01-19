using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[SelectionBase]
public class S_ChangeColorBehaviour : S_AbstractBehaviour
{
    public List<GameObject> TargetsObject = new List<GameObject>();
    public float Duration;
    public Material TargetMaterial;
    public Color TargetColor;

    private List<Color> m_InitialColor = new List<Color>();
    private List<Color> m_TargetColor = new List<Color>();
    private List<Color> m_EndColor = new List<Color>();

    private List<Color> m_CurrentColor = new List<Color>();

    private Color m_backupTargetColor;

    public S_ChangeColorBehaviour(List<GameObject> _targets, Color _targetColor, float _duration, bool _loop, bool _backAndForth) : base(_targets, _duration)
    {
        TargetColor = _targetColor;
        m_backupTargetColor = _targetColor;
        Duration = _duration;
        Loop = _loop;
        BackAndForth = _backAndForth;
    }

    public override void Start()
    {
        Init();
        S_BehaviourManager.Instance.AddBehaviour(this, true);
    }

    public void ResetCurrentColor()
    {
        m_repeated = 0;
        m_CurrentColor = null;
    }

    public override void Reset()
    {
        base.Reset();

        if (m_InitialColor != null)
        {
            if (m_InitialColor.Count > 0)
            {
                for (int i = 0; i < m_Targets.Count; i++)
                {
                    if (BackAndForth)
                    {
                        if (m_CurrentColor[i].Equals(m_backupTargetColor))
                        {
                            m_TargetColor[i] = m_InitialColor[i];
                        }
                        else
                        {
                            m_TargetColor[i] = m_backupTargetColor;
                        }
                    }
                    else
                    {
                        if (Loop)
                        {
                            TargetColor = m_backupTargetColor;
                            m_Targets[i].transform.GetComponentInChildren<Renderer>().material.color = m_InitialColor[i];
                            m_CurrentColor = null;
                        }
                    }
                }
            }
        }
    }

    protected override void Init()
    {
        Set(TargetsObject, Duration);
        bool firstRun = (m_CurrentColor == null);
        if (firstRun)
        {
            m_CurrentColor = new List<Color>();
            m_InitialColor = new List<Color>();
            m_TargetColor = new List<Color>();
            m_EndColor = new List<Color>();
        }
        else
        {
            m_EndColor = new List<Color>();
        }
        for (int i = 0; i < m_Targets.Count; i++)
        {
            if (firstRun)
            {
                m_InitialColor.Add(m_Targets[i].transform.GetComponentInChildren<Renderer>().material.color);
                m_CurrentColor.Add(m_Targets[i].transform.GetComponentInChildren<Renderer>().material.color);
                m_TargetColor.Add(TargetColor);
                m_backupTargetColor = TargetColor;
            }
            else
            {
                m_InitialColor[i] = m_CurrentColor[i];
            }
            m_EndColor.Add(m_TargetColor[i]);
        }
    }

    protected override void Update()
    { 
        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_Targets[i].transform.GetComponentInChildren<Renderer>().material.color = Color.Lerp(m_InitialColor[i], m_EndColor[i], m_CompletionRatio);
        }
    }

    protected override void OnFinish()
    {
        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_CurrentColor[i] = m_EndColor[i];
            m_Targets[i].transform.GetComponentInChildren<Renderer>().material.color = m_EndColor[i];
        }
    }
}
