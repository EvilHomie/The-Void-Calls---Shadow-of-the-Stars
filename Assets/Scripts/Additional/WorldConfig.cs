using System.Collections.Generic;
using UnityEngine;

public static class WorldConfig
{
    public const float WorldUnitMod = 0.01f; // модификатор мирового пространства. т.е. при 100, 1 еденица пространства это 100 метров  
    public const float WorldUnitModReversed = 100f; // модификатор мирового пространства. т.е. при 100, 1 еденица пространства это 100 метров  
    public const float InertiaDampingForce = 50f; // значение торможения за еденицу drag у корабля. Будто пассивное торможение 

    public const float MaxResistance = 0.9f;

    public const float AsteroidBaseMass = 10; // масса для астероидов в тоннах при scale = 1;
    public const float AsteroidTonHP = 10; // кол-во хп для одной тонны

    public const float ClusterAsteroidAngularDamping = 0.1f;
    public const float ClusterAsteroidLinearDamping = 0.2f;
    public const float DriftingAsteroidAngularDamping = 0;
    public const float DriftingAsteroidLinearDamping = 0;
    public const int MaxMainWeaponSlotsCount = 5;

    public static readonly Dictionary<AsteroidType, float> AsteroidMassModByType = new() // доп модификатор массы в зависимости от типа
    {
        {AsteroidType.Empty, 1f },
        {AsteroidType.Titanium, 1.5f },
        {AsteroidType.Copper, 2f },
        {AsteroidType.Silver, 2.5f },
        {AsteroidType.Nickel, 3f },
        {AsteroidType.Gold, 3.5f }
    };
}

public static class SpriteSortingOrders
{
    //public const int StarryCanvas = -150;
    //public const int WeaponsUnderHull = -100;
    //public const int Engine = -50;
    //public const int Ship = 0;
    //public const int Asteroid = 0;
    //public const int WeaponsOnHull = 50;
    //public const int Projectile = 200;
}

public static class LayersId
{
    public static int ShieldLayer = LayerMask.NameToLayer("Shield");
    public static int ArmorLayer = LayerMask.NameToLayer("Armor");
    public static int HullLayer = LayerMask.NameToLayer("Hull");
}