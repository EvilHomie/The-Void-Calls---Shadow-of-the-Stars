using DefenseLayers;
using DI;
using Registries;
using System.Collections.Generic;
using UnityEngine;
using EventBus = General.EventBus;

namespace CoreGameSystems
{
    public class DamageVizualizeSystem : MonoBehaviour
    {
        [SerializeField] PoolReference sparksPoolBlue;
        [SerializeField] PoolReference sparksPoolYellow;
        [SerializeField] PoolReference sparksPoolGrey;
        [SerializeField] PoolReference _shieldCollisionPoolEffect;
        [SerializeField] PoolReference _shieldHitPoolEffect;
        private ShieldsEffectRegistry _shieldsEffectRegistry;
        private HitEffectRegistry _hitEffectRegistry;

        private const float _collisionEffectDuration = 0.2f;
        private const float _collisionEffectDurationReversed = 1 / _collisionEffectDuration;
        private const float _shieldEffectDeffSize = 0.05f;


        private uint _shieldHitPoolId;
        private uint _shieldCollisionPoolId;
        private uint _sparksPoolBlueId;
        private uint _sparksPoolYellowId;
        private uint _sparksPoolGreyId;


        private static readonly int EffectUVId = Shader.PropertyToID("_EffectUV");
        private static readonly int HitSizeId = Shader.PropertyToID("_HitSize");


        private HashSet<ShieldDefenseLayer> _shieldWithEffects = new(100);
        private HashSet<ShieldDefenseLayer> _shieldWithEffectsToAdd = new(100);
        private HashSet<ShieldDefenseLayer> _shieldWithEffectsToRemove = new(100);

        [Inject]
        public void Construct(HitEffectRegistry hitEffectRegistry, ShieldsEffectRegistry shieldsEffectRegistry)
        {
            _hitEffectRegistry = hitEffectRegistry;
            _shieldsEffectRegistry = shieldsEffectRegistry;

            _shieldHitPoolId = _shieldHitPoolEffect.Id;
            _shieldCollisionPoolId = _shieldCollisionPoolEffect.Id;
            _sparksPoolBlueId = sparksPoolBlue.Id;
            _sparksPoolYellowId = sparksPoolYellow.Id;
            _sparksPoolGreyId = sparksPoolGrey.Id;

            EventBus.ShieldDamagedAction += OnShieldHit;
            EventBus.ArmorDamagedAction += OnArmorHit;
            EventBus.HullDamagedAction += OnHullHit;
            EventBus.AsteroidDamagedAction += OnAsteroidHit;
        }
        public void Execute(float deltaTime)
        {
            UpdateShieldEffects(deltaTime);
            UpdateActiveSparks();
        }

        //private void OnDefenseLayerCollisionAction(DefenseLayerBase layerBase, HitData hitData)
        //{
        //    switch (layerBase.LayerType)
        //    {
        //        case DefenseLayerType.Shield:
        //            OnShieldCollision(layerBase, hitData);
        //            break;
        //        case DefenseLayerType.Armor:
        //            OnArmorDamaged(hitData);
        //            break;
        //        case DefenseLayerType.Hull:
        //            OnHullDamaged(hitData);
        //            break;
        //        case DefenseLayerType.AsteroidHull:
        //            OnAsteroidHullDamaged(hitData);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        private void UpdateActiveSparks()
        {
            foreach (var hitParticle in _hitEffectRegistry.ActiveHitParticles)
            {
                if (!hitParticle.IsPlaying)
                {
                    _hitEffectRegistry.RequestRemoveActiveHitParticle(hitParticle);
                }
            }
        }

        private void OnHullHit(HullDefenseLayer layer, HitData hitData)
        {
            SpawnSpark(_sparksPoolGreyId, hitData);
        }
        private void OnArmorHit(HullDefenseLayer layer, HitData hitData)
        {
            SpawnSpark(_sparksPoolYellowId, hitData);
        }
        private void OnAsteroidHit(AsteroidDefenseLayer layer, HitData hitData)
        {
            SpawnSpark(_sparksPoolGreyId, hitData);
        }

        private void OnShieldHit(ShieldDefenseLayer layer, HitData hitData)
        {
            SpawnSpark(_sparksPoolBlueId, hitData);
            SpawnShieldEffect(layer, hitData, _shieldHitPoolId);
        }

        private void OnShieldCollision(ShieldDefenseLayer layer, HitData hitData)
        {
            SpawnShieldEffect(layer, hitData, _shieldCollisionPoolId);
        }



        private void SpawnSpark(uint sparkId, in HitData hitData)
        {
            var effect = _hitEffectRegistry.Get(sparkId);
            var transform = effect.Transform;
            transform.position = hitData.Position;
            transform.localScale = Vector2.one * GameConfig.SizeMap[hitData.Size];
            effect.IsPlaying = true;
        }

        private void SpawnShieldEffect(ShieldDefenseLayer layer, in HitData hitData, uint effectPoolId)
        {
            var effect = _shieldsEffectRegistry.Get(effectPoolId);
            ref readonly var shieldTransformRuntimeData = ref layer.ShieldTransformRuntimeData;

            effect.OwnerShield = layer;
            effect.RemainingLifeTime = _collisionEffectDuration;

            Vector2 local = layer.Transform.InverseTransformPoint(hitData.Position);
            local.x += 0.5f;
            local.y += 0.5f;

            var materialBlock = effect.MaterialBlock;
            materialBlock.SetVector(EffectUVId, local);

            var relativeSize = GameConfig.SizeMap[hitData.Size] / GameConfig.SizeMap[layer.Size];
            materialBlock.SetFloat(HitSizeId, _shieldEffectDeffSize * relativeSize);
            effect.SpriteRenderer.SetPropertyBlock(materialBlock);
        }

        private void UpdateShieldEffects(float deltaTime)
        {
            foreach (var effect in _shieldsEffectRegistry.ActiveEffects)
            {
                UpdateShieldEffect(effect);

                effect.RemainingLifeTime -= deltaTime;

                if (effect.RemainingLifeTime <= 0 || effect.OwnerShield == null)
                {
                    _shieldsEffectRegistry.RequestRemove(effect);
                    continue;
                }
            }
        }

        private void UpdateShieldEffect(ShieldEffect effect)
        {
            ref readonly var shieldTransformRuntimeData = ref effect.OwnerShield.ShieldTransformRuntimeData;

            var effectTransform = effect.Transform;
            effectTransform.SetPositionAndRotation(shieldTransformRuntimeData.WorldPosition, shieldTransformRuntimeData.WorldRotation);
            effectTransform.localScale = shieldTransformRuntimeData.CurrentLossyScale;
            var newAlpha = effect.RemainingLifeTime * _collisionEffectDurationReversed;
            effect.SpriteAlpha = newAlpha;
            var color = effect.SpriteRenderer.color;
            color.a = newAlpha;
            effect.SpriteRenderer.color = color;
        }
    }
}