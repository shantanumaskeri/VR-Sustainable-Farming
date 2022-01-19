// <copyright file="S_ReloadLimit.cs" company="apelab sàrl">
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
// <date>2015-07-07</date>
namespace SpatialStories
{
    /// <summary>
    /// Number of times the trigger can be reloaded<br>
    ///     INFINITE = can be reloaded infinitely while not expired
    ///     FINITE = the number of reload is limited to the specified number
    /// </summary>
    public enum S_ReloadLimit
    {
        INFINITE,
        FINITE
    }
}