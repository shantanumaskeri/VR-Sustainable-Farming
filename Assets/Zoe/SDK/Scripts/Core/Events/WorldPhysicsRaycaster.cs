using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Extension of PhysicsRaycaster to support ray casting with world space rays instead of just screen-space
/// pointer positions
/// </summary>
public class WorldPhysicsRaycaster : PhysicsRaycaster
{
    public override void Raycast(PointerEventData _pointerEventData, List<RaycastResult> _raycastResults)
    {
        if (!(_pointerEventData is WorldPointerData))
        {
            base.Raycast(_pointerEventData, _raycastResults);
            return;
            //throw new System.Exception("WorldPhysicsRaycaster cannot be used with another pointer type than WorldPointerData");
        }

        float dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

        WorldPointerData worldPointerData = (WorldPointerData)_pointerEventData;

        RaycastHit[]  hits = Physics.RaycastAll(worldPointerData.worldSpaceRay, dist, finalEventMask);

        if (hits.Length == 0)
        {
            return;
        }

        if (hits.Length > 1)
        {
            System.Array.Sort(hits, (r1, r2) => r1.distance.CompareTo(r2.distance));
        }

        foreach (RaycastHit hit in hits)
        {
            RaycastResult result = new RaycastResult
            {
                gameObject = hit.collider.gameObject,
                module = this,
                distance = hit.distance,
                worldPosition = hit.point,
                worldNormal = hit.normal,
                screenPosition = eventCamera.WorldToScreenPoint(hit.point),
                index = _raycastResults.Count,
                sortingLayer = 0,
                sortingOrder = 0
            };

            _raycastResults.Add(result);
        }
    }
}
