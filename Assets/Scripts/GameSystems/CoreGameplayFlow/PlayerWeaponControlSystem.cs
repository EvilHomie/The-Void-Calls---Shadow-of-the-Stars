using DI;
using General;
using Helpers;
using PlayerInput;
using Registries;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreGameSystems
{
    public class PlayerWeaponControlSystem : MonoBehaviour
    {
        private ShipRegistry _shipRegistry;
        private PlayerIntentData _playerIntentData;

        private readonly Dictionary<Key, WeaponGroup> _groupBindings = new()
        {
            { Key.Digit1, WeaponGroup.Group1 },
            { Key.Digit2, WeaponGroup.Group2 },
            { Key.Digit3, WeaponGroup.Group3 },
            { Key.Digit4, WeaponGroup.Group4 },
            { Key.Digit5, WeaponGroup.Group5 },
        };

        [Inject]
        public void Construct(ShipRegistry shipRegistry, PlayerIntentData playerIntentData)
        {
            _shipRegistry = shipRegistry;
            _playerIntentData = playerIntentData;
        }

        public void Execute()
        {
            var toggleAttackSignal = _playerIntentData.InputSnapshot.ToggleAttackSignal;

            if (toggleAttackSignal != SignalState.None)
            {
                OnPlayerChangeAttackState(toggleAttackSignal);
            }

            var newWeaponsGroupKey = _playerIntentData.InputSnapshot.NewWeaponsGroupKey;

            if (newWeaponsGroupKey != Key.None)
            {
                OnSwitchWeaponGroupAction(newWeaponsGroupKey);
            }
        }

        private void OnSwitchWeaponGroupAction(Key key)
        {
            var playerShip = _shipRegistry.PlayerShip;
            ref var controlData = ref playerShip.ControlData;
            var newActiveGroup = _groupBindings[key];
            controlData.ActiveWeaponGroup = newActiveGroup;

            if (!controlData.AttackIsActive) // проста€ смена состо€ни€ слота т.к. не стрел€ет
            {
                foreach (var slot in playerShip.MainWeaponsSlots)
                {
                    if (slot.Weapon == null)
                    {
                        slot.IsInActiveGroup = false;
                        continue;
                    }

                    bool isInNewGroup = slot.WeaponGroup.ContainsAny(newActiveGroup);
                    slot.IsInActiveGroup = isInNewGroup;
                }

                return;
            }

            foreach (var slot in playerShip.MainWeaponsSlots) // логика в случае если мен€ютс€ группы во врем€ аттаки
            {
                var weapon = slot.Weapon;

                if (weapon == null)
                {
                    slot.IsInActiveGroup = false;
                    continue;
                }

                bool isInOldGroup = slot.IsInActiveGroup;
                bool isInNewGroup = slot.WeaponGroup.ContainsAny(newActiveGroup);

                if (isInNewGroup && !isInOldGroup)
                {
                    slot.IsInActiveGroup = true;
                    EventBus.WeaponChangeAttackStateAction?.Invoke(weapon, true);
                }
                else if (isInOldGroup && !isInNewGroup)
                {
                    slot.IsInActiveGroup = false;
                    EventBus.WeaponChangeAttackStateAction?.Invoke(weapon, false);
                }
            }
        }

        private void OnPlayerChangeAttackState(SignalState newSignal)
        {
            var playerShip = _shipRegistry.PlayerShip;
            ref var controlData = ref playerShip.ControlData;
            var attackState = newSignal == SignalState.Performed;
            controlData.AttackIsActive = attackState;

            foreach (var slot in playerShip.MainWeaponsSlots)
            {
                if (slot.Weapon == null || !slot.IsInActiveGroup) continue;

                EventBus.WeaponChangeAttackStateAction?.Invoke(slot.Weapon, attackState);
            }
        }
    }
}