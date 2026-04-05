using System;

namespace Ship
{
    [Serializable]
    public struct ShipMovementData
    {
        public float Throttle;
        public bool InertiaDamping;
        public float RotatePowerValue;
        public float DirectAccelerationPower;
        public float SideAcceleration;

        public float DirectMaxSpeed;
        public float DirectAcceleration;
        public float ReverseMaxSpeed;
        public float ReverseAcceleration;
        public float StrafeMaxSpeed;
        public float StrafeAcceleration;
        public float RotateSpeed;
        public float TargetSpeed;

        public bool InertiaDampingLastState;
        public float LastThrottleWithDamping;
    }
}