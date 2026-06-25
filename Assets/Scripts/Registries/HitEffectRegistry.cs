using DI;
using GamePools;
using General;
using HitParticles;
using System.Collections.Generic;
using UnityEngine;

namespace Registries
{
    public class HitEffectRegistry : MonoBehaviour, ICorePreUpdateTickObserver
    {
        public IReadOnlyCollection<HitEffectParticle> ActiveHitParticles => _activeHitParticles;

        private readonly HashSet<HitEffectParticle> _activeHitParticles = new(200);
        private readonly HashSet<HitEffectParticle> _activeHitParticlesToAdd = new(20);
        private readonly HashSet<HitEffectParticle> _activeHitParticlesToRemove = new(20);

        private HitParticlesPool _hitParticlesPool;

        [Inject]
        public void Construct(GameFlowSystem gameFlowSystem, HitParticlesPool hitParticlesPool)
        {
            _hitParticlesPool = hitParticlesPool;
            gameFlowSystem.AddTickObserver(this);
        }

        public void CorePreUpdateTick()
        {
            Sync();
        }

        public HitEffectParticle Get(uint poolId)
        {
            var hitParticle = _hitParticlesPool.Getitem(poolId);
            RequestAddActiveHitParticle(hitParticle);
            return hitParticle;
        }

        public void RequestAddActiveHitParticle(HitEffectParticle hitParticle)
        {
            _activeHitParticlesToRemove.Remove(hitParticle);
            _activeHitParticlesToAdd.Add(hitParticle);
        }

        public void RequestRemoveActiveHitParticle(HitEffectParticle hitParticle)
        {
            _activeHitParticlesToAdd.Remove(hitParticle);
            _activeHitParticlesToRemove.Add(hitParticle);
        }

        public void Sync()
        {
            foreach (var hitParticle in _activeHitParticlesToRemove)
            {
                _hitParticlesPool.ReleaseItem(hitParticle);
                _activeHitParticles.Remove(hitParticle);
            }

            _activeHitParticlesToRemove.Clear();

            foreach (var hitParticle in _activeHitParticlesToAdd)
            {
                _activeHitParticles.Add(hitParticle);
            }

            _activeHitParticlesToAdd.Clear();
        }
    }
}