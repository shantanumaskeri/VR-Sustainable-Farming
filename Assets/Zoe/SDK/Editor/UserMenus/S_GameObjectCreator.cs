using Gaze;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    public class S_GameObjectCreator : MonoBehaviour
    {
        private static void ParentAndUndo(GameObject go, GameObject parent = null)
        {
            GameObjectUtility.SetParentAndAlign(go, parent);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeGameObject = go;
        }

        /// <summary>
        /// Creates the base structure of a TriggerAnimAudio.
        /// </summary>
        public static GameObject CreateInteractiveObject()
        {
            return Instantiate(Resources.Load("Interactive Object") as GameObject);
        }

        public static GameObject CreateInteractiveCamera()
        {
            GameObject cam = new GameObject();
            cam.AddComponent<Camera>();
            ConvertInteractiveCamera(cam);

            return cam as GameObject;
        }

        /// <summary>
        /// Converts a GameObject into a Gazable.
        /// </summary>
        /// <returns>The Gazable Root.</returns>
        /// <param name="go">The reference GameObject</param>
        public static GameObject ConvertInteractiveObject(GameObject go)
        {
            Transform gameObjectToConvertParent = go.transform.parent;
            int gameObjectToConvertSiblingIndex = go.transform.GetSiblingIndex();

            // Bit of a hack, in the specific case of creating IOs through the menu
            // we want to copy the visuals transform into the IO root's transform
            // but we also want to reset the transform from the Visuals except its scale
            S_IODefinition ioDefinition = SpatialStoriesAPI.CreateIODefinition(go);
            ioDefinition.Visuals.transform.position = Vector3.zero;
            ioDefinition.Visuals.transform.rotation = Quaternion.identity;

            S_InteractiveObject io = SpatialStoriesAPI.CreateInteractiveObject(ioDefinition, APICreationOptions.INTERACTIVE_OBJECTS);

            if (gameObjectToConvertParent != null)
            {
                io.transform.parent = gameObjectToConvertParent;
            }

            io.transform.SetSiblingIndex(gameObjectToConvertSiblingIndex);

            return io.gameObject;
        }


        public static GameObject CovertCameraUsingPrefab(GameObject _go, string _prefabName)
        {
            Transform cameraToConvertParent = _go.transform.parent;
            int cameraToConvertSiblingIndex = _go.transform.GetSiblingIndex();

            // Load the interactivce camera
            GameObject InteractiveCamera = (GameObject)PrefabUtility.InstantiatePrefab(Resources.Load(_prefabName));

            InteractiveCamera.transform.position = _go.transform.position;

            if (cameraToConvertParent != null)
            {
                InteractiveCamera.transform.parent = cameraToConvertParent;
            }

            InteractiveCamera.transform.SetSiblingIndex(cameraToConvertSiblingIndex);

            DestroyImmediate(_go);

            return InteractiveCamera;
        }

        /// <summary>
        /// Converts a Standard Camera into a SpatialStories one.
        /// </summary>
        /// <returns>The Gazable Root.</returns>
        /// <param name="go">The reference GameObject</param>
        public static GameObject ConvertInteractiveCamera(GameObject go)
        {
            return CovertCameraUsingPrefab(go, "Camera (IO)");
        }

        public static void AdjustColliders(GameObject _selectedGameObject)
        {
            S_InteractiveObjectVisuals vis = _selectedGameObject.GetComponentInChildren<S_InteractiveObjectVisuals>();
            Transform t = vis.transform.GetChild(0);
            BoxCollider collider = null;
            if (t != null)
            {
                collider = t.gameObject.AddComponent<BoxCollider>();
            }
            BoxCollider[] colliders = _selectedGameObject.GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider c in colliders)
            {
                c.center = collider.center;
                c.size = collider.size;
            }
            GameObject.DestroyImmediate(collider);
        }

        [MenuItem("GameObject/Zoe/Convert into Interactive Object", false, 12)]
        public static void GameObjectMenuGameObjectConversion(MenuCommand _menuCommand)
        {
            Selection.activeGameObject = ConvertInteractiveObject(Selection.activeGameObject);
        }

        [MenuItem("Zoe/Convert/Into Interactive Object")]
        public static void MainMenuGameObjectConversion()
        {
            Selection.activeGameObject = ConvertInteractiveObject(Selection.activeGameObject);
        }

        [MenuItem("GameObject/Zoe/Convert into Interactive Camera", false, 12)]
        public static void GameObjectMenuCameraConversion(MenuCommand _menuCommand)
        {
            Selection.activeGameObject = ConvertInteractiveCamera(Selection.activeGameObject);
        }

        [MenuItem("Zoe/Convert/Into Interactive Camera")]
        public static void MainMenuCameraConversion()
        {
            Selection.activeGameObject = ConvertInteractiveCamera(Selection.activeGameObject);
        }

        [MenuItem("Zoe/Convert/Into Interactive Object", true)]
        public static bool ValidateMainMenuGameObjectConversion()
        {
            return ValidateGameObjectSelection();
        }

        [MenuItem("Zoe/Convert/Into Interactive Camera", true)]
        public static bool ValidateMainMenuCameraConversion()
        {
            return ValidateCameraSelection();
        }

        private static bool ValidateGameObjectSelection()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() == null;
        }

        public static bool ValidateCameraSelection()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() != null;
        }
    }
}
