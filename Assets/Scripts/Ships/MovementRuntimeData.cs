using System;
using UnityEngine;

namespace Ships
{
    [Serializable]
    public struct MovementRuntimeData
    {  
        public float Throttle;
        public float TargetSpeed;
        public float LastThrottleWithDamping;
        public bool InertiaDampingState;
        public float RotatePower;
        public float DirectMovePower;
        public float StrafeMovePower;        

        public Vector2 LinearVelocity;
        public float AngularVelocity;
        public float Rotation;
    }
}