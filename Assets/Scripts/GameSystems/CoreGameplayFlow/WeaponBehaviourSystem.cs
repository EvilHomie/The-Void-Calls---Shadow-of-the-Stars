using DI;
using General;
using Helpers;
using Registries;
using UnityEngine;
using Weapons;

namespace CoreGameSystems
{
    public class WeaponBehaviourSystem : MonoBehaviour
    {
        private WeaponsStrategy _weaponsBehaviour;
        private WeaponRegistry _weaponRegistry;

        [Inject]
        public void Construct(WeaponRegistry weaponRegistry, HitRegistrationSystem hitRegistrationSystem)
        {
            _weaponRegistry = weaponRegistry;
            _weaponsBehaviour = new WeaponsStrategy(hitRegistrationSystem);
            EventBus.WeaponChangeAttackStateAction += OnWeaponChangeAttackStateAction;
        }

        public void Execute()
        {
            ProceedShooting();
        }
        private void OnWeaponChangeAttackStateAction(WeaponBase weapon, bool state)
        {
            if (state)
            {
                _weaponRegistry.RequestAddOnStartAttack(weapon);
                _weaponsBehaviour.HandleStartShoot(weapon);
            }
            else
            {
                _weaponRegistry.RequestRemoveOnStopAttack(weapon);
                _weaponsBehaviour.HandleCancelShoot(weapon);
            }
        }

        private void ProceedShooting()
        {
            foreach (var weapon in _weaponRegistry.ActiveWeapons)
            {
                _weaponsBehaviour.ProcessShooting(weapon);
            }
        }
    }
}
