using System;

namespace Ships
{
    [Serializable]
    public struct MovementRuntimeData
    {  
        public float DirectThrottle;
        public float MainEnginePower;

        public float StrafeThrottle;
        public float StrafePower;

        public float RotatePower;

        public bool InertiaDampingIsActive;
        public float LastDampingThrottle;

        public bool BoostersIsActive;
        public float BoostersPower;
    }
}