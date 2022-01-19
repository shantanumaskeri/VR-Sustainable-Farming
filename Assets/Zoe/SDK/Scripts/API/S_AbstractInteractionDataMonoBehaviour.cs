using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    /// Holds the data necessary to create conditions, actions or dependencies during the
    /// creationg process of an Interactive Object using the API.
    /// a monobehaivour too, for now is used for actions and is an exact copy of S_AbstractInteractionComponentData
    public abstract class S_AbstractInteractionDataMonoBehaviour : S_AbstractEntity
    {

        public List<object> creationData = new List<object>();
        public abstract void SetupUsingApi(GameObject _interaction);

        public bool LogoEitherHandValue = false;
        public bool LogoLeftHandValue = false;
        public bool LogoRigthHandValue = false;
        public bool LogoBothHandsValue = false;

        /// <summary>
        /// Set the selected hand
        /// </summary>
        public void SetHands(bool _LogoEitherHandValue, bool _LogoLeftHandValue, bool _LogoRigthHandValue, bool _LogoBothHandsValue)
        {
            LogoEitherHandValue = _LogoEitherHandValue;
            LogoLeftHandValue = _LogoLeftHandValue;
            LogoRigthHandValue = _LogoRigthHandValue;
            LogoBothHandsValue = _LogoBothHandsValue;
        }

        /// <summary>
        /// Check if there is any hand selected
        /// </summary>
        protected bool NoButtonSelected()
        {
            return !LogoEitherHandValue && !LogoLeftHandValue && !LogoRigthHandValue && !LogoBothHandsValue;
        }

        /// <summary>
        /// Needed in order to create conditions and dependencies that will be lazy configured.
        /// </summary>
        public S_AbstractInteractionDataMonoBehaviour() { }

        /// <summary>
        /// Adds some creation data in order to setup the
        /// condition / dependency in the moment of its 
        /// creation
        /// </summary>
        /// <param name="_data">Data required to setup the condition</param>
        public void AddCreationData(params object[] _data)
        {
            creationData.AddRange(_data);
        }

        /// <summary>
        /// Modifies the creation data stored in the specified index
        /// </summary>
        /// <param name="_data"> The new creation data </param>
        /// <param name="index"> Index to replace </param>
        public void ModifyCreationDataByIndex(object _data, int _index)
        {
            if (_index < 0 || _index >= creationData.Count)
            {
                Debug.Log(string.Format("SpatialStoriesAPI > The specified index {0} is not correct.", _index));
            }
            else
            {
                creationData[_index] = _data;
            }
        }

        /// <summary>
        /// Clears all the creation data of a condition / dependency
        /// </summary>
        public void ClearCreationData()
        {
            creationData.Clear();
        }
    }

}
