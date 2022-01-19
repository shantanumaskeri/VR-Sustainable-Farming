
using System;

namespace SpatialStories
{
    [Serializable]
    public struct S_DependencyStruct
    {
        public S_Interaction ConditionsManager;

        public S_DependencyStruct(S_Interaction _manager)
        {
            ConditionsManager = _manager;
        }
    }
}