using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Extension of GraphicRaycaster to support ray casting with world space rays instead of just screen-space
/// pointer positions
/// </summary>
[RequireComponent(typeof(Canvas))]
public class WorldGraphicRaycaster : GraphicRaycaster
{
    public int sortOrder = 0;

    [NonSerialized]
    private Canvas m_Canvas;

    private Canvas canvas
    {
        get
        {
            if (m_Canvas != null)
                return m_Canvas;

            m_Canvas = GetComponent<Canvas>();
            return m_Canvas;
        }
    }

    public override Camera eventCamera
    {
        get
        {
            return canvas.worldCamera;
        }
    }

    public override int sortOrderPriority
    {
        get
        {
            return sortOrder;
        }
    }

    protected override void Awake()
    {
        if (canvas.renderMode != RenderMode.WorldSpace)
        {
            throw new Exception("You cannot use the WorldGraphicRaycaster with a canvas that is not in world space");
        }
    }

    /// <summary>
    /// For the given ray, find graphics on this canvas which it intersects and are not blocked by other
    /// world objects
    /// This function is heavily inspired by GraphicRaycaster's Raycast and is slightly altered to work with
    /// world space rays
    /// </summary>
    [NonSerialized] private List<Tuple<Graphic, RaycastResult>> m_GraphicsAndRaycastResults = new List<Tuple<Graphic, RaycastResult>>();
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        WorldPointerData worldPointerData = (WorldPointerData)eventData;

        if (canvas == null)
            return;

        float hitDistance = float.MaxValue;

        if (blockingObjects != BlockingObjects.None)
        {
            float dist = eventCamera.farClipPlane;

            if (blockingObjects == BlockingObjects.ThreeD || blockingObjects == BlockingObjects.All)
            {
                RaycastHit[] hits = Physics.RaycastAll(worldPointerData.worldSpaceRay, dist, m_BlockingMask);

                if (hits.Length > 0 && hits[0].distance < hitDistance)
                {
                    hitDistance = hits[0].distance;
                }
            }

            if (blockingObjects == BlockingObjects.TwoD || blockingObjects == BlockingObjects.All)
            {
                RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(worldPointerData.worldSpaceRay, dist, m_BlockingMask);

                if (hits.Length > 0 && hits[0].fraction * dist < hitDistance)
                {
                    hitDistance = hits[0].fraction * dist;
                }
            }
        }

        m_GraphicsAndRaycastResults.Clear();

        Raycast(worldPointerData, m_GraphicsAndRaycastResults);

        for (int index = 0; index < m_GraphicsAndRaycastResults.Count; index++)
        {
            GameObject go = m_GraphicsAndRaycastResults[index].Item1.gameObject;
            bool appendGraphic = true;

            if (ignoreReversedGraphics)
            {
                // If we have a camera compare the direction against the cameras forward.
                Vector3 cameraFoward = worldPointerData.worldSpaceRay.direction;
                Vector3 dir = go.transform.rotation * Vector3.forward;
                appendGraphic = Vector3.Dot(cameraFoward, dir) > 0;
            }

            if (appendGraphic)
            {
                if (m_GraphicsAndRaycastResults[index].Item2.distance >= hitDistance)
                {
                    continue;
                }

                RaycastResult raycastResult = m_GraphicsAndRaycastResults[index].Item2;
                raycastResult.index = resultAppendList.Count;
                resultAppendList.Add(raycastResult);
            }
        }
    }

    /// <summary>
    /// Perform a raycast into the screen and collect all graphics underneath it.
    /// Again, this is mostly taken from Unity's GraphicRaycaster. It is simply altered
    /// for the intersection computation where we first find what the world space plane defined
    /// by the graphic object is and then find the screen position of the intersection with the ray
    /// </summary>
    [NonSerialized]
    readonly List<Tuple<Graphic, RaycastResult>> m_SortedGraphicsAndRaycastResults = new List<Tuple<Graphic, RaycastResult>>();
    private void Raycast(WorldPointerData _worldPointerData, List<Tuple<Graphic, RaycastResult>> results)
    {
        // Necessary for the event system
        IList<Graphic> foundGraphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
        m_SortedGraphicsAndRaycastResults.Clear();
        for (int i = 0; i < foundGraphics.Count; ++i)
        {
            Graphic graphic = foundGraphics[i];

            if (graphic == null)
            {
                continue;
            }

            // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
            if (graphic.depth == -1 || !graphic.raycastTarget || graphic.canvasRenderer.cull)
            {
                continue;
            }

            Vector3 worldPosition;
            if (!GetWorldSpaceHitPointOnPlaneFromRectangle(graphic.rectTransform, _worldPointerData.worldSpaceRay, out worldPosition))
            {
                continue;
            }

            Vector2 pointerPosition = eventCamera.WorldToScreenPoint(worldPosition);

            if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera))
                continue;

            // mask/image intersection - See Unity docs on eventAlphaThreshold for when this does anything
            if (graphic.Raycast(pointerPosition, eventCamera))
            {
                RaycastResult raycastResult = new RaycastResult
                {
                    gameObject = graphic.gameObject,
                    module = this,
                    distance = Vector3.Distance(_worldPointerData.worldSpaceRay.origin, worldPosition),
                    depth = graphic.depth,
                    screenPosition = pointerPosition,
                    worldPosition = worldPosition
                };

                m_SortedGraphicsAndRaycastResults.Add(new Tuple<Graphic, RaycastResult>(graphic, raycastResult));
            }
        }

        m_SortedGraphicsAndRaycastResults.Sort((g1, g2) => g2.Item1.depth.CompareTo(g1.Item1.depth));

        for (int i = 0; i < m_SortedGraphicsAndRaycastResults.Count; ++i)
        {
            results.Add(m_SortedGraphicsAndRaycastResults[i]);
        }
    }

    /// <summary>
    /// A handy function that retrieves the world space intersection between a ray and a plane defined by a RectTransform
    /// </summary>
    /// <param name="rectTransform">The ReactTransform used to define the world space plane</param>
    /// <param name="ray">The ray used for ray casting</param>
    /// <param name="worldPosition">The world position returned. A zero vector is returned if no intersection was found</param>
    /// <returns>Whether the ray passed as a parameter intersects the plane defined by RectTransfrom</returns>
    static public bool GetWorldSpaceHitPointOnPlaneFromRectangle(RectTransform rectTransform, Ray ray, out Vector3 worldPosition)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Plane plane = new Plane(corners[0], corners[1], corners[2]);

        float distanceToPlane;
        if (!plane.Raycast(ray, out distanceToPlane))
        {
            worldPosition = Vector3.zero;

            return false;
        }

        worldPosition = ray.GetPoint(distanceToPlane);

        return true;
    }

    /// <summary>
    /// A handy function that retrieves the screen space intersection between a ray and a plane defined by a RectTransform
    /// </summary>
    /// <param name="rectTransform">The ReactTransform used to define the plane</param>
    /// <param name="ray">The ray used for ray casting</param>
    /// <param name="camera">The camera used to compute the screen position</param>
    /// <param name="screenPosition">The screen position returned. A zero vector is returned if no intersection was found</param>
    /// <returns></returns>
    static public bool GetScreenSpaceHitPointOnPlaneFromRectangle(RectTransform rectTransform, Ray ray, Camera camera, out Vector2 screenPosition)
    {
        Vector3 worldPosition;
        if (!GetWorldSpaceHitPointOnPlaneFromRectangle(rectTransform, ray, out worldPosition))
        {
            screenPosition = Vector2.zero;

            return false;
        }

        screenPosition = camera.WorldToScreenPoint(worldPosition);

        return true;
    }
}
