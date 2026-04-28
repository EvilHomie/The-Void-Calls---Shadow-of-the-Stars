using UnityEngine;

namespace GamePools
{
    public abstract class PoolObjectBase : MonoBehaviour
    {
        public PoolReference PoolReference { get; private set; }
        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }

        public void Init(PoolReference poolReference)
        {
            PoolReference = poolReference;
            GameObject = gameObject;
            Transform = transform;
        }
    }
}