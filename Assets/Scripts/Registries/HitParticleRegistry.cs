using DI;
using GameSystems;
using HitParticles;
using System.Collections.Generic;
using UnityEngine;

namespace Registries
{
    public class HitParticleRegistry : MonoBehaviour, IPreUpdateTickObserver
    {
        public IReadOnlyCollection<HitParticle> ActiveHitParticles => _activeHitParticles;

        private readonly HashSet<HitParticle> _activeHitParticles = new(200);
        private readonly HashSet<HitParticle> _activeHitParticlesToAdd = new(20);
        private readonly HashSet<HitParticle> _activeHitParticlesToRemove = new(20);

        [Inject]
        public void Construct(GameFlowSystem gameFlowSystem)
        {
            gameFlowSystem.AddTickObserver(this);
        }

        public void PreUpdateTick()
        {
            Sync();
        }

        public void RequestAddActiveHitParticle(HitParticle hitParticle)
        {
            _activeHitParticlesToRemove.Remove(hitParticle);
            _activeHitParticlesToAdd.Add(hitParticle);
        }

        public void RequestRemoveActiveHitParticle(HitParticle hitParticle)
        {
            _activeHitParticlesToAdd.Remove(hitParticle);
            _activeHitParticlesToRemove.Add(hitParticle);
        }

        public void Sync()
        {
            foreach (var hitParticle in _activeHitParticlesToRemove)
            {
                EventBus.ReturnHitParticleInPool(hitParticle);
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