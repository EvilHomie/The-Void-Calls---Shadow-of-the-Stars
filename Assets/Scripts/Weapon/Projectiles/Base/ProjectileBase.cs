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
        //public uint OwnerId;
        public Collider2D[] IgnoredColliders = new Collider2D[12];
        public int IgnoredCount;

        public abstract ProjectileType ProjectileType { get; }
    }
}