using Configs;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public const float WorldUnitMod = 0.01f; // модификатор мирового пространства. т.е. при 100, 1 еденица пространства это 100 метров  
    public const float WorldUnitModReversed = 1 / WorldUnitMod; // модификатор мирового пространства. т.е. при 100, 1 еденица пространства это 100 метров  
    public const float InertiaDampingForce = 50f; // значение торможения за еденицу drag у корабля. Будто пассивное торможение 

    public const float AsteroidBaseMass = 10; // масса для астероидов в тоннах при scale = 1;
    public const float AsteroidTonHP = 10; // кол-во хп для одной тонны

    public const float ClusterAsteroidAngularDamping = 0.1f;
    public const float ClusterAsteroidLinearDamping = 0.2f;
    public const float DriftingAsteroidAngularDamping = 0;
    public const float DriftingAsteroidLinearDamping = 0;
    public const int MaxMainWeaponSlotsCount = 20;
    public const float ConstantBeamHitRate = 15;

    public static readonly Dictionary<AsteroidType, float> AsteroidMassModByType = new() // доп модификатор массы в зависимости от типа
    {
        {AsteroidType.Empty, 1f },
        {AsteroidType.Titanium, 1.5f },
        {AsteroidType.Copper, 2f },
        {AsteroidType.Silver, 2.5f },
        {AsteroidType.Nickel, 3f },
        {AsteroidType.Gold, 3.5f }
    };

    public static readonly Dictionary<SizeType, float> SizeMap = new()
    {
        {SizeType.S, 1 },
        {SizeType.M, 2 },
        {SizeType.L, 4 },
        {SizeType.XL, 8 }
    };

    [SerializeField] WeaponsBaseStatsConfig mainWeaponsBaseStats;
    [SerializeField] WeaponsBaseStatsConfig turretsBaseStats;
    [SerializeField] ChassisBaseStatsConfig chassisBaseStats;

    public static WeaponsBaseStatsConfig MainWeaponsBaseStats { get; private set; }
    public static WeaponsBaseStatsConfig TurretsBaseStats { get; private set; }
    public static ChassisBaseStatsConfig ChassisBaseStats { get; private set; }

    private void Awake()
    {
        mainWeaponsBaseStats.FillCollection();
        MainWeaponsBaseStats = mainWeaponsBaseStats;

        turretsBaseStats.FillCollection();
        TurretsBaseStats = turretsBaseStats;

        chassisBaseStats.FillCollection();
        ChassisBaseStats = chassisBaseStats;
    }
}

//public static class SpriteSortingOrders
//{
//    //public const int StarryCanvas = -150;
//    //public const int WeaponsUnderHull = -100;
//    //public const int Engine = -50;
//    //public const int Ship = 0;
//    //public const int Asteroid = 0;
//    //public const int WeaponsOnHull = 50;
//    //public const int Projectile = 200;
//}

public static class GameLayers
{
    public static readonly int WeaponLayer = LayerMask.NameToLayer("Weapon");
    public static readonly int AsteroidsLayer = LayerMask.NameToLayer("Asteroid");
    public static readonly int ShipLayer = LayerMask.NameToLayer("Ship");
    public static readonly int StationLayer = LayerMask.NameToLayer("Station");
    public static readonly int ProjectileLayer = LayerMask.NameToLayer("Projectile");
    public static readonly int InterceptableProjectileLayer = LayerMask.NameToLayer("InterceptableProjectile");

    public static readonly int WeaponMask = LayerMask.GetMask("Weapon");
    public static readonly int AsteroidsMask = LayerMask.GetMask("Asteroid");
    public static readonly int ShipMask = LayerMask.GetMask("Ship");
    public static readonly int StationMask = LayerMask.GetMask("Station");
    public static readonly int Projectile = LayerMask.GetMask("Projectile");
    public static readonly int InterceptableProjectile = LayerMask.GetMask("InterceptableProjectile");
    public static readonly int DamageableMask = AsteroidsMask | ShipMask | StationMask | InterceptableProjectile | WeaponMask;
    public static readonly int InterceptMask = AsteroidsMask;
}