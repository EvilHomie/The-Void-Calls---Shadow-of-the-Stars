using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GamePools
{
    public abstract class AbstractPool<T> : MonoBehaviour where T : PoolObjectBase
    {
        private readonly Dictionary<PoolReference, ObjectPool<T>> _pools = new();
        private readonly Dictionary<PoolReference, T> _prefabs = new();
        private readonly Dictionary<PoolReference, Transform> _poolsParents = new();

        private void Awake()
        {
            AwakeInit();
        }
        protected abstract void AwakeInit();

        public T Getitem(PoolReference poolDefinition)
        {
            return FindPool(poolDefinition).Get();
        }

        public void ReleaseItem(T item)
        {
            FindPool(item.PoolReference).Release(item);
        }

        protected void CreateItemPool(PoolData poolData, int startCapacity, int maxCapacity, Transform parent = null, int prewarmCount = 1)
        {
            var poolReference = poolData.PoolReference;
            T cast = poolData.Prefab.GetComponent<T>();
            _prefabs.Add(poolReference, cast);

            var newPool = new ObjectPool<T>(

                   createFunc: () => OnCreate(poolReference, parent),
                   actionOnGet: OnGet,
                   actionOnRelease: OnRelease,
                   actionOnDestroy: OnDestroyItem,
                   defaultCapacity: startCapacity,
                   maxSize: maxCapacity
               );

            _poolsParents.Add(poolReference, parent);
            _pools.Add(poolReference, newPool);

            PrewarmPool(newPool, prewarmCount);
        }

        private T OnCreate(PoolReference poolReference, Transform parent)
        {
            var prefab = _prefabs[poolReference];
            var instance = Instantiate(prefab, parent);
            instance.Init(poolReference);
            instance.Transform.SetParent(_poolsParents[instance.PoolReference]);
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

        private ObjectPool<T> FindPool(PoolReference poolReference)
        {
            if (!_pools.TryGetValue(poolReference, out var pool))
            {
                throw new Exception($"Не найден пул с {poolReference.name}");
            }

            return pool;
        }
    }
}
