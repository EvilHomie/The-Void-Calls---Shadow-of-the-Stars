using UnityEngine;

namespace Weapons
{
    public readonly struct BoltWeaponShootData
    {
        public readonly PoolReference PoolReference;
        public readonly float DestroyTime;
        public readonly Vector2 FirePointPosition;
        public readonly Vector2 Velocity;

        public BoltWeaponShootData(PoolReference poolReference, float destroyTime, Vector2 firePointPosition, Vector2 velocity)
        {
            PoolReference = poolReference;
            DestroyTime = destroyTime;
            FirePointPosition = firePointPosition;
            Velocity = velocity;
        }
    }
}