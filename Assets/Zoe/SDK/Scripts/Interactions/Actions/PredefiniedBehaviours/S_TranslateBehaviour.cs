using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[SelectionBase]
public class S_TranslateBehaviour : S_AbstractBehaviour
{
    public List<GameObject> TargetsObject = new List<GameObject>();
    public float Duration;
    public float ParabolicHeight = 0;
    public int Flips = 0;
    public float Offset = 0;
    public bool InterpolateRotation = false;

    public bool Random = true;
    public List<Transform> TargetsPosition = new List<Transform>();

    private List<bool> m_started;

    private List<Vector3> m_startPosition;
    private List<Vector3> m_startRotation;
    private List<Vector3> m_endRotation;

    private List<float> m_startTime;
    private List<float> m_endTime;
    private List<float> m_remainingTime;
    private List<int> m_currentTarget;
    private List<int> m_totalIterations;
    private List<float> m_durationIteration;

    private List<List<int>> m_targetsToVisit;

    public S_TranslateBehaviour(List<GameObject> _targets, float _duration, Transform _targetPosition, float _parabolicHeight, bool _interpolateRotation, int _flips, float _offset) : base(_targets, _duration)
    {
        TargetsObject = _targets;
        Duration = _duration;
        TargetsPosition.Add(_targetPosition);
        ParabolicHeight = _parabolicHeight;
        Flips = _flips;
        Offset = _offset;
        InterpolateRotation = _interpolateRotation;
    }

    public S_TranslateBehaviour(List<GameObject> _targets, float _duration, List<Transform> _targetsPosition, float _parabolicHeight, bool _interpolateRotation, int _flips, float _offset) : base(_targets, _duration)
    {
        TargetsObject = _targets;
        Duration = _duration;
        for (int i = 0; i < _targetsPosition.Count; i++)
        {
            TargetsPosition.Add(_targetsPosition[i]);
        }        
        ParabolicHeight = _parabolicHeight;
        Flips = _flips;
        Offset = _offset;
        InterpolateRotation = _interpolateRotation;
    }

    protected override void Init()
    {
        Set(TargetsObject, Duration);
        m_started = new List<bool>();

        m_startPosition = new List<Vector3>();
        m_startRotation = new List<Vector3>();
        m_endRotation = new List<Vector3>();

        m_startTime = new List<float>();
        m_endTime = new List<float>();
        m_remainingTime = new List<float>();
        m_currentTarget = new List<int>();
        m_totalIterations = new List<int>();
        m_durationIteration = new List<float>();

        m_targetsToVisit = new List<List<int>>();

        for (int i = 0; i < m_Targets.Count; i++)
        {
            m_currentTarget.Add(-1);
            m_totalIterations.Add(0);
            m_started.Add(false);

            m_startPosition.Add(Vector3.zero);
            m_startRotation.Add(Vector3.zero);
            m_endRotation.Add(Vector3.zero);

            m_startTime.Add(0);
            m_endTime.Add(0);
            m_remainingTime.Add(0);
            m_durationIteration.Add(0);

            List<int> targetsToVisit = new List<int>();
            for (int k = 0; k < TargetsPosition.Count; k++)
            {
                targetsToVisit.Add(k);
            }
            m_targetsToVisit.Add(targetsToVisit);
        }
    }

    protected override void Update()
    {
        for (int i = 0; i < m_Targets.Count; i++)
        {
            if (!m_started[i])
            {
                m_totalIterations[i]++;
                if (m_totalIterations[i] > TargetsPosition.Count) return;

                if (!Random)
                {
                    m_currentTarget[i] = m_currentTarget[i] + 1;
                }
                else
                {
                    int indexTarget = UnityEngine.Random.Range(0, m_targetsToVisit[i].Count);
                    m_currentTarget[i] = m_targetsToVisit[i][indexTarget];
                    m_targetsToVisit[i].RemoveAt(indexTarget);
                }

                m_started[i] = true;

                m_startPosition[i] = m_Targets[i].transform.position;
                m_startRotation[i] = m_Targets[i].transform.rotation.eulerAngles;
                if (!InterpolateRotation)
                {
                    m_endRotation[i] = m_Targets[i].transform.rotation.eulerAngles + Vector3.forward * 360 * Flips;
                }
                else
                {
                    m_endRotation[i] = TargetsPosition[m_currentTarget[i]].transform.rotation.eulerAngles;
                }

                m_startTime[i] = Time.time;
                m_durationIteration[i] = (Duration / TargetsPosition.Count);
                m_endTime[i] = Time.time + m_durationIteration[i];
                m_remainingTime[i] = m_endTime[i] - m_startTime[i];
            }

            if (m_remainingTime[i] < 0)
            {
                m_started[i] = false;
                return;
            }

            m_remainingTime[i] -= Time.deltaTime;
            if (Offset > 0)
            {
                if (Vector3.Distance(m_Targets[i].transform.position, TargetsPosition[m_currentTarget[i]].position) < Offset)
                {
                    m_remainingTime[i] = -1;
                }
            }
            if (m_remainingTime[i] > 0)
            {
                float t = 1 - (m_remainingTime[i] / m_durationIteration[i]);
                var height = Mathf.Sin(Mathf.PI * t) * ParabolicHeight;
                m_Targets[i].transform.position = Vector3.Lerp(m_startPosition[i], TargetsPosition[m_currentTarget[i]].position, t) + Vector3.up * height;
                m_Targets[i].transform.rotation = Quaternion.Euler(Vector3.Lerp(m_startRotation[i], m_endRotation[i], t));
            }
            else
            {
                if (Offset == 0)
                {
                    m_Targets[i].transform.position = TargetsPosition[m_currentTarget[i]].position;
                    m_Targets[i].transform.rotation = Quaternion.Euler(m_endRotation[i]);
                }
            }
        }
    }

    protected override void OnFinish()
    {
        if (Offset == 0)
        {
            for (int i = 0; i < m_Targets.Count; i++)
            {
                m_Targets[i].transform.position = TargetsPosition[m_currentTarget[i]].position;
                m_Targets[i].transform.rotation = Quaternion.Euler(m_endRotation[i]);
            }
        }
    }
}
