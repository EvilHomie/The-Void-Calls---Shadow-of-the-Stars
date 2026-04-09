using System;

namespace Ships
{
    [Serializable]
    public struct ChassisData
    {
        public Size Size;
        public float Mass;
        public float DirectDrag;
        public float ReverseDrag;
        public float StrafeDrag;
        public float RotateDrag;

        public float DirectMaxSpeed;
        public float DirectMaxAcceleration;
        public float ReverseMaxSpeed;
        public float ReverseMaxAcceleration;
        public float StrafeMaxSpeed;
        public float StrafeMaxAcceleration;
        public float RotateMaxSpeed;
    }
}