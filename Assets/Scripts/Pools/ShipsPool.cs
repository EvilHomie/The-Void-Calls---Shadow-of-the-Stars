using Ships;
using System.Collections.Generic;
using UnityEngine;

namespace GamePools
{
    public class ShipsPool : AbstractPool<ShipInstance>
    {
        [SerializeField] PoolReference[] _poolsReference;
        [SerializeField] int _startCapacity;
        [SerializeField] int _maxCapacity;
        [SerializeField] int _prewarmAmount;

        private readonly Dictionary<ShipId, uint> _poolIdByShipId = new();
        protected override void AwakeInit()
        {
            foreach (var reference in _poolsReference)
            {
                var container = new GameObject($"Pool_{reference.Prefab.name}").transform;
                container.SetParent(transform);
                CreateItemPool(reference, _startCapacity, _maxCapacity, container, _prewarmAmount);

                var shipId = reference.Prefab.GetComponent<ShipInstance>().Id;
                _poolIdByShipId.Add(shipId, reference.Id);
            }
        }

        public uint GetPoolIdByShipId(ShipId shipId)
        {
            //foreach (var data in _poolIdByShipId)
            //{
            //    Debug.LogError($" {data.Key}   {data.Value}");
            //}


            return _poolIdByShipId[shipId];
        }
    }
}