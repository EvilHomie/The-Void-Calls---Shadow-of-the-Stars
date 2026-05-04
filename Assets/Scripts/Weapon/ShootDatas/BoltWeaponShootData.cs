using UnityEngine;

namespace Weapons
{
    public readonly struct BoltWeaponShootData
    {
        public readonly PoolReference PoolReference;
        public readonly float DestroyTime;
        public readonly Vector2 FirePointPosition;
        public readonly Vector2 Velocity;
        public readonly Vector2 Direction;
        public readonly LayerMask HitLayers;

        public BoltWeaponShootData(PoolReference poolReference, float destroyTime, Vector2 firePointPosition, Vector2 velocity, Vector2 direction, LayerMask hitLayers)
        {
            HitLayers = hitLayers;
            PoolReference = poolReference;
            DestroyTime = destroyTime;
            FirePointPosition = firePointPosition;
            Velocity = velocity;
            Direction = direction;
        }
    }
}