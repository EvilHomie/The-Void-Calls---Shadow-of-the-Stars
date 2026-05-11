using DefenseLayers;
using System;
using UnityEngine;

namespace Ships
{
    public class ShipInstance : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public SpriteRenderer BodySprite { get; private set; }
        [field: SerializeField] public WeaponSlot[] WeaponSlots { get; private set; }
        [field: SerializeField] public DefenseLayerBase[] DefenseLayers { get; private set; }
        public uint Id;

        public bool IsAttacking;
        public WeaponGroup ActiveWeaponGroup;
        public TargetData TargetData;
        public MovementStats MovementStats;
        public MovementRuntimeData MovementRuntimeData;

        public EquipData Equip;
        public View View;

        public ResistanceStatsData ResistanceStats;
    }
}

[Serializable]
public struct ResistanceStatsData
{
    public float Energy;
    public float Kinetic;
}