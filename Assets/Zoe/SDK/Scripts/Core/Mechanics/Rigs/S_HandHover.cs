using SpatialStories;
using UnityEngine;

public class S_HandHover : MonoBehaviour
{
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(S_UserLayers.HANDHOVER);
    }
}