using GamePools;
using GameSystems;
using Projectiles;
using UnityEngine;

public class ProjectilesPool : AbstractPool<ProjectileBase>
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
            ProjectileBase projectile = data.Prefab.GetComponent<ProjectileBase>();
            CreateItemPool(projectile, data.PoolName, _startCapacity, _maxCapacity, container, _prewarmAmount);
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

    private ProjectileBase GetProjectile(string name)
    {
        var pr = Getitem(name);
        EventBus.ProjectileFetched(pr);
        return pr;
    }
}