﻿//-----------------------------------------------------------------------
// <copyright file="Gaze_ControllerCollisionEventArgs.cs" company="apelab sàrl">
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

public class S_ControllerCollisionEventArgs : EventArgs
{
    public GameObject Sender { get; private set; }
    public GameObject Other { get; private set; }
    public S_CollisionTypes CollisionType { get; private set; }
    public S_GrabManager GrabManger { get; private set; }

    public S_ControllerCollisionEventArgs(GameObject _sender, GameObject _other, S_CollisionTypes _collisionType, S_GrabManager _grabManager)
    {
        Sender = _sender;
        Other = _other;
        CollisionType = _collisionType;
        GrabManger = _grabManager;
    }
}