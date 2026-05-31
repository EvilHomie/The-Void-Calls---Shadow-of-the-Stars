using Ships;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IShipModule
    {
        
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public Transform ShootPoint { get; private set; }
        public AimData AimData { get; private set; }
        public HashSet<Collider2D> IgnoredColliders;

        [HideInInspector] public DamageData DamageData;
        [HideInInspector] public ShootPointData ShootPointData;
        public WeaponBaseStats BaseStats;
        public AimStats AimStats;

        public void Init(AimData targetData, SizeType size)
        {
            AimData = targetData;
            Size = size;
            ShootPointData.ZDepth = ShootPoint.position.z;
        }
    }

    [Serializable]
    public struct DamageData
    {
        public float Energy;
        public float Kinetic;
        public float Asteroid;
    }

    [Serializable]
    public struct AimStats
    {
        public float MaxDistance;
        public float MaxRotateAngle;
        public float RotateSpeed;
    }

    [Serializable]
    public struct WeaponBaseStats
    {
        public float DamageEnergy;
        public float DamageKinetic;
        public float DamageMultipliersEnergy;
        public float DamageMultipliersKinetic;
        public float DamageMultipliersAsteroid;
    }

    [Serializable]
    public struct ShootPointData
    {
       public Vector2 Position;
       public Vector2 Direction;
       public float ZDepth;
    }

    [Serializable]
    public struct HitData
    {
        public Vector2 Position;
        public SizeType Size;
    }
}