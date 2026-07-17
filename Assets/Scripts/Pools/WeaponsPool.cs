using UnityEngine;
using Weapons;

namespace GamePools
{
    public class WeaponsPool : AbstractPool<WeaponBase>
    {
        [SerializeField] PoolReference[] _poolsReference;
        [SerializeField] int _startCapacity;
        [SerializeField] int _maxCapacity;
        [SerializeField] int _prewarmAmount;
        protected override void AwakeInit()
        {
            foreach (var reference in _poolsReference)
            {
                var container = new GameObject($"Pool_{reference.Prefab.name}").transform;
                container.SetParent(transform);
                CreateItemPool(reference, _startCapacity, _maxCapacity, container, _prewarmAmount);
            }
        }
    }
}