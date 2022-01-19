// <copyright file="Gaze_InputsMapEntry.cs" company="apelab sàrl">
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

using Gaze;
using SpatialStories;
using System;
using UnityEngine;

[System.Serializable]
public class S_InputsMapEntry : IComparable
{
    /// <summary>
    /// UI vars used to make the InputCondition Editor more friendly
    /// </summary>
    public S_Controllers UISelectedPlatform;
    public int UIControllerSpecificInput;

    // input name
    public S_InputTypes InputType;
    public S_InputTypes OppositeType;

    public S_SingleHandsEnum UIHandSelected;
    public S_BaseInputTypes UIBaseInputType;
    public S_DirectionTypes UIDirectionType;
    public S_StickDirections UIStickDirection;

    public S_SingleHandsEnum HandSelected;
    public S_BaseInputTypes BaseInputType;
    public S_DirectionTypes DirectionType;
    public S_StickDirections StickDirection;

    public bool IsRelease = false;

    // has the input been used by the user
    public bool IsValid;

    public S_InputsMapEntry()
    {
    }

    public S_InputsMapEntry(S_InputTypes _type)
    {
        InputType = _type;
    }

    public void Set(S_SingleHandsEnum _handSelected, S_BaseInputTypes _baseInputType, S_DirectionTypes _directionType, S_StickDirections _stickDirections)
    {
        HandSelected = _handSelected;
        BaseInputType = _baseInputType;
        DirectionType = _directionType;
        StickDirection = _stickDirections;

        // Debug.LogError("HandSelected="+ HandSelected.ToString());
        // Debug.LogError("BaseInputType=" + BaseInputType.ToString());
        // Debug.LogError("DirectionType=" + DirectionType.ToString());

        switch (HandSelected)
        {
            //////////////////////////////////////////////////////////////////////
            case S_SingleHandsEnum.LEFT:
                switch (BaseInputType)
                {
                    case S_BaseInputTypes.START_BUTTON:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED:
                            case S_DirectionTypes.TOUCHED:
                                InputType = S_InputTypes.MENU_BUTTON;
                                break;

                            case S_DirectionTypes.STAY:
                                break;

                            case S_DirectionTypes.RELEASED:
                            case S_DirectionTypes.UNTOUCHED:
                                break;
                        }
                        break;
                    case S_BaseInputTypes.PRIMARY_BUTTON:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED: InputType = S_InputTypes.LEFT_PRIMARY_BUTTON_PRESS; OppositeType = S_InputTypes.LEFT_PRIMARY_BUTTON_RELEASE; break;
                            case S_DirectionTypes.TOUCHED: InputType = S_InputTypes.LEFT_PRIMARY_BUTTON_TOUCH; OppositeType = S_InputTypes.LEFT_PRIMARY_BUTTON_UNTOUCH; break;
                            case S_DirectionTypes.RELEASED: InputType = S_InputTypes.LEFT_PRIMARY_BUTTON_RELEASE; OppositeType = S_InputTypes.LEFT_PRIMARY_BUTTON_PRESS;  break;
                            case S_DirectionTypes.UNTOUCHED: InputType = S_InputTypes.LEFT_PRIMARY_BUTTON_UNTOUCH; OppositeType = S_InputTypes.LEFT_PRIMARY_BUTTON_TOUCH;  break;
                        }
                        break;
                    case S_BaseInputTypes.SECONDARY_BUTTON:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED: InputType = S_InputTypes.LEFT_SECONDARY_BUTTON_PRESS; OppositeType = S_InputTypes.LEFT_SECONDARY_BUTTON_RELEASE; break;
                            case S_DirectionTypes.TOUCHED: InputType = S_InputTypes.LEFT_SECONDARY_BUTTON_TOUCH; OppositeType = S_InputTypes.LEFT_SECONDARY_BUTTON_UNTOUCH; break;
                            case S_DirectionTypes.RELEASED: InputType = S_InputTypes.LEFT_SECONDARY_BUTTON_RELEASE; OppositeType = S_InputTypes.LEFT_SECONDARY_BUTTON_PRESS; break;
                            case S_DirectionTypes.UNTOUCHED: InputType = S_InputTypes.LEFT_SECONDARY_BUTTON_UNTOUCH; OppositeType = S_InputTypes.LEFT_SECONDARY_BUTTON_TOUCH; break;
                        }
                        break;
                    case S_BaseInputTypes.INDEX:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED: InputType = S_InputTypes.LEFT_TRIGGER_BUTTON_PRESS; OppositeType = S_InputTypes.LEFT_TRIGGER_BUTTON_RELEASE; break;
                            case S_DirectionTypes.RELEASED: InputType = S_InputTypes.LEFT_TRIGGER_BUTTON_RELEASE; OppositeType = S_InputTypes.LEFT_TRIGGER_BUTTON_PRESS; break;
                        }
                        break;
                    case S_BaseInputTypes.GRIP:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED:
                            case S_DirectionTypes.TOUCHED:
                                InputType = S_InputTypes.LEFT_GRIP_BUTTON_PRESS;
                                OppositeType = S_InputTypes.LEFT_GRIP_BUTTON_RELEASE;
                                break;

                            case S_DirectionTypes.STAY:
                                OppositeType = S_InputTypes.LEFT_GRIP_BUTTON_RELEASE;
                                break;

                            case S_DirectionTypes.RELEASED:
                            case S_DirectionTypes.UNTOUCHED:
                                InputType = S_InputTypes.LEFT_GRIP_BUTTON_RELEASE;
                                OppositeType = S_InputTypes.LEFT_GRIP_BUTTON_PRESS;
                                break;
                        }
                        break;

                    case S_BaseInputTypes.STICK:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED:
                            case S_DirectionTypes.TOUCHED:
                            case S_DirectionTypes.STAY:
                                switch (StickDirection)
                                {
                                    case S_StickDirections.DOWN: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_DOWN_TILT; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_DOWN_TILT_RELEASE; break;
                                    case S_StickDirections.UP: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_UP_TILT; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_UP_TILT_RELEASE; break;
                                    case S_StickDirections.LEFT: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_LEFT_TILT; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_LEFT_TILT_RELEASE; break;
                                    case S_StickDirections.RIGHT: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_RIGHT_TILT; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_RIGHT_TILT_RELEASE; break;
                                    case S_StickDirections.PRESSED: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_PRESS; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_RELEASE; break;
                                    case S_StickDirections.RELEASED: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_RELEASE; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_PRESS; break;
                                }
                                break;

                            case S_DirectionTypes.RELEASED:
                            case S_DirectionTypes.UNTOUCHED:
                                switch (StickDirection)
                                {
                                    case S_StickDirections.DOWN: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_DOWN_TILT_RELEASE; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_DOWN_TILT; break;
                                    case S_StickDirections.UP: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_UP_TILT_RELEASE; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_UP_TILT; break;
                                    case S_StickDirections.LEFT: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_LEFT_TILT_RELEASE; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_LEFT_TILT; break;
                                    case S_StickDirections.RIGHT: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_RIGHT_TILT_RELEASE; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_RIGHT_TILT; break;
                                    case S_StickDirections.PRESSED: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_RELEASE; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_PRESS; break;
                                    case S_StickDirections.RELEASED: InputType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_PRESS; OppositeType = S_InputTypes.LEFT_PRIMARY_2D_AXIS_BUTTON_RELEASE; break;
                                }
                                break;
                        }
                        break;
                }
                break;

            //////////////////////////////////////////////////////////////////////
            case S_SingleHandsEnum.RIGHT:
                switch (BaseInputType)
                {
                    case S_BaseInputTypes.START_BUTTON:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED:
                            case S_DirectionTypes.TOUCHED:
                                InputType = S_InputTypes.MENU_BUTTON;
                                break;

                            case S_DirectionTypes.STAY:
                                break;

                            case S_DirectionTypes.RELEASED:
                            case S_DirectionTypes.UNTOUCHED:
                                break;
                        }
                        break;

                    case S_BaseInputTypes.PRIMARY_BUTTON:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED: InputType = S_InputTypes.RIGHT_PRIMARY_BUTTON_PRESS; OppositeType = S_InputTypes.RIGHT_PRIMARY_BUTTON_RELEASE; break;
                            case S_DirectionTypes.TOUCHED: InputType = S_InputTypes.RIGHT_PRIMARY_BUTTON_TOUCH; OppositeType = S_InputTypes.RIGHT_PRIMARY_BUTTON_UNTOUCH; break;
                            case S_DirectionTypes.RELEASED: InputType = S_InputTypes.RIGHT_PRIMARY_BUTTON_RELEASE; OppositeType = S_InputTypes.RIGHT_PRIMARY_BUTTON_PRESS; break;
                            case S_DirectionTypes.UNTOUCHED: InputType = S_InputTypes.RIGHT_PRIMARY_BUTTON_UNTOUCH; OppositeType = S_InputTypes.RIGHT_PRIMARY_BUTTON_TOUCH; break;
                        }
                        break;
                    case S_BaseInputTypes.SECONDARY_BUTTON:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED: InputType = S_InputTypes.RIGHT_SECONDARY_BUTTON_PRESS; OppositeType = S_InputTypes.RIGHT_SECONDARY_BUTTON_RELEASE; break;
                            case S_DirectionTypes.TOUCHED: InputType = S_InputTypes.RIGHT_SECONDARY_BUTTON_TOUCH; OppositeType = S_InputTypes.RIGHT_SECONDARY_BUTTON_UNTOUCH; break;
                            case S_DirectionTypes.RELEASED: InputType = S_InputTypes.RIGHT_SECONDARY_BUTTON_RELEASE; OppositeType = S_InputTypes.RIGHT_SECONDARY_BUTTON_PRESS; break;
                            case S_DirectionTypes.UNTOUCHED: InputType = S_InputTypes.RIGHT_SECONDARY_BUTTON_UNTOUCH; OppositeType = S_InputTypes.RIGHT_SECONDARY_BUTTON_TOUCH; break;
                        }
                        break;
                    case S_BaseInputTypes.INDEX:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED: InputType = S_InputTypes.RIGHT_TRIGGER_BUTTON_PRESS; OppositeType = S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE; break;
                            case S_DirectionTypes.RELEASED: InputType = S_InputTypes.RIGHT_TRIGGER_BUTTON_RELEASE; OppositeType = S_InputTypes.RIGHT_TRIGGER_BUTTON_PRESS; break;
                        }
                        break;
                    case S_BaseInputTypes.GRIP:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED:
                            case S_DirectionTypes.TOUCHED:
                                InputType = S_InputTypes.RIGHT_GRIP_BUTTON_PRESS;
                                OppositeType = S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE;
                                break;
                            case S_DirectionTypes.RELEASED:
                            case S_DirectionTypes.UNTOUCHED:
                                InputType = S_InputTypes.RIGHT_GRIP_BUTTON_RELEASE;
                                OppositeType = S_InputTypes.RIGHT_GRIP_BUTTON_PRESS;
                                break;
                        }
                        break;

                    case S_BaseInputTypes.STICK:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED:
                            case S_DirectionTypes.TOUCHED:
                            case S_DirectionTypes.STAY:
                                switch (StickDirection)
                                {
                                    case S_StickDirections.DOWN: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_DOWN_TILT; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_DOWN_TILT_RELEASE; break;
                                    case S_StickDirections.UP: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_UP_TILT; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_UP_TILT_RELEASE; break;
                                    case S_StickDirections.LEFT: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_LEFT_TILT; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_LEFT_TILT_RELEASE; break;
                                    case S_StickDirections.RIGHT: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_RIGHT_TILT; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_RIGHT_TILT_RELEASE; break;
                                    case S_StickDirections.PRESSED: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_PRESS; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_RELEASE; break;
                                    case S_StickDirections.RELEASED: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_RELEASE; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_PRESS; break;
                                }
                                break;

                            case S_DirectionTypes.RELEASED:
                            case S_DirectionTypes.UNTOUCHED:
                                switch (StickDirection)
                                {
                                    case S_StickDirections.DOWN: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_DOWN_TILT_RELEASE; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_DOWN_TILT; break;
                                    case S_StickDirections.UP: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_UP_TILT_RELEASE; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_UP_TILT; break;
                                    case S_StickDirections.LEFT: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_LEFT_TILT_RELEASE; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_LEFT_TILT; break;
                                    case S_StickDirections.RIGHT: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_RIGHT_TILT_RELEASE; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_RIGHT_TILT; break;
                                    case S_StickDirections.PRESSED: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_RELEASE; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_PRESS; break;
                                    case S_StickDirections.RELEASED: InputType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_PRESS; OppositeType = S_InputTypes.RIGHT_PRIMARY_2D_AXIS_BUTTON_RELEASE; break;
                                }
                                break;
                        }
                        break;
                }
                break;

            //////////////////////////////////////////////////////////////////////
            case S_SingleHandsEnum.BOTH:
                switch (BaseInputType)
                {
                    case S_BaseInputTypes.START_BUTTON:
                        switch (DirectionType)
                        {
                            case S_DirectionTypes.PRESSED:
                            case S_DirectionTypes.TOUCHED:
                                InputType = S_InputTypes.MENU_BUTTON;
                                break;

                            case S_DirectionTypes.STAY:
                                break;

                            case S_DirectionTypes.RELEASED:
                            case S_DirectionTypes.UNTOUCHED:
                                break;
                        }
                        break;
                }
                break;
        }

        UIControllerSpecificInput = (int)InputType;
    }

    public S_InputsMapEntry(S_InputTypes _type, bool _valid)
    {
        InputType = _type;
        IsValid = _valid;
    }

    public void CheckIfIsRelease()
    {
        string s = InputType.ToString().ToLower();
        IsRelease = s.Contains("up") || s.Contains("release") || s.Contains("untouch");
    }

    public int CompareTo(object obj)
    {
        return ((S_InputsMapEntry)obj).InputType.CompareTo(InputType);
    }
}