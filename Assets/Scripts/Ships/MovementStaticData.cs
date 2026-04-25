using System;

namespace Ships
{
    [Serializable]
    public struct MovementStaticData
    {
        public float DirectMaxSpeed;
        public float DirectAcceleration;
        public float DirectDampingAcceleration;

        public float ReverseMaxSpeed;
        public float ReverseAcceleration;
        public float ReverseDampingAcceleration;

        public float StrafeMaxSpeed;
        public float StrafeAcceleration;
        public float StrafeDampingAcceleration;

        public float RotateSpeed;

        public float BoostersMaxSpeed;
        public float BoostersAcceleration;
        public float BoostersMaxPower;
    }
}