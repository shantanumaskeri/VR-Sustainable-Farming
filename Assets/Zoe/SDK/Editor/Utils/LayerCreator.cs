using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SpatialStories
{
    [InitializeOnLoad]
    class LayerCreator
    {
        static LayerCreator()
        {
            CreateLayersIfNeeded();
        }

        private static bool CreateLayerIfNeeded(string _layerName, SerializedProperty _layersProperty)
        {
            if (LayerMask.NameToLayer(_layerName) != -1)
            {
                return false;
            }

            int availableLayerSlot = FindNextAvailableLayerSlot(_layersProperty);
            if (availableLayerSlot == -1)
            {
                throw new IndexOutOfRangeException(string.Format("There is no layer slot available to create {0}, please delete some unused layer(s) to make room for {0}.", _layerName));
            }

            SerializedProperty layerProperty = _layersProperty.GetArrayElementAtIndex(availableLayerSlot);

            layerProperty.stringValue = _layerName;

            return true;
        }

        private static int FindNextAvailableLayerSlot(SerializedProperty _layersProperty)
        {
            // we start at 8, 8 being the first slot for User layer ID since the builtin ones cannot be modified
            for (int i = 8; i < _layersProperty.arraySize; i++)
            {
                SerializedProperty layerProperty = _layersProperty.GetArrayElementAtIndex(i);

                string layerName = layerProperty.stringValue;

                if (!string.IsNullOrEmpty(layerName))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        private static List<string> CreateLayersIfNeeded()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProperty = tagManager.FindProperty("layers");

            List<string> createdLayerNames = new List<string>();

            try
            {
                foreach (string layer in S_UserLayers.AllLayers)
                {
                    if (CreateLayerIfNeeded(layer, layersProperty))
                    {
                        createdLayerNames.Add(layer);
                    }
                }
            }
            catch (IndexOutOfRangeException _exception)
            {
                EditorUtility.DisplayDialog("Error while creating layers",
                    string.Format("{0}\n\nIf you are running your application, it may not run correctly.", _exception.Message),
                    "OK");

                throw _exception;
            }

            if (createdLayerNames.Count != 0)
            {
                string formattedCreatedLayerList = "";
                foreach (string createdLayerName in createdLayerNames)
                {
                    formattedCreatedLayerList += string.Format("- {0}\n", createdLayerName);
                }

                EditorUtility.DisplayDialog("New layers created!",
                    string.Format("The following required layers have been added to your existing layer list:\n" +
                    "{0}", formattedCreatedLayerList),
                    "OK");
            }

            tagManager.UpdateIfRequiredOrScript();
            tagManager.ApplyModifiedProperties();

            return createdLayerNames;
        }

        [MenuItem("Zoe/Utils/Create Required Layers")]
        private static void CreateLayersRequestFromMenu()
        {
            List<string> createdLayers = CreateLayersIfNeeded();

            if (createdLayers.Count == 0)
            {
                EditorUtility.DisplayDialog("No layers created",
                    "No layers needed creation.",
                    "OK");
            }
        }
    }
}
