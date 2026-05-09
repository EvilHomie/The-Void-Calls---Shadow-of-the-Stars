//using System;
//using UnityEngine;

//namespace Damage
//{
//    public class HealthComponent : MonoBehaviour
//    {
//        [field: SerializeField] public ObjectType ObjectType { get; private set; }
//        public HealthStatsData Health;
//        public ResistanceStatsData Resistance;
//        public ResistanceStatsData ResistanceMultipliers;

//        public Action ShieldDestroyedAction;
//        public Action ArmorDestroyedAction;
//        public Action HullDestroyedAction;
//    }

//    [Serializable]
//    public struct HealthStatsData
//    {
//        public float HullPoints;
//        public float ShieldPoints;
//        public float ArmorPoints;
//    }

//    [Serializable]
//    public struct ResistanceStatsData
//    {
//        public float Energy;
//        public float Kinetic;
//    }

//    public enum ObjectType
//    {
//        Ship,
//        Asteroid,
//        Station
//    }

//    // у астероидов только hull
//    // у кораблей hull, armor, shield
//    // у станций только hull, shield
//}