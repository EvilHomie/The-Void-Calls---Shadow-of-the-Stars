using GamePool;
using GameSystem;
using Projectile;
using UnityEngine;

public class HitParticlesPool : AbstractPool<ProjectileBase> 
{
    [SerializeField] PoolDataSO[] _poolsData;
    [SerializeField] int _startCapacity;
    [SerializeField] int _maxCapacity;
    [SerializeField] int _prewarmAmount;
    private Transform _enemiesContainer;
    protected override void AwakeInit()
    {
        //_enemiesContainer = new GameObject("Pool_Container_Projectiles").transform;
        //_enemiesContainer.SetParent(transform);

        //foreach (var data in _poolsData)
        //{
        //    CreateItemPools(data, _startCapacity, _maxCapacity, _enemiesContainer, _prewarmAmount);
        //}
    }

    protected override void Subscribe()
    {
        //EventBus.GetProjectile += OnGetProjectile;
        //EventBus.ReturnProjectile += Release;
    }

   

    protected override void Unsubscribe()
    {
        //EventBus.GetProjectile -= OnGetProjectile;
        //EventBus.ReturnProjectile -= Release;
    }

    //private ProjectileBase OnGetProjectile(string name)
    //{
    //    //var pr = Getitem(name);
    //    //EventBus.ProjectileFetched(pr);
    //    //return pr;
    //}
}