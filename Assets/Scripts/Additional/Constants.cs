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

    public static readonly int WorldUnitMod = 100; // модификатор мирового пространства. т.е. при 100, 1 еденица пространства это 100 метров
    public static readonly float DeffCameraOrtoSize = 5f;
}
