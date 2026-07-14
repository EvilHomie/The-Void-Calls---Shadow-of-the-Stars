using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "ChassisBaseStatsConfig", menuName = "Scriptable Objects/ChassisBaseStatsConfig")]
    public class ChassisBaseStatsConfig : ScriptableObject
    {
        [field: SerializeField] public ChassisBaseStats[] ChassisBaseStats { get; private set; }

        private Dictionary<ShipId, ChassisBaseStats> _statsById;

        public void FillCollection()
        {
            _statsById ??= new();

            for (int i = 0; i < ChassisBaseStats.Length; i++)
            {
                var stats = ChassisBaseStats[i];
                _statsById.Add(stats.Id, stats);
            }
        }

        public SizeType GetShipSizeType(ShipId shipId)
        {
            int id = (int)shipId;

            if (id < 100)
                return SizeType.S;

            if (id < 200)
                return SizeType.M;

            if (id < 300)
                return SizeType.L;

            return SizeType.XL;
        }

        public ChassisBaseStats GetStats(ShipId shipId)
        {
            return _statsById[shipId];
        }

#if UNITY_EDITOR
        private void OnValidate() // просто для красоты чтобы в коллекции вместо номера элемента была конкретика
        {
            for (int i = 0; i < ChassisBaseStats.Length; i++)
            {
                var shipId = ChassisBaseStats[i].Id;
                int id = (int)shipId;

                if (id == 0)
                {
                    Debug.LogWarning("one of the chassis without an ID");
                    return;
                }

                var name = $" {shipId} {GetShipSizeType(shipId)}";
                ChassisBaseStats[i].Name = name;
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }

    [Serializable]
    public struct ChassisBaseStats
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public ShipId Id { get; private set; }
        [field: SerializeField, Min(1)] public int HullPoints { get; private set; }
        [field: SerializeField, Min(0.001f)] public float Mass { get; private set; }
        [field: SerializeField, Min(0.001f)] public float DirectDrag { get; private set; }
        [field: SerializeField, Min(0.001f)] public float ReverseDrag { get; private set; }
        [field: SerializeField, Min(0.001f)] public float StrafeDrag { get; private set; }
        [field: SerializeField, Min(0.001f)] public float RotateDrag { get; private set; }
        [field: SerializeField, Min(1)] public int CargoSize { get; private set; }
    }
}
public enum ShipId
{
    None = 0,

    // 1-99 S
    Courier = 1,
    Pulsar = 2,

    // 100-199 M
    Nemesis = 100,
    Manorina = 101,

    // 200-299 L
    Odysseus = 200
}