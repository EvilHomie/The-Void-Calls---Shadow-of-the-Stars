using GamePools;
using UnityEngine;

namespace DefenseLayers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShieldEffect : PoolObjectBase
    {
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        public MaterialPropertyBlock MaterialBlock { get; private set; }
        public Transform ShieldTransform;
        public float RemainingLifetime;
        public Color Color;

        public override void Init(uint poolId)
        {
            base.Init(poolId);
            MaterialBlock = new();
            Color = SpriteRenderer.color;
        }
    }
}

