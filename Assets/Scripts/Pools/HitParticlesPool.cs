using GameSystems;
using HitParticles;
using UnityEngine;

namespace GamePools
{
    public class HitParticlesPool : AbstractPool<HitParticle>
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
            EventBus.GetHitParticle += OnGetProjectile;
        }

        protected override void Unsubscribe()
        {
            EventBus.GetHitParticle -= OnGetProjectile;
        }

        private HitParticle OnGetProjectile(PoolReference poolReference)
        {
            var hitParticles = Getitem(poolReference);
            EventBus.HitParticleFetched?.Invoke(hitParticles);
            return hitParticles;
        }
    }
}