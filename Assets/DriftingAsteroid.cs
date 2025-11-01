using UnityEngine;

namespace Asteroid
{
    public class DriftingAsteroid : AsteroidBase, IPoolable
    {
        [field: SerializeField] public string PoolName { get; }
        public GameObject CachedGameObject { get; set; }
        public Transform CachedTransform { get; set; }
        public void Init() => AsteroidHelper.Init(this);
        public void ResetParams() => AsteroidHelper.ResetParams(this);
    }
}