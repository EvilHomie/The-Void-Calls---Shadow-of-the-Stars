using System;
using UnityEngine;

namespace Weapons
{
    public class ProjectileWeapon : WeaponBase, IRigidBodyDependentWeapon, IPoolDependentWeapon
    {
        public ParticleSystem ShootEffectPS { get; private set; }
        public Rigidbody2D ShipRB { get; private set; }
        public uint ProjectilePoolId { get; private set; }

        public BoltWeaponFireStats RuntimeFireStats;
        public BoltRepeaterLogicStats LogicStats;
        public float NextShootTime;

        public void CacheRigidBody(Rigidbody2D rb)
        {
            ShipRB = rb;
        }
        public void CachePool(uint poolId)
        {
            ProjectilePoolId = poolId;
        }

        protected override void OnCacheDependencies()
        {
            ShootEffectPS = GetComponentInChildren<ShootEffect>().GetComponent<ParticleSystem>();
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