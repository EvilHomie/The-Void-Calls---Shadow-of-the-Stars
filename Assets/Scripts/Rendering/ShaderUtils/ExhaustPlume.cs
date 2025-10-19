using UnityEngine;

public class ExhaustPlume : MonoBehaviour
{
    [SerializeField] float _masterThrust;
    private SpriteRenderer _sr;
    private MaterialPropertyBlock _block;

    private static readonly int _masterThrustID = Shader.PropertyToID("_MasterThrust");

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _block = new MaterialPropertyBlock();
        _sr.maskInteraction = SpriteMaskInteraction.None;
        SetThrustValue(0);
        _masterThrust = 0;
    }

    private void OnValidate()
    {
        if (_sr == null)
            _sr = GetComponent<SpriteRenderer>();
        if (_block == null)
            _block = new MaterialPropertyBlock();

        _masterThrust = Mathf.Clamp01(_masterThrust);
        SetThrustValue(_masterThrust);
    }

    public void SetThrustValue(float value)
    {
        value = Mathf.Clamp01(value);

        _sr.GetPropertyBlock(_block);
        _block.SetFloat(_masterThrustID, value);
        _sr.SetPropertyBlock(_block);
    }
}
