using UnityEngine;

namespace GamePools
{
    public abstract class PoolObjectBase : MonoBehaviour
    {
        public PoolReference PoolReference { get; private set; }
        public GameObject CachedGameObject { get; private set; }
        public Transform CachedTransform { get; private set; }
        public bool InPool { get; private set; }

        public void Init(PoolReference poolReference)
        {
            PoolReference = poolReference;
            CachedGameObject = gameObject;
            CachedTransform = transform;
        }

        public void SetPoolState(bool inPool)
        {
            InPool = inPool;
        }
    }
}