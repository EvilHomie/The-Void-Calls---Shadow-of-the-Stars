using UnityEngine;

namespace Weapons
{
    public readonly struct HomingMissileWeaponShootData
    {
        public readonly PoolReference PoolReference;
        public readonly float DestroyTime;
        public readonly Vector2 Position;
        public readonly Vector2 Velocity;
        public readonly Transform TargetTransform;
        public readonly float AccelerationSpeed;
        public readonly float BrakingSpeed;
        public readonly float RotateSpeed;

        public HomingMissileWeaponShootData(PoolReference poolReference, float destroyTime, Vector2 spawnPosition, Vector2 velocity, Transform targetTransform, float accelerationSpeed, float brakingSpeed, float rotateSpeed)
        {
            PoolReference = poolReference;
            DestroyTime = destroyTime;
            Position = spawnPosition;
            Velocity = velocity;
            TargetTransform = targetTransform;
            AccelerationSpeed = accelerationSpeed;
            BrakingSpeed = brakingSpeed;
            RotateSpeed = rotateSpeed;
        }
    }
}