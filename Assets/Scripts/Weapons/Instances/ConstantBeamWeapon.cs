using GamePools;
using UnityEngine;

namespace Weapons
{
    public class ConstantBeamWeapon : WeaponBase
    {
        public LineRenderer BeamLineLR { get; private set; }
        public ParticleSystem ShootSpotPS { get; private set; }

        public float NextHitTime;
        public float HitDelay;

        protected override void OnResolveDependencies()
        {
            base.OnResolveDependencies();
            BeamLineLR = GetComponentInChildren<BeamLineRenderer>().GetComponent<LineRenderer>();
            ShootSpotPS = GetComponentInChildren<ShootEffect>().GetComponent<ParticleSystem>();
        }
    }
}