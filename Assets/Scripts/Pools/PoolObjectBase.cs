using UnityEngine;

namespace Enviroment
{
    public abstract class PoolObjectBase : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public string PoolName { get; private set; }
        public GameObject CachedGameObject { get; private set; }
        public Transform CachedTransform { get; private set; }

        public virtual void Init()
        {
            CachedGameObject = gameObject;
            CachedTransform = transform;
        }

        public abstract void ResetParams();
    }
}

