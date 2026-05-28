using System;
using UnityEngine;

namespace Weapons
{
    public class MiningDrill : WeaponBase
    {
        public MiningDrillData Data;
        [field: SerializeField] public LineRenderer BeamLineLR { get; private set; }
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
    }

    [Serializable]
    public struct MiningDrillData
    {
        public int HitRate;
        public float NextHitTime;
        public float HitDelay;
        public Vector2 NoHitTargetPoint;
    }
}