namespace SpatialStories
{
    public static class S_UserLayers
    {
        public static string TELEPORT = "Teleport";
        public static string SOLID = "Custom Collision";
        public static string GAZE = "Gaze_Gaze";
        public static string PROXIMITY = "Gaze_Proximity";
        public static string HANDHOVER = "Gaze_HandHover";

        public static string[] AllLayers = new string[]
        {
            TELEPORT,
            SOLID,
            GAZE,
            PROXIMITY,
            HANDHOVER
        };
    }
}
