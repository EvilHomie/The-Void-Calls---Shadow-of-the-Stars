using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MainEnginePlume : PlumeBase
{
    [field: SerializeField] public ParticleSystem ForwardParticles { get; private set; }
    [field: SerializeField] public ParticleSystem ReverseParticles { get; private set; }
}
