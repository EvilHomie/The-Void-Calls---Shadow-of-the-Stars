using UnityEngine;

namespace Weapon
{
    public class MiningDrill : WeaponBase
    {
        [field: SerializeField] public Vector3 HitPos { get; set; }
        public float HitParticleAccumulator { get; set; }
        public override WeaponType WeaponType => WeaponType.MiningDrill;

        [field: SerializeField] public LineRenderer BeamLine { get; private set; }
        [field: SerializeField] public GameObject BeamLineGO { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpot { get; private set; }
        [field: SerializeField] public ParticleSystem HitSpot { get; private set; }
        [field: SerializeField] public Transform HitSpotT { get; private set; }
        [field: SerializeField] public float SparksRate { get; private set; }
    }
}