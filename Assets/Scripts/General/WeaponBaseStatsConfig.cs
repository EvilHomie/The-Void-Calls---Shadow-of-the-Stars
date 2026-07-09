using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponBaseStatsConfig", menuName = "Scriptable Objects/WeaponBaseStatsConfig")]
public class WeaponBaseStatsConfig : ScriptableObject
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
            BaseStats[i].Name = BaseStats[i].WeaponType.ToString();

            for (int j = 0; j < BaseStats[i].StatsBySize.Length; j++)
            {
                BaseStats[i].StatsBySize[j].Size = BaseStats[i].StatsBySize[j].SizeType.ToString();
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
    [HideInInspector] public string Size;
    [field: SerializeField] public SizeType SizeType { get; private set; }
    [field: SerializeField] public GeneralStats GeneralStats { get; private set; }
    [field: SerializeField] public ProjectileWeaponStats ProjectileWeaponStats { get; private set; }

}

[Serializable]
public struct GeneralStats
{
    [field: SerializeField] public float Distance { get; private set; }
    [field: SerializeField] public float RotateAngle { get; private set; }
    [field: SerializeField] public float RotateSpeed { get; private set; }
    [field: SerializeField] public float DamageEnergy { get; private set; }
    [field: SerializeField] public float DamageKinetic { get; private set; }
    [field: SerializeField] public float AsteroidMultiplier { get; private set; }
    [field: SerializeField] public float HullPoints { get; private set; }
    [field: SerializeField] public float ArmorPoints { get; private set; }
}

[Serializable]
public struct ProjectileWeaponStats
{
    [field: SerializeField] public float FireRate { get; private set; }
    [field: SerializeField] public float SpreadAngle { get; private set; }
    [field: SerializeField] public float ProjectileSpeed { get; private set; }
    [field: SerializeField] public PoolReference PoolReference { get; private set; }
}