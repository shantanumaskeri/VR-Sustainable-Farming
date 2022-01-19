namespace SpatialStories
{
    public static class S_StringUtils
    {
        // -------------------------------------------
        /* 
		 * Will trim a string to fit a maximum number of characters
		 */
        public static string Trim(string _value, int _maxChars = 8)
        {
            if (_value.Length > _maxChars)
            {
                return _value.Substring(0, _maxChars);
            }
            else
            {
                return _value;
            }
        }

        // -------------------------------------------
        /* 
		 * Check if the string can be converted to a number
		 */
        public static bool IsInteger(string _value)
        {
            int valueInteger = -1;
            if (int.TryParse(_value, out valueInteger))
            {
                return true;
            }

            return false;
        }


        // -------------------------------------------
        /* 
		 * Check if the string can be converted to a number
		 */
        public static bool IsFloat(string _value)
        {
            float valueFloat = -1;
            if (float.TryParse(_value, out valueFloat))
            {
                return true;
            }

            return false;
        }

        // -------------------------------------------
        /* 
		 * Check if the string can be converted to a number
		 */
        public static bool IsDouble(string _value)
        {
            float valueDouble = -1;
            if (float.TryParse(_value, out valueDouble))
            {
                return true;
            }

            return false;
        }

        // -------------------------------------------
        /* 
		 * Check if the string can be converted to a vector3
		 */
        public static bool IsVector3(string _value)
        {
            float valueFloat = -1;
            string[] vector = _value.Split(',');
            if (vector.Length == 3)
            {
                for (int i = 0; i < vector.Length; i++)
                {
                    if (!float.TryParse(vector[i], out valueFloat))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
