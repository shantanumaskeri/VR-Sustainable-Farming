using UnityEngine;

public static class RaycastHitExtensions
{
    public static void SortClosestToFursthest(this RaycastHit[] hits)
    {
        RaycastHit temp;
        for (int i = 0; i < hits.Length; i++)
        {
            for (int j = 0; j < hits.Length - 1; j++)
            {
                if (hits[j].distance > hits[j + 1].distance)
                {
                    temp = hits[j + 1];
                    hits[j + 1] = hits[j];
                    hits[j] = temp;
                }
            }
        }
    }

    public static void RaycastSort(this RaycastHit[] hits, Ray _ray)
    {
        // Bubble sort the hits
        RaycastHit temp;
        float d1, d2;
        for (int i = 0; i < hits.Length; i++)
        {
            for (int j = 0; j < hits.Length - 1; j++)
            {
                // NOTE(4nc3str4l): Get the distance from the raycast line to the center for each element
                // As closer of the center the more interest of the user to select it. (Distance doesn't effect)
                d1 = Vector3.Cross(_ray.direction, hits[j].collider.bounds.center - _ray.origin).magnitude;
                d2 = Vector3.Cross(_ray.direction, hits[j + 1].collider.bounds.center - _ray.origin).magnitude;
                if (d1 > d2)
                {
                    temp = hits[j + 1];
                    hits[j + 1] = hits[j];
                    hits[j] = temp;
                }
            }
        }
    }
}
