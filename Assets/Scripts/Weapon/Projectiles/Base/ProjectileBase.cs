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
        public Vector2 AimPos;
        public WeaponRuntimeDamage DamageData;
        public HashSet<Collider2D> IgnoredColliders = new();

        public abstract ProjectileType ProjectileType { get; }

        public Vector2 CurrentPos;
        public Vector2 Velocity;
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