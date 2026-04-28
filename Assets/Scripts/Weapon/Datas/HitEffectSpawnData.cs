using UnityEngine;

namespace Weapons
{
    public readonly struct HitEffectSpawnData
    {
        public readonly PoolReference PoolReference;
        public readonly Vector2 Position;

        public HitEffectSpawnData(PoolReference poolReference, Vector2 hitPosition)
        {
            PoolReference = poolReference;
            Position = hitPosition;
        }
    }
}