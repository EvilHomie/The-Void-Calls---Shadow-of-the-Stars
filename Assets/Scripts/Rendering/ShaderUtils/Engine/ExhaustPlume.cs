using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class ExhaustPlume : MonoBehaviour
{
    private SpriteRenderer _sr;
    private MaterialPropertyBlock _matBlock;
    private static readonly int _masterThrustID = Shader.PropertyToID("_MasterThrust");

    private float _lastPowerValue = -1; //для инициализации

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _sr = GetComponent<SpriteRenderer>();
        _matBlock = new MaterialPropertyBlock();
        _sr.maskInteraction = SpriteMaskInteraction.None;
        SetThrustValue(0);
    }
    public void SetThrustValue(float value)
    {
        if(_lastPowerValue == value) return;

        _lastPowerValue = value;
        //_sr.GetPropertyBlock(_matBlock);
        _matBlock.SetFloat(_masterThrustID, value);
        _sr.SetPropertyBlock(_matBlock);
    }

//#if UNITY_EDITOR
//    [SerializeField] float _masterThrust;
//    private void OnValidate()
//    {
//        if (_sr == null)
//            _sr = GetComponent<SpriteRenderer>();
//        if (_matBlock == null)
//            _matBlock = new MaterialPropertyBlock();

//        _masterThrust = Mathf.Clamp01(_masterThrust);
//        SetThrustValue(_masterThrust);
//    }
//#endif
}
