using UnityEngine;

[CreateAssetMenu(fileName = "WeaponBaseStatsConfig", menuName = "Scriptable Objects/WeaponBaseStatsConfig")]
public class WeaponBaseStatsConfig : ScriptableObject
{
    [Header("AimS")]
    public float Distance;
    public float RotateAngle;
    public float RotateSpeed;
    public float FireRate;
    public float SpreadAngle;
    public float ProjectileSpeed;
}

public struct WeaponBaseStats
{
    [field: SerializeField] public WeaponType Type;
    [field: SerializeField] public SizeType Size;
}