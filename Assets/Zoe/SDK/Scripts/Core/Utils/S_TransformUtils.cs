using UnityEngine;

namespace SpatialStories
{
    public class S_TransformUtils
    {
        // -------------------------------------------
        /* 
		 * Returns true if _go is a child of _target
		 */
        public static bool IsChildOf(GameObject _rootGameObjectToSearch, GameObject _gameObjectToFind)
        {
            if (_rootGameObjectToSearch == _gameObjectToFind)
            {
                return true;
            }

            foreach (Transform child in _rootGameObjectToSearch.transform)
            {
                if (child == _gameObjectToFind)
                {
                    return true;
                }
            }

            return false;
        }


        // -------------------------------------------
        /* 
		 * Will look fot the gameobject in the childs
		 */
        public static bool FindGameObjectInChilds(GameObject _go, GameObject _target)
        {
            if (_go == _target)
            {
                return true;
            }
            bool output = false;
            foreach (Transform child in _go.transform)
            {
                output = output || FindGameObjectInChilds(child.gameObject, _target);
            }
            return output;
        }

        // -------------------------------------------
        /* 
		 * Will look fot the gameobject in the parent
		 */
        public static bool FindGameObjectInParent(GameObject _go, GameObject _target)
        {
            if (_go == _target)
            {
                return true;
            }
            bool output = false;
            if (_go.transform.parent != null)
            {
                output = output || FindGameObjectInParent(_go.transform.parent.gameObject, _target);
            }
            return output;
        }
    }
}
