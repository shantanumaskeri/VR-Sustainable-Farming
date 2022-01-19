using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("SpatialStoriesAPI")]
namespace SpatialStories
{
    /// <summary>
    /// Creates and adds Unity Game Objects to the current scene using
    /// S_IODefinition data.
    /// </summary>
    public static class SpatialStoriesFactory
    {
        /// <summary>
        /// List of interactions to be created through UnityEngine.Object.Instantiate calls
        /// </summary>
        private static List<KeyValuePair<S_Interaction, S_InteractionDefinition>> interactionsToInstantiate = new List<KeyValuePair<S_Interaction, S_InteractionDefinition>>();
        /// <summary>
        /// List of all interactions / Definitions wich dependencies hasn't been wire up
        /// </summary>
        private static List<KeyValuePair<GameObject, S_InteractionDefinition>> depenenciesToWire = new List<KeyValuePair<GameObject, S_InteractionDefinition>>();

        /// <summary>
        /// List of all the interactions that are pending for creation
        /// </summary>
        /// <returns></returns>
        private static List<KeyValuePair<GameObject, S_InteractionDefinition>> interactionsToCreate = new List<KeyValuePair<GameObject, S_InteractionDefinition>>();

        /// <summary>
        /// Creates a full interactive object including interactions, conditions... By using
        /// S_IODefinition as an argument
        /// NOTE :This method can be only be called from the SpatialStories API.
        /// </summary>
        /// <param name="_definition">An interactive object definition</param>
        /// <param name="_setupInteractions"> Do the interactins will be wired now? (set this to false to wait for all the IOS to be created before seting up the interacions)</param>
        /// <param name="_wireDependencies"> Do the dependencies will be wired now ? (set that to false if you have circular dependencies that need
        /// to be post processed when all the ios and interactions have been created) </param> 
        /// <returns>An interactive object ready to use</returns>
        internal static S_InteractiveObject CreateInteractiveObject(S_IODefinition _definition, bool _setupInteractions, bool _wireDependencies, bool network=false)
        {
            // Create the interactive object from the prefab
            GameObject ioGameObject = GameObject.Instantiate(Resources.Load(network? "Interactive Object - networked" : "Interactive Object") as GameObject);

            // Begin the crafting process
            ioGameObject.name = _definition.Name + (network? " (IO)(Networked)":" (IO)");

            // Create the interactions once the IO is ready
            S_InteractiveObject io = ioGameObject.GetComponent<S_InteractiveObject>();
            io.Metadata = _definition.Metadata;

            // Enable the appropiate manipualtion mode
            io.EnableManipulationMode(_definition.ManipulationMode);
            
            S_GravityManager.ChangeGravityState(io, _definition.AffectedByGravity ? S_GravityRequestType.ACTIVATE_AND_DETACH : S_GravityRequestType.DEACTIVATE_AND_ATTACH, false);
            S_GravityManager.ChangeGravityState(io, S_GravityRequestType.SET_AS_DEFAULT, false);

            // If some visuals where specified add them to the new game object.
            if(_definition.Visuals != null)
            {
                AddNestedVisuals(_definition.Visuals, io);
                ScaleCollidersToMesh(io);
            }

            ioGameObject.transform.position = _definition.Position;
            ioGameObject.transform.rotation = _definition.Rotation;
            ioGameObject.transform.localScale = Vector3.one;

            CreateInteractionsForGameObject(_definition, io, _setupInteractions, _wireDependencies);

            return io;
        }

        /// <summary>
        /// Creates a full interactive object without actually instantiate the gameobject
        /// holding the S_Interaction script
        /// </summary>
        /// <param name="_definition">An interactive object definition</param>
        /// <returns>An interactive object ready to use</returns>
        internal static S_InteractiveObject CreateInteractiveObjectWithoutInteractionInstantiation(S_IODefinition _definition, bool network=false)
        {
            // Create the interactive object from the prefab
            GameObject ioGameObject = GameObject.Instantiate(Resources.Load(network? "Interactive Object - networked":"Interactive Object") as GameObject);

            // Begin the crafting process
            ioGameObject.name = _definition.Name + (network ? " (IO)(Networked)" : " (IO)");

            // Create the interactions once the IO is ready
            S_InteractiveObject io = ioGameObject.GetComponent<S_InteractiveObject>();
            io.Metadata = _definition.Metadata;

            // Enable the appropiate manipualtion mode
            io.EnableManipulationMode(_definition.ManipulationMode);

            S_GravityManager.ChangeGravityState(io, _definition.AffectedByGravity ? S_GravityRequestType.ACTIVATE_AND_DETACH : S_GravityRequestType.DEACTIVATE_AND_ATTACH, false);
            S_GravityManager.ChangeGravityState(io, S_GravityRequestType.SET_AS_DEFAULT, false);

            // If some visuals where specified add them to the new game object.
            if (_definition.Visuals != null)
            {
                AddNestedVisuals(_definition.Visuals, io);
                ScaleCollidersToMesh(io);
            }

            ioGameObject.transform.position = _definition.Position;
            ioGameObject.transform.rotation = _definition.Rotation;
            ioGameObject.transform.localScale = _definition.Scale;

            CreateInteractionsForGameObjectWithoutInstantiation(_definition, io);

            return io;
        }

        /// <summary>
        /// Calls UnityEngine.Object.Instantiate in order to instantiate all pending interactions.
        /// This is useful when timing is important because otherwise, Start() method from S_Interaction
        /// can be called at an impractical time
        /// </summary>
        /// <param name="_setupInteractions">Whether interactions should also be setup</param>
        /// <param name="_wireDependencies">Whether dependencies should be wired</param>
        internal static void InstantiatePendingInteractions(bool _setupInteractions, bool _wireDependencies)
        {
            foreach (KeyValuePair<S_Interaction, S_InteractionDefinition> interactionToInstantiate in interactionsToInstantiate)
            {
                InstantiateInteraction(interactionToInstantiate.Value, interactionToInstantiate.Key, _setupInteractions, _wireDependencies);
            }
            interactionsToInstantiate.Clear();
        }

        /// <summary>
        /// Connects all the io's dependencies that were pending and clears the list
        /// </summary>
        internal static void WirePendingDependencies()
        {
            foreach (KeyValuePair<GameObject, S_InteractionDefinition> dep in depenenciesToWire)
            {
                CreateDependenciesForInteraction(dep.Value, dep.Key);
            }
            depenenciesToWire.Clear();
        }

        /// <summary>
        /// Creates all the pending interactions
        /// </summary>
        /// <param name="_wireDependencies"> Do the dependencies will be wired now ? (set that to false if you have circular dependencies that need
        /// to be post processed when all the ios and interactions have been created) </param>
        internal static void SetupPendingInteractions(bool _wireDependencies)
        {
            foreach (KeyValuePair<GameObject, S_InteractionDefinition> inter in interactionsToCreate)
            {
                CreateConditionsAndActionsForInteraction(inter.Key, inter.Value, _wireDependencies);
                inter.Key.GetComponent<S_Interaction>().ConditionsSetup();
            }
            interactionsToCreate.Clear();
        }

        /// <summary>
        /// Setup the interaction to be instantiated at a later time by simply giving the model interaction
        /// it will be based on and the interaction definition to set it up
        /// </summary>
        /// <param name="_ioDef"></param>
        /// <param name="_io"></param>
        private static void CreateInteractionsForGameObjectWithoutInstantiation(S_IODefinition _ioDef, S_InteractiveObject _io)
        {
            // Get the base interaction in order to replicate it as many times as needed
            S_Interaction modelInteraction = _io.GetComponentInChildren<S_Interaction>();
            modelInteraction.InitializeMetadata();

            // Create all the interactions specified on the io definition
            int numInteractions = _ioDef.Interactions.Count;
            for (int i = 0; i < numInteractions; i++)
            {
                interactionsToInstantiate.Add(new KeyValuePair<S_Interaction, S_InteractionDefinition>(modelInteraction, _ioDef.Interactions[i]));
            }
        }

        /// <summary>
        /// Creates all the interactions specified in the IO definition and binds them into the 
        /// newly created Interactive Object
        /// </summary>
        /// <param name="_ioDef"> The definition of the IO in process of creation </param>
        /// <param name="_io"> The instance of the interactive object being created </param>
        /// <param name="_setupInteractions"> Do the interactions will be wired now? (set this to false to wait for all the IOS to be created before seting up the interacions)</param>
        /// <param name="_wireDependencies"> Do the dependencies will be wired now ? (set that to false if you have circular dependencies that need
        /// to be post processed when all the ios and interactions have been created) </param>
        private static void CreateInteractionsForGameObject(S_IODefinition _ioDef, S_InteractiveObject _io, bool _setupInteractions, bool _wireDependencies)
        {
            // Get the base interaction in order to replicate it as many times as needed
            S_Interaction modelInteraction = _io.GetComponentInChildren<S_Interaction>();
            modelInteraction.InitializeMetadata();

            // Create all the interactions specified on the io definition
            int numInteractions = _ioDef.Interactions.Count;
            for (int i = 0; i < numInteractions; i++)
            {
                InstantiateInteraction(_ioDef.Interactions[i], modelInteraction, _setupInteractions, _wireDependencies);
            }
        }

        /// <summary>
        /// Handy method to instantiate an interaction from a model interaction and an interaction definition
        /// </summary>
        private static void InstantiateInteraction(S_InteractionDefinition _interactionDefinition, S_Interaction modelInteraction, bool _setupInteractions, bool _wireDependencies)
        {
            // Create a new copy of the model interation and parent it to the interactions of the io
            GameObject interaction = GameObject.Instantiate(modelInteraction.gameObject, modelInteraction.transform.parent);

            if (_setupInteractions)
            {
                // Create the conditions and actions for the interaction
                CreateConditionsAndActionsForInteraction(interaction, _interactionDefinition, _wireDependencies);
            }
            else
            {
                interactionsToCreate.Add(new KeyValuePair<GameObject, S_InteractionDefinition>(interaction, _interactionDefinition));
            }
        }

        /// <summary>
        /// Create the Conditions and Actions of an interaction
        /// </summary>
        /// <param name="_interaction"> Interaction Object </param>
        /// <param name="_interDef"> Interaction Definition </param>
        /// <param name="_wireDependencies"> Do the dependencies will be wired now ? (set that to false if you have circular dependencies that need
        /// to be post processed when all the ios and interactions have been created) </param>
        private static void CreateConditionsAndActionsForInteraction(GameObject _interaction, S_InteractionDefinition _interDef, bool _wireDependencies)
        {
            // Set the interaction name
            _interaction.name = _interDef.Name;

            // Copy the metadata for the interaction
            _interaction.GetComponent<S_Interaction>().Metadata = _interDef.Metadata;
            
            // Create the interaction conditions
            CreateConditionsForInteraction(_interDef, _interaction);

            // Create the interaction Actions
            CreateActionsForInteraction(_interDef, _interaction);

            // Create the interaction dependencies if necessary
            if (_wireDependencies)
            {
                CreateDependenciesForInteraction(_interDef, _interaction);
            }
            else
            {
                depenenciesToWire.Add(new KeyValuePair<GameObject, S_InteractionDefinition>(_interaction, _interDef));
            }
        }

        /// <summary>
        /// Creates all the specified conditions for the interaction being created
        /// </summary>
        /// <param name="_interDef">The interaction definition file</param>
        /// <param name="_interaction"> The interaction in process of creation </param>
        private static void CreateConditionsForInteraction(S_InteractionDefinition _interDef, GameObject _interaction)
        {
            int numConditions = _interDef.ConditionsAndDependencies.Count;
            for (int i = 0; i < numConditions; i++)
            {
                S_AbstractCondition tempCondition = _interDef.ConditionsAndDependencies[i];
                
                if (!(tempCondition is SC_InteractionDependencies))
                {
                    // Create the final action using the temporary condition type
                    Component comp = _interaction.AddComponent(tempCondition.GetType());

                    // Cast to get the S_AbstractCondition of the action
                    S_AbstractCondition finalCondition = ((S_AbstractCondition)comp);

                    // Copy the temp condition data to the final data
                    finalCondition.creationData = new List<object>(tempCondition.creationData);

                    // Setup te metadata to have a guid and a creation timestamp
                    finalCondition.InitializeMetadata();

                    // Setupt the final condition
                    finalCondition.SetupUsingApi(_interaction);
                }
            }
        }

        /// <summary>
        /// Creates all the dependencies for the interaction being created, we should consider
        /// exposing this method in order to allow the creation of circular dependencies, like that
        /// the user will be able to first, create all the ios, conditions, actions and then wire up
        /// all the dependencies without having to care about the order of creation of the ios.
        /// </summary>
        /// <param name="_interDef"> The definition of the interaciton being created </param>
        /// <param name="_interaction"> The interaction in process of creation </param>
        private static void CreateDependenciesForInteraction(S_InteractionDefinition _interDef, GameObject _interaction)
        {
            int numConditions = _interDef.ConditionsAndDependencies.Count;
            for (int i = 0; i < numConditions; i++)
            {
                S_AbstractCondition tempCondition = _interDef.ConditionsAndDependencies[i];
                // We need to make sure that the object is a dependency (a dependency is a condition as well)
                if (tempCondition is SC_InteractionDependencies)
                {
                    // Create the final action using the temporary condition type
                    Component comp = _interaction.AddComponent(tempCondition.GetType());

                    // Cast to get the S_AbstractCondition of the action
                    S_AbstractCondition finalCondition = ((S_AbstractCondition)comp);

                    // Copy the temp condition data to the final data
                    finalCondition.creationData = new List<object>(tempCondition.creationData);

                    // Setup te metadata to have a guid and a creation timestamp
                    finalCondition.InitializeMetadata();

                    // Setupt the final condition
                    finalCondition.SetupUsingApi(_interaction);
                }
            }
        }

        /// <summary>
        /// Creates all the actions defined on the S_Interaction definitions
        /// </summary>
        /// <param name="_interDef"> Interaction definition object </param>
        /// <param name="_interaction"> Interaction </param>
        private static void CreateActionsForInteraction(S_InteractionDefinition _interDef, GameObject _interaction)
        {
            int numActions = _interDef.Actions.Count;
            for (int i = 0; i < numActions; i++)
            {
                // Get the temporary action
                S_AbstractAction tempAction = _interDef.Actions[i];

                // Create the final action using the temporary aciton type
                Component comp = _interaction.AddComponent(tempAction.GetType());

                // Cast to get the S_AbstractAction of the action
                S_AbstractAction finalAction = ((S_AbstractAction)comp);

                // Copy the temp action data to the final data
                finalAction.creationData = new List<object>(tempAction.creationData);

                // Setup te metadata to have a guid and a creation timestamp
                finalAction.InitializeMetadata();

                // Setupt the final action
                finalAction.SetupUsingApi(_interaction);
            }
        }

        /// <summary>
        /// Scale all the colliders to the mesh size
        /// </summary>
        /// <param name="_io"></param>
        private static void ScaleCollidersToMesh(S_InteractiveObject _io)
        {
            // If the Game object doesn't contain a renderer just stop, the bounding collider won't be correct
            if(_io.gameObject.GetComponentInChildren<Renderer>() == null)
            {
                return;
            }

            BoxCollider boundingCollider = S_Utils.BoundColliderToMeshes(_io.gameObject);

            BoxCollider proximityCollider = _io.GetComponentInChildren<S_Proximity>().GetComponent<BoxCollider>();
            BoxCollider manipulationCollider = _io.GetComponentInChildren<S_Manipulation>().GetComponent<BoxCollider>();
            BoxCollider gaze = _io.GetComponentInChildren<S_Gaze>().GetComponent<BoxCollider>();
            BoxCollider handHover = _io.GetComponentInChildren<S_HandHover>().GetComponent<BoxCollider>();
            
            MoveRigColliderToVisualCenter(boundingCollider, proximityCollider, manipulationCollider, gaze, handHover);
        }

        /// <summary>
        /// Makes all the colliders match with the position and scale of the visuals colliders
        /// </summary>
        /// <param name="_visualsCollider">The collider wich is surrounding all the visuals</param>
        /// <param name="_colliders">The list of colliders that will be resized</param>
        private static void MoveRigColliderToVisualCenter(BoxCollider _visualsCollider, params BoxCollider[] _colliders)
        {
            for(int i = 0; i < _colliders.Length; i++)
            {
                BoxCollider collider = _colliders[i];
                collider.size = _visualsCollider.size;
                collider.center = _visualsCollider.center;
            }
        }

        /// <summary>
        /// Sets the object given as a parameter as visuals of the io in process of creation and copies its transform
        /// if necessary.
        /// </summary>
        /// <param name="_visuals"> The object that will represent the visuals of the io </param>
        /// <param name="_io"> The io in process of creation</param>
        private static void AddNestedVisuals(GameObject _visuals, S_InteractiveObject _io)
        {
            S_InteractiveObjectVisuals visualsParent = _io.GetComponentInChildren<S_InteractiveObjectVisuals>();
            _visuals.transform.SetParent(visualsParent.transform);
        }
    }
}
