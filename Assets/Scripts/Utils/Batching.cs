using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Batching : MonoBehaviour
{
    private SpriteRenderer _sr;
    private MaterialPropertyBlock _matBlock;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _matBlock = new();
        _sr.SetPropertyBlock(_matBlock);
    }

    private void OnEnable()
    {
        _sr.SetPropertyBlock(_matBlock);
    }
}