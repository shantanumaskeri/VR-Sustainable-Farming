namespace SpatialStories
{        /// <summary>
        /// Creation Options for the IOS
        /// </summary>
        public enum APICreationOptions
        {
            ///<summary>Create only the interactive object, interactions and dependencie will be created later</summary>
            INTERACTIVE_OBJECTS,
            ///<summary>Create the interactive object and it's interactions dependencies will be created later</summary>
            INTERACTIVE_OBJECT_AND_INTERACTIONS,
            ///<summary>Create the interactive object the interactions and the dependencies</summary>
            INTERACTIVE_OBJECT_INTERACTIONS_AND_DEPENDENCIES,
            /// <summary>Create the interactive object but without instantiate the actual S_Interaction gameobject</summary>
            INTERACTIVE_OBJECT_WITHOUT_INTERACTION_INSTANTIATION
        }
}