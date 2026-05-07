namespace Weapons
{
    public class BeamEmitter : WeaponBase
    {
        public override WeaponType WeaponType => WeaponType.BeamEmitter;

        public BeamEmitterStatsData WeaponStats;
        public DamageData DamageData;

        public override ref AimStatsData AimStats => ref WeaponStats.BaseStats.AimStats;
        public override ref DamageData Damage => ref DamageData;
        public override ref BaseDamageData BaseDamage => ref WeaponStats.BaseStats.BaseDamage;
        public override ref DamageMultipliersData DamageMultipliers => ref WeaponStats.BaseStats.DamageMultipliers;
    }
}

