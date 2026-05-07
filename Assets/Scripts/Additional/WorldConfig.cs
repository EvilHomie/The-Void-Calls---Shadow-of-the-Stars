public class WorldConfig
{
    public static readonly float WorldUnitMod = 0.01f; // модификатор мирового пространства. т.е. при 100, 1 еденица пространства это 100 метров  
    public static readonly float WorldUnitModReversed = 100f; // модификатор мирового пространства. т.е. при 100, 1 еденица пространства это 100 метров  
    public static readonly float InertiaDampingForce = 50f; // значение торможения за еденицу drag у корабля. Будто пассивное торможение 

    public static readonly float MaxResistance = 0.9f;
}