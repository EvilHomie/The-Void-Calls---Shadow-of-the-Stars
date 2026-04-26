using UnityEngine;

namespace Weapons
{
    public class MiningDrill : WeaponBase
    {
        public override WeaponType WeaponType => WeaponType.MiningDrill;
        public Vector3 HitPos;
        public float NextHitTime;
        public float HitDelay { get; private set; }
        [field: SerializeField] public LineRenderer BeamLineLR { get; private set; }
        [field: SerializeField] public GameObject BeamLineGO { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        [field: SerializeField] public ParticleSystem HitSpotPS { get; private set; }
        [field: SerializeField] public Transform HitSpotT { get; private set; }
        [field: SerializeField] public float HitRate { get; private set; }

        public override void Init()
        {
            HitDelay = 1 / HitRate;
        }
    }
}