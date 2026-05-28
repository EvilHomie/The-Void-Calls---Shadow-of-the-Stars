using System;
using UnityEngine;

namespace Weapons
{
    public class BoltRepeater : WeaponBase, IShipVelocityAware
    {
        public BoltWeaponData Data;

        [field: SerializeField] PoolReference projectilePool;
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }        
        public Rigidbody2D ShipRB { get; private set; }
        public uint PoolId { get; private set; }

        public void SetShipRigidbody(Rigidbody2D rb)
        {
            ShipRB = rb;
            PoolId = projectilePool.Id;
        }
    }

    [Serializable]
    public struct BoltWeaponData
    {
        [Header("CONFIG")]
        public float FireRate;
        public float SpreadAngle;
        public float ProjectileSpeed;
        [Header("RUNTIMEDATA")]
        public float NextShootTime;
        public float ShootDelay;
        public float InvProjectileSpeed;
        public float ProjectileLifeTime;
    }
}