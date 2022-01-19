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

using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public static class S_CatmullRomSpline
    {
        // distance between each point created between control points
        private static float m_StepsDistance = .1f;

        public static float StepDistance { set { m_StepsDistance = value; } }

        // has to be at least 4 control points
        private static List<Vector3> m_ControlPoints = new List<Vector3>();

        // all the points for the spline curve
        private static Vector3[] m_SplinePoints;

        public static Vector3[] SplinePoints
        {
            set
            {
                m_SplinePoints = new Vector3[value.Length];
                m_SplinePoints = value;
            }
        }

        private static float m_SplinePointsIndex = 0;

        public static Vector3[] getSplinePoints(Vector3[] _controlPoints)
        {
            m_ControlPoints.Clear();
            m_ControlPoints.AddRange(_controlPoints);

            //Draw the Catmull-Rom lines between the points
            //Cant draw between the endpoints
            //Neither do we need to draw from the second to the last endpoint
            for (int i = 1; i < m_ControlPoints.Count - 2; i++)
            {
                createSplineAtPosition(i);
            }

            return m_SplinePoints;
        }

        //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
        private static void createSplineAtPosition(int pos)
        {
            //Clamp to allow looping
            Vector3 p0 = m_ControlPoints[ClampListPos(pos - 1)];
            Vector3 p1 = m_ControlPoints[pos];
            Vector3 p2 = m_ControlPoints[ClampListPos(pos + 1)];
            Vector3 p3 = m_ControlPoints[ClampListPos(pos + 2)];


            //Just assign a tmp value to this
            Vector3 lastPos = Vector3.zero;

            //t is always between 0 and 1 and determines the resolution of the spline
            //0 is always at p1
            for (float t = 0f; t < 1f; t += m_StepsDistance)
            {
                //Find the coordinates between the control points with a Catmull-Rom spline
                Vector3 newPos = ReturnCatmullRom(t, p0, p1, p2, p3);

                // add the new position to the list of spline points
                m_SplinePointsIndex = ((pos - 1) * 10f) + (t * 10f);
                m_SplinePoints[(int)m_SplinePointsIndex] = newPos;

                lastPos = newPos;
            }
        }

        //Returns a position between 4 Vector3 with Catmull-Rom Spline algorithm
        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        private static Vector3 ReturnCatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 a = 0.5f * (2f * p1);
            Vector3 b = 0.5f * (p2 - p0);
            Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
            Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

            Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

            return pos;
        }

        //Clamp the list positions to allow looping
        //start over again when reaching the end or beginning
        private static int ClampListPos(int pos)
        {
            if (pos < 0)
            {
                pos = m_ControlPoints.Count - 1;
            }

            if (pos > m_ControlPoints.Count)
            {
                pos = 1;
            }
            else if (pos > m_ControlPoints.Count - 1)
            {
                pos = 0;
            }

            return pos;
        }
    }
}