using DefenseLayers;
using GamePools;
using UnityEngine;

namespace Asteroids
{
    public class Asteroid : PoolObjectBase
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public AsteroidType AsteroidType { get; private set; }
        [field: SerializeField] public AsteroidDefenseLayer AsteroidHullLayer { get; private set; }
        [field: SerializeField] public SpriteRenderer BodySprite { get; private set; }

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