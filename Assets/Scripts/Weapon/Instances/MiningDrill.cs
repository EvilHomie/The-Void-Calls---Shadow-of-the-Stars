using UnityEngine;

namespace Weapons
{
    public class MiningDrill : WeaponBase
    {
        public override WeaponType WeaponType => WeaponType.MiningDrill;
        [field: SerializeField] public LineRenderer BeamLineLR { get; private set; }
        [field: SerializeField] public Transform BeamLineTransform { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }

        public MiningDrillStatsData WeaponStats;
        public DamageData DamageData;

        public override ref AimStatsData AimStats => ref WeaponStats.BaseStats.AimStats;
        public override ref DamageData Damage => ref DamageData;
        public override ref BaseDamageData BaseDamage => ref WeaponStats.BaseStats.BaseDamage;
        public override ref DamageMultipliersData DamageMultipliers => ref WeaponStats.BaseStats.DamageMultipliers;
    }
}