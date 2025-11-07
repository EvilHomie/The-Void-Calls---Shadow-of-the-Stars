using UnityEngine;

namespace Enviroment
{
    public abstract class PoolObjectBase : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolDataSO PoolData { get; private set; }
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

