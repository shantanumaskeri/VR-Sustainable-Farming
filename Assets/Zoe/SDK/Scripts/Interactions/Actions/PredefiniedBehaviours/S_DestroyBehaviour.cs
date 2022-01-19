using System.Collections.Generic;
using UnityEngine;

public class S_DestroyBehaviour : S_AbstractBehaviour
{
    public S_DestroyBehaviour(List<GameObject> _targets, float _duration) : base(_targets, _duration)
    {
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
            GameObject.Destroy(m_Targets[i]);
        }
        m_Targets.Clear();
    }
}
