using GameSystem;
using System;
using UnityEngine;

namespace Ship
{
    [Serializable]
    public class ShipWeaponData
    {
        [field: SerializeField] public WeaponSlot[] WeaponSlots { get; private set; }

        public void Init()
        {
            foreach (var slot in WeaponSlots)
            {
                EventBus.CreateWeaponAction?.Invoke(slot.Weapon);
            }
        }
    }
}