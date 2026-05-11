using DI;
using Registries;

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