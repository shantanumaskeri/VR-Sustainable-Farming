using UnityEngine;

public static class ComponentExtensions
{
    public static T GetComponentInParent<T>(this Component _component, bool _includeInactive = false) where T : Component
    {
        if (!_includeInactive)
        {
            return _component.GetComponentInParent<T>();
        }

        T[] foundComponents = _component.GetComponentsInParent<T>(_includeInactive);

        if (foundComponents.Length == 0)
        {
            return null;
        }

        return foundComponents[0];
    }
}
