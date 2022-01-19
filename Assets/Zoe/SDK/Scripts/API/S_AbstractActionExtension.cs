namespace SpatialStories
{
    /// <summary>
    /// A partial definition of the S_Abstract behaivour used to
    /// being able to create actions by using the
    /// SpatialStories API.
    /// </summary>
    public abstract partial class S_AbstractAction : S_AbstractInteractionDataMonoBehaviour
    {
        // This constructor is needed in order to lazy initialize actions
        public S_AbstractAction() { }
    }

}
