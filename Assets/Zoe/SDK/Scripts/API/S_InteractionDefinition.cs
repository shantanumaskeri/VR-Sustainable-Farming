using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    /// <summary>
    /// Stores all the necessary information about an interaction such as
    /// name, dependencies, conditions and actions.
    /// </summary>
    public class S_InteractionDefinition : S_AbstractDefinition
    {
        public List<S_AbstractCondition> ConditionsAndDependencies;
        public List<S_AbstractAction> Actions;

        public S_InteractionDefinition(string _name) : base(_name)
        {
            ConditionsAndDependencies = new List<S_AbstractCondition>();
            Actions = new List<S_AbstractAction>();
        }

        /// <summary>
        /// Creates a new condition or dependency object for the interaction, then the user needs to
        /// populate it with the correct data.
        /// </summary>
        /// <typeparam name="T">An objects that inherits from S_AbstractCondition and has an empty
        /// constructor (0 arguments)</typeparam>
        /// <returns> The specified condition / dependency ready to be configured </returns>
        public T CreateCondition<T>() where T : S_AbstractCondition, new()
        {
            T cond = new T();
            ConditionsAndDependencies.Add(cond);
            return cond;
        }

        /// <summary>
        /// Creates a new condition or dependency object for the interaction and adds some initial data to it
        /// </summary>
        /// <typeparam name="T">An objects that inherits from S_AbstractCondition and has an empty
        /// constructor (0 arguments)</typeparam>
        /// <param name="_data">Initial data for the condition / dependency </param>
        /// <returns> The specified condition / dependency ready to be configured </returns>
        public T CreateCondition<T>(params object[] _data) where T : S_AbstractCondition, new()
        {
            T cond = new T();
            ConditionsAndDependencies.Add(cond);
            cond.AddCreationData(_data);
            return cond;
        }

        /// <summary>
        /// Creates an action ready with its initial data
        /// </summary>
        /// <typeparam name="T">A S_AbstractAction</typeparam>
        /// <param name="_data">Initial data for the action</param>
        /// <returns>An action ready to use</returns>
        public T CreateAction<T>(params object[] _data) where T : S_AbstractAction, new()
        {
            T action = new T();
            Actions.Add(action);
            action.AddCreationData(_data);
            return action;
        }

        /// <summary>
        /// Creates an action ready to setup
        /// </summary>
        /// <typeparam name="T">A S_AbstractAction</typeparam>
        /// <returns>An action ready to setup</returns>
        public T CreateAction<T>() where T : S_AbstractAction, new()
        {
            T action = new T();
            Actions.Add(action);
            return action;
        }

        /// <summary>
        /// Sets custom metadata for the Interaction, usefull if you are using external storage systems, and the object
        /// needs to be identified after the application closes or after destoryed
        /// </summary>
        /// <param name="_guid"> A unique id for the object </param>
        /// <param name="_creationTime"> (Optional) When this objects has been created </param>
        /// <returns></returns>
        public S_InteractionDefinition SetCustomMetadata(string _guid, string _creationTime = null)
        {
            Metadata.GUID = _guid;
            if(_creationTime != null)
                Metadata.CreationTime = _creationTime;
            return this;
        }

    }
}