using System;

namespace Weapons
{
    [Serializable]
    public struct BoltWeaponStatsData
    {
        public WeaponBaseStats BaseStats;
        public float FireRate;
        public float SpreadAngle;
        public float ProjectileSpeed;

        public float NextShootTime;
        public float ShootDelay;
        public float InvProjectileSpeed;
    }
}