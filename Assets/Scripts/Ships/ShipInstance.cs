using DefenseLayers;
using GamePools;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Weapons;

namespace Ships
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipInstance : PoolObjectBase
    {
        [field: SerializeField] public ShipId Id { get; private set; }
        public ShieldDefenseLayer Shield { get; private set; }
        public HullDefenseLayer Hull { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public WeaponSlot[] AllWeaponSlots { get; private set; }
        public List<WeaponSlot> MainWeaponsSlots { get; private set; } = new();
        public List<WeaponSlot> TurretsSlots { get; private set; } = new();
        public HashSet<Collider2D> OwnColliders = new();

        public AimData AimData;
        public MovementStats MovementStats;
        public MovementRuntimeData MovementRuntimeData;

        public EquipData Equip;
        public MovementView MovementView;

        public ShipControl Control;

        protected override void OnResolveDependencies()
        {
            AimData = new();
            Rigidbody = GetComponent<Rigidbody2D>();
            AllWeaponSlots = GetComponentInChildren<WeaponSlotsContainer>().GetComponentsInChildren<WeaponSlot>();

            foreach (var weaponSlot in AllWeaponSlots)
            {
                weaponSlot.CacheDependencies();
            }

            Hull = GetComponentInChildren<HullDefenseLayer>();
            Hull.CacheDependencies();
            Shield = GetComponentInChildren<ShieldDefenseLayer>();
            Shield.CacheDependencies();
        }
    }

    [Serializable]
    public class AimData
    {
        public Rigidbody2D TargetRigidBody;
        public Vector2 AimPosition;
        public WeaponBase FastetsBoltWeapon;
        public float FastestBoltSpeed;
    }

    [Serializable]
    public struct ShipControl
    {
        public Vector2 MoveDirection;
        public bool DamperEnabled;
        public bool EngineDisabled;
        public bool BoostersEnabled;
        public bool IsShooting;
    }

    [Flags]
    public enum BehaviourState
    {
        None = 0,
        Move = 1 << 0,
        Attack = 1 << 1,
        Escape = 1 << 2,
    }
}