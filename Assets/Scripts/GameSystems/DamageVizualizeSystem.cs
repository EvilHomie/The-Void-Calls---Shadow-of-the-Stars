using DefenseLayers;
using DI;
using Registries;
using UnityEngine;

namespace GameSystems
{
    public class DamageVizualizeSystem : GameSystemBase, ICoreUpdateTickObserver
    {
        [SerializeField] PoolReference sparksPoolBlue;
        [SerializeField] PoolReference sparksPoolYellow;
        [SerializeField] PoolReference sparksPoolGrey;
        [SerializeField] PoolReference _shieldCollisionPoolEffect;
        [SerializeField] PoolReference _shieldHitPoolEffect;
        ShieldsEffectRegistry _shieldsEffectRegistry;
        private HitEffectRegistry _hitEffectRegistry;

        private readonly float _collisionEffectDuration = 0.2f;
        private float _reversedEffectDuration;


        uint _shieldHitPoolId;
        uint _shieldCollisionPoolId;
        uint _sparksPoolBlueId;
        uint _sparksPoolYellowId;
        uint _sparksPoolGreyId;



        private static readonly int EffectUVId = Shader.PropertyToID("_EffectUV");

        [Inject]
        public void Construct(HitEffectRegistry hitEffectRegistry, ShieldsEffectRegistry shieldsEffectRegistry)
        {
            _hitEffectRegistry = hitEffectRegistry;
            _shieldsEffectRegistry = shieldsEffectRegistry;
            _reversedEffectDuration = 1 / _collisionEffectDuration;

            _shieldHitPoolId = _shieldHitPoolEffect.Id;
            _shieldCollisionPoolId = _shieldCollisionPoolEffect.Id;
            _sparksPoolBlueId = sparksPoolBlue.Id;
            _sparksPoolYellowId = sparksPoolYellow.Id;
            _sparksPoolGreyId = sparksPoolGrey.Id;
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            EventBus.DefenseLayerHitAction += OnDefenseLayerHitAction;
            EventBus.DefenseLayerCollisionAction += OnDefenseLayerCollisionAction;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.DefenseLayerHitAction -= OnDefenseLayerHitAction;
            EventBus.DefenseLayerCollisionAction -= OnDefenseLayerCollisionAction;
        }
        public void CoreUpdateTick(float deltaTime)
        {
            UpdateShieldEffects();
        }

        private void OnDefenseLayerHitAction(DefenseLayerBase layerBase, Vector2 hitPosition)
        {
            switch (layerBase.LayerType)
            {
                case DefenseLayerType.Shield:
                    OnShieldHit(layerBase, hitPosition);
                    break;
                case DefenseLayerType.Armor:
                    OnArmorDamaged(hitPosition);
                    break;
                case DefenseLayerType.Hull:
                    OnHullDamaged(hitPosition);
                    break;
                case DefenseLayerType.AsteroidHull:
                    OnAsteroidHullDamaged(hitPosition);
                    break;
                default:
                    break;
            }
        }

        private void OnDefenseLayerCollisionAction(DefenseLayerBase layerBase, Vector2 hitPosition)
        {
            switch (layerBase.LayerType)
            {
                case DefenseLayerType.Shield:
                    OnShieldCollision(layerBase, hitPosition);
                    break;
                case DefenseLayerType.Armor:
                    OnArmorDamaged(hitPosition);
                    break;
                case DefenseLayerType.Hull:
                    OnHullDamaged(hitPosition);
                    break;
                case DefenseLayerType.AsteroidHull:
                    OnAsteroidHullDamaged(hitPosition);
                    break;
                default:
                    break;
            }
        }

        private void OnHullDamaged(Vector2 hitPosition)
        {
            var hitEffect = _hitEffectRegistry.Get(_sparksPoolGreyId);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;
        }
        private void OnArmorDamaged(Vector2 hitPosition)
        {
            var hitEffect = _hitEffectRegistry.Get(_sparksPoolYellowId);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;
        }

        private void OnShieldHit(DefenseLayerBase layerBase, Vector2 hitPosition)
        {
            var effect = _hitEffectRegistry.Get(_sparksPoolBlueId);
            effect.Transform.position = hitPosition;
            effect.IsPlaying = true;

            SetUpShieldEffect(layerBase, hitPosition, _shieldHitPoolId);
        }

        private void OnShieldCollision(DefenseLayerBase layerBase, Vector2 hitPosition)
        {
            SetUpShieldEffect(layerBase, hitPosition, _shieldCollisionPoolId);
        }

        private void OnAsteroidHullDamaged(Vector2 hitPosition)
        {
            var hitEffect = _hitEffectRegistry.Get(_sparksPoolGreyId);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;
        }

        private void SetUpShieldEffect(DefenseLayerBase shieldLayer, Vector2 worldHitPos, uint effectPoolId)
        {
            var effect = _shieldsEffectRegistry.Get(effectPoolId);

            var materialBlock = effect.MaterialBlock;
            var shieldTransform = shieldLayer.Transform;
            effect.ShieldTransform = shieldTransform;
            var effectTransform = effect.Transform;
            effect.RemainingLifetime = _collisionEffectDuration;

            effect.Color.a = 1;
            effect.SpriteRenderer.color = effect.Color;

            effectTransform.SetPositionAndRotation(shieldTransform.position, shieldTransform.rotation);
            effectTransform.localScale = shieldTransform.lossyScale;

            Vector2 local = shieldTransform.InverseTransformPoint(worldHitPos);
            local.x += 0.5f;
            local.y += 0.5f;

            materialBlock.SetVector(EffectUVId, local);
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
                effectTransform.localScale = shieldTransform.lossyScale;
                effect.Color.a = effect.RemainingLifetime * _reversedEffectDuration;
                effect.SpriteRenderer.color = effect.Color;
            }
        }
    }
}

