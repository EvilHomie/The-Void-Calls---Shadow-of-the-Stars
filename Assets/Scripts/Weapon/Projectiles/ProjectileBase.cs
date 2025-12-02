using GamePools;
using GameSystems;
using UnityEngine;
using Weapons;

namespace Projectiles
{
    public class ProjectileBase : PoolObjectBase
    {
        [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
        [field: SerializeField] public PoolDataSO HitData { get; private set; }
        [field: SerializeField] public Transform HitCollider { get; private set; }
        public WeaponBase Weapon { get; set; }
        public float LifeTime { get; set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            EventBus.ProjectileHit?.Invoke(this, other);
        }
    }
}