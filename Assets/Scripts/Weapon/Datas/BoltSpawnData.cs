using UnityEngine;

namespace Weapons
{
    public readonly struct BoltSpawnData
    {
        public readonly PoolReference PoolReference;
        public readonly float DestroyTime;
        public readonly Vector2 Position;
        public readonly Vector2 Velocity;

        public BoltSpawnData(PoolReference poolReference, float destroyTime, Vector2 spawnPosition, Vector2 velocity)
        {
            PoolReference = poolReference;
            DestroyTime = destroyTime;
            Position = spawnPosition;
            Velocity = velocity;
        }
    }
}