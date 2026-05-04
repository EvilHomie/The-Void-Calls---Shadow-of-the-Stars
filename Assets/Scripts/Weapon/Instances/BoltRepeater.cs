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

        public BoltWeaponStatsData Stats;
        public override ref WeaponBaseStats BaseStats => ref Stats.BaseStats;

        public override void Init()
        {
            Stats.ShootDelay = 1 / Stats.FireRate;
            Stats.InvProjectileSpeed = 1 / Stats.ProjectileSpeed;
        }

        public void SetShipRigidbody(Rigidbody2D rb)
        {
            ShipRigidBody = rb;
        }
    }
}