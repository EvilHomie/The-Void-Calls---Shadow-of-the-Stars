using Ships;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IShipModule
    {
        public WeaponBaseStats BaseStats;
        public CurrentDamageData CurrentDamageData;
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public Transform ShootPoint { get; private set; }

        public AimData AimData { get; private set; }

        public HashSet<Collider2D> IgnoredColliders;

        public void Init(AimData targetData)
        {
            AimData = targetData;
        }
    }

    [Serializable]
    public struct CurrentDamageData
    {
        public float Energy;
        public float Kinetic;
        public float Asteroid;
    }

    [Serializable]
    public struct WeaponBaseStats
    {
        public float MaxDistance;
        public float MaxRotateAngle;
        public float RotateSpeed;
        public float BaseDamageEnergy;
        public float BaseDamageKinetic;
        public float EnergyDamageMultipliers;
        public float KineticDamageMultipliers;
        public float AsteroidDamageMultipliers;
    }
}