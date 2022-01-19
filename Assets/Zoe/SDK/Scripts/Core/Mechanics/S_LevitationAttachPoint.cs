using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public class S_LevitationAttachPoint : MonoBehaviour
    {
        public static List<S_LevitationAttachPoint> AttachPoints = new List<S_LevitationAttachPoint>();

        void Start()
        {
            AttachPoints.Add(this);
        }

        void OnDestroy()
        {
            AttachPoints.Remove(this);
        }

        public static void DestroyAllAttachPoints()
        {
            for(int i = AttachPoints.Count -1; i >= 0; --i)
            {
                Destroy(AttachPoints[i].gameObject);
                AttachPoints.RemoveAt(i);
            }
        }
    }
}
