using CoreGameSystems;
using DefenseLayers;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipInstance : MonoBehaviour
    {
        [field: SerializeField] public ShipId Id { get; private set; }
        public ShieldDefenseLayer Shield { get; private set; }
        public Transform Transform { get; private set; }
        public HullDefenseLayer Hull { get; private set; }
        public Transform WeaponSlotsContainer { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public List<WeaponSlot> MainWeaponsSlots { get; private set; } = new();
        public List<WeaponSlot> TurretsSlots { get; private set; } = new();
        public HashSet<Collider2D> OwnColliders = new();

        public bool IsAttacking;
        public WeaponGroup ActiveWeaponGroup;
        public AimData AimData;
        public MovementStats MovementStats;
        public MovementRuntimeData MovementRuntimeData;

        public EquipData Equip;
        public MovementView MovementView;

        public ShipIntentData IntentData;

        public void Init()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            WeaponSlotsContainer = GetComponentInChildren<WeaponSlotsContainer>().transform;
            Hull = GetComponentInChildren<HullDefenseLayer>();
            Shield = GetComponentInChildren<ShieldDefenseLayer>();
            Transform = GetComponent<Transform>();
        }
    }
}