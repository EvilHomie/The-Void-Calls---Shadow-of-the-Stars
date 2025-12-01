using UnityEngine;

namespace Weapon
{
    public class HitParticle : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particleSystem;

        void OnParticleSystemStopped()
        {
            Debug.Log("System has stopped!");
        }
    }
}