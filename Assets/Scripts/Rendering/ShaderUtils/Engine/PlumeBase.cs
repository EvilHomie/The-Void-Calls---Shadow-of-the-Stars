using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class PlumeBase : MonoBehaviour
{
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    public MaterialPropertyBlock MatBlock;

    public void Init()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        MatBlock = new();
    }
}