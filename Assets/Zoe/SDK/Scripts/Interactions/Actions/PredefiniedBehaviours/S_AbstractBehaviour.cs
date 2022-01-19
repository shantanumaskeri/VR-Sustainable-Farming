using System.Collections.Generic;
using UnityEngine;

public abstract class S_AbstractBehaviour
{
    public bool Loop = false;
    public bool BackAndForth = false;

    protected float m_CompletionRatio { get { return 1 - m_RemainingTime / m_Duration; } }

    protected float m_InitialDuration;
    protected float m_InitialRemainingTime;
    protected int m_repeated = 0;

    protected float m_Duration;
    protected float m_RemainingTime;
    protected List<GameObject> m_Targets = new List<GameObject>();

    protected abstract void Init();
    protected abstract void Update();
    protected abstract void OnFinish();

    public List<GameObject> Targets
    {
        get { return m_Targets; }
    }

    public S_AbstractBehaviour(List<GameObject> _targets, float _duration)
    {
        Set(_targets, _duration);
    }

    public void Set(List<GameObject> _targets, float _duration)
    {
        m_Targets = _targets;

        m_InitialDuration = _duration;
        m_InitialRemainingTime = _duration;

        ResetTimeout();
    }

    public virtual void Reset()
    {
        ResetTimeout();
    }

    private void ResetTimeout()
    {
        m_RemainingTime = m_InitialRemainingTime;
        m_Duration = m_InitialDuration;
    }

    public virtual void Start()
    {
        Init();
        S_BehaviourManager.Instance.AddBehaviour(this);
    }

    public void Kill()
    {
        S_BehaviourManager.Instance.RemoveBehaviour(this);
    }

    public void Tick()
    {
        if (m_RemainingTime > 0)
        {
            m_RemainingTime -= Time.deltaTime;
            Update();
            if (m_RemainingTime <= 0)
            {
                bool repeat = false;
                if (Loop) repeat = true;
                if (BackAndForth && !Loop && (m_repeated<1)) repeat = true;
                if (repeat)
                {
                    m_repeated++;
                    TimeOut();
                    Reset();
                    Start();
                }
            }
        }
        else
        {
            TimeOut();
        }            
    }

    protected void TimeOut()
    {
        OnFinish();
        S_BehaviourManager.Instance.OnFinished(this);
    }    
}
