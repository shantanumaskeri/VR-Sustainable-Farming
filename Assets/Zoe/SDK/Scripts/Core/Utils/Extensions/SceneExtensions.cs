using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneExtensions
{
    public static T GetComponentInRootGameObjects<T>(this Scene _scene) where T : Component
    {
        foreach (GameObject rootGameObject in _scene.GetRootGameObjects())
        {
            T component = rootGameObject.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }
}
