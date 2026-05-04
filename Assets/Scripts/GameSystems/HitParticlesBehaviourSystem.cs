using DI;
using Projectiles;
using Registries;
using UnityEngine;

namespace GameSystems
{
    public class HitParticlesBehaviourSystem : GameSystemBase, IUpdateTickObserver
    {
        private HitEffectRegistry _hitEffectRegistry;

        [Inject]
        public void Construct(HitEffectRegistry hitParticleRegistry)
        {
            _hitEffectRegistry = hitParticleRegistry;
            ActiveGameState = GameState.CoreGameplay;
        }

        public void UpdateTick(float deltaTime)
        {
            if (!SystemIsActive) return;

            CheckActive();
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            EventBus.BoltHitAction += OnBoltHit;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.BoltHitAction -= OnBoltHit;
        }

        private void OnBoltHit(Bolt bolt, Collider2D hitCollider, Vector2 position)
        {
            var hitEffect = _hitEffectRegistry.Get(bolt.PoolReference);
            hitEffect.Transform.position = position;
            hitEffect.IsPlaying = true;
        }

        private void CheckActive()
        {
            foreach (var hitParticle in _hitEffectRegistry.ActiveHitParticles)
            {
                if (!hitParticle.IsPlaying)
                {
                    _hitEffectRegistry.RequestRemoveActiveHitParticle(hitParticle);
                }
            }
        }
    }
}