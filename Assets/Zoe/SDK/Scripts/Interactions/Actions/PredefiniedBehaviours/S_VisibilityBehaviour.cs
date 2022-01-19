using System.Collections.Generic;
using UnityEngine;

public class S_VisibilityBehaviour : S_AbstractBehaviour
{
    private bool m_SetVisible;

    public S_VisibilityBehaviour(List<GameObject> _targets, float _duration, bool _setVisible) : base(_targets, _duration)
    {
        m_SetVisible = _setVisible;
    }

    protected override void Init()
    {
    }

    protected override void Update()
    {
    }

    protected override void OnFinish()
    {
        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_Targets[i].SetActive(m_SetVisible);
        }
    }
}
