using UnityEngine;

namespace Weapons
{
    public class BoltRepeater : WeaponBase, IShipVelocityAware
    {
        public override WeaponType WeaponType => WeaponType.BoltRepeater;
        [field: SerializeField] PoolReference projectilePool;
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        [field: SerializeField] public Transform ShootPoint { get; private set; }
        public Rigidbody2D ShipRB { get; private set; }

        public BoltWeaponStatsData WeaponStats;
        public DamageData DamageData;

        public override ref AimStatsData AimStats => ref WeaponStats.BaseStats.AimStats;
        public override ref BaseDamageData BaseDamage => ref WeaponStats.BaseStats.BaseDamage;
        public override ref DamageMultipliersData DamageMultipliers => ref WeaponStats.BaseStats.DamageMultipliers;
        public override ref DamageData Damage => ref DamageData;

        public uint PoolId { get; private set; }

        public void SetShipRigidbody(Rigidbody2D rb)
        {
            ShipRB = rb;
            PoolId = projectilePool.Id;
        }
    }
}