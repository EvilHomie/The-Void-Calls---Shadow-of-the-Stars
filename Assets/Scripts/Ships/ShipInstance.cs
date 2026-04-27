using Damage;
using System;
using UnityEngine;

namespace Ships
{
    public class ShipInstance : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public WeaponSlot[] WeaponSlots { get; private set; }

        public TargetData TargetData;
        public MovementStaticData MovementStaticData;
        public MovementRuntimeData MovementRuntimeData;

        public EquipData Equip;
        public View View;
    }

    [Serializable]
    public struct TargetData
    {
        public Transform Transform;
        public Vector2 Position;
    }
}