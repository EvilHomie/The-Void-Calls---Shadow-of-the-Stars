using UnityEngine;
using Weapons;

namespace Ships
{
    public class WeaponSlot : MonoBehaviour
    {
        [field: SerializeField] public bool IsActive { get; set; }
        [field: SerializeField] public WeaponBase Weapon { get; set; }
    }
}