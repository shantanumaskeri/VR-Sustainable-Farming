using System;

namespace SpatialStories
{
    /// <summary>
    /// Holds a guid and a name for objects that will be created
    /// with the spatial stories API such as Interactions and 
    /// Interactive Objects.
    /// </summary>
    public abstract class S_AbstractDefinition
    {
        public string Name { get; private set; }
        public S_Metadata Metadata;

        public S_AbstractDefinition(string _name) : base()
        {
            Name = _name;
            Metadata = new S_Metadata(S_AbstractEntity.GenerateNewGUID(), DateTime.Now.ToString());
        }

        public void SetName(string _newName)
        {
            Name = _newName;
        }
    }
}