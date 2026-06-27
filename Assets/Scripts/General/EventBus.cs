using CoreGameSystems;
using DefenseLayers;
using Ships;
using System;
using UnityEngine;
using Weapons;

namespace General
{
    public class EventBus : MonoBehaviour
    {
        public static Action<WeaponBase, bool> WeaponChangeAttackStateAction;

        public delegate void DamageDelegate(WeaponRuntimeDamage damageData, DefenseLayerBase layer, HitData hitData);
        public static DamageDelegate DamageAction;

        public delegate void BoltWeaponShootDelegate(in BoltSpawnData data);
        public static BoltWeaponShootDelegate BoltWeaponShootAction;

        public static Action<Collider2D, Transform, bool> OnShieldCross;

        public static Action PlayerSwitchWeaponsGroupAction;
        public static Action<ShipInstance, Rigidbody2D> ChangeTargetAction;

        public static Action<ShipInstance> PlayerShipSpawned;

        public static Action<ShieldDefenseLayer, HitData> ShieldDamagedAction;
        public static Action<HullDefenseLayer, HitData> ArmorDamagedAction;
        public static Action<HullDefenseLayer, HitData> HullDamagedAction;
        public static Action<AsteroidDefenseLayer, HitData> AsteroidDamagedAction;
    }
}

