using GamePools;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Projectiles
{
    public abstract class ProjectileBase : PoolObjectBase
    {
        public SizeType Size;
        public float DestroyTime;
        public float HitTime;
        public LayerMask HitLayers;
        public WeaponRuntimeDamage DamageData;
        public HashSet<Collider2D> IgnoredColliders = new();

        public abstract ProjectileType ProjectileType { get; }

        public Vector3 Position;
        public Vector3 Velocity;
        public bool IsMissed;
    }

    public enum ProjectileType
    {
        Bolt,
        StraightMissile,
        HomingMissile,
        Plasma
    }
}