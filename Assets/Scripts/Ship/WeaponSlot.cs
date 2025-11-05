using UnityEngine;
using Weapon;

namespace Ship
{
    public class WeaponSlot : MonoBehaviour
    {
        [field: SerializeField] public bool IsActive { get; set; }
        [field: SerializeField] public WeaponBase Weapon { get; set; }
    }
}