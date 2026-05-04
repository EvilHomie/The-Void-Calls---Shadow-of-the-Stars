namespace Weapons
{
    public class BeamEmitter : WeaponBase
    {
        public override WeaponType WeaponType => WeaponType.BeamEmitter;

        public BeamEmitterStatsData Stats;
        public override ref WeaponBaseStats BaseStats => ref Stats.BaseStats;

        public override void Init()
        {
        }
    }
}

