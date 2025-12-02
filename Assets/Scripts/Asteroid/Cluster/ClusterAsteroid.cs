using UnityEngine;

namespace Asteroids
{
    public class ClusterAsteroid : MonoBehaviour
    {
        public AsteroidType AsteroidType;
        [field: SerializeField] public Rigidbody2D RB { get; set; }
        [field: SerializeField] public HealthData HealthData { get; set; }

        private void OnBecameInvisible()
        {
            RB.simulated = false;
        }

        private void OnBecameVisible()
        {
            RB.simulated = true;
        }
    }
}