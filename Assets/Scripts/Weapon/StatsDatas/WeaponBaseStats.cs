using System;

namespace Weapons
{
    [Serializable]
    public struct WeaponBaseStats
    {
        public AimStatsData AimStats;
        public BaseDamageData BaseDamage;
        public DamageMultipliersData DamageMultipliers;
    }

    [Serializable]
    public struct AimStatsData
    {
        public float MaxDistance;
        public float MaxRotateAngle;
        public float RotateSpeed;
    }

    [Serializable]
    public struct BaseDamageData
    {
        public float Energy;
        public float Kinetic;
    }

    [Serializable]
    public struct DamageMultipliersData
    {
        public float Energy;
        public float Kinetic;
        public float Asteroid;
    }
}