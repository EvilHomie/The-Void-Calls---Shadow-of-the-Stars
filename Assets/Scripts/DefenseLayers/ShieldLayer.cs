using UnityEngine;

namespace DefenseLayers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShieldLayer : DefenseLayerBase
    {        
        public override DefenseLayerType LayerType => DefenseLayerType.Shield;
        public SpriteRenderer SpriteRenderer { get; private set; }

        public override void Init(float HP)
        {
            base.Init(HP);
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}

