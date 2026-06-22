using DefenseLayers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{
    public class ShipInstance : MonoBehaviour
    {
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public List<WeaponSlot> WeaponSlots { get; private set; }
        [field: SerializeField] public DefenseLayerBase[] DefenseLayers { get; private set; }
        [field: SerializeField] public Collider2D HullCollider { get; private set; }
        public HashSet<Collider2D> OwnColliders = new();

        public bool IsAttacking;
        public WeaponGroup ActiveWeaponGroup;
        public AimData AimData;
        public MovementStats MovementStats;
        public MovementRuntimeData MovementRuntimeData;

        public EquipData Equip;
        public MovementView MovementView;

        public ResistanceStatsData ResistanceStats;
    }
}

[Serializable]
public struct ResistanceStatsData
{
    public float Energy;
    public float Kinetic;
}

[Serializable]
public enum CollisionType
{
    Asteroid,
    S,
    M,
    L,
    Xl,
    SShield,
    MShield,
    LShield,
    XlShield
}