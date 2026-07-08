using System;
using UnityEngine;
using Weapons;

namespace Ships
{
    public class WeaponSlot : MonoBehaviour
    {
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public WeaponMountType WeaponMountType { get; private set; }
        public WeaponBase Weapon { get; private set; }
        public WeaponGroup WeaponGroup;
        public bool IsInActiveGroup;

        public void Init()
        {
            Weapon = GetComponentInChildren<WeaponBase>();
        }
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