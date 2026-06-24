using UnityEngine;

namespace Weapons
{
    public class ConstantBeamWeapon : WeaponBase
    {        
        [field: SerializeField] public LineRenderer BeamLineLR { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }

        public float NextHitTime;
        public float HitDelay;
    }
}