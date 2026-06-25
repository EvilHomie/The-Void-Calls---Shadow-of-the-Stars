using DefenseLayers;
using DI;
using Registries;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        private readonly float _collisionEffectDuration = 0.2f;
        private float _collisionEffectDurationReversed;


        private uint _shieldHitPoolId;
        private uint _shieldCollisionPoolId;
        private uint _sparksPoolBlueId;
        private uint _sparksPoolYellowId;
        private uint _sparksPoolGreyId;


        private float _deffShieldHitSize = 0.1f;
        private Dictionary<SizeType, float> _sizeMap;
        private Vector3 _vector3One = Vector3.one;
        private static readonly int EffectUVId = Shader.PropertyToID("_EffectUV");
        private static readonly int HitSizeId = Shader.PropertyToID("_HitSize");

        [Inject]
        public void Construct(HitEffectRegistry hitEffectRegistry, ShieldsEffectRegistry shieldsEffectRegistry)
        {
            _hitEffectRegistry = hitEffectRegistry;
            _shieldsEffectRegistry = shieldsEffectRegistry;
            _collisionEffectDurationReversed = 1 / _collisionEffectDuration;

            _shieldHitPoolId = _shieldHitPoolEffect.Id;
            _shieldCollisionPoolId = _shieldCollisionPoolEffect.Id;
            _sparksPoolBlueId = sparksPoolBlue.Id;
            _sparksPoolYellowId = sparksPoolYellow.Id;
            _sparksPoolGreyId = sparksPoolGrey.Id;

            _sizeMap = new()
            {
                {SizeType.S, 1 },
                {SizeType.M, 5 },
                {SizeType.L, 15 },
                {SizeType.XL, 125 }
            };

            EventBus.ShieldDamagedAction += OnShieldHit;
            EventBus.ArmorDamagedAction += OnArmorDamaged;
            EventBus.HullDamagedAction += OnHullDamaged;
            EventBus.AsteroidDamagedAction += OnAsteroidHullDamaged;
        }
        public void Execute(float deltaTime)
        {
            UpdateShieldEffects();
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

        private void OnHullDamaged(DefenseLayerBase layerBase, HitData hitData)
        {
            var hitEffect = _hitEffectRegistry.Get(_sparksPoolGreyId);
            hitEffect.Transform.position = hitData.Position;
            hitEffect.IsPlaying = true;
        }
        private void OnArmorDamaged(DefenseLayerBase layerBase, HitData hitData)
        {
            var hitEffect = _hitEffectRegistry.Get(_sparksPoolYellowId);
            hitEffect.Transform.position = hitData.Position;
            hitEffect.IsPlaying = true;
        }

        private void OnShieldHit(ShieldDefenseLayer layerBase, HitData hitData)
        {
            var effect = _hitEffectRegistry.Get(_sparksPoolBlueId);
            effect.Transform.position = hitData.Position;
            effect.Transform.localScale = _vector3One * _sizeMap[hitData.Size];
            effect.IsPlaying = true;

            SetUpShieldEffect(layerBase, hitData, _shieldHitPoolId);
        }

        private void OnShieldCollision(ShieldDefenseLayer layerBase, HitData hitData)
        {
            SetUpShieldEffect(layerBase, hitData, _shieldCollisionPoolId);
        }

        private void OnAsteroidHullDamaged(DefenseLayerBase layerBase, HitData hitData)
        {
            var hitEffect = _hitEffectRegistry.Get(_sparksPoolGreyId);
            hitEffect.Transform.position = hitData.Position;
            hitEffect.IsPlaying = true;
        }

        private void SetUpShieldEffect(ShieldDefenseLayer shieldLayer, HitData hitData, uint effectPoolId)
        {
            var effect = _shieldsEffectRegistry.Get(effectPoolId);

            var materialBlock = effect.MaterialBlock;
            var shieldTransform = shieldLayer.Transform;
            effect.ShieldTransform = shieldTransform;
            var effectTransform = effect.Transform;
            effect.RemainingLifetime = _collisionEffectDuration;

            effect.SpriteAlpha = 1;
            effect.SpriteRenderer.color = effect.SpriteRenderer.color.WithAlpha(1);

            effectTransform.SetPositionAndRotation(shieldTransform.position, shieldTransform.rotation);
            effectTransform.localScale = shieldTransform.lossyScale;

            Vector2 local = shieldTransform.InverseTransformPoint(hitData.Position);
            local.x += 0.5f;
            local.y += 0.5f;


            var sizeRelative = _sizeMap[hitData.Size] / _sizeMap[shieldLayer.Size];

            materialBlock.SetVector(EffectUVId, local);
            materialBlock.SetFloat(HitSizeId, _deffShieldHitSize * sizeRelative);
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
                var newAlpha = effect.RemainingLifetime * _collisionEffectDurationReversed;
                effect.SpriteAlpha = newAlpha;
                effect.SpriteRenderer.color = effect.SpriteRenderer.color.WithAlpha(newAlpha);
            }
        }
    }
}