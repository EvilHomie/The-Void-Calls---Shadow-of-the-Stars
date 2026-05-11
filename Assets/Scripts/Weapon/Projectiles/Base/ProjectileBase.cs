using GamePools;
using UnityEngine;
using Weapons;

namespace Projectiles
{
    public abstract class ProjectileBase : PoolObjectBase
    {
        public float DestroyTime;
        public LayerMask HitLayers;
        public DamageData DamageData;
        public uint OwnerId;

        public abstract ProjectileType ProjectileType { get; }
    }
}