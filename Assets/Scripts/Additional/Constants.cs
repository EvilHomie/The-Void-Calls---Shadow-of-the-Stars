using UnityEngine;

public static class Constants
{
    public static readonly Vector2 Vector2Up = Vector2.up;
    public static readonly Vector2 Vector2Right = Vector2.right;
    public static readonly Vector2 Vector2Left = Vector2.left;
    public static readonly Vector2 Vector2Down = Vector2.down;
    public static readonly Vector2 Vector2Zero = Vector2.zero;
    public static readonly SideEnginesPower SideEnginesPowerZero = new();

    //[Range(0.9f, 0.99f)]
    //public float _dampingStrengthMin = 1f; // базовая сила гашения
    //[Range(0.97f, 0.999f)]
    //public float _dampingStrengthMax = 1f; // базовая сила гашения
}
