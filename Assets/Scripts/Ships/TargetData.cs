using System;
using UnityEngine;

namespace Ships
{
    [Serializable]
    public class TargetData
    {
        public Rigidbody2D TargetRigidBody;
        public Vector2 TargetPosition;
        public Vector2 TargetVelocity;
    }
}