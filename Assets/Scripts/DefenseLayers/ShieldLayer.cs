using UnityEngine;

namespace DefenseLayers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShieldLayer : DefenseLayerBase
    {
        public override DefenseLayerType LayerType => DefenseLayerType.Shield;
        public SpriteRenderer SpriteRenderer;
        public MaterialPropertyBlock MaterialBlock;

        public override void Init()
        {
            base.Init();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            MaterialBlock = new MaterialPropertyBlock();
        }
    }
}

