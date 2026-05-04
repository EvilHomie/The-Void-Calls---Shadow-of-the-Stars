using UnityEngine;

namespace Weapons
{
    public class MiningDrill : WeaponBase
    {
        public override WeaponType WeaponType => WeaponType.MiningDrill;
        [field: SerializeField] public LineRenderer BeamLineLR { get; private set; }
        [field: SerializeField] public GameObject BeamLineGO { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        [field: SerializeField] public ParticleSystem HitSpotPS { get; private set; }
        [field: SerializeField] public Transform HitSpotT { get; private set; }

        public Vector2 HitPos;
        public bool IsHit;

        public MiningDrillStatsData Stats;
        public override ref WeaponBaseStats BaseStats => ref Stats.BaseStats;
        public override void Init() { }
    }
}