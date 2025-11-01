using UnityEngine;

public class Batching : MonoBehaviour
{
    private SpriteRenderer _sr;
    private MaterialPropertyBlock _matBlock;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _matBlock = new();
    }

    private void OnEnable()
    {
        _sr.SetPropertyBlock(_matBlock);
    }
}