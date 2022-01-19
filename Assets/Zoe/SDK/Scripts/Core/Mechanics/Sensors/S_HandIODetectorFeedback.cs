using Gaze;
using System;
using UnityEngine;

namespace SpatialStories
{
    public class S_HandIODetectorFeedback
    {
        public enum FeedbackModes { Default, Colliding }
        public FeedbackModes ActualFeedbackMode = FeedbackModes.Default;

        private S_GrabManager m_GrabManager;
        private S_LaserEventArgs m_GazeLaserEventArgs;
        private S_HandIODetectorKernel m_DetectorKernel;
        
        // This is used on the function ShowDistantGrabFeedbacks
        private SpriteRenderer m_IntrctvDstntGrbFdbckSprRndrr;

        public S_HandIODetectorFeedback(S_GrabManager _owner)
        {
            m_GrabManager = _owner;
            m_GazeLaserEventArgs = new S_LaserEventArgs();
            m_GazeLaserEventArgs.Sender = _owner;
        }

        public void Setup()
        {
            m_DetectorKernel = m_GrabManager.IoDetectorKernel;
        }

        public void Update()
        {
            // If it's the detector kernel who draws the line just don't do anything
            if (ActualFeedbackMode != FeedbackModes.Default)
                return;

            ShowDefaultRay();            
        }

        /// <summary>
        /// When the gaze hand detector kernel is not detecting anything the ray will be displayed by this method
        /// </summary>
        private void ShowDefaultRay()
        {
            if (m_GrabManager.LaserPointer == null)
                return;

            Vector3 _targetPosition = m_GrabManager.DistantGrabOrigin.transform.forward;
            Vector3 endPosition = m_GrabManager.transform.position + (m_GrabManager.LaserLength * _targetPosition);
            ShowDistantGrabLaser(m_GrabManager.DistantGrabOrigin.transform.position, endPosition, m_GrabManager.DistantGrabOrigin.transform.forward, false, false, m_GrabManager.LaserStartWidth, m_GrabManager.LaserEndWidth);
        }

        public void ShowDistantGrabFeedbacks(Vector3 _targetPosition, Vector3 _direction, float _length, bool _intersects, bool _inRange, float _startWidth, float _endWidth)
        {
            if (m_GrabManager.CloserIO != null)
                _length = m_GrabManager.CloserDistance;

            m_GrabManager.IntersectsWithInteractiveIO = _intersects;

            if (m_GrabManager.LaserPointer == null)
                return;

            // This will check if the raycast intersecs with a valid grabbable object
            if (Math.Abs(_length - float.MaxValue) < 0.01f)
            {
                _length = m_GrabManager.DefaultDistantGrabRayLength;
                m_GrabManager.IntersectsWithInteractiveIO = false;
            }

            Vector3 endPosition = _targetPosition + (_length * _direction);

            ShowDistantGrabLaser(_targetPosition, endPosition, _direction, m_GrabManager.IntersectsWithInteractiveIO, _inRange, _startWidth, _endWidth);
        }

        private void ShowDistantGrabLaser(Vector3 _targetPosition, Vector3 _endPosition, Vector3 _direction, bool _intersectsWithIo, bool _iOInRange, float _startWidth, float _endWidth)
        {
            if (!m_GrabManager.DisplayGrabPointer)
                return;

            Color startColor;
            Color endColor;

            m_GrabManager.LaserPointer.SetPosition(0, _targetPosition);
            m_GrabManager.LaserPointer.SetPosition(1, _endPosition);

            m_GrabManager.SingleLaserPointObject.SetActive(false);
            m_GrabManager.SingleInteractableDistantGrabFeedback.SetActive(false);
            m_GrabManager.SingleNotInteractableDistantGrabFeedback.SetActive(false);
            GameObject referenceInteratableObject = null;
            if (_intersectsWithIo)
            {
                if (_iOInRange)
                {
                    referenceInteratableObject = m_GrabManager.SingleInteractableDistantGrabFeedback;
                    endColor = m_GrabManager.InteractableInRangeDistantGrabColor;
                    startColor = endColor;
                }
                else
                {
                    referenceInteratableObject = m_GrabManager.SingleNotInteractableDistantGrabFeedback;
                    endColor = m_GrabManager.NotInteractableDistantGrabColor;
                    startColor = endColor;
                }
            }
            else
            {
                referenceInteratableObject = m_GrabManager.SingleLaserPointObject;
                startColor = Color.white;
                endColor = m_GrabManager.LaserColor;
            }

            m_GrabManager.LaserPointer.startColor = startColor;
            m_GrabManager.LaserPointer.endColor = endColor;
            m_GrabManager.LaserPointer.startWidth = _startWidth;
            m_GrabManager.LaserPointer.endWidth = _endWidth;
            m_GazeLaserEventArgs.StartPosition = _targetPosition;
            m_GazeLaserEventArgs.EndPosition = _endPosition;
            if (referenceInteratableObject != null)
            {
                referenceInteratableObject.SetActive(true);
                referenceInteratableObject.transform.position = _endPosition;
            }
            m_GazeLaserEventArgs.LaserHits = m_DetectorKernel.Hits;
            S_EventManager.FireLaserEvent(m_GazeLaserEventArgs);
        }
    }
}
