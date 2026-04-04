using DI;
using GameInput;
using Helper;
using Ship;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class WeaponShootSystem : GameSystemBase
    {
        private WeaponsBehaviour _weaponsBehaviour;
        private readonly HashSet<ShipInstance> _attackingShips = new(200);
        private readonly HashSet<ShipInstance> _stopAttackingShips = new(200);

        private ShipInstance _playerShip;
        private IPlayerInput _playerInput;

        [Inject]
        public void Construct(IPlayerInput playerInput, ShipInstance playerShip)
        {
            _playerShip = playerShip;
            _playerInput = playerInput;
            _weaponsBehaviour = new WeaponsBehaviour();
        }

        protected override void Init()
        {

        }

        protected override void Subscribe()
        {
            GameFlow.UpdateTick += OnUpdateTick;
            _playerInput.ChangeAtackState += OnPlayerChangeAttackState;
            EventBus.ChangeAttackState += OnOtherChangeAttackState;
        }

        protected override void Unsubscribe()
        {
            GameFlow.UpdateTick -= OnUpdateTick;
            _playerInput.ChangeAtackState -= OnPlayerChangeAttackState;
            EventBus.ChangeAttackState += OnOtherChangeAttackState;
        }

        private void OnUpdateTick(float dTime)
        {
            ClearCollections();
            PlayerAim(dTime);
            OthersAim(dTime);
            OthersAttack();
        }

        private void ClearCollections()
        {
            foreach (var ship in _stopAttackingShips)
            {
                _attackingShips.Remove(ship);
            }

            _stopAttackingShips.Clear();
        }

        private void OnPlayerChangeAttackState(bool state)
        {
            if (state)
            {
                GameFlow.UpdateTick += PlayerAttack;
            }
            else
            {
                GameFlow.UpdateTick -= PlayerAttack;
            }

            OnChangeAttackState(_playerShip, state);
        }

        private void OnOtherChangeAttackState(ShipInstance ship, bool state)
        {
            if (state)
            {
                _attackingShips.Add(ship);
            }
            else
            {
                _stopAttackingShips.Add(ship);
            }

            OnChangeAttackState(ship, state);
        }

        private void OnChangeAttackState(ShipInstance ship, bool state)
        {
            foreach (var slot in ship.ShipData.EquipData.WeaponSlots)
            {
                if (!slot.IsActive)
                {
                    return;
                }

                if (state)
                {
                    _weaponsBehaviour.StartShoot(slot.Weapon);
                }
                else
                {
                    _weaponsBehaviour.CancelShoot(slot.Weapon);
                }
            }
        }

        private void OthersAttack()
        {
            foreach (var ship in _attackingShips)
            {
                ProceedWeaponShoot(ship.ShipData.EquipData.WeaponSlots);
            }
        }

        private void ProceedWeaponShoot(WeaponSlot[] weaponSlots)
        {
            foreach (var slot in weaponSlots)
            {
                if (!slot.IsActive)
                {
                    continue;
                }

                _weaponsBehaviour.ProceedShoot(slot.Weapon);
            }
        }

        private void PlayerAim(float dTime)
        {
            var weaponSlots = _playerShip.ShipData.EquipData.WeaponSlots;
            AimToTarget(weaponSlots, _playerShip.ShipData.TargetPos, dTime);
        }

        private void PlayerAttack(float dTime)
        {
            ProceedWeaponShoot(_playerShip.ShipData.EquipData.WeaponSlots);
        }

        private void OthersAim(float dTime)
        {
            foreach (var ship in _attackingShips)
            {
                var weaponSlots = ship.ShipData.EquipData.WeaponSlots;
                AimToTarget(weaponSlots, _playerShip.ShipData.TargetPos, dTime);
            }
        }

        private void AimToTarget(WeaponSlot[] weaponSlots, Vector2 targetpos, float dTime)
        {
            foreach (var weaponSlot in weaponSlots)
            {
                if (weaponSlot.IsActive)
                {
                    WeaponSystemHelper.AimAtTarget(weaponSlot.Weapon, dTime, targetpos);
                }
            }
        }
    }
}
