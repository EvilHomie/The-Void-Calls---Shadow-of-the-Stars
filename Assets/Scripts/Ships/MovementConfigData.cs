using System;

namespace Ships
{
    [Serializable]
    public struct MovementConfigData
    {
        public float DirectMaxSpeed;
        public float DirectMaxAcceleration;

        public float ReverseMaxSpeed;
        public float ReverseMaxAcceleration;

        public float StrafeMaxSpeed;
        public float StrafeMaxAcceleration;

        public float RotateMaxSpeed;
    }
}