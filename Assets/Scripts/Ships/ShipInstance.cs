using CoreGameSystems;
using DefenseLayers;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipInstance : MonoBehaviour
    {
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public Transform WeaponSlotsContainer { get; private set; }
        [field: SerializeField] public ShieldDefenseLayer Shield { get; private set; }
        [field: SerializeField] public HullDefenseLayer Hull { get; private set; }
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

            var weaponsSlots = WeaponSlotsContainer.GetComponentsInChildren<WeaponSlot>();
            MainWeaponsSlots.Clear();
            TurretsSlots.Clear();

            foreach (var slot in weaponsSlots)
            {
                var weaponsCollection = slot.WeaponMountType == WeaponMountType.MainWeapon ? MainWeaponsSlots : TurretsSlots;
                weaponsCollection.Add(slot);
            }
        }
    }
}