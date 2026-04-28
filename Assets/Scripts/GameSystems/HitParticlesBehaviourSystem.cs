using DI;
using Registries;
using Weapons;

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
            EventBus.SpawnHitEffectAction += SpawnHitEffect;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.SpawnHitEffectAction -= SpawnHitEffect;
        }

        private void SpawnHitEffect(in HitEffectSpawnData spawnData)
        {
            var hitEffect = _hitEffectRegistry.Get(spawnData.PoolReference);
            hitEffect.Transform.position = spawnData.Position;
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