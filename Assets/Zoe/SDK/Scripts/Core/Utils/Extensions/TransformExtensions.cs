using UnityEngine;

public static class TransformExtensions
{
    public static T GetComponentInDirectChildren<T>(this Transform _transform) where T : Component
    {
        foreach (Transform transform in _transform)
        {
            T component = transform.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }
}
