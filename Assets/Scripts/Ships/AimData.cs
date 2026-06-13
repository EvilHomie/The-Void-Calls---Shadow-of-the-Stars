using System;
using UnityEngine;
using Weapons;

namespace Ships
{
    [Serializable]
    public class AimData
    {
        public Rigidbody2D TargetRigidBody;
        public Vector2 AimPosition;
        public WeaponBase FastetsBoltWeapon;
        public float FastestBoltSpeed;
    }
}