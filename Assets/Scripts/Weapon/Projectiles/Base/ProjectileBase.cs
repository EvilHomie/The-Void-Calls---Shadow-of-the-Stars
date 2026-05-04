using GamePools;
using UnityEngine;

namespace Projectiles
{
    public abstract class ProjectileBase : PoolObjectBase
    {
        public float DestroyTime;
        public LayerMask HitLayers;

        public abstract ProjectileType ProjectileType { get; }
    }
}