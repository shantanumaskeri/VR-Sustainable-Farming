using System;
using UnityEngine;

namespace SpatialStories
{
    public class S_SchedulerTask
    {
        public Action ActionToPerform;
        public Action FrameActionToPerform;
        public long TaskID;
        public bool IsAlive = true;
        public float TimeOut;
        public float TotalDelay;

        private S_SchedulerTaskType m_TaskType;
        private bool m_reset;
        
        /// <summary>
        /// If a task gets interrupted it will store how much time had before firing
        /// (Usefull when reescheduling it)
        /// </summary>
        private float m_RemainingTimeOnInterruption = 0;

        public S_SchedulerTask(Action _action, S_SchedulerTaskType _type, float _timeOut, bool _reset, float _totalDelay = 0, Action _frameAction = null)
        {
            m_TaskType = _type;
            ActionToPerform = _action;
            TaskID = S_Scheduler.GlobalTaskCounter++;
            TimeOut = _timeOut;
            m_reset = _reset;
            TotalDelay = _totalDelay;
            FrameActionToPerform = _frameAction;
        }
        
        public void Interrupt()
        {
            if (m_reset)
            {
                IsAlive = false;
                m_RemainingTimeOnInterruption = TotalDelay;
            }
            else
            {
                m_RemainingTimeOnInterruption = GetRemainingTime();
            }
        }

        public S_SchedulerTask ReSchedule(float _delay = -1)
        {
            if(_delay == -1)
            {
                _delay = m_RemainingTimeOnInterruption;
            }

            if(m_TaskType == S_SchedulerTaskType.NEXT_FRAME)
            {
                return S_Scheduler.AddTaskAtNextFrame(ActionToPerform);
            }
            else
            {
                return S_Scheduler.AddTask(_delay, m_reset, ActionToPerform);
            }
        }

        private float GetRemainingTime()
        {
            return TimeOut - Time.time;
        }
    }
}

