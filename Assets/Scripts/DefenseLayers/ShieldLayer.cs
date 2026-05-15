using DI;
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




        private static readonly int CollisionUVId = Shader.PropertyToID("_CollisionUV");
        private static readonly int PowerValueId = Shader.PropertyToID("_PowerValue");
        private static readonly int HitPositionUVId = Shader.PropertyToID("_HitPositionUV");
        private static readonly int HitTimeId = Shader.PropertyToID("_HitTime");

        ShieldsEffectRegistry _shieldsEffectRegistry;
        [SerializeField] PoolReference _shieldCollisionPoolEffect;

        float _time = 0;
        float _delay = 0.5f;
        float _collisionEffectDuration = 0.2f;
        float _reversedEffectDuration;

        [Inject]
        public void Test(ShieldsEffectRegistry shieldsEffectRegistry)
        {
            _shieldsEffectRegistry = shieldsEffectRegistry;
            _reversedEffectDuration = 1 / _collisionEffectDuration;
        }


        private void Start()
        {
            Init(100f);
        }


       
        private void Update()
        {
            //UpdateShieldEffects();
            _time -= Time.deltaTime;

            if (_time > 0) return;

            _time = _delay;
            var screenPostition = Mouse.current.position.ReadValue();
            var worldPostition = Camera.main.ScreenToWorldPoint(screenPostition);

            EventBus.DefenseLayerDamagedAction?.Invoke(this, worldPostition);

            //OnShieldCollision((DefenseLayerBase)this, worldPostition);
        }       

        private void OnShieldCollision(DefenseLayerBase shieldLayer, Vector2 worldHitPos)
        {
            var effect = _shieldsEffectRegistry.Get(_shieldCollisionPoolEffect.Id);

            var materialBlock = effect.MaterialBlock;
            var shieldTransform = shieldLayer.Transform;
            effect.ShieldTransform = shieldTransform;
            var effectTransform = effect.Transform;
            effect.RemainingLifetime = _collisionEffectDuration;

            effect.Color.a = 1;
            effect.SpriteRenderer.color = effect.Color;

            effectTransform.SetPositionAndRotation(shieldTransform.position, shieldTransform.rotation);
            effectTransform.localScale = shieldTransform.localScale;

            Vector2 local = shieldTransform.InverseTransformPoint(worldHitPos);
            local.x += 0.5f;
            local.y += 0.5f;

            materialBlock.SetVector(CollisionUVId, local);
            effect.SpriteRenderer.SetPropertyBlock(materialBlock);
        }

        private void UpdateShieldEffects()
        {
            foreach (var effect in _shieldsEffectRegistry.ActiveEffects)
            {
                effect.RemainingLifetime -= Time.deltaTime;

                if (effect.RemainingLifetime <= 0 || effect.ShieldTransform == null)
                {
                    _shieldsEffectRegistry.RequestRemove(effect);
                    continue;
                }

                var shieldTransform = effect.ShieldTransform;
                var effectTransform = effect.Transform;
                effectTransform.SetPositionAndRotation(shieldTransform.position, shieldTransform.rotation);
                effectTransform.localScale = shieldTransform.localScale;
                effect.Color.a = effect.RemainingLifetime * _reversedEffectDuration;
                effect.SpriteRenderer.color = effect.Color;
            }
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

