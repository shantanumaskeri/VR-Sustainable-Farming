using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("S_AbstractBehaviour")]
public class S_BehaviourManager : MonoBehaviour
{
    // ----------------------------------------------
    // SINGLETON
    // ----------------------------------------------	
    private static S_BehaviourManager _instance;

    public static S_BehaviourManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType(typeof(S_BehaviourManager)) as S_BehaviourManager;
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "S_BehaviourManager";
                    _instance = container.AddComponent(typeof(S_BehaviourManager)) as S_BehaviourManager;
                }
            }
            return _instance;
        }
    }

    private List<S_AbstractBehaviour> m_ActiveBehaviours = new List<S_AbstractBehaviour>();
    
    private void Update()
    {
        for (int i = m_ActiveBehaviours.Count - 1; i >= 0; --i)
            m_ActiveBehaviours[i].Tick();
    }

    private bool CheckTargets(List<GameObject> _source, List<GameObject> _target)
    {
        if (_target == null) return false;
        if (_target.Count == 0) return false;

        for (int i = 0; i < _target.Count; i++)
        {
            if (_source.Contains(_target[i]))
            {
                return true;
            }
        }

        return false;
    }


    private void KillPreviousBehaviourForTarget(S_AbstractBehaviour _behaviour)
    {
        for (int i = 0; i < m_ActiveBehaviours.Count; i++)
        {
            if (CheckTargets(m_ActiveBehaviours[i].Targets, _behaviour.Targets))
            {
                m_ActiveBehaviours[i].Kill();
                i = 0;
            }
        }            
    }

    public void OnFinished(S_AbstractBehaviour _behaviour)
    {
        m_ActiveBehaviours.Remove(_behaviour);
    }

    internal void AddBehaviour(S_AbstractBehaviour _newBehaviour, bool _killPrevious = false)
    {
        KillBehaviourOfSameType(_newBehaviour);
        if (_killPrevious) KillPreviousBehaviourForTarget(_newBehaviour);
        m_ActiveBehaviours.Add(_newBehaviour);
    }

    internal void KillBehaviourOfSameType(S_AbstractBehaviour newBehaviour)
    {
        foreach (S_AbstractBehaviour behaviour in m_ActiveBehaviours)
        {
            if (behaviour.GetType() == newBehaviour.GetType() &&
                CheckTargets(behaviour.Targets, newBehaviour.Targets))
            {
                behaviour.Kill();

                break;
            }
        }
    }

    internal void RemoveBehaviour(S_AbstractBehaviour _newBehaviour)
    {
        m_ActiveBehaviours.Remove(_newBehaviour);
    }

    public void Clear()
    {
        m_ActiveBehaviours.Clear();
    }

}
