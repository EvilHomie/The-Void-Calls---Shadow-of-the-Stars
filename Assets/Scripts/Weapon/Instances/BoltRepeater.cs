using System;
using UnityEngine;

namespace Weapons
{
    public class BoltRepeater : WeaponBase, IBoltWeapon
    {
        [HideInInspector] public BoltWeaponRuntimeData RuntimeData;

        [field: SerializeField] public float FireRate { get; private set; }
        [field: SerializeField] public float SpreadAngle { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; }

        [field: SerializeField] PoolReference projectilePool;
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        public Rigidbody2D ShipRB { get; private set; }
        public uint ProjectilePoolId { get; private set; }
        public void InitBoltWeapon(Rigidbody2D rb)
        {
            ShipRB = rb;
            ProjectilePoolId = projectilePool.Id;
        }
    }

    [Serializable]
    public struct BoltWeaponRuntimeData
    {
        public float NextShootTime;
        public float ShootDelay;
        public float ProjectileSpeed;
        public float InvProjectileSpeed;
        public float ProjectileLifeTime;
    }
}