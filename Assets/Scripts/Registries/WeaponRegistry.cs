using DI;
using GameSystems;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Registries
{
    public class WeaponRegistry : MonoBehaviour, ICorePreUpdateTickObserver
    {
        public IReadOnlyCollection<WeaponBase> ActiveWeapons => _activeWeapons;

        private readonly HashSet<WeaponBase> _activeWeapons = new(200);
        private readonly HashSet<WeaponBase> _activeWeaponToAdd = new(20);
        private readonly HashSet<WeaponBase> _activeWeaponToRemove = new(20);

        [Inject]
        public void Construct(GameFlowSystem gameFlowSystem)
        {
            gameFlowSystem.AddTickObserver(this);
        }

        public void CorePreUpdateTick()
        {
            Sync();
        }

        public void RequestAddOnStartAttack(WeaponBase weapon)
        {
            _activeWeaponToRemove.Remove(weapon);
            _activeWeaponToAdd.Add(weapon);
        }

        public void RequestRemoveOnStopAttack(WeaponBase weapon)
        {
            _activeWeaponToAdd.Remove(weapon);
            _activeWeaponToRemove.Add(weapon);
        }

        public void Sync()
        {
            foreach (var weapon in _activeWeaponToRemove)
            {
                _activeWeapons.Remove(weapon);
            }

            _activeWeaponToRemove.Clear();

            foreach (var weapon in _activeWeaponToAdd)
            {
                _activeWeapons.Add(weapon);
            }

            _activeWeaponToAdd.Clear();
        }
    }
}