// <copyright file="Gaze_HandleEventArgs.cs" company="apelab sàrl">
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
using SpatialStories;
using System;

namespace Gaze
{
    public class S_IODestroyEventArgs : EventArgs
    {
        public object Sender { get; private set; }
        public S_InteractiveObject IO { private set; get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gaze.S_HandleEventArgs"/> class.
        /// </summary>
        /// <param name="_sender">Sender.</param>
        /// <param name="_isColliding">If set to <c>true</c> is colliding.</param>
        public S_IODestroyEventArgs(object _sender, S_InteractiveObject io)
        {
            Sender = _sender;
            IO = io;
        }
    }
}