using DI;
using General;
using Helpers;
using Registries;
using UnityEngine;

namespace CoreGameSystems
{
    public class PlayerWeaponControlSystem : MonoBehaviour
    {
        private ShipRegistry _shipRegistry;

        [Inject]
        public void Construct(ShipRegistry shipRegistry)
        {
            _shipRegistry = shipRegistry;
        }

        public void Execute()
        {
            var playerShip = _shipRegistry.PlayerShip;
            ref readonly var intentData = ref playerShip.IntentData;

            if (intentData.AttackChangeSignal != ChangeSignal.None)
            {
                OnPlayerChangeAttackState(intentData.AttackChangeSignal);
            }

            if (intentData.ChangeWeaponGroup != playerShip.ActiveWeaponGroup)
            {
                OnSwitchWeaponGroupAction(intentData.ChangeWeaponGroup);
            }
        }

        private void OnSwitchWeaponGroupAction(WeaponGroup newActiveGroup)
        {
            var playerShip = _shipRegistry.PlayerShip;

            if (playerShip.ActiveWeaponGroup == newActiveGroup) return;

            var lastActiveGroup = playerShip.ActiveWeaponGroup;
            playerShip.ActiveWeaponGroup = newActiveGroup;

            if (!playerShip.IsAttacking)
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

                EventBus.PlayerSwitchWeaponsGroupAction?.Invoke();
                return;
            }

            foreach (var slot in playerShip.MainWeaponsSlots)
            {
                var weapon = slot.Weapon;

                if (weapon == null)
                {
                    slot.IsInActiveGroup = false;
                    continue;
                }

                bool isInNewGroup = slot.WeaponGroup.ContainsAny(newActiveGroup);
                bool isInOldGroup = slot.WeaponGroup.ContainsAny(lastActiveGroup);

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

            EventBus.PlayerSwitchWeaponsGroupAction?.Invoke();
        }

        private void OnPlayerChangeAttackState(ChangeSignal signal)
        {
            var playerShip = _shipRegistry.PlayerShip;

            var isAttack = signal == ChangeSignal.Performed;
            playerShip.IsAttacking = isAttack;

            foreach (var slot in playerShip.MainWeaponsSlots)
            {
                bool isInGroup = slot.WeaponGroup.ContainsAny(playerShip.ActiveWeaponGroup);

                if (!isInGroup || slot.Weapon == null) continue;

                EventBus.WeaponChangeAttackStateAction?.Invoke(slot.Weapon, isAttack);
            }
        }
    }
}