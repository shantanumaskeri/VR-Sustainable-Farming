using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    /// <summary>
    /// Stores all data necesary for the creation of an Interactive Object
    /// </summary>
    public class S_IODefinition : S_AbstractDefinition
    {
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 Scale { get; private set; } = new Vector3(1, 1, 1); 
        public GameObject Visuals { get; private set; }
        public S_ManipulationModes ManipulationMode { get; private set; }
        public bool AffectedByGravity { get; private set; } = false;

        public List<S_InteractionDefinition> Interactions { get; private set; }

        /// <summary>
        /// Creates an io definition using a gameobject as visuals, a position
        /// a rotation and a scale
        /// </summary>
        /// <param name="_visuals">Object to use for the visuals</param>
        /// <param name="_position">The starting position</param>
        /// <param name="_rotation">The starting rotation</param>
        /// <param name="_scale">The starting scale</param>
        public S_IODefinition(GameObject _visuals, Vector3 _position, Quaternion _rotation, Vector3 _scale) :
            base (_visuals.name)
        {
            // Since we provide the position, rotation and scale
            // we do not need to save the ones from the visuals
            // (we probably never want that) so pass false
            SetVisuals(_visuals, false);
            Position = _position;
            Rotation = _rotation;
            Scale = _scale;
            Interactions = new List<S_InteractionDefinition>();
            ManipulationMode = S_ManipulationModes.NONE;
        }

        /// <summary>
        /// Creates an io definition using a game object as basis, the _visuals GameObject
        /// will be nested under the visuals game object after the io creation
        /// </summary>
        /// <param name="_visuals"> Object that will be used as visuals of the new Interactive Object </param>
        /// <param name="_usePositionRotationAndScale"> If true the created interactive object will use the position
        /// the rotation and the scale of the _visuals object as the starting position, rotation and scale</param>
        public S_IODefinition(GameObject _visuals, bool _usePositionRotationAndScale) : base(_visuals.name)
        {
            SetVisuals(_visuals, _usePositionRotationAndScale);
            Interactions = new List<S_InteractionDefinition>();
            ManipulationMode = S_ManipulationModes.NONE;
        }

        /// <summary>
        /// Create an Interactive Object definition object.
        /// </summary>
        /// <param name="_name"> The new object's name </param>
        public S_IODefinition(string _name) : base(_name)
        {
            Interactions = new List<S_InteractionDefinition>();
            ManipulationMode = S_ManipulationModes.NONE;
        }

        /// <summary>
        /// Defines where the new interactive object will be place
        /// </summary>
        /// <param name="_newPosition">Desired position of the game object in world coordinates</param>
        /// <returns>A reference to the same io definition. (usefull for concatenate method calls) </returns>
        public S_IODefinition SetPosition(Vector3 _newPosition)
        {
            Position = _newPosition;
            return this;
        }

        /// <summary>
        /// Defines the initial rotation of the new interactive object
        /// </summary>
        /// <param name="_rotation"> Desired Rotation </param>
        /// <returns>A reference to the same io definition. (usefull for concatenate method calls) </returns>
        public S_IODefinition SetRotation(Vector3 _rotation)
        {
            Rotation = Quaternion.Euler(_rotation);
            return this;
        }

        /// <summary>
        /// Defines the initial rotation of the new interactive object
        /// </summary>
        /// <param name="_rotation"> Desired Rotation </param>
        /// <returns>A reference to the same io definition. (usefull for concatenate method calls) </returns>
        public S_IODefinition SetRotation(Quaternion _rotation)
        {
            Rotation = _rotation;
            return this;
        }

        /// <summary>
        /// Defines the initial scale of the new interactive object
        /// </summary>
        /// <param name="_rotation"> Desired Scale </param>
        /// <returns>A reference to the same io definition. (usefull for concatenate method calls) </returns>
        public S_IODefinition SetScale(Vector3 _newScale)
        {
            Scale = _newScale;
            return this;
        }

        /// <summary>
        /// Defines how the object can be manipulated
        /// </summary>
        /// <param name="_manipulationMode"> New manipulation mode for the io </param>
        /// <returns></returns>
        public S_IODefinition SetManipulationMode(S_ManipulationModes _manipulationMode)
        {
            ManipulationMode = _manipulationMode;
            return this;
        }

        /// <summary>
        /// Is the object affected by gravity
        /// </summary>
        /// <param name="_affectedByGravity"></param>
        /// <returns></returns>
        public S_IODefinition SetIsAffectedByGravity(bool _affectedByGravity)
        {
            AffectedByGravity = _affectedByGravity;
            return this;
        }

        /// <summary>
        /// Sets the game object that will represent the visuals of the new interactive 
        /// object.
        /// </summary>
        /// <param name="_visuals">The visuals game object</param>
        /// <param name="_usePositionRotationAndScale"> If true the created interactive object will use the position
        /// the rotation and the scale of the _visuals object as the starting position, rotation and scale </param>
        /// <returns>A reference to the same io definition. (usefull for concatenate method calls) </returns>
        public S_IODefinition SetVisuals(GameObject _visuals, bool _usePositionRotationAndScale = true)
        {
            Visuals = _visuals;
            if (_usePositionRotationAndScale)
            {
                Position = _visuals.transform.position;
                Rotation = _visuals.transform.rotation;
                Scale = _visuals.transform.localScale;
            }
            return this;
        }

        /// <summary>
        /// Create a new interaction definition for the game object, used to add new interactions
        /// </summary>
        /// <param name="_name"> Name for the new interaction </param>
        /// <returns>An interaction definition, that can be modified in order to create dependencies,
        /// conditions and action. </returns>
        public S_InteractionDefinition CreateInteractionDefinition(string _name)
        {
            S_InteractionDefinition definition = new S_InteractionDefinition(_name);
            Interactions.Add(definition);
            return definition;
        }

        /// <summary>
        ///  Sets custom metadata for the IO, usefull if you are using external storage systems, and the object
        /// needs to be identified after the application closes or after destoryed
        /// </summary>
        /// <param name="_guid"> A unique id for the object </param>
        /// <param name="_creationTime"> (Optional) When this objects has been created </param>
        /// <returns></returns>
        public S_IODefinition SetCustomMetadata(string _guid, string _creationTime = null)
        {
            Metadata.GUID = _guid;
            if(_creationTime != null)
                Metadata.CreationTime = _creationTime;
            return this;
        }
    }
}
