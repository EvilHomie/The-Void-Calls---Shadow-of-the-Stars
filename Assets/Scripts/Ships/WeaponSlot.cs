using System;
using UnityEngine;
using Weapons;

namespace Ships
{
    public class WeaponSlot : MonoBehaviour
    {
        public WeaponGroup WeaponGroup;
        public WeaponMountType WeaponMountType;
        [field: SerializeField] public WeaponBase Weapon { get; set; }
    }
}

[Flags]
public enum WeaponGroup
{
    None = 0,
    Group1 = 1 << 0,
    Group2 = 1 << 1,
    Group3 = 1 << 2,
    Group4 = 1 << 3,
    Group5 = 1 << 4,
}

public enum WeaponMountType
{
    MainWeapon,
    Turret
}