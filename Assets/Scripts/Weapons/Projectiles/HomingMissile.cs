using UnityEngine;

namespace Projectiles
{
    public class HomingMissile : ProjectileBase
    {
        public override ProjectileType ProjectileType => ProjectileType.HomingMissile;

        public Transform TargetTransform;

        public float AccelerationSpeed;
        public float BrakingSpeed;
        public float RotateSpeed;
    }
}