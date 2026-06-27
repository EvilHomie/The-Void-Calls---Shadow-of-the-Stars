using System;
using UnityEngine;

namespace Weapons
{
    public class BoltRepeater : WeaponBase, IRigidBodyDependentWeapon
    {
        public override WeaponType WeaponType => WeaponType.BoltRepeater;
        [SerializeField] PoolReference projectilePool;
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        public Rigidbody2D ShipRB { get; private set; }
        public uint ProjectilePoolId { get; private set; }

        public BoltWeaponFireStats BaseFireStats;
        public BoltWeaponFireStats RuntimeFireStats;
        public float NextShootTime;
        public BoltRepeaterLogicStats LogicStats;

        public void Init(Rigidbody2D rb)
        {
            ShipRB = rb;
            ProjectilePoolId = projectilePool.Id;
        }
    }

    [Serializable]
    public struct BoltWeaponFireStats
    {
        public float FireRate;
        public float SpreadAngle;
        public float ProjectileSpeed;
    }

    [Serializable]
    public struct BoltRepeaterLogicStats
    {        
        public float ShootDelay;
        public float InvProjectileSpeed;
        public float ProjectileLifeTime;
        public float SpreadTimeMultiplier;
    }
}