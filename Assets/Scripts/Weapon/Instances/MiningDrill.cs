using System;
using UnityEngine;

namespace Weapons
{
    public class MiningDrill : WeaponBase
    {
        [HideInInspector] public MiningDrillRuntimeData RuntimeData;
        [field: SerializeField] public LineRenderer BeamLineLR { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }

    }

    [Serializable]
    public struct MiningDrillRuntimeData
    {
        public float NextHitTime;
        public float HitDelay;
        public Vector2 NoHitTargetPoint;
    }
}