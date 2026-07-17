using UnityEngine;

namespace Projectiles
{
    public class HomingMissile : ProjectileBase
    {
        public Rigidbody2D Target;

        public float AccelerationSpeed;
        public float BrakingSpeed;
        public float RotateSpeed;

        protected override void OnResolveDependencies()
        {
            ProjectileType = ProjectileType.HomingMissile;
        }
    }
}