using GameSystems;
using Registries;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefenseLayers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShieldLayer : DefenseLayerBase
    {
        public override DefenseLayerType LayerType => DefenseLayerType.Shield;
        public SpriteRenderer SpriteRenderer { get; private set; }
        public MaterialPropertyBlock MaterialBlock { get; private set; }

        public override void Init(float HP)
        {
            base.Init(HP);
            SpriteRenderer = GetComponent<SpriteRenderer>();
            MaterialBlock = new MaterialPropertyBlock();
        }


        private void Start()
        {
            Init(100f);
        }


        float _time = 0;
        float _delay = 1;
        private void Update()
        {
            _time -= Time.deltaTime;

            if (_time > 0) return;

            _time = _delay;
            var screenPostition = Mouse.current.position.ReadValue();
            var worldPostition = Camera.main.ScreenToWorldPoint(screenPostition);
            OnShieldHit(this, transform.position);
            OnShieldCollided(this, worldPostition);
        }

        private static readonly int CollisionUVId = Shader.PropertyToID("_CollisionUV");
        private static readonly int CollisionTimeId = Shader.PropertyToID("_CollisionTime");
        private static readonly int HitPositionUVId = Shader.PropertyToID("_HitPositionUV");
        private static readonly int HitTimeId = Shader.PropertyToID("_HitTime");

        private void OnShieldCollided(DefenseLayerBase layerBase, Vector2 hitPosition)
        {
            var shieldLayer = (ShieldLayer)layerBase;
            var materialBlock = shieldLayer.MaterialBlock;
            var layerTransform = shieldLayer.Transform;

            Vector3 local = layerTransform.InverseTransformPoint(hitPosition);
            Vector2 uv = new(local.x + 0.5f, local.y + 0.5f);

            materialBlock.SetVector(CollisionUVId, uv);
            materialBlock.SetFloat(CollisionTimeId, Time.time);
            shieldLayer.SpriteRenderer.SetPropertyBlock(materialBlock);
        }

        private void OnShieldHit(ShieldLayer shieldLayer, Vector2 hitPosition)
        {
            var materialBlock = shieldLayer.MaterialBlock;

            //Vector3 local = shieldLayer.Transform.InverseTransformPoint(hitPosition);
            //var bounds = shieldLayer.SpriteRenderer.sprite.bounds;
            //Vector2 uv = new(local.x / bounds.size.x + 0.5f, local.y / bounds.size.y + 0.5f);
            var layerTransform = shieldLayer.Transform;
            Vector3 local = layerTransform.InverseTransformPoint(hitPosition);
            Vector2 uv = new(local.x + 0.5f, local.y + 0.5f);

            materialBlock.SetVector(HitPositionUVId, uv);
            materialBlock.SetFloat(HitTimeId, Time.time);

            shieldLayer.SpriteRenderer.SetPropertyBlock(materialBlock);
        }
    }
}

