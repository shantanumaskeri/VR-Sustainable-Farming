namespace SpatialStories
{
    /// <summary>
    /// A partial definition of the S_Abstract condition used to
    /// being able to create conditions and dependencies by using the
    /// SpatialStories API.
    /// </summary>
    public abstract partial class S_AbstractCondition : S_AbstractInteractionDataMonoBehaviour
    {
        // This constructor is needed in order to lazy initialize conditions
        public S_AbstractCondition() { }
    }
}