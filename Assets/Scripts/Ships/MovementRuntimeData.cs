using System;

namespace Ships
{
    [Serializable]
    public struct MovementRuntimeData
    {  
        public float Throttle;

        public float DirectPower;
        public float StrafePower;
        public float RotatePower;

        public bool InertiaDampingActive;
        public float LastDampingThrottle;
    }
}