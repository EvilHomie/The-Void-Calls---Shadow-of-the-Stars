//using System;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace Configs
//{
//    [CreateAssetMenu(fileName = "ShieldsBaseStatsConfig ", menuName = "Scriptable Objects/ShieldsBaseStatsConfig ")]
//    public class ShieldsBaseStatsConfig : ScriptableObject
//    {
//        [field: SerializeField] public MainEngineBaseStats[] BaseStats { get; private set; }

//        private Dictionary<(MainEngineId, SizeType), MainEngineStatsBySize> _statsByEngine;

//        public void FillCollection()
//        {
//            _statsByEngine ??= new();

//            for (int i = 0; i < BaseStats.Length; i++)
//            {
//                for (int j = 0; j < BaseStats[i].StatsBySize.Length; j++)
//                {
//                    var stats = BaseStats[i].StatsBySize[j];
//                    _statsByEngine.Add((BaseStats[i].Id, stats.Size), stats);
//                }
//            }
//        }

//        public MainEngineStatsBySize GetStats(MainEngineId id, SizeType sizeType)
//        {
//            return _statsByEngine[(id, sizeType)];
//        }

//#if UNITY_EDITOR
//        private void OnValidate() // просто для красоты чтобы в коллекции вместо номера элемента была конкретика
//        {
//            for (int i = 0; i < BaseStats.Length; i++)
//            {
//                var name = BaseStats[i].Id.ToString();
//                BaseStats[i].Name = name;

//                for (int j = 0; j < BaseStats[i].StatsBySize.Length; j++)
//                {
//                    BaseStats[i].StatsBySize[j].Name = $"{name} {BaseStats[i].StatsBySize[j].Size} ";
//                }
//            }

//            EditorUtility.SetDirty(this);
//        }
//#endif
//    }


//    [Serializable]
//    public struct MainEngineBaseStats
//    {
//        [HideInInspector] public string Name;
//        [field: SerializeField] public MainEngineId Id { get; private set; }
//        [field: SerializeField] public MainEngineStatsBySize[] StatsBySize { get; private set; }
//    }

//    [Serializable]
//    public struct MainEngineStatsBySize
//    {
//        [HideInInspector] public string Name;
//        [field: SerializeField] public SizeType Size { get; private set; }
//        [field: SerializeField] public float DirectThrust { get; private set; }
//        [field: SerializeField] public float ReverseThrust { get; private set; }
//        [field: SerializeField] public float BoostThrust { get; private set; }
//        [field: SerializeField] public float BoostMaxTime { get; private set; }
//        [field: SerializeField] public float BoostRechargeSpeed { get; private set; }
//    }
//}

//public enum ShieldId
//{
//    None = 0,
//    AroundMK1 = 1,
//    FrontMK1 = 2,
//}