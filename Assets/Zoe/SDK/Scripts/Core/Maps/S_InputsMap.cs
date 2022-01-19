// <copyright file="Gaze_InputsMap.cs" company="apelab sàrl">
// © apelab. All Rights Reserved.
//
// This source is subject to the apelab license.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2014-06-01</date>
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    [System.Serializable]
    public class S_InputsMap
    {
        [SerializeField]
        public List<S_InputsMapEntry> InputsEntries;
        public bool AreDependenciesSatisfied;
        
        public S_InputsMap()
        {
            InputsEntries = new List<S_InputsMapEntry>();
        }

        public bool Delete(S_InputsMapEntry d)
        {
            if (InputsEntries.Contains(d))
                // Destroy the dependency from the list
                return InputsEntries.Remove(d);

            return false;
        }

        public S_InputsMapEntry Add()
        {
            S_InputsMapEntry d = new S_InputsMapEntry();
            InputsEntries.Add(d);

            return d;
        }

        public bool IsEmpty()
        {
            return InputsEntries.Count == 0;
        }

        public void Reset(bool _reloadDependencies)
        {
            if (_reloadDependencies)
                AreDependenciesSatisfied = false;
        }
    }
}