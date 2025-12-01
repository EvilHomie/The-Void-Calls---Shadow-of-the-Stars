using DI;
using Helper;
using Player;
using Ship;
using System.Collections.Generic;
using Weapon;

namespace GameSystem
{
    public class WeaponSystem : GameSystemBase
    {
        private WeaponsBehaviour _weaponsBehaviour;
        private readonly HashSet<WeaponBase> _activeWeapons = new(200);
        private readonly HashSet<WeaponBase> _weaponsPendingRemoval = new(200);
        private PlayerShip _playerShip;
        private IPlayerInput _playerInput;

        [Inject]
        public void Construct(IPlayerInput playerInput, PlayerShip ship)
        {
            _playerShip = ship;
            _playerInput = playerInput;
            _weaponsBehaviour = new WeaponsBehaviour();
        }

        protected override void Init()
        {

        }

        protected override void Subscribe()
        {
            GameFlow.FixedGameTick += OnFixedGameTick;
            _playerInput.ChangeAtackState += PlayerToggleAtack;
            EventBus.CreateWeaponAction += _weaponsBehaviour.CancelShoot;
            EventBus.WeaponChangeShootState += OnWeaponChangeState;
        }

        protected override void Unsubscribe()
        {
            GameFlow.FixedGameTick -= OnFixedGameTick;
            _playerInput.ChangeAtackState -= PlayerToggleAtack;
            EventBus.CreateWeaponAction -= _weaponsBehaviour.CancelShoot;
            EventBus.WeaponChangeShootState -= OnWeaponChangeState;
        }

        

        private void OnFixedGameTick(float dTime)
        {
            UpdateActiveWeapons(dTime);
            PlayerAim(dTime);
            EnemyAim();
        }

        private void UpdateActiveWeapons(float dTime)
        {
            foreach (var weapon in _weaponsPendingRemoval) _activeWeapons.Remove(weapon);
            _weaponsPendingRemoval.Clear();
            foreach (var weapon in _activeWeapons) _weaponsBehaviour.ProceedShoot(weapon, dTime);
        }

        private void OnWeaponChangeState(WeaponBase weapon, bool activeState)
        {
            if (activeState) _activeWeapons.Add(weapon);
            else _weaponsPendingRemoval.Add(weapon);
        }

        private void PlayerToggleAtack(bool state)
        {
            OnChangeAtackState(_playerShip.ShipData.WeaponData, state);
        }

        private void OnChangeAtackState(ShipWeaponData shipWeaponData, bool state)
        {
            foreach (var weaponSlot in shipWeaponData.WeaponSlots)
            {
                if (weaponSlot.IsActive) SwitchWeaponShootingState(weaponSlot.Weapon, state);
            }
        }

        private void SwitchWeaponShootingState(WeaponBase weapon, bool state)
        {
            if (state) _weaponsBehaviour.StartShoot(weapon);
            else _weaponsBehaviour.CancelShoot(weapon);
        }

        private void PlayerAim(float dTime)
        {
            foreach (var weaponSlot in _playerShip.ShipData.WeaponData.WeaponSlots)
            {
                if (weaponSlot.IsActive) WeaponSystemHelper.AimAtTarget(weaponSlot.Weapon, dTime, _playerShip.MousePos);
            }
        }

        private void EnemyAim()
        {

        }
    }
}
