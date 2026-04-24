using System;

namespace Ships
{
    [Serializable]
    public struct MovementRuntimeData
    {  
        public float DirectThrottle;
        public float DirectPower;

        public float StrafeThrottle;
        public float StrafePower;

        public float RotatePower;

        public bool InertiaDampingActive;
        public float LastDampingThrottle;
    }
}