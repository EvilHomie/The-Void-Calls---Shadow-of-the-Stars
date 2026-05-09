using DefenseLayers;
using GamePools;
using Helpers;
using UnityEngine;

namespace Asteroids
{
    public class Asteroid : PoolObjectBase
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public AsteroidType AsteroidType { get; private set; }
        [field: SerializeField] public AsteroidHullLayer AsteroidHullLayer { get; private set; }

        private void OnBecameInvisible()
        {
            Rigidbody.simulated = false;
        }

        private void OnBecameVisible()
        {
            Rigidbody.simulated = true;
        }

        // тестовая часть
        private void Start()
        {
            Init(null);
            InitHelper.InitAsteroid(this);
        }
    }
}