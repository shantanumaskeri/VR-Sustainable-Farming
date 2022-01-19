using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace SpatialStories
{
    [InitializeOnLoad]
    public class S_MetadataSystem
    {
        private static Dictionary<string, S_AbstractEntity> m_AlreadyRegisteredEntities = new Dictionary<string, S_AbstractEntity>();
        private static int m_LastEntitiesLenght;

        static S_MetadataSystem()
        {
            EditorApplication.hierarchyWindowChanged += OnEditorChanged;
        }
        
        /// <summary>
        /// Fired every time that an editor event has been produced
        /// </summary>
        private static void OnEditorChanged()
        {
            S_AbstractEntity[] allEntities = Object.FindObjectsOfType<S_AbstractEntity>();
            if (m_LastEntitiesLenght == allEntities.Length)
                return;
            
            InitializeMetadataIfNeeded(allEntities);
            ValidateCopyPasteEvents(allEntities);
            PurgeRegisteredEntitiesDictionaryIfNeeded(allEntities);
            m_LastEntitiesLenght = allEntities.Length;
        }
        
        /// <summary>
        /// Checks if there are some objects on the scene that doesn't contain metadata
        /// </summary>
        /// <param name="_allEntities"></param>
        private static void InitializeMetadataIfNeeded(S_AbstractEntity[] _allEntities)
        {
            for (int i = 0; i < _allEntities.Length; i++)
            {
                S_AbstractEntity entity = _allEntities[i];
                if (!entity.IsMetadataInitialized())
                {
                    entity.InitializeMetadata();
                    Debug.Log("S_MetadataSystem > Entity: " + entity.name + "'s metadata initialized!");
                }
            }
        }

        /// <summary>
        /// Checks if the user has performed a copy paste in order to keep the
        /// consistency on the metadata
        /// </summary>
        /// <param name="_allEntities"></param>
        public static void ValidateCopyPasteEvents(S_AbstractEntity[] _allEntities)
        {
            for(int i = 0; i < _allEntities.Length; i++)
            {
                // Get the testing entity
                S_AbstractEntity testingEntity = _allEntities[i];

                // If the registered entities dictionary contains the registered entity
                if (m_AlreadyRegisteredEntities.ContainsKey(testingEntity.Metadata.GUID))
                {
                    S_AbstractEntity registeredEntity = m_AlreadyRegisteredEntities[testingEntity.Metadata.GUID];
                    // If the registered entity is not the testing one we have detected a duplication
                    if (testingEntity != registeredEntity)
                    {
                        // Register the testing entity with a new metadata
                        testingEntity.InitializeMetadata();
                        m_AlreadyRegisteredEntities.Add(testingEntity.Metadata.GUID, testingEntity);
                    }
                }
                else
                {
                    // If the entity is not there is because we need to register it
                    m_AlreadyRegisteredEntities.Add(testingEntity.Metadata.GUID, testingEntity);
                }
            }
        }

        /// <summary>
        /// /Removed all the deleted entities from the dictionary in order to avoid 
        /// null pointer exceptions
        /// </summary>
        /// <param name="_allEntities"></param>
        private static void PurgeRegisteredEntitiesDictionaryIfNeeded(S_AbstractEntity[] _allEntities)
        {
            if(_allEntities.Length < m_AlreadyRegisteredEntities.Count)
            {
                var removedEntities = (from kv in m_AlreadyRegisteredEntities
                                        where kv.Value == null
                                        select kv).ToDictionary(kv => kv.Key, kv => kv.Value);

                foreach (string toDelete in removedEntities.Keys)
                {
                    m_AlreadyRegisteredEntities.Remove(toDelete);
                }
            }
        }
    }
}

