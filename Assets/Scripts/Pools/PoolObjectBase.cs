using UnityEngine;

namespace GamePools
{
    public abstract class PoolObjectBase : MonoBehaviour, IPoolable
    {
        public string PoolName { get; set; }
        public GameObject CachedGameObject { get; private set; }
        public Transform CachedTransform { get; private set; }
        public bool InPool { get; set; }

        public virtual void Init()
        {
            CachedGameObject = gameObject;
            CachedTransform = transform;
        }
    }
}