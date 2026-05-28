using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public readonly struct BoltWeaponShootData
    {
        public readonly uint ProjectilePoolId;
        public readonly HashSet<Collider2D> IgnoredColliders;
        public readonly float DestroyTime;
        public readonly float HitTime;
        public readonly Vector3 FirePointPosition;
        public readonly Vector2 Velocity;
        public readonly Vector2 Direction;
        public readonly LayerMask HitLayers;
        public readonly CurrentDamageData DamageData;

        public BoltWeaponShootData(uint poolId, HashSet<Collider2D> ignoredColliders, float destroyTime, float hitTime, Vector3 firePointPosition, Vector2 velocity, Vector2 direction, LayerMask hitLayers, CurrentDamageData damageData)
        {
            IgnoredColliders = ignoredColliders;
            HitLayers = hitLayers;
            ProjectilePoolId = poolId;
            DestroyTime = destroyTime;
            Direction = direction;
            HitTime = hitTime;
            FirePointPosition = firePointPosition;
            Velocity = velocity;
            DamageData = damageData;
        }
    }
}