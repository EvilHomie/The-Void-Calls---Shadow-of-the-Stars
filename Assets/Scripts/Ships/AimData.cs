using System;
using UnityEngine;

namespace Ships
{
    [Serializable]
    public class AimData
    {
        public Rigidbody2D TargetRigidBody;
        public Vector2 AimPosition;
        public float FastestProjectileSpeed;
    }
}