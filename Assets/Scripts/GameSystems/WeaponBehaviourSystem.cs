using DI;
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
        private ShipRegistry _shipRegistry;

        [Inject]
        public void Construct(WeaponRegistry weaponRegistry, ShipRegistry shipRegistry)
        {
            _weaponRegistry = weaponRegistry;
            _shipRegistry = shipRegistry;
            _weaponsBehaviour = new WeaponsStrategy();
            EventBus.WeaponChangeAttackStateAction += OnWeaponChangeAttackStateAction;
        }

        public void Execute(float deltaTime)
        {
            Aim(deltaTime);
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

        private void Aim(float dTime)
        {
            var playerShip = _shipRegistry.PlayerShip;
            WeaponSystemHelper.AimAtTarget(playerShip, dTime);

            foreach (var ship in _shipRegistry.ShipsInFight)
            {
                WeaponSystemHelper.AimAtTarget(ship, dTime);
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
