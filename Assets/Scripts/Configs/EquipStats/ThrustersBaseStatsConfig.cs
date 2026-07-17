using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "ThrustersBaseStatsConfig ", menuName = "Scriptable Objects/EquipStats/ThrustersBaseStatsConfig ")]
    public class ThrustersBaseStatsConfig : ScriptableObject
    {
        [field: SerializeField] public ThrusterBaseStats[] BaseStats { get; private set; }

        private Dictionary<(ThrusterId, SizeType), ThrusterStatsBySize> _statsByThruster;

        public void FillCollection()
        {
            _statsByThruster ??= new();

            for (int i = 0; i < BaseStats.Length; i++)
            {
                for (int j = 0; j < BaseStats[i].StatsBySize.Length; j++)
                {
                    var stats = BaseStats[i].StatsBySize[j];
                    _statsByThruster.Add((BaseStats[i].Id, stats.Size), stats);
                }
            }
        }

        public ThrusterStatsBySize GetStats(ThrusterId id, SizeType sizeType)
        {
            return _statsByThruster[(id, sizeType)];
        }

#if UNITY_EDITOR
        private void OnValidate() // просто для красоты чтобы в коллекции вместо номера элемента была конкретика
        {
            for (int i = 0; i < BaseStats.Length; i++)
            {
                var name = BaseStats[i].Id.ToString();
                BaseStats[i].Name = name;

                for (int j = 0; j < BaseStats[i].StatsBySize.Length; j++)
                {
                    BaseStats[i].StatsBySize[j].Name = $"{name} {BaseStats[i].StatsBySize[j].Size} ";
                }
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }


    [Serializable]
    public struct ThrusterBaseStats
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public ThrusterId Id { get; private set; }
        [field: SerializeField] public ThrusterStatsBySize[] StatsBySize { get; private set; }
    }

    [Serializable]
    public struct ThrusterStatsBySize
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public float StrafeThrust { get; private set; }
        [field: SerializeField] public float RotateThrust { get; private set; }
    }
}

public enum ThrusterId
{
    None = 0,
    RotationMK1 = 1,
    BalancedMK1 = 2,
    StrafeMK1 = 3
}