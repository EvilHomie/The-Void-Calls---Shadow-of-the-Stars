using GamePools;
using UnityEngine;

namespace Projectiles
{
    public abstract class ProjectileBase : PoolObjectBase
    {
        public float DestroyTime;
        [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
        [field: SerializeField] public Transform ColliderTransform { get; private set; }

        public abstract ProjectileType ProjectileType { get; }
    }
}