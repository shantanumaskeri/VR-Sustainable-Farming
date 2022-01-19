using UnityEngine;

namespace SpatialStories
{
    public class S_CloningUtils
    {
        public static Vector3 Clone(Vector3 _vector)
        {
            Vector3 output = new Vector3();
            output.x = _vector.x;
            output.y = _vector.y;
            output.z = _vector.z;
            return output;
        }

        public static Quaternion Clone(Quaternion _quaternion)
        {
            Quaternion output = new Quaternion();
            output.x = _quaternion.x;
            output.y = _quaternion.y;
            output.z = _quaternion.z;
            output.w = _quaternion.w;
            return output;
        }
    }
}
