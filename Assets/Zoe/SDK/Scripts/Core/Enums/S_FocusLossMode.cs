namespace SpatialStories
{
    /// <summary>
    /// Focus modes :<br>
    ///     NONE = maintain focus amount
    ///     INSTANT = lose all focus
    ///     FADE = decrease focus progressively
    /// </summary>
    public enum S_FocusLossMode
    {
        RESET_PROGRESS,
        KEEP_PROGRESS,
        DECREASE_PROGRESSIVELY        
    }
}