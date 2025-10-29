using UnityEngine;

namespace Enviroment
{
    public abstract class PoolObjectBase : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public string ObjectName { get; private set; }
        public GameObject CachedGameObject { get; private set; }
        public Transform CachedTransform { get; private set; }

        public void Init()
        {
            CachedGameObject = gameObject;
            CachedTransform = transform;
        }
    }
}

