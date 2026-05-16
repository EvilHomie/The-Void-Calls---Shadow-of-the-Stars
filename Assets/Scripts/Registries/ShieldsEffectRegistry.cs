using DefenseLayers;
using DI;
using GamePools;
using GameSystems;
using System.Collections.Generic;
using UnityEngine;

namespace Registries
{
    public class ShieldsEffectRegistry : MonoBehaviour, ICorePreUpdateTickObserver
    {
        public IReadOnlyCollection<ShieldEffect> ActiveEffects => _activeShieldEffects;

        private readonly HashSet<ShieldEffect> _activeShieldEffects = new(200);
        private readonly HashSet<ShieldEffect> _activeShieldEffectsToAdd = new(200);
        private readonly HashSet<ShieldEffect> _activeShieldEffectsToRemove = new(200);

        private ShieldEffectsPool _effectsPool;

        [Inject]
        public void Construct(GameFlowSystem gameFlowSystem, ShieldEffectsPool  effectsPool)
        {
            _effectsPool = effectsPool;
            gameFlowSystem.AddTickObserver(this);
        }

        public void CorePreUpdateTick()
        {
            Sync();
        }

        public ShieldEffect Get(uint poolId)
        {
            var shieldEffect = _effectsPool.Getitem(poolId);
            RequestAdd(shieldEffect);
            return shieldEffect;
        }

        public void RequestAdd(ShieldEffect shieldEffect)
        {
            _activeShieldEffectsToRemove.Remove(shieldEffect);
            _activeShieldEffectsToAdd.Add(shieldEffect);
        }

        public void RequestRemove(ShieldEffect shieldEffect)
        {
            _activeShieldEffectsToAdd.Remove(shieldEffect);
            _activeShieldEffectsToRemove.Add(shieldEffect);
        }

        public void Sync()
        {
            foreach (var shieldEffect in _activeShieldEffectsToRemove)
            {
                _effectsPool.ReleaseItem(shieldEffect);
                _activeShieldEffects.Remove(shieldEffect);
            }

            _activeShieldEffectsToRemove.Clear();

            foreach (var shieldEffect in _activeShieldEffectsToAdd)
            {
                _activeShieldEffects.Add(shieldEffect);
            }

            _activeShieldEffectsToAdd.Clear();
        }
    }
}