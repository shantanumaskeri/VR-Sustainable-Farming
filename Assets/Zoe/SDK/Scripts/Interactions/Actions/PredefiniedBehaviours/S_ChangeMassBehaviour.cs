using System.Collections.Generic;
using UnityEngine;

public class S_ChangeMassBehaviour : S_AbstractBehaviour
{
    private List<float> m_InitialMass = new List<float>();
    private float m_TargetMass;
    private bool m_ResetInitialMass;

    public S_ChangeMassBehaviour(List<GameObject> _targets, float _duration, float _targetMass, bool _resetInitialMass) : base(_targets, _duration)
    {
        m_TargetMass = _targetMass;
        m_ResetInitialMass = _resetInitialMass;
    }

    protected override void Init()
    {
        m_InitialMass = new List<float>();
        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_InitialMass.Add(m_Targets[i].GetComponent<Rigidbody>().mass);
            m_Targets[i].GetComponent<Rigidbody>().mass = m_TargetMass;
        }
    }

    protected override void Update()
    {
    }

    protected override void OnFinish()
    {
        if (m_ResetInitialMass)
        {
            for (int i = 0; i < m_Targets.Count; i++)
            {
                m_Targets[i].GetComponent<Rigidbody>().mass = m_InitialMass[i];
            }
        }
    }

}
