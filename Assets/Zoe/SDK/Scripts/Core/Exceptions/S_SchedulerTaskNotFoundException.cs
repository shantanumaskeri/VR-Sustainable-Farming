using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public class S_SchedulerTaskNotFoundException : Exception
    {
        public S_SchedulerTaskNotFoundException(long _taskID) :
            base(string.Format("S_Exception: The requested task with id {0} was not found", _taskID))
        {

        }
    }
}
