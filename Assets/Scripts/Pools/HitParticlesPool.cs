using GamePools;
using GameSystems;
using UnityEngine;
using Weapons;

public class HitParticlesPool : AbstractPool<HitParticle>
{
    [SerializeField] PoolDataSO[] _poolsData;
    [SerializeField] int _startCapacity;
    [SerializeField] int _maxCapacity;
    [SerializeField] int _prewarmAmount;
    protected override void AwakeInit()
    {
        foreach (var data in _poolsData)
        {
            var container = new GameObject($"{data.PoolName}").transform;
            container.SetParent(transform);
            var hitParticle = data.Prefab.GetComponent<HitParticle>();
            CreateItemPool(hitParticle, data.PoolName, _startCapacity, _maxCapacity, container, _prewarmAmount);
        }
    }

    protected override void Subscribe()
    {
        EventBus.GetHitParticle += OnGetProjectile;
        EventBus.ReturnHitParticle += ReleaseItem;
    }

    protected override void Unsubscribe()
    {
        EventBus.GetHitParticle -= OnGetProjectile;
        EventBus.ReturnHitParticle -= ReleaseItem;
    }

    private HitParticle OnGetProjectile(string name)
    {
        return Getitem(name);
    }
}