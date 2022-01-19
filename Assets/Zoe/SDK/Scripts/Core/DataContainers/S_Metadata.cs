namespace SpatialStories
{
    [System.Serializable]
    public struct S_Metadata
    {
        public string GUID;
        public string CreationTime;

        public S_Metadata(string _guid, string _creationTime)
        {
            GUID = _guid;
            CreationTime = _creationTime;
        }
    }
}
