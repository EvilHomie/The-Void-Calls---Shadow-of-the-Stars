using System.Collections.Generic;

public class WorldConfig
{
    public static readonly float WorldUnitMod = 0.01f; // модификатор мирового пространства. т.е. при 100, 1 еденица пространства это 100 метров  
    public static readonly float WorldUnitModReversed = 100f; // модификатор мирового пространства. т.е. при 100, 1 еденица пространства это 100 метров  
    public static readonly float InertiaDampingForce = 50f; // значение торможения за еденицу drag у корабля. Будто пассивное торможение 

    public static readonly float MaxResistance = 0.9f;

    public static readonly float AsteroidBaseMass = 10; // масса для астероидов в тоннах при scale = 1;

    public static readonly float ClusterAsteroidAngularDamping = 0.1f;
    public static readonly float ClusterAsteroidLinearDamping = 0.2f;
    public static readonly float DriftingAsteroidAngularDamping = 0;
    public static readonly float DriftingAsteroidLinearDamping = 0;

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