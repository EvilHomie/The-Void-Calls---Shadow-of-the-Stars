using GamePools;
using UnityEngine;

namespace DefenseLayers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShieldEffect : PoolObjectBase
    {
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        public MaterialPropertyBlock MaterialBlock { get; private set; }
        public ShieldDefenseLayer OwnerShield;
        public float RemainingLifeTime;
        public float SpriteAlpha;

        protected override void OnResolveDependencies()
        {
            MaterialBlock = new();
            SpriteAlpha = 1;
        }
    }
}