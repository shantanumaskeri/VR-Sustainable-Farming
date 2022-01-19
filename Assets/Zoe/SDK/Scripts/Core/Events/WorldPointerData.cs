using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class extending the PointerEventData to simply add the possibility
/// to use a world spay ray rather than a screen space one. That way we
/// can define a ray starting and ending anywhere in the worl and use this
/// as our pointer
/// </summary>
public class WorldPointerData : PointerEventData
{
    public Ray worldSpaceRay;

    public WorldPointerData(EventSystem _eventSystem)
        : base(_eventSystem)
    {
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>Position</b>: " + position);
        sb.AppendLine("<b>delta</b>: " + delta);
        sb.AppendLine("<b>eligibleForClick</b>: " + eligibleForClick);
        sb.AppendLine("<b>pointerEnter</b>: " + pointerEnter);
        sb.AppendLine("<b>pointerPress</b>: " + pointerPress);
        sb.AppendLine("<b>lastPointerPress</b>: " + lastPress);
        sb.AppendLine("<b>pointerDrag</b>: " + pointerDrag);
        sb.AppendLine("<b>worldSpaceRay</b>: " + worldSpaceRay);
        sb.AppendLine("<b>Use Drag Threshold</b>: " + useDragThreshold);
        return sb.ToString();
    }
}
