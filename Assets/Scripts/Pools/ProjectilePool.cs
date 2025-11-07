using GamePool;
using GameSystem;
using Projectile;
using UnityEngine;

public class ProjectilePool : AbstractPool<ProjectileBase> 
{
    [SerializeField] ProjectileBase[] _projectilePrefabs;
    [SerializeField] int _startCapacity;
    [SerializeField] int _maxCapacity;
    [SerializeField] int _prewarmAmount;
    private Transform _enemiesContainer;
    protected override void AwakeInit()
    {
        _enemiesContainer = new GameObject("Pool_Container_Projectile").transform;
        _enemiesContainer.SetParent(transform);
        CreateItemPools(_projectilePrefabs, _startCapacity, _maxCapacity, _enemiesContainer, _prewarmAmount);
    }

    protected override void Subscribe()
    {
        EventBus.GetProjectile += OnGetProjectile;
        EventBus.ReturnProjectile += Release;
    }

   

    protected override void Unsubscribe()
    {
        EventBus.GetProjectile -= OnGetProjectile;
        EventBus.ReturnProjectile -= Release;
    }

    private ProjectileBase OnGetProjectile(string name)
    {
        var pr = Getitem(name);
        EventBus.ProjectileFetched(pr);
        return pr;
    }
}