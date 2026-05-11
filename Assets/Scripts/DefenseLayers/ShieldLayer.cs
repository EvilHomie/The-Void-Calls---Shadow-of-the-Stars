using UnityEngine;

namespace DefenseLayers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShieldLayer : DefenseLayerBase
    {
        public override DefenseLayerType LayerType => DefenseLayerType.Shield;
        public SpriteRenderer SpriteRenderer { get; private set; }
        public MaterialPropertyBlock MaterialBlock { get; private set; }

        public override void Init(float HP, uint ownerId)
        {
            base.Init(HP, ownerId);
            SpriteRenderer = GetComponent<SpriteRenderer>();
            MaterialBlock = new MaterialPropertyBlock();
        }
    }
}

