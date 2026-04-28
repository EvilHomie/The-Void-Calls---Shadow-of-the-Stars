using UnityEngine;

namespace Weapons
{
    public readonly struct StraightMissileSpawnData
    {
        public readonly PoolReference PoolReference;
        public readonly float DestroyTime;
        public readonly Vector2 Position;
        public readonly Vector2 Velocity;
        public readonly float AccelerationSpeed;

        public StraightMissileSpawnData(PoolReference poolReference, float destroyTime, Vector2 spawnPosition, Vector2 velocity, float accelerationSpeed)
        {
            PoolReference = poolReference;
            DestroyTime = destroyTime;
            Position = spawnPosition;
            Velocity = velocity;
            AccelerationSpeed = accelerationSpeed;
        }
    }
}