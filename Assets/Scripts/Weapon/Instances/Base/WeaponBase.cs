using Ships;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IShipModule
    {
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public Transform WeaponTransform { get; private set; }
        [field: SerializeField] public Transform ShootPointTransform { get; private set; }
        [field: SerializeField] public Collider2D Collider { get; private set; }
        public AimData AimData { get; private set; }
        public HashSet<Collider2D> IgnoredColliders { get; private set; }

        public ShootPointTransformData ShootPointTransformData;
        public WeaponBaseDamage BaseDamage;
        public WeaponRuntimeDamage RuntimeDamage;
        public WeaponAimStats BaseAimStats;
        public WeaponAimStats RuntimeAimStats;


        public void InitBase(AimData targetData, SizeType size, HashSet<Collider2D> ignoredColliders)
        {
            AimData = targetData;
            Size = size;
            IgnoredColliders = ignoredColliders;
        }
    }

    [Serializable]
    public struct WeaponBaseDamage
    {
        public float DamageEnergy;
        public float DamageKinetic;
        public float AsteroidMultiplier;
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
    public struct ShootPointTransformData
    {
        public Vector2 Position;
        public Vector2 Direction;
    }
}