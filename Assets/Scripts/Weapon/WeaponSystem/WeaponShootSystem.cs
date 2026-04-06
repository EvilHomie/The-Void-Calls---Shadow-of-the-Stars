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

        private IPlayerInput _playerInput;
        private ObjectsStorage _objectsStorage;

        [Inject]
        public void Construct(IPlayerInput playerInput, ObjectsStorage objectsStorage)
        {
            _playerInput = playerInput;
            _objectsStorage = objectsStorage;
            _weaponsBehaviour = new WeaponsBehaviour();
        }

        protected override void AwakeInit()
        {

        }

        protected override void Subscribe()
        {
            GameFlow.PreUpdateTick += ClearCollections;
            GameFlow.UpdateTick += OnUpdateTick;
            _playerInput.ChangeAtackState += OnPlayerChangeAttackState;
            EventBus.NonPlayerChangeAttackState += OnNonPlayerChangeAttackState;
        }

        protected override void Unsubscribe()
        {
            GameFlow.PreUpdateTick -= ClearCollections;
            GameFlow.UpdateTick -= OnUpdateTick;
            _playerInput.ChangeAtackState -= OnPlayerChangeAttackState;
            EventBus.NonPlayerChangeAttackState += OnNonPlayerChangeAttackState;
        }

        private void OnUpdateTick(float dTime)
        {
            //Aim(dTime);

            PlayerAim(dTime);
            //OthersAim(dTime);
            //OthersAttack();
        }

        private void ClearCollections()
        {
            foreach (var ship in _stopAttackingShips)
            {
                _attackingShips.Remove(ship);
            }

            _stopAttackingShips.Clear();
        }

        private void Aim(float dTime)
        {
            foreach (var shipData in _objectsStorage.NonPlayerShipsData)
            {
                AimToTarget(shipData.EquipData.WeaponSlots, shipData.TargetPos, dTime);
            }
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

            ref var playerShipData = ref _objectsStorage.PlayerShipData;
            OnChangeAttackState(playerShipData.EquipData.WeaponSlots, state);
        }

        private void OnNonPlayerChangeAttackState(ShipInstance ship, bool state)
        {
            //if (state)
            //{
            //    _attackingShips.Add(ship);
            //}
            //else
            //{
            //    _stopAttackingShips.Add(ship);
            //}

            //OnChangeAttackState(ship, state);
        }

        private void OnChangeAttackState(WeaponSlot[] weaponSlots, bool state)
        {
            foreach (var slot in weaponSlots)
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
                ProceedWeaponShoot(ship.ShipInitialData.EquipData.WeaponSlots);
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
            ref var playerShipData = ref _objectsStorage.PlayerShipData;
            var weaponSlots = playerShipData.EquipData.WeaponSlots;
            AimToTarget(weaponSlots, playerShipData.TargetPos, dTime);
        }

        private void PlayerAttack(float dTime)
        {
            ref var shipData = ref _objectsStorage.PlayerShipData;

            ProceedWeaponShoot(shipData.EquipData.WeaponSlots);
        }

        private void OthersAim(float dTime)
        {
            foreach (var ship in _attackingShips)
            {

                //ref var shipData = ref _objectsStorage.ShipsData[ship.ShipInitialData.Index];
                //var weaponSlots = ship.ShipInitialData.EquipData.WeaponSlots;
                //AimToTarget(weaponSlots, _playerShip.ShipInitialData.TargetPos, dTime);
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
