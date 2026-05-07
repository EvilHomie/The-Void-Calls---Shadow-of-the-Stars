using System;

namespace Weapons
{
    [Serializable]
    public struct BoltWeaponStatsData
    {
        public WeaponBaseStats BaseStats;
        public BoltWeaponConfigStats Config;
        public BoltWeaponRuntimeStats Runtime;
        public BoltWeaponCachedStats Cached;
    }

    [Serializable]
    public struct BoltWeaponConfigStats
    {
        public float FireRate;
        public float SpreadAngle;
        public float ProjectileSpeed;
    }

    [Serializable]
    public struct BoltWeaponRuntimeStats
    {
        public float NextShootTime;
    }

    [Serializable]
    public struct BoltWeaponCachedStats
    {
        public float ShootDelay;
        public float InvProjectileSpeed;
    }
}