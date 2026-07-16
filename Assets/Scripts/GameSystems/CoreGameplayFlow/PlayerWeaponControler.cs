using General;
using Helpers;
using Ships;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace CoreGameSystems
{
    public class PlayerWeaponControler
    {
        private readonly Dictionary<Key, WeaponGroup> _groupBindings = new()
        {
            { Key.Digit1, WeaponGroup.Group1 },
            { Key.Digit2, WeaponGroup.Group2 },
            { Key.Digit3, WeaponGroup.Group3 },
            { Key.Digit4, WeaponGroup.Group4 },
            { Key.Digit5, WeaponGroup.Group5 },
        };

        public void OnSwitchWeaponGroupAction(Key key, ShipInstance shipInstance, bool isShooting)
        {
            var newActiveGroup = _groupBindings[key];

            if (!isShooting) // проста€ смена состо€ни€ слота т.к. не стрел€ет
            {
                foreach (var slot in shipInstance.MainWeaponsSlots)
                {
                    if (slot.Weapon == null)
                    {
                        slot.IsInActiveGroup = false;
                        continue;
                    }

                    slot.IsInActiveGroup = slot.WeaponGroup.ContainsAny(newActiveGroup);
                }

                return;
            }

            foreach (var slot in shipInstance.MainWeaponsSlots) // логика в случае если мен€ютс€ группы во врем€ аттаки
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

        public void OnPlayerChangeAttackState(ShipInstance shipInstance, bool isShooting)
        {
            foreach (var slot in shipInstance.MainWeaponsSlots)
            {
                if (slot.Weapon == null || !slot.IsInActiveGroup) continue;

                EventBus.WeaponChangeAttackStateAction?.Invoke(slot.Weapon, isShooting);
            }
        }
    }
}