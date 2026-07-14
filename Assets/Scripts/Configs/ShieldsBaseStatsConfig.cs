using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "ShieldsBaseStatsConfig ", menuName = "Scriptable Objects/ShieldsBaseStatsConfig ")]
    public class ShieldsBaseStatsConfig : ScriptableObject
    {
        [field: SerializeField] public ShieldBaseStats[] BaseStats { get; private set; }

        private Dictionary<(ShieldId, SizeType), ShieldStatsBySize> _statsByShield;

        public void FillCollection()
        {
            _statsByShield ??= new();

            for (int i = 0; i < BaseStats.Length; i++)
            {
                for (int j = 0; j < BaseStats[i].StatsBySize.Length; j++)
                {
                    var stats = BaseStats[i].StatsBySize[j];
                    _statsByShield.Add((BaseStats[i].Id, stats.Size), stats);
                }
            }
        }

        public ShieldStatsBySize GetStats(ShieldId id, SizeType sizeType)
        {
            return _statsByShield[(id, sizeType)];
        }

#if UNITY_EDITOR
        private void OnValidate() // просто для красоты чтобы в коллекции вместо номера элемента была конкретика
        {
            for (int i = 0; i < BaseStats.Length; i++)
            {
                var name = BaseStats[i].Id.ToString();
                BaseStats[i].Name = name;

                var shape = BaseStats[i].ShieldShape;

                for (int j = 0; j < BaseStats[i].StatsBySize.Length; j++)
                {
                    BaseStats[i].StatsBySize[j].Name = $"{name} {BaseStats[i].StatsBySize[j].Size} ";
                    BaseStats[i].StatsBySize[j].ShieldShape = shape;
                }
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }


    [Serializable]
    public struct ShieldBaseStats
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public ShieldId Id { get; private set; }
        [field: SerializeField] public Sprite ShieldShape { get; private set; }
        [field: SerializeField] public ShieldStatsBySize[] StatsBySize { get; private set; }
    }

    [Serializable]
    public struct ShieldStatsBySize
    {
        [HideInInspector] public string Name;
        [HideInInspector] public Sprite ShieldShape;
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public float Capacity { get; private set; }
        [field: SerializeField] public float RegRate { get; private set; }
    }
}

public enum ShieldId
{
    None = 0,
    AroundMK1 = 1,
    FrontMK1 = 2,
}