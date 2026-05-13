using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MainEnginePlume : MonoBehaviour
{
    [SerializeField] ParticleSystem forwardParticles;
    [SerializeField] ParticleSystem reverseParticles;
    private SpriteRenderer _spriteRenderer;
    private MaterialPropertyBlock _matBlock;
    private static readonly int _powerValueID = Shader.PropertyToID("_PowerValue");

    private float _lastPowerValue = -100;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        //_spriteRenderer.sortingOrder = SpriteSortingOrders.Engine;
        _matBlock = new MaterialPropertyBlock();
        SetPowerValue(0);
    }
    public void SetPowerValue(float value)
    {
        if (_lastPowerValue == value)
        {
            return;
        }

        _lastPowerValue = value;
        _matBlock.SetFloat(_powerValueID, value);
        _spriteRenderer.SetPropertyBlock(_matBlock);

        if (value == 0)
        {
            forwardParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            reverseParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            return;
        }

        if (value > 0 && !forwardParticles.isPlaying)
        {
            forwardParticles.Play();
            reverseParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else if (!reverseParticles.isPlaying)
        {
            reverseParticles.Play();
            forwardParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
