using UnityEngine;

namespace Weapons
{
    public class BoltRepeater : WeaponBase
    {
        public override WeaponType WeaponType => WeaponType.BoltRepeater;
        [field: SerializeField] public PoolDataSO ProjectileData { get; private set; }        
        [field: SerializeField] public ParticleSystem ShootSpotPS { get; private set; }
        [field: SerializeField] public Transform ShootSpotT { get; private set; }
        [field: SerializeField] public float FireRate { get; private set; }
        [field: SerializeField] public float SpreadAngle { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; }
        public Rigidbody2D ShipRigidBody { get; set; }
        public float FireCooldown { get; set; }
        public float TimePerShot => 1f / FireRate;
    }
}