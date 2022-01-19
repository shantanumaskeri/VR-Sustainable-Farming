//-----------------------------------------------------------------------
// <copyright file="LevitationEventArgs.cs" company="apelab sàrl">
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
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2016-01-25</date>
// </copyright>
//-----------------------------------------------------------------------
using SpatialStories;
using System;
using UnityEngine;

namespace Gaze
{
    public class S_LevitationEventArgs : EventArgs
    {
        public object Sender;
        public GameObject ObjectToLevitate;
        public S_LevitationTypes Type;
        public S_HandsEnum Hand;

        public S_LevitationEventArgs(object _sender, GameObject _objectToLevitate, S_LevitationTypes _type, S_HandsEnum _actualHand)
        {
            Sender = _sender;
            ObjectToLevitate = _objectToLevitate;
            Type = _type;
            Hand = _actualHand;
        }
    }
}