using DefenseLayers;
using GamePools;
using UnityEngine;

namespace Asteroids
{
    public class Asteroid : PoolObjectBase
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public AsteroidType AsteroidType { get; private set; }
        [field: SerializeField] public AsteroidHullLayer AsteroidHullLayer { get; private set; }
        public uint Id;
        private void OnBecameInvisible()
        {
            Rigidbody.simulated = false;
        }

        private void OnBecameVisible()
        {
            Rigidbody.simulated = true;
        }
    }
}