using System;

namespace Ships
{
    [Serializable]
    public struct MovementRuntimeData
    {  
        public float Throttle;
        public float StrafePower;
        public float RotatePower;

        public bool InertiaDampingState;
        public float LastDampingThrottle;
    }
}