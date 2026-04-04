using UnityEngine;

namespace Ship
{
    public class ShipData : MonoBehaviour
    {
        public Vector3 TargetPos;
        [field: SerializeField] public ShipChassisData ChassisData { get; private set; }
        [field: SerializeField] public ShipEquipData EquipData { get; private set; }
        [field: SerializeField] public ShipMovementData MovementData { get; private set; }
    }
}