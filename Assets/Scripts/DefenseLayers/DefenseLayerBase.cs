using UnityEngine;

namespace DefenseLayers
{
    public abstract class DefenseLayerBase : MonoBehaviour
    {
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public Collider2D Collider { get; private set; }
    }

    public enum DefenseLayerType
    {
        Shield,
        Hull,
        Asteroid
    }
}