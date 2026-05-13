using GamePools;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Projectiles
{
    public abstract class ProjectileBase : PoolObjectBase
    {
        public float DestroyTime;
        public LayerMask HitLayers;
        public DamageData DamageData;
        public HashSet<Collider2D> IgnoredColliders = new();

        public abstract ProjectileType ProjectileType { get; }
    }
}