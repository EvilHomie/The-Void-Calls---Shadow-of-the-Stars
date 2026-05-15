using UnityEngine;

namespace GamePools
{
    public abstract class PoolObjectBase : MonoBehaviour
    {
        public uint PoolId { get; private set; }
        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }

        public virtual void Init(uint poolId)
        {
            PoolId = poolId;
            GameObject = gameObject;
            Transform = transform;
        }
    }
}