using System;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public struct MiningDrillStatsData
    {
        public WeaponBaseStats BaseStats;
        public ConstantBeamWeaponConfigStats Config;
        public ConstantBeamWeaponRuntimeStats Runtime;
        public ConstantBeamWeaponCachedStats Cached;
    }

    [Serializable]
    public struct ConstantBeamWeaponConfigStats
    {
        public int HitRate;
    }

    [Serializable]
    public struct ConstantBeamWeaponRuntimeStats
    {
        public float NextHitTime;
    }

    [Serializable]
    public struct ConstantBeamWeaponCachedStats
    {
        public float HitDelay;
        public Vector2 NoHitTargetPoint;
    }
}