using DefenseLayers;
using DI;
using Registries;
using UnityEngine;

namespace GameSystems
{
    public class DamageVizualizeSystem : GameSystemBase, IUpdateTickObserver
    {
        [SerializeField] PoolReference sparksPoolBlue;
        [SerializeField] PoolReference sparksPoolYellow;
        [SerializeField] PoolReference sparksPoolGrey;
        [SerializeField] PoolReference _shieldCollisionPoolEffect;
        ShieldsEffectRegistry _shieldsEffectRegistry;
        private HitEffectRegistry _hitEffectRegistry;

        private readonly float _collisionEffectDuration = 0.2f;
        private float _reversedEffectDuration;

        private static readonly int CollisionUVId = Shader.PropertyToID("_CollisionUV");

        [Inject]
        public void Construct(HitEffectRegistry hitEffectRegistry, ShieldsEffectRegistry shieldsEffectRegistry)
        {
            _hitEffectRegistry = hitEffectRegistry;
            _shieldsEffectRegistry = shieldsEffectRegistry;
            _reversedEffectDuration = 1 / _collisionEffectDuration;
            ActiveGameState = GameState.CoreGameplay;
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            EventBus.DefenseLayerDamagedAction += OnDefenseLayerDamagedAction;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.DefenseLayerDamagedAction -= OnDefenseLayerDamagedAction;
        }
        public void UpdateTick(float deltaTime)
        {
            UpdateShieldEffects();
        }

        private void OnDefenseLayerDamagedAction(DefenseLayerBase layerBase, Vector2 hitPosition)
        {
            switch (layerBase.LayerType)
            {
                case DefenseLayerType.Shield:
                    OnShieldDamaged(layerBase, hitPosition);
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
            var hitEffect = _hitEffectRegistry.Get(sparksPoolGrey.Id);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;
        }
        private void OnArmorDamaged(Vector2 hitPosition)
        {
            var hitEffect = _hitEffectRegistry.Get(sparksPoolYellow.Id);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;
        }

        private void OnShieldDamaged(DefenseLayerBase layerBase, Vector2 hitPosition)
        {
            var hitEffect = _hitEffectRegistry.Get(sparksPoolBlue.Id);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;

            ShowShieldCollision(layerBase, hitPosition);
        }

        private void OnAsteroidHullDamaged(Vector2 hitPosition)
        {
            var hitEffect = _hitEffectRegistry.Get(sparksPoolGrey.Id);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;
        }

        private void ShowShieldCollision(DefenseLayerBase shieldLayer, Vector2 worldHitPos)
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
    }
}

