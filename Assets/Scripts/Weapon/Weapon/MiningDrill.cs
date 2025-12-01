using UnityEngine;

namespace Weapon
{
    public class MiningDrill : WeaponBase
    {
        public override WeaponType WeaponType => WeaponType.MiningDrill;
        [field: SerializeField] public Vector3 HitPos { get; set; }
        public float HitParticleAccumulator { get; set; }
        [field: SerializeField] public LineRenderer BeamLineLR { get; private set; }
        [field: SerializeField] public GameObject BeamLineGO { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        [field: SerializeField] public ParticleSystem HitSpotPS { get; private set; }
        [field: SerializeField] public Transform HitSpotT { get; private set; }
        [field: SerializeField] public float HitRate { get; private set; }
    }
}