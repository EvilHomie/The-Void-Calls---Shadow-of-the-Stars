using DI;
using HitParticles;
using Registries;

namespace GameSystems
{
    public class HitParticlesBehaviourSystem : GameSystemBase
    {
        private HitParticleRegistry _hitParticleRegistry;

        [Inject]
        public void Construct(HitParticleRegistry hitParticleRegistry)
        {
            _hitParticleRegistry = hitParticleRegistry;
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            EventBus.HitParticleFetched += OnHitParticleFetched;
            EventBus.HitParticleStopped += OnHitParticleStopped;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.HitParticleFetched -= OnHitParticleFetched;
            EventBus.HitParticleStopped -= OnHitParticleStopped;
        }

        private void OnHitParticleFetched(HitParticle hitParticle)
        {
            _hitParticleRegistry.RequestAddActiveHitParticle(hitParticle);
        }
        private void OnHitParticleStopped(HitParticle hitParticle)
        {
            _hitParticleRegistry.RequestRemoveActiveHitParticle(hitParticle);
        }
    }
}

