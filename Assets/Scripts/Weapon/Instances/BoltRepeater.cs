using UnityEngine;

namespace Weapons
{
    public class BoltRepeater : WeaponBase, IShipVelocityAware
    {
        public override WeaponType WeaponType => WeaponType.BoltRepeater;
        [field: SerializeField] public PoolReference PoolReference { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        [field: SerializeField] public Transform ShootPoint { get; private set; }
        [field: SerializeField] public float FireRate { get; private set; }
        [field: SerializeField] public float SpreadAngle { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; }
        [field: SerializeField] public Rigidbody2D ShipRigidBody { get; private set; }
        public float ShootDelay { get; private set; }

        public float NextShootTime;
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