using DefenseLayers;
using Projectiles;
using System;
using UnityEngine;
using Weapons;

namespace GameSystems
{
    public class EventBus : MonoBehaviour
    {
        public static Action<WeaponBase, bool> WeaponChangeAttackStateAction;

        // делегаты для лучего оружия (без промежуточных элементов по типу снаряда). Хит регистрируется сразу в оружии.
        public delegate void BeamHitDelegate(in DamageData damageData , DefenseLayerBase layer, Vector2 position);
        public static BeamHitDelegate BeamHitAction;


        // делегаты для оружия со снарядами Bolt
        public delegate void BoltWeaponShootDelegate(in BoltWeaponShootData data);
        public static BoltWeaponShootDelegate BoltWeaponShootAction;

        public delegate void BoltHitDelegate(Bolt bolt, DefenseLayerBase layer, Vector2 position);
        public static BoltHitDelegate BoltHitAction;

        public static Action<DefenseLayerBase, Vector2> DefenseLayerHitAction;
        public static Action<DefenseLayerBase, Vector2> DefenseLayerCollisionAction;

        public static Action<Collider2D, Transform, bool> OnShieldCross;
    }
}

