using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public readonly struct BoltSpawnData
    {
        public readonly uint ProjectilePoolId;
        public readonly HashSet<Collider2D> IgnoredColliders;
        public readonly float LifeTime;
        public readonly Vector2 SpawnPos;
        public readonly Vector2 AimPos;
        public readonly Vector2 Velocity;
        public readonly Vector2 Direction;
        public readonly WeaponRuntimeDamage DamageData;
        public readonly SizeType Size;

        public BoltSpawnData(uint poolId, SizeType size, HashSet<Collider2D> ignoredColliders, float lifeTime, Vector2 spawnPos, Vector2 aimPos, Vector2 velocity, Vector2 direction, WeaponRuntimeDamage damageData)
        {
            IgnoredColliders = ignoredColliders;
            ProjectilePoolId = poolId;
            LifeTime = lifeTime;
            Direction = direction;
            SpawnPos = spawnPos;
            AimPos = aimPos;
            Velocity = velocity;
            DamageData = damageData;
            Size = size;
        }
    }
}