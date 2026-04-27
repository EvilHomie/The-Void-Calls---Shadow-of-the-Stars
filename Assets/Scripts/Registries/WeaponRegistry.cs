using DI;
using GameSystems;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Registries
{
    public class WeaponRegistry : MonoBehaviour, IPreUpdateTickObserver
    {
        public IReadOnlyCollection<WeaponBase> ActiveWeapons => _activeWeapons;
        public IReadOnlyCollection<WeaponBase> AllWeapons => _allWeapons;

        private readonly HashSet<WeaponBase> _activeWeapons = new(200);
        private readonly HashSet<WeaponBase> _allWeapons = new(200);

        private readonly HashSet<WeaponBase> _activeWeaponToAdd = new(20);
        private readonly HashSet<WeaponBase> _activeWeaponToRemove = new(20);

        private readonly HashSet<WeaponBase> _weaponToAdd = new(20);
        private readonly HashSet<WeaponBase> _weaponToRemove = new(20);

        [Inject]
        public void Construct(GameFlowSystem gameFlowSystem)
        {
            gameFlowSystem.AddTickObserver(this);
        }

        public void PreUpdateTick()
        {
            Sync();
        }

        public void RequestAddOnCreate(WeaponBase weapon)
        {
            _weaponToRemove.Remove(weapon);
            _weaponToAdd.Add(weapon);
        }

        public void RequestRemoveOnDestroy(WeaponBase weapon)
        {
            _weaponToAdd.Remove(weapon);
            _weaponToRemove.Add(weapon);
            _activeWeaponToAdd.Remove(weapon);
            _activeWeaponToRemove.Add(weapon);
        }

        public void RequestAddOnStartAttack(WeaponBase weapon)
        {
            _weaponToRemove.Remove(weapon);
            _weaponToAdd.Add(weapon);
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

            foreach (var weapon in _weaponToRemove)
            {
                _allWeapons.Remove(weapon);
            }

            _weaponToRemove.Clear();

            foreach (var weapon in _weaponToAdd)
            {
                _allWeapons.Add(weapon);
            }

            _weaponToAdd.Clear();
        }
    }
}