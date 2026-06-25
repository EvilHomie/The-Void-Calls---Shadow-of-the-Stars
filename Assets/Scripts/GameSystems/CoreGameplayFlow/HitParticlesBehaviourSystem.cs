using DI;
using Registries;

namespace CoreGameSystems
{
    public class HitParticlesBehaviourSystem : GameSystemBase, ICoreUpdateTickObserver
    {
        private HitEffectRegistry _hitEffectRegistry;

        [Inject]
        public void Construct(HitEffectRegistry hitParticleRegistry)
        {
            _hitEffectRegistry = hitParticleRegistry;
        }

        public void CoreUpdateTick(float deltaTime)
        {
            CheckActive();
        }

        protected override void Subscribe()
        {
            base.Subscribe();
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
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