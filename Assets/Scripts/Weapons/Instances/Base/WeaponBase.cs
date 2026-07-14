using DefenseLayers;
using ShipModules;
using Ships;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IShipModule
    {
        [field: SerializeField] public ModuleType ModuleType { get; private set; }
        public SizeType Size { get; private set; }
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        public Transform ShootPointTransform { get; private set; }
        public HullDefenseLayer HullDefenseLayer { get; private set; }
        public Transform Transform { get; private set; }
        public AimData AimData { get; private set; }
        public HashSet<Collider2D> IgnoredColliders { get; private set; }

        public float RotateAngle;
        public ShootPointRuntimeData ShootPointRuntimeData;
        public WeaponRuntimeDamage RuntimeDamage;
        public WeaponAimStats RuntimeAimStats;

        public void CacheDependencies()
        {
            HullDefenseLayer = GetComponentInChildren<HullDefenseLayer>();
            ShootPointTransform = GetComponentInChildren<ShootPoint>().transform;
            Transform = transform;
            OnCacheDependencies();
        }

        protected virtual void OnCacheDependencies() { }

        public void SetupBase(AimData targetData, SizeType size, HashSet<Collider2D> ignoredColliders)
        {
            AimData = targetData;
            Size = size;
            IgnoredColliders = ignoredColliders;            
        }
    }    

    [Serializable]
    public struct WeaponRuntimeDamage
    {
        public float DamageShield;
        public float DamageArmor;
        public float DamageHull;
        public float DamageAsteroid;
    }

    [Serializable]
    public struct WeaponAimStats
    {
        public float MaxDistance;
        public float MaxRotateAngle;
        public float RotateSpeed;
    }

    [Serializable]
    public struct ShootPointRuntimeData
    {
        public Vector2 Position;
        public Vector2 Direction;
    }
}