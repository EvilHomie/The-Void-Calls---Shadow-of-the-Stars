using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GamePools
{
    public abstract class AbstractPool<T> : MonoBehaviour where T : PoolObjectBase
    {
        private readonly Dictionary<uint, ObjectPool<T>> _pools = new();
        private readonly Dictionary<uint, T> _prefabs = new();
        private readonly Dictionary<uint, Transform> _poolsParents = new();

        private void Awake()
        {
            AwakeInit();
        }
        protected abstract void AwakeInit();

        public T Getitem(uint poolId)
        {
            return FindPool(poolId).Get();
        }

        public void ReleaseItem(T item)
        {
            FindPool(item.PoolId).Release(item);
        }

        protected void CreateItemPool(PoolData poolData, int startCapacity, int maxCapacity, Transform parent = null, int prewarmCount = 1)
        {
            var poolId = poolData.PoolReference.Id;
            T cast = poolData.Prefab.GetComponent<T>();
            _prefabs.Add(poolId, cast);

            var newPool = new ObjectPool<T>(

                   createFunc: () => OnCreate(poolId, parent),
                   actionOnGet: OnGet,
                   actionOnRelease: OnRelease,
                   actionOnDestroy: OnDestroyItem,
                   defaultCapacity: startCapacity,
                   maxSize: maxCapacity
               );

            _poolsParents.Add(poolId, parent);
            _pools.Add(poolId, newPool);

            PrewarmPool(newPool, prewarmCount);
        }

        private T OnCreate(uint poolId, Transform parent)
        {
            var prefab = _prefabs[poolId];
            var instance = Instantiate(prefab, parent);
            instance.Init(poolId);
            instance.Transform.SetParent(_poolsParents[instance.PoolId]);
            return instance;
        }

        private void OnGet(T item)
        {
            //item.CachedTransform.SetParent(null);
            item.GameObject.SetActive(true);
        }
        private void OnRelease(T item)
        {
            //item.CachedTransform.SetParent(_poolsParents[item.PoolReference]);
            item.GameObject.SetActive(false);
        }

        private void OnDestroyItem(T item)
        {
            Destroy(item.GameObject);
        }

        private void PrewarmPool(ObjectPool<T> pool, int count)
        {
            var items = new T[count];

            for (int i = 0; i < count; i++)
            {
                var instance = pool.Get();
                items[i] = instance;

            }

            foreach (var item in items)
            {
                pool.Release(item);
            }
        }

        private ObjectPool<T> FindPool(uint poolId)
        {
            if (!_pools.TryGetValue(poolId, out var pool))
            {
                throw new Exception($"Не найден пул с Id {poolId}");
            }

            return pool;
        }
    }
}
