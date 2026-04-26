using GameSystems;
using Projectiles;
using UnityEngine;

namespace GamePools
{
    public class ProjectilesPool : AbstractPool<ProjectileBase>
    {
        [SerializeField] PoolData[] _poolsData;
        [SerializeField] int _startCapacity;
        [SerializeField] int _maxCapacity;
        [SerializeField] int _prewarmAmount;
        protected override void AwakeInit()
        {
            foreach (var data in _poolsData)
            {
                var container = new GameObject($"Pool_{data.PoolReference.name}").transform;
                container.SetParent(transform);
                CreateItemPool(data, _startCapacity, _maxCapacity, container, _prewarmAmount);
            }
        }

        protected override void Subscribe()
        {
            EventBus.GetProjectile += GetProjectile;
            EventBus.ReturnProjectile += ReleaseItem;
        }

        protected override void Unsubscribe()
        {
            EventBus.GetProjectile -= GetProjectile;
            EventBus.ReturnProjectile -= ReleaseItem;
        }

        private ProjectileBase GetProjectile(PoolReference poolReference)
        {
            var pr = Getitem(poolReference);
            EventBus.ProjectileFetched(pr);
            return pr;
        }
    }
}