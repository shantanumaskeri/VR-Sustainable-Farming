using SpatialStories;
using UnityEngine;

public class TemplateCondition : S_AbstractCondition
{
    public override void Dispose()
    {
        // Undo any requirements you made in Setup
    }

    public override void Reload()
    {
        // Logic that happens upon reloading the interaction
        // goes here
        // Usually it is a simple call to Invalidate();
    }

    public override void Setup()
    {
        // Prepare requirements needed for your condition validation
    }

    public override void SetupUsingApi(GameObject _interaction)
    {
        // This function is necessary for the script to compile.
        // You can leave it empty.
    }
}
