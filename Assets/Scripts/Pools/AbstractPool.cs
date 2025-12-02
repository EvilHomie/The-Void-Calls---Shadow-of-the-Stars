using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GamePools
{
    public abstract class AbstractPool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
    {
        private readonly Dictionary<string, ObjectPool<T>> _poolByName = new();
        private readonly Dictionary<string, T> _prefabByName = new();
        private readonly Dictionary<string, Transform> _parentByName = new();

        private void Awake()
        {
            AwakeInit();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        protected abstract void Subscribe();
        protected abstract void Unsubscribe();
        protected abstract void AwakeInit();

        public T Getitem(string itemName)
        {
            return FindPool(itemName).Get();
        }

        public void ReleaseItem(T item)
        {
            FindPool(item.PoolName).Release(item);
        }

        //protected void CreateItemPools(T[] prefab, int startCapacity, int maxCapacity, Transform parent = null, int prewarmCount = 1)
        //{
        //    foreach (var prefabItem in prefab)
        //    {
        //        CreateItemPool(prefabItem, startCapacity, maxCapacity, parent, prewarmCount);
        //    }
        //}

        protected void CreateItemPool(T prefab, string poolName, int startCapacity, int maxCapacity, Transform parent = null, int prewarmCount = 1)
        {
            var item = Instantiate(prefab, parent);
            item.gameObject.SetActive(false);  
            _prefabByName.Add(poolName, item);

            var newPool = new ObjectPool<T>(

                   createFunc: () => OnCreate(poolName, parent),
                   actionOnGet: OnGet,
                   actionOnRelease: OnRelease,
                   actionOnDestroy: OnDestroyItem,
                   defaultCapacity: startCapacity,
                   maxSize: maxCapacity
               );

            _parentByName.Add(poolName, parent);
            _poolByName.Add(poolName, newPool);

            PrewarmPool(newPool, prewarmCount);
        }

        private T OnCreate(string poolName, Transform parent)
        {
            var prefab = _prefabByName[poolName];
            var instance = Instantiate(prefab, parent);
            instance.PoolName = poolName;
            instance.Init();
            return instance;
        }

        private void OnGet(T item)
        {
            item.InPool = false;
            item.CachedTransform.SetParent(null);
            item.CachedGameObject.SetActive(true);
        }
        private void OnRelease(T item)
        {
            item.InPool = true;
            item.CachedTransform.SetParent(_parentByName[item.PoolName]);
            item.CachedGameObject.SetActive(false);
        }

        private void OnDestroyItem(T item)
        {
            Destroy(item.CachedGameObject);
        }

        private void PrewarmPool(ObjectPool<T> pool, int count)
        {
            List<T> items = new();

            for (int i = 0; i < count; i++)
            {
                var instance = pool.Get();
                items.Add(instance);
            }

            foreach (T item in items)
            {
                pool.Release(item);
            }
        }

        private ObjectPool<T> FindPool(string name)
        {
            if (!_poolByName.TryGetValue(name, out var pool))
            {
                throw new Exception($"Не найден пул с {name}");
            }

            return pool;
        }
    }
}
