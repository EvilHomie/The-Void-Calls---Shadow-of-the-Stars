using DefenseLayers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{
    public class ShipInstance : MonoBehaviour
    {
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public Transform WeaponSlotsContainer { get; private set; }
        [field: SerializeField] public ShieldDefenseLayer Shield { get; private set; }
        [field: SerializeField] public HullDefenseLayer Hull { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public WeaponSlot[] WeaponSlots;
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
            WeaponSlots = WeaponSlotsContainer.GetComponentsInChildren<WeaponSlot>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }
    }
}

[Serializable]
public struct ShipIntentData
{
    // Относится к движению (обрабатывается в физическом тике)
    public Vector2 MoveDirection;
    public bool DamperEnabled;
    public bool ResetThrottle;
    public bool BoostersIsActive;

    // Вне физического тика
    public ChangeSignal AttackChangeSignal;
    public WeaponGroup ChangeWeaponGroup;
    public float ChangeZoom;

}

[Serializable]
public enum ChangeSignal
{
    None,
    Performed,
    Canceled
}