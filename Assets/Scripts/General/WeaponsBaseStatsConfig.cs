using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "WeaponsBaseStatsConfig", menuName = "Scriptable Objects/WeaponsBaseStatsConfig")]
    public class WeaponsBaseStatsConfig : ScriptableObject
    {
        [field: SerializeField] public WeaponBaseStats[] BaseStats { get; private set; }

        private Dictionary<(WeaponType, SizeType), WeaponStatsBySize> _statsByWeapon;

        public void FillCollection()
        {
            _statsByWeapon ??= new();

            for (int i = 0; i < BaseStats.Length; i++)
            {
                for (int j = 0; j < BaseStats[i].StatsBySize.Length; j++)
                {
                    var stats = BaseStats[i].StatsBySize[j];
                    _statsByWeapon.Add((BaseStats[i].WeaponType, stats.SizeType), stats);
                }
            }
        }

        public WeaponStatsBySize GetStats(WeaponType weaponType, SizeType sizeType)
        {
            return _statsByWeapon[(weaponType, sizeType)];
        }

#if UNITY_EDITOR
        private void OnValidate() // просто для красоты чтобы в коллекции вместо номера элемента была конкретика
        {
            for (int i = 0; i < BaseStats.Length; i++)
            {
                var name = BaseStats[i].WeaponType.ToString();
                BaseStats[i].Name = name;

                for (int j = 0; j < BaseStats[i].StatsBySize.Length; j++)
                {
                    BaseStats[i].StatsBySize[j].Name = name;
                    BaseStats[i].StatsBySize[j].WeaponType = BaseStats[i].WeaponType;
                }
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }


    [Serializable]
    public struct WeaponBaseStats
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public WeaponStatsBySize[] StatsBySize { get; private set; }
    }

    [Serializable]
    public struct WeaponStatsBySize
    {
        [HideInInspector] public string Name;
        [HideInInspector] public WeaponType WeaponType;
        [field: SerializeField] public SizeType SizeType { get; private set; }
        [field: SerializeField] public AimStats AimStats { get; private set; }
        [field: SerializeField] public DamageStats DamageStats { get; private set; }
        [field: SerializeField] public HullStats HullStats { get; private set; }
        [field: SerializeField] public ProjectileWeaponStats ProjectileWeaponStats { get; private set; }
        [field: SerializeField] public ConstantBeamWeaponStats ConstantBeamWeaponStats { get; private set; }
    }

    [Serializable]
    public struct AimStats
    {
        [field: SerializeField, Min(0.001f)] public float Distance { get; private set; }
        [field: SerializeField, Min(0)] public float RotateAngle { get; private set; }
        [field: SerializeField, Min(0)] public float RotateSpeed { get; private set; }
    }

    [Serializable]
    public struct ProjectileWeaponStats
    {
        [field: SerializeField, Min(0.001f)] public float FireRate { get; private set; }
        [field: SerializeField, Min(0)] public float SpreadAngle { get; private set; }
        [field: SerializeField, Min(0.001f)] public float ProjectileSpeed { get; private set; }
        [field: SerializeField] public PoolReference PoolReference { get; private set; }
    }

    [Serializable]
    public struct ConstantBeamWeaponStats
    {
        public float TESTFIELD;
        //[field: SerializeField] public float FireRate { get; private set; }
        //[field: SerializeField] public float SpreadAngle { get; private set; }
        //[field: SerializeField] public float ProjectileSpeed { get; private set; }
        //[field: SerializeField] public PoolReference PoolReference { get; private set; }
    }

    [Serializable]
    public struct DamageStats
    {
        [field: SerializeField, Min(0)] public float DamageEnergy { get; private set; }
        [field: SerializeField, Min(0)] public float DamageKinetic { get; private set; }
        [field: SerializeField, Min(0)] public float AsteroidBonusPercent { get; private set; }
    }

    [Serializable]
    public struct HullStats
    {
        [field: SerializeField, Min(1)] public float HullPoints { get; private set; }
        [field: SerializeField, Min(0)] public float ArmorPoints { get; private set; }
    }
}