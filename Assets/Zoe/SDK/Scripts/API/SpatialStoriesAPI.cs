using UnityEngine;

namespace SpatialStories
{
    public static class SpatialStoriesAPI
    {
        /// <summary>
        /// Creates a new IO definition, usefull to begin the process of
        /// creation of an interactive object.
        /// </summary>
        /// <param name="_name">Name of the io that will be created with this definition</param>
        /// <returns></returns>
        public static S_IODefinition CreateIODefinition(string _name)
        {
            return new S_IODefinition(_name);
        }

        /// <summary>
        /// Creates a new IO definition using a GameObject as starting point (it will be
        /// the interactive object visuals and use its name)
        /// </summary>
        /// <param name="_visuals"> The visuals of the new interactive object </param>
        /// <param name="_copyPositionAndRotation"> Use the visuals position as rotation as starting point for the io</param>
        /// <returns></returns>
        public static S_IODefinition CreateIODefinition(GameObject _visuals, bool _copyPositionAndRotation = true)
        {
            return new S_IODefinition(_visuals, _copyPositionAndRotation);
        }


        /// <summary>
        /// Creates a new Interactive Object using the definition.
        /// </summary>
        /// <param name="_definition"> Definition of the game object to create< </param>
        /// <param name="_options"> Creation options  </param>
        /// <returns></returns>
        public static S_InteractiveObject CreateInteractiveObject(S_IODefinition _definition,  APICreationOptions _options, bool network=false)
        {
            S_InteractiveObject io = null;
            switch(_options)
            {
                case APICreationOptions.INTERACTIVE_OBJECTS:
                    io = SpatialStoriesFactory.CreateInteractiveObject(_definition, false, false, network);
                    break;
                case APICreationOptions.INTERACTIVE_OBJECT_AND_INTERACTIONS:
                    io = SpatialStoriesFactory.CreateInteractiveObject(_definition, true, false, network);
                    break;
                case APICreationOptions.INTERACTIVE_OBJECT_INTERACTIONS_AND_DEPENDENCIES:
                    io = SpatialStoriesFactory.CreateInteractiveObject(_definition, true, true, network);
                    break;
                case APICreationOptions.INTERACTIVE_OBJECT_WITHOUT_INTERACTION_INSTANTIATION:
                    io = SpatialStoriesFactory.CreateInteractiveObjectWithoutInteractionInstantiation(_definition, network);
                    break;
            }
            return io;
        }

        /// <summary>
        /// Wrapper method to SpatialStoriesFactory.InstantiatePendingInteractions
        /// </summary>
        /// <param name="_setupInteractions"></param>
        /// <param name="_wireDependencies"></param>
        public static void InstantiatePendingInteractions(bool _setupInteractions, bool _wireDependencies)
        {
            SpatialStoriesFactory.InstantiatePendingInteractions(_setupInteractions, _wireDependencies);
        }

        /// <summary>
        /// Creates all the pending interactions
        /// </summary>
        /// <param name="_wireDependencies"> Do we want to wire the dependencies now if not call  WirePendingDependencies to create them later</param>
        public static void CreatePendingInteractions(bool _wireDependencies)
        {
            SpatialStoriesFactory.SetupPendingInteractions(_wireDependencies);
        }

        /// <summary>
        /// Connects all the pending dependencies created by the api, when _wireDependencies is set to false
        /// </summary>
        public static void WirePendingDependencies()
        {
            SpatialStoriesFactory.WirePendingDependencies();
        }

        /// <summary>
        /// Finds an already created object by GUID.
        /// </summary>
        /// <param name="_guid">The guid of the object</param>
        /// <returns></returns>
        public static GameObject GetObjectOfTypeWithGUID(string _guid)
        {
            S_AbstractEntity[] guids = GameObject.FindObjectsOfType<S_AbstractEntity>();

            for (int i = 0; i < guids.Length; i++)
            {
                S_AbstractEntity entity = guids[i];
                if (entity.Metadata.GUID != null && entity.Metadata.GUID.Equals(_guid))
                {
                    return entity.gameObject;
                }
            }
            Debug.LogWarning(string.Format("Spatial Stories API {0} not found!", _guid));
            return null;
        }

        /// <summary>
        /// Finds an interactive object using it's GUID
        /// </summary>
        /// <param name="_guid">Guid of the Interactive Object</param>
        /// <returns></returns>
        public static S_InteractiveObject GetInteractiveObjectWithGUID(string _guid)
        {
            // Get the object to touch
            GameObject ioObj = SpatialStoriesAPI.GetObjectOfTypeWithGUID(_guid);
            S_InteractiveObject io = null;
            if (ioObj != null)
            {
                io = ioObj.GetComponent<S_InteractiveObject>();
            }
            else
            {
                Debug.Log(string.Format("IO with GUID {0} Not found", _guid));
            }
            return io;
        }
    }
}