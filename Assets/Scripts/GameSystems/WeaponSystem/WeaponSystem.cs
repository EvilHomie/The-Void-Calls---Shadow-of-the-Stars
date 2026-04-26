//using DI;
//using GameCamera;
//using GameInput;
//using Helpers;
//using Ships;
//using System.Collections.Generic;
//using UnityEngine;
//using Weapons;

//namespace GameSystems
//{
//    public class WeaponSystem : GameSystemBase
//    {
//        private WeaponsBehaviour _weaponsBehaviour;
//        private readonly HashSet<WeaponBase> _activeWeapons = new(200);
//        private readonly HashSet<WeaponBase> _disabledWeapons = new(200);

//        private IPlayerInput _playerInput;
//        private ShipInstance _playerShip;
//        private MouseCursor _mouseCursor;
//        private bool _playerAttacking;

//        [Inject]
//        public void Construct(IPlayerInput playerInput, ShipInstance playerShip, MouseCursor mouseCursor)
//        {
//            _playerInput = playerInput;
//            _playerShip = playerShip;
//            _weaponsBehaviour = new WeaponsBehaviour();
//        }

//        protected override void AwakeInit()
//        {

//        }

//        protected override void Subscribe()
//        {
//            GameFlowSystem.PreUpdateTick += ClearCollections;
//            GameFlowSystem.UpdateTick += OnUpdateTick;
//            _playerInput.ChangeAtackState += OnPlayerChangeAttackState;
//            EventBus.NonPlayerChangeAttackState += OnNonPlayerChangeAttackState;
//        }

//        protected override void Unsubscribe()
//        {
//            GameFlowSystem.PreUpdateTick -= ClearCollections;
//            GameFlowSystem.UpdateTick -= OnUpdateTick;
//            _playerInput.ChangeAtackState -= OnPlayerChangeAttackState;
//            EventBus.NonPlayerChangeAttackState += OnNonPlayerChangeAttackState;
//        }

//        private void OnUpdateTick(float dTime)
//        {
//            PlayerAim(dTime);

//            Aim(dTime);

//            OthersAim(dTime);
//            OthersAttack();
//        }

//        private void ClearCollections()
//        {
//            foreach (var ship in _disabledWeapons)
//            {
//                _activeWeapons.Remove(ship);
//            }

//            _disabledWeapons.Clear();
//        }

//        private void PlayerAim(float dTime)
//        {
//            foreach (var weaponSlot in _playerShip.WeaponSlots)
//            {
//                WeaponSystemHelper.AimAtTarget(weaponSlot.Weapon, dTime, _mouseCursor.WorldPostition);
//            }
//        }

//        private void Aim(float dTime)
//        {
//            for (int i = 0; i <= _objectsStorage.LastUsedIndex; i++)
//            {
//                ref var view = ref _objectsStorage.Views[i];
//                ref var aimPos = ref _objectsStorage.TargetPositions[i];
//                AimToTarget(view.WeaponSlots, aimPos, dTime);
//            }
//        }

//        private void OnPlayerChangeAttackState(bool state)
//        {
//            _playerAttacking = state;

//            OnChangeAttackState(_playerShip.WeaponSlots, state);
//        }

//        private void OnNonPlayerChangeAttackState(ShipInstance ship, bool state)
//        {
//            if (state)
//            {
//                _attackingShips.Add(ship);
//            }
//            else
//            {
//                _stopAttackingShips.Add(ship);
//            }

//            OnChangeAttackState(ship, state);
//        }

//        private void OnChangeAttackState(WeaponSlot[] weaponSlots, bool state)
//        {
//            foreach (var slot in weaponSlots)
//            {
//                if (!slot.IsActive)
//                {
//                    return;
//                }

//                if (state)
//                {
//                    _weaponsBehaviour.StartShoot(slot.Weapon);
//                }
//                else
//                {
//                    _weaponsBehaviour.CancelShoot(slot.Weapon);
//                }
//            }
//        }

//        private void OthersAttack()
//        {
//            foreach (var ship in _attackingShips)
//            {
//                ProceedWeaponShoot(ship.ShipInitialData.EquipData.WeaponSlots);
//            }
//        }

//        private void ProceedWeaponShoot(WeaponSlot[] weaponSlots)
//        {
//            foreach (var slot in weaponSlots)
//            {
//                if (!slot.IsActive)
//                {
//                    continue;
//                }

//                _weaponsBehaviour.ProceedShoot(slot.Weapon);
//            }
//        }

//        private void PlayerAttack(float dTime)
//        {
//            ref var view = ref _objectsStorage.Views[_objectsStorage.PlayerIndex];

//            ProceedWeaponShoot(view.WeaponSlots);
//        }

//        private void OthersAim(float dTime)
//        {
//            foreach (var ship in _attackingShips)
//            {

//                ref var shipData = ref _objectsStorage.ShipsData[ship.ShipInitialData.Index];
//                var weaponSlots = ship.ShipInitialData.EquipData.WeaponSlots;
//                AimToTarget(weaponSlots, _playerShip.ShipInitialData.TargetPos, dTime);
//            }
//        }

//        private void AimToTarget(WeaponSlot[] weaponSlots, Vector2 targetpos, float dTime)
//        {
//            foreach (var weaponSlot in weaponSlots)
//            {
//                if (weaponSlot.IsActive)
//                {
//                    WeaponSystemHelper.AimAtTarget(weaponSlot.Weapon, dTime, targetpos);
//                }
//            }
//        }
//    }
//}
