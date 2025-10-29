using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GamePool
{
    public abstract class AbstractPool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
    {
        public readonly HashSet<T> ActiveItems = new();
        //protected Config Config;
        //protected GameEventBus EventBus;
        //protected GameFlowSystem GameFlowSystem;
        private readonly Dictionary<string, ObjectPool<T>> _poolByName = new();
        private readonly Dictionary<string, T> _prefabByName = new();
        private readonly Dictionary<string, Transform> _parentByName = new();
        private readonly List<T> _pendingReleases = new();

        //[Inject]
        //public void Construct(Config config, GameEventBus eventBus, GameFlowSystem gameFlowSystem)    //пока закоммитил. Скорее всего буду использовать конфиги.
        //{
        //    Config = config; 
        //    EventBus = eventBus;
        //    GameFlowSystem = gameFlowSystem;
        //}

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

        public T Getitem(string itemName)
        {
            return FindPool(itemName).Get();
        }
        public void ReleasePendingItems()
        {
            foreach (var item in _pendingReleases)
            {
                FindPool(item.ObjectName).Release(item);
                ActiveItems.Remove(item);
            }

            _pendingReleases.Clear();
        }

        protected void CreateItemPools(T[] prefab, int startCapacity, int maxCapacity, Transform parent = null, int prewarmCount = 1)
        {
            foreach (var prefabItem in prefab)
            {
                CreateItemPool(prefabItem, startCapacity, maxCapacity, parent, prewarmCount);
            }
        }

        protected void CreateItemPool(T prefab, int startCapacity, int maxCapacity, Transform parent = null, int prewarmCount = 1) 
        {
            prefab.gameObject.SetActive(false);
            _prefabByName.Add(prefab.ObjectName, prefab);

            var newPool = new ObjectPool<T>(

                   createFunc: () => OnCreate(prefab.ObjectName, parent),
                   actionOnGet: OnGet,
                   actionOnRelease: OnRelease,
                   actionOnDestroy: OnDestroyItem,
                   defaultCapacity: startCapacity,
                   maxSize: maxCapacity
               );

            _parentByName.Add(prefab.ObjectName, parent);
            _poolByName.Add(prefab.ObjectName, newPool);
            PrewarmPool(newPool, prewarmCount);
        }        

        protected void ScheduleForRelease(T item)
        {
            _pendingReleases.Add(item);
            item.CachedGameObject.SetActive(false);
        }       

        protected void ReleaseAll()
        {
            foreach (var item in ActiveItems)
            {
                ScheduleForRelease(item);
            }

            ActiveItems.Clear();
            ReleasePendingItems();
        }        

        private T OnCreate(string itemName, Transform parent)
        {
            var prefab = _prefabByName[itemName];
            var instance = Instantiate(prefab, parent);            
            instance.Init();
            return instance;
        }

        private void OnGet(T item)
        {
            ActiveItems.Add(item);
        }
        private void OnRelease(T item)
        {
            ActiveItems.Remove(item);
            item.CachedTransform.SetParent(_parentByName[item.ObjectName]);
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
