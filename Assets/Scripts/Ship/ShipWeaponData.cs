using GameSystems;
using System;
using UnityEngine;

namespace Ship
{
    [Serializable]
    public class ShipWeaponData
    {
        [field: SerializeField] public WeaponSlot[] WeaponSlots { get; private set; }
    }
}