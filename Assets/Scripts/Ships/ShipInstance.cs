using UnityEngine;

namespace Ships
{
    public class ShipInstance : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public WeaponSlot[] WeaponSlots { get; private set; }

        public Transform LockOnTransform;

        public HealthData HealthData;

        public MovementStaticData MovementStaticData;
        public MovementRuntimeData MovementRuntimeData;

        public EquipData Equip;
        public View View;
    }
}