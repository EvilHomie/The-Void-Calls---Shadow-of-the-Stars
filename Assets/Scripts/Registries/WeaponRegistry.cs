using DI;
using GameSystems;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Registries
{
    public class WeaponRegistry : MonoBehaviour, ISyncable
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
        public WeaponRegistry(RegistrySyncSystem registrySyncSystem)
        {
            registrySyncSystem.Add(this);
            Debug.LogError($"InjectConstructor {registrySyncSystem == null}");
        }

        [Inject]
        public WeaponRegistry(Camera registrySyncSystem)
        {
            Debug.LogError($"CameraConstructor {registrySyncSystem == null}");
        }

        [Inject]
        public WeaponRegistry()
        {
            Debug.LogError("Empty Constructor");
        }

        [Inject]
        public void Init(RegistrySyncSystem registrySyncSystem)
        {
            Debug.LogError($"METHOD   {registrySyncSystem == null}");
        }

        [Inject]
        public void Init2(RegistrySyncSystem registrySyncSystem)
        {
            Debug.LogError($"METHOD2   {registrySyncSystem == null}");
        }

        public void RequestCreate(WeaponBase weapon)
        {
            _weaponToRemove.Remove(weapon);
            _weaponToAdd.Add(weapon);
        }

        public void RequestDestroy(WeaponBase weapon)
        {
            _weaponToAdd.Remove(weapon);
            _weaponToRemove.Add(weapon);
            _activeWeaponToAdd.Remove(weapon);
            _activeWeaponToRemove.Add(weapon);
        }

        public void RequestStartAttack(WeaponBase weapon)
        {
            _weaponToRemove.Remove(weapon);
            _weaponToAdd.Add(weapon);
            _activeWeaponToRemove.Remove(weapon);
            _activeWeaponToAdd.Add(weapon);
        }

        public void RequestStopAttack(WeaponBase weapon)
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