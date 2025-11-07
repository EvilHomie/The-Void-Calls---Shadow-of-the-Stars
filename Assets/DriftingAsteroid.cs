using UnityEngine;

namespace Asteroid
{
    public class DriftingAsteroid : AsteroidBase, IPoolable
    {
        [field: SerializeField] public PoolDataSO PoolData { get; private set; }
        public GameObject CachedGameObject { get; set; }
        public Transform CachedTransform { get; set; }
        public bool InPool { get; set; }
        public void Init() => AsteroidHelper.Init(this);
        public void ResetParams() => AsteroidHelper.ResetParams(this);
    }
}