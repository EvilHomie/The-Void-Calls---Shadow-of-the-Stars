using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MainEnginePlume : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    private SpriteRenderer _sr;
    private MaterialPropertyBlock _matBlock;
    private static readonly int _powerValueID = Shader.PropertyToID("_PowerValue");

    private float _lastPowerValue = 0;

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
        if (_lastPowerValue == value)
        {
            return;
        }

        if (value != 0 && !particles.isPlaying)
        {
            particles.Play();
        }
        else if(particles.isPlaying) 
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        _lastPowerValue = value;
        //_sr.GetPropertyBlock(_matBlock);
        _matBlock.SetFloat(_powerValueID, value);
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
