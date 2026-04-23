using System;

namespace Ships
{
    [Serializable]
    public struct MovementStaticData
    {
        public float DirectMaxSpeed;
        public float DirectAcceleration;

        public float ReverseMaxSpeed;
        public float ReverseAcceleration;

        public float StrafeMaxSpeed;
        public float StrafeAcceleration;

        public float RotateSpeed;
    }
}