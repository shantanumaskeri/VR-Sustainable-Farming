using UnityEngine;

namespace SpatialStories
{
    public class S_RaycastUtils
    {
        // ---------------------------------------------------
        /**
		 @brief We get the whole RaycastHit information of the collision, with the mask to consider
		 */
        public static bool GetRaycastHitInfoByRayWithMask(Vector3 _origin, Vector3 _forward, ref RaycastHit _hitCollision, params string[] _masksToConsider)
        {
            Vector3 fwd = new Vector3(_forward.x, _forward.y, _forward.z);
            fwd.Normalize();

            int layerMask = 0;
            if (_masksToConsider != null)
            {
                for (int i = 0; i < _masksToConsider.Length; i++)
                {
                    layerMask |= (1 << LayerMask.NameToLayer(_masksToConsider[i]));
                }
            }
            bool result = false;
            if (layerMask == 0)
            {
                result = Physics.Raycast(_origin, fwd, out _hitCollision, Mathf.Infinity);
            }
            else
            {
                result = Physics.Raycast(_origin, fwd, out _hitCollision, Mathf.Infinity, layerMask);
            }
            return result;
        }
    }
}
