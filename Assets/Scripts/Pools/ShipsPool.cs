using Ships;
using System.Collections.Generic;
using UnityEngine;

namespace GamePools
{
    public class ShipsPool : AbstractPool<ShipInstance>
    {
        [SerializeField] PoolData[] _poolsData;
        [SerializeField] int _startCapacity;
        [SerializeField] int _maxCapacity;
        [SerializeField] int _prewarmAmount;

        private readonly Dictionary<ShipId, uint> _poolIdByShipId = new();
        protected override void AwakeInit()
        {
            foreach (var data in _poolsData)
            {
                var container = new GameObject($"Pool_{data.PoolReference.name}").transform;
                container.SetParent(transform);
                CreateItemPool(data, _startCapacity, _maxCapacity, container, _prewarmAmount);

                var shipId = data.Prefab.GetComponent<ShipInstance>().Id;
                _poolIdByShipId.Add(shipId, data.PoolReference.Id);
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