using UnityEngine;

namespace Weapons
{
    public class BoltRepeater : WeaponBase
    {
        public override WeaponType WeaponType => WeaponType.BoltRepeater;
        [field: SerializeField] public PoolDataSO ProjectilePoolData { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        [field: SerializeField] public Transform ShootPoint { get; private set; }
        [field: SerializeField] public float FireRate { get; private set; }
        [field: SerializeField] public float SpreadAngle { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; }
        [field: SerializeField] public Rigidbody2D ShipRigidBody { get; private set; }

        public float NextShootTime;
        public float ShootDelay;

        private void Awake()
        {
            ShootDelay = 1 / FireRate;
            ShipRigidBody = GetComponentInParent<Rigidbody2D>();
        }
    }
}