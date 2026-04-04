using UnityEngine;

public static class Constants
{
    public static readonly Vector2 Vector2Up = Vector2.up;
    public static readonly Vector2 Vector2Right = Vector2.right;
    public static readonly Vector2 Vector2Left = Vector2.left;
    public static readonly Vector2 Vector2Down = Vector2.down;
    public static readonly Vector2 Vector2Zero = Vector2.zero;
    public static readonly Vector3 Vector3Zero = Vector3.zero;
    public static readonly Vector3 Vector3One = Vector3.one;
    public static readonly Vector3 Vector3Right = Vector3.right;
    public static readonly Vector3 Vector3Back = Vector3.back;
    public static readonly SideEnginesPower SideEnginesPowerZero = new();

    public static readonly float WorldUnitMod = 0.01f; // модификатор мирового пространства. т.е. при 100, 1 еденица пространства это 100 метров    

    public static readonly int RotateMod = 45; // базовая скорость поворота при силе равной сопротивлению
    public static readonly int RotateAngleTreshhold = 2; // отбраковка минимального угла поворота
    private static readonly float StabilizationPower = 0.6f; // модификатор при движении без ускорения при включеном гасителе инерции. Будто мощность для поддержания скорости
    private static readonly float SmoothZone = 0.5f; // чем больше тем раньше начнется плавность
    private static readonly float MaxSmooth = 0.05f; // чем меньше тем более плавно (дольше) добираются последние "метры" скорости
}
