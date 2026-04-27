using DI;
using HitParticles;
using Registries;

namespace GameSystems
{
    public class HitParticlesBehaviourSystem : GameSystemBase, IUpdateTickObserver
    {
        private HitParticleRegistry _hitParticleRegistry;

        [Inject]
        public void Construct(HitParticleRegistry hitParticleRegistry)
        {
            _hitParticleRegistry = hitParticleRegistry;
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
            EventBus.HitParticleFetched += OnHitParticleFetched;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.HitParticleFetched -= OnHitParticleFetched;
        }

        private void  CheckActive()
        {
            foreach (var  hitParticle  in _hitParticleRegistry.ActiveHitParticles)
            {
                if (!hitParticle.IsPlaying)
                {
                    _hitParticleRegistry.RequestRemoveActiveHitParticle(hitParticle);
                }
            }
        }

        private void OnHitParticleFetched(HitParticle hitParticle)
        {
            _hitParticleRegistry.RequestAddActiveHitParticle(hitParticle);
        }
    }
}

