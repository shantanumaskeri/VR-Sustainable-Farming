using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpatialStories
{
    /// <summary>
    /// All the spatial stories classes holds metadata about the objects
    /// </summary>
    public class S_AbstractEntity : MonoBehaviour
    {
        [SerializeField]
        public S_Metadata Metadata;

        public void InitializeMetadata()
        {
            Metadata = new S_Metadata(GenerateNewGUID(), DateTime.Now.ToString());
        }

        public static string GenerateNewGUID()
        {
            return Guid.NewGuid().ToString();
        }

        public bool IsMetadataInitialized()
        {
            return Metadata.GUID != null && Metadata.GUID != string.Empty && Metadata.GUID.Length > 0;
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            Event e = Event.current;

            if (e == null)
                return;

            if (e.type == EventType.ValidateCommand && e.commandName == "Paste")
            {
                Debug.Log("validate paste");
                e.Use(); // without this line we won't get ExecuteCommand
                // S_MetadataSystem.ValidateCopyPasteEvents();
            }

            if (e.type == EventType.ExecuteCommand && e.commandName == "Paste")
            {
                Debug.Log("Pasting: " + EditorGUIUtility.systemCopyBuffer);
                // S_MetadataSystem.ValidateCopyPasteEvents();
            }
        }
#endif
    }
}
