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

        public float NextHitTime;
        public float HitRate;

        public Vector3 HitPos;
        public float HitDelay;

        public override void Init()
        {
            HitDelay = 1 / HitRate;
        }
    }
}