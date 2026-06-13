using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BoosterPlume : PlumeBase
{
    [field: SerializeField] public ParticleSystem BoosterParticles {  get; private set; }
}
