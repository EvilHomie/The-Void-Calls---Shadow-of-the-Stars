using UnityEngine;

namespace Ship
{
    public class ShipData : MonoBehaviour
    {        
        [field: SerializeField] public Size Size { get; private set; }
        [field: SerializeField] public ShipChassisData ChassisData { get; private set; }
        [field: SerializeField] public ShipMovementData MovementData { get; private set; }
        [field: SerializeField] public ShipMovementView MovementView { get; private set; }
        [field: SerializeField] public ShipWeaponData WeaponData { get; private set; }
    }
}