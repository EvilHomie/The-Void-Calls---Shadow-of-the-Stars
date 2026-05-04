using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ThrusterPlume : MonoBehaviour
{
    private SpriteRenderer _sr;
    private MaterialPropertyBlock _matBlock;
    private static readonly int _powerValueID = Shader.PropertyToID("_PowerValue");

    private float _lastPowerValue = -100f;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _sr = GetComponent<SpriteRenderer>();
        _matBlock = new MaterialPropertyBlock();
        _sr.maskInteraction = SpriteMaskInteraction.None;
        SetPowerValue(0);
    }
    public void SetPowerValue(float value)
    {
        if (_lastPowerValue == value) return;

        _lastPowerValue = value;
        _matBlock.SetFloat(_powerValueID, value);
        _sr.SetPropertyBlock(_matBlock);
    }
}
