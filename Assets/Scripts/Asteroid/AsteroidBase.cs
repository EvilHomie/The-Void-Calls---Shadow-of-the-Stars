using UnityEngine;

namespace Asteroid
{
    public abstract class AsteroidBase : MonoBehaviour, IDamageable
    {
        [field: SerializeField] public DamageProfile DefaultDamageProfile { get; set; }
        [field: SerializeField] public Rigidbody2D RB { get; set; }

        public DamageProfile CurrentDamageProfile;
        public AsteroidBehaviourType AsteroidBehaviourType { get; set; }        
        public ref DamageProfile GetCurrentDamageProfile() => ref CurrentDamageProfile;
    }
}