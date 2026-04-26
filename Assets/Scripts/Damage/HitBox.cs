using UnityEngine;

namespace Damage
{
    public class HitBox : MonoBehaviour
    {
        [field: SerializeField] public HealthComponent HealthComponent { get; private set; }
    }
}