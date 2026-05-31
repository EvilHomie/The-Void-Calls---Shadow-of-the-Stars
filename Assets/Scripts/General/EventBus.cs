using DefenseLayers;
using Projectiles;
using Ships;
using System;
using UnityEngine;
using Weapons;

namespace GameSystems
{
    public class EventBus : MonoBehaviour
    {
        public static Action<WeaponBase, bool> WeaponChangeAttackStateAction;

        // делегаты для лучего оружия (без промежуточных элементов по типу снаряда). Хит регистрируется сразу в оружии.
        public delegate void BeamHitDelegate(DamageData damageData, DefenseLayerBase layer, HitData hitData);
        public static BeamHitDelegate BeamHitAction;


        // делегаты для оружия со снарядами Bolt
        public delegate void BoltWeaponShootDelegate(in BoltWeaponShootData data);
        public static BoltWeaponShootDelegate BoltWeaponShootAction;

        public delegate void BoltHitDelegate(Bolt bolt, DefenseLayerBase layer, HitData hitData);
        public static BoltHitDelegate BoltHitAction;

        public static Action<DefenseLayerBase, HitData> DefenseLayerHitAction;
        public static Action<DefenseLayerBase, HitData> DefenseLayerCollisionAction;

        public static Action<Collider2D, Transform, bool> OnShieldCross;

        public static Action PlayerSwitchWeaponGroupAction;
        public static Action<ShipInstance, Rigidbody2D> ChangeTargetAction;
    }
}

