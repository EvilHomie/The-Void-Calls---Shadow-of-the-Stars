using DefenseLayers;
using Ships;
using System;
using UnityEngine;
using Weapons;

namespace GameSystems
{
    public class EventBus : MonoBehaviour
    {
        public static Action<WeaponBase, bool> WeaponChangeAttackStateAction;

        public delegate void HitDelegate(DamageData damageData, DefenseLayerBase layer, HitData hitData);
        public static HitDelegate HitAction;

        public delegate void BoltWeaponShootDelegate(in BoltWeaponShootData data);
        public static BoltWeaponShootDelegate BoltWeaponShootAction;

        public static Action<DefenseLayerBase, HitData> DefenseLayerHitAction;
        public static Action<DefenseLayerBase, HitData> DefenseLayerCollisionAction;

        public static Action<Collider2D, Transform, bool> OnShieldCross;

        public static Action PlayerSwitchWeaponGroupAction;
        public static Action<ShipInstance, Rigidbody2D> ChangeTargetAction;
    }
}

