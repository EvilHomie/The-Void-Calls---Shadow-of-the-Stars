using DI;
using Helper;
using Player;
using Ship;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

namespace GameSystem
{
    public class WeaponSystem : GameSystemBase
    {
        private readonly Dictionary<WeaponType, WeaponInvoker> _invokers = new();
        private readonly HashSet<WeaponBase> _activeWeapons = new();
        private PlayerShip _playerShip;
        private IPlayerInput _playerInput;

        [Inject]
        public void Construct(IPlayerInput playerInput, PlayerShip ship)
        {
            _playerShip = ship;
            _playerInput = playerInput;
            RegisterInvokers();
        }

        protected override void Init()
        {
            
        }

        protected override void Subscribe()
        {
            GameFlow.FixedGameTick += OnFixedGameTick;
            _playerInput.ChangeAtackState += PlayerToggleAtack;
            EventBus.CreateWeaponAction += CancelShoot;
        }

        protected override void Unsubscribe()
        {
            GameFlow.FixedGameTick -= OnFixedGameTick;
            _playerInput.ChangeAtackState -= PlayerToggleAtack;
            EventBus.CreateWeaponAction -= CancelShoot;
        }

        private void RegisterInvokers()
        {
            MiningDrillBehaviour miningDrillBehaviour = new();
            var invoker = WeaponSystemHelper.CreateInvoker(WeaponType.MiningDrill, miningDrillBehaviour);
            _invokers[WeaponType.MiningDrill] = invoker;
        }

        private void StartShoot(WeaponBase weapon)
            => _invokers[weapon.WeaponType].StartShoot(weapon);

        private void CancelShoot(WeaponBase weapon)
            => _invokers[weapon.WeaponType].CancelShoot(weapon);

        private void ProceedShoot(WeaponBase weapon, float dTime)
            => _invokers[weapon.WeaponType].ProceedShoot(weapon, dTime);

        private void OnFixedGameTick(float dTime)
        {
            PlayerAim(dTime);

            foreach (var weapon in _activeWeapons)
            {
                ProceedShoot(weapon, dTime);
            }

            EnemyAim();
        }

        private void PlayerToggleAtack(bool state)
        {
            OnChangeAtackState(_playerShip.ShipData.WeaponData, state);
        }

        private void OnChangeAtackState(ShipWeaponData shipWeaponData, bool state)
        {
            foreach (var weaponSlot in shipWeaponData.WeaponSlots)
            {
                if (weaponSlot.IsActive)
                {
                    OnWeaponChangeState(weaponSlot.Weapon, state);
                }
            }
        }

        private void OnWeaponChangeState(WeaponBase weapon, bool state)
        {
            if (state)
            {
                StartShoot(weapon);
                _activeWeapons.Add(weapon);
            }
            else
            {
                CancelShoot(weapon);
                _activeWeapons.Remove(weapon);
            }
        }

        private void PlayerAim(float dTime)
        {
            foreach (var weaponSlot in _playerShip.ShipData.WeaponData.WeaponSlots)
            {
                if (weaponSlot.IsActive)
                {
                    WeaponSystemHelper.AimAtTarget(weaponSlot.Weapon, dTime, _playerShip.MousePos);
                }
            }
        }

        private void EnemyAim()
        {

        }
    }
}
