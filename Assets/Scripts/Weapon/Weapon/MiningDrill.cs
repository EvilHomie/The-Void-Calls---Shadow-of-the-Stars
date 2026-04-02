using UnityEngine;

namespace Weapons
{
    public class MiningDrill : WeaponBase
    {
        public override WeaponType WeaponType => WeaponType.MiningDrill;
        public Vector3 HitPos;
        public float NextHitTime;
        public float HitDelay;
        [field: SerializeField] public LineRenderer BeamLineLR { get; private set; }
        [field: SerializeField] public GameObject BeamLineGO { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        [field: SerializeField] public ParticleSystem HitSpotPS { get; private set; }
        [field: SerializeField] public Transform HitSpotT { get; private set; }
        [field: SerializeField] public float HitRate { get; private set; }

        private void Awake()
        {
            HitDelay = 1 / HitRate;
        }
    }
}