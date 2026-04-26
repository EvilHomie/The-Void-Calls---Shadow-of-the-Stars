using UnityEngine;
using Weapons;

namespace Ships
{
    public class WeaponSlot : MonoBehaviour
    {
        public bool IsActive;
        [field: SerializeField] public WeaponBase Weapon { get; set; }
    }
}