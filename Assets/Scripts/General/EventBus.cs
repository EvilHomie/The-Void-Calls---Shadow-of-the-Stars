using Projectiles;
using Ships;
using System;
using UnityEngine;
using Weapons;

namespace GameSystems
{
    public class EventBus : MonoBehaviour
    {
        public static Action<ShipInstance, bool> NonPlayerChangeAttackState { get; set; }
        public static Action<WeaponBase, Collider2D> BeamHit { get; set; }


        // делегаты для оружия со снарядами Bolt
        public delegate void BoltWeaponShootDelegate(in BoltWeaponShootData data);
        public static BoltWeaponShootDelegate BoltWeaponShootAction;

        public delegate void BoltHitDelegate(Bolt bolt, Collider2D collider2D);
        public static BoltHitDelegate BoltHitAction;
    }
}

