using GamePool;
using GameSystem;
using Projectile;
using UnityEngine;

public class ProjectilesPool : AbstractPool<ProjectileBase>
{
    [SerializeField] PoolDataSO[] _poolsData;
    [SerializeField] int _startCapacity;
    [SerializeField] int _maxCapacity;
    [SerializeField] int _prewarmAmount;
    private Transform _enemiesContainer;
    protected override void AwakeInit()
    {
        _enemiesContainer = new GameObject("Pool_Container_Projectiles").transform;
        _enemiesContainer.SetParent(transform);

        foreach (var data in _poolsData)
        {
            ProjectileBase projectile = data.Prefab.GetComponent<ProjectileBase>();
            CreateItemPool(projectile, data.PoolName, _startCapacity, _maxCapacity, _enemiesContainer, _prewarmAmount);
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