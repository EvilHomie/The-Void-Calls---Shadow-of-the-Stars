using GamePools;
using GameSystems;
using UnityEngine;

namespace Projectiles
{
    public abstract class ProjectileBase : PoolObjectBase
    {
        public float DestroyTime;
        [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
        [field: SerializeField] public Transform HitCollider { get; private set; }

        public abstract ProjectileType ProjectileType { get; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            EventBus.ProjectileHitAction?.Invoke(this, other);
        }
    }
}