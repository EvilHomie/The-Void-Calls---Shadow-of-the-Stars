using DefenseLayers;
using DI;
using Registries;
using UnityEngine;

namespace GameSystems
{
    public class DamageVizualizeSystem : GameSystemBase, IUpdateTickObserver
    {
        [SerializeField] PoolReference shieldHitEffectReference;
        [SerializeField] PoolReference armorHitEffectReference;
        [SerializeField] PoolReference hullHitEffectReference;
        private HitEffectRegistry _hitEffectRegistry;

        private static readonly int CollisionUVId = Shader.PropertyToID("_CollisionUV");
        private static readonly int CollisionTimeId = Shader.PropertyToID("_CollisionTime");

        [Inject]
        public void Construct(HitEffectRegistry hitEffectRegistry)
        {
            _hitEffectRegistry = hitEffectRegistry;
            ActiveGameState = GameState.CoreGameplay;
        }

        protected override void AwakeInit()
        {

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
            var hitEffect = _hitEffectRegistry.Get(hullHitEffectReference.Id);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;
        }
        private void OnArmorDamaged(Vector2 hitPosition)
        {
            var hitEffect = _hitEffectRegistry.Get(armorHitEffectReference.Id);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;
        }

        private void OnShieldDamaged(DefenseLayerBase layerBase, Vector2 hitPosition)
        {
            var hitEffect = _hitEffectRegistry.Get(shieldHitEffectReference.Id);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;

            var shieldLayer = (ShieldLayer)layerBase;
            var materialBlock = shieldLayer.MaterialBlock;
            var layerTransform = shieldLayer.Transform;

            Vector3 local = layerTransform.InverseTransformPoint(hitPosition);
            Vector2 uv = new(local.x + 0.5f, local.y + 0.5f);

            materialBlock.SetVector(CollisionUVId, uv);
            materialBlock.SetFloat(CollisionTimeId, Time.time);
            shieldLayer.SpriteRenderer.SetPropertyBlock(materialBlock);
        }

        private void OnAsteroidHullDamaged(Vector2 hitPosition)
        {
            var hitEffect = _hitEffectRegistry.Get(hullHitEffectReference.Id);
            hitEffect.Transform.position = hitPosition;
            hitEffect.IsPlaying = true;
        }
    }
}

