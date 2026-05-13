using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public readonly struct BoltWeaponShootData
    {
        public readonly uint PoolId;
        public readonly HashSet<Collider2D> IgnoredColliders;
        public readonly float DestroyTime;
        public readonly Vector3 FirePointPosition;
        public readonly Vector2 Velocity;
        public readonly Vector2 Direction;
        public readonly LayerMask HitLayers;
        public readonly DamageData DamageData;

        public BoltWeaponShootData(uint poolId, HashSet<Collider2D> ignoredColliders, float destroyTime, Vector3 firePointPosition, Vector2 velocity, Vector2 direction, LayerMask hitLayers, DamageData damageData)
        {
            IgnoredColliders = ignoredColliders;
            HitLayers = hitLayers;
            PoolId = poolId;
            DestroyTime = destroyTime;
            FirePointPosition = firePointPosition;
            Velocity = velocity;
            Direction = direction;
            DamageData = damageData;
        }
    }
}