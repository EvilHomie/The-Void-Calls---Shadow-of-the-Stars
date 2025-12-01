using System.Collections.Generic;
using UnityEngine;

namespace Asteroid
{
    public class AsteroidsCluster : MonoBehaviour
    {
        [field: SerializeField] public List<ClusterAsteroid> Asteroids { get; set; }

        private void Start()
        {
            foreach (var asteroid in Asteroids)
            {
                var rTorque = Random.Range(-20f, 20f);
                asteroid.RB.angularVelocity = rTorque;
            }
        }
    }
}