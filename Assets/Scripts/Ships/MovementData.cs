using System;

namespace Ships
{
    [Serializable]
    public struct MovementData
    {  
        public float Throttle;
        public float TargetSpeed;
        public float LastThrottleWithDamping;
        public bool InertiaDampingState;
        public float RotatePower;
        public float DirectMovePower;
        public float StrafeMovePower;
    }
}