using UnityEngine;

namespace Weapons
{
    public class BoltRepeater : WeaponBase, IShipVelocityAware
    {
        public override WeaponType WeaponType => WeaponType.BoltRepeater;
        [field: SerializeField] public PoolReference PoolReference { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        [field: SerializeField] public Transform ShootPoint { get; private set; }
        public Rigidbody2D ShipRigidBody { get; private set; }

        public float FireRate;
        public float SpreadAngle;
        public float ProjectileSpeed;

        public float NextShootTime;
        public float ShootDelay;
        public float InvProjectileSpeed;

        public override void Init()
        {
            ShootDelay = 1 / FireRate;
            InvProjectileSpeed = 1 / ProjectileSpeed;
        }

        public void SetShipRigidbody(Rigidbody2D rb)
        {
            ShipRigidBody = rb;
        }
    }
}