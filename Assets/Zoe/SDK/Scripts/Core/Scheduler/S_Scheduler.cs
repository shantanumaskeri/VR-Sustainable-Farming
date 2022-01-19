using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public class S_Scheduler : MonoBehaviour
    {
        /// <summary>
        /// Used to assing tasks IDs
        /// </summary>
        public static long GlobalTaskCounter = 0;

        private static List<S_SchedulerTask> m_ActionsToPerform = new List<S_SchedulerTask>();
        private static List<S_SchedulerTask> m_ActionsToPerformAtNextFrame = new List<S_SchedulerTask>();

        private static List<Action> m_MethodsToCallOnUpdate = new List<Action>();
        private static List<Action> m_MethodsToCallOnFixedUpdate = new List<Action>();
        private static List<Action> m_MethodsToCallOnLateUpdate = new List<Action>();

        public static void Clear()
        {
            GlobalTaskCounter = 0;
            m_ActionsToPerform.Clear();
            m_ActionsToPerformAtNextFrame.Clear();
            m_MethodsToCallOnUpdate.Clear();
            m_MethodsToCallOnFixedUpdate.Clear();
            m_MethodsToCallOnLateUpdate.Clear();
        }

        public static S_SchedulerTask AddTask(float _delay, bool _reset, Action _actionToPerform, Action _frameAction = null)
        {
            float taskTimeOut = Time.time + _delay;
            S_SchedulerTask task = new S_SchedulerTask(_actionToPerform, S_SchedulerTaskType.NORMAL, taskTimeOut, _reset, _delay, _frameAction);
            m_ActionsToPerform.Add(task);
            return task;
        }

        public static bool RemoveTask(Action _action)
        {
            foreach (S_SchedulerTask item in m_ActionsToPerform)
            {
                if (item.ActionToPerform == _action)
                {
                    m_ActionsToPerform.Remove(item);
                    return true;
                }
            }
            return false;
        }

        public static S_SchedulerTask AddTaskAtNextFrame(Action _actionToPerform)
        {
            S_SchedulerTask task = new S_SchedulerTask(_actionToPerform, S_SchedulerTaskType.NEXT_FRAME, Time.time, false);
            m_ActionsToPerformAtNextFrame.Add(task);
            return task;
        }

        public static S_SchedulerTask AddUniqueTask(float _delay, Action _actionToPerform)
        {
            if (CheckIfActionIsUniqueInList(_actionToPerform, m_ActionsToPerform))
            {
                return AddTask(_delay, false, _actionToPerform);
            }
            return null;
        }

        public static S_SchedulerTask AddUniqueTaskAtNextFrame(Action _actionToPerform)
        {
            if (CheckIfActionIsUniqueInList(_actionToPerform, m_ActionsToPerformAtNextFrame))
            {
                return AddTaskAtNextFrame(_actionToPerform);
            }
            return null;
        }

        public static void AddMethodToExecuteEachUpdate(Action _action)
        {
            if (!m_MethodsToCallOnUpdate.Contains(_action))
                m_MethodsToCallOnUpdate.Add(_action);
        }

        public static void RemoveMethodToExecuteEachUpdate(Action _action)
        {
            if (m_MethodsToCallOnUpdate.Contains(_action))
                m_MethodsToCallOnUpdate.Remove(_action);
        }

        public static void AddMethodToExecuteEachLateUpdate(Action _action)
        {
            if (!m_MethodsToCallOnLateUpdate.Contains(_action))
                m_MethodsToCallOnLateUpdate.Add(_action);
        }

        public static void RemoveMethodToExecuteEachLateUpdate(Action _action)
        {
            if (m_MethodsToCallOnLateUpdate.Contains(_action))
                m_MethodsToCallOnLateUpdate.Remove(_action);
        }

        public static void AddMethodToExecuteEachFixedUpdate(Action _action)
        {
            if (!m_MethodsToCallOnFixedUpdate.Contains(_action))
                m_MethodsToCallOnFixedUpdate.Add(_action);
        }

        public static void RemoveMethodToExecuteEachFixedUpdate(Action _action)
        {
            if (m_MethodsToCallOnFixedUpdate.Contains(_action))
                m_MethodsToCallOnFixedUpdate.Remove(_action);
        }

        public static bool CheckIfActionIsUniqueInList(Action _action, List<S_SchedulerTask> _list)
        {
            int counter = _list.Count;
            for (int i = 0; i < counter; i++)
            {
                Action act = _list[i].ActionToPerform;
                if (act.Target == _action.Target && _action.Method == _action.Method)
                {
                    return false;
                }
            }
            return true;
        }

        private void Update()
        {
            // Execute all the tasks scheduled for this frame
            ExecuteNextFrameRequests();

            // start from the end of the list to be able to delete elements (without null pointer exceptions)
            for (int i = m_ActionsToPerform.Count - 1; i >= 0; i--)
            {
                // Get the task
                S_SchedulerTask task = m_ActionsToPerform[i];

                if(!task.IsAlive)
                {
                    m_ActionsToPerform.RemoveAt(i);
                }
                // if the delay for this task is finished, it's time to execute it !
                else if(Time.time >= task.TimeOut)
                {
                    // Excecute the acttion
                    task.ActionToPerform();

                    // Set the task as dead
                    task.IsAlive = false;

                    // remove the last action in the list (the one we executed above)
                    m_ActionsToPerform.RemoveAt(i);
                }
                else
                {
                    if (task.FrameActionToPerform != null)
                    {
                        task.FrameActionToPerform();
                    }
                }
            }

            // Execute all the each grame tasks
            ExecuteMethodList(m_MethodsToCallOnUpdate);
        }

        private void FixedUpdate()
        {
            ExecuteMethodList(m_MethodsToCallOnFixedUpdate);
        }

        private void LateUpdate()
        {
            ExecuteMethodList(m_MethodsToCallOnLateUpdate);
        }

        public void ExecuteNextFrameRequests()
        {
            int counter = m_ActionsToPerformAtNextFrame.Count;
            S_SchedulerTask task; 
            for (int i = counter - 1; i >= 0; i--)
            {
                task = m_ActionsToPerformAtNextFrame[i];
                if(task.IsAlive)
                {
                    task.ActionToPerform.Invoke();
                    task.IsAlive = false;
                }
                m_ActionsToPerformAtNextFrame.RemoveAt(i);

            }
        }

        public static void ExecuteMethodList(List<Action> _list)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
                _list[i].Invoke();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
