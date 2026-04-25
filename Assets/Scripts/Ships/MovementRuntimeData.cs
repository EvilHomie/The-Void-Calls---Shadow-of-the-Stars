using System;

namespace Ships
{
    [Serializable]
    public struct MovementRuntimeData
    {  
        public float DirectThrottle;
        public float MainEnginePower;

        public float StrafeThrottle;
        public float ThrustersPower;

        public float RotatePower;

        public bool InertiaDampingActive;
        public float LastDampingThrottle;
    }
}