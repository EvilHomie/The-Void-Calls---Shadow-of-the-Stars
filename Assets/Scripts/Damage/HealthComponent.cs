using System;
using UnityEngine;

namespace Damage
{
    public class HealthComponent : MonoBehaviour
    {
        public HealthData HealthData;
    }

    [Serializable]
    public struct HealthData
    {
        [field: SerializeField] public HealthType HealthType { get; private set; }

        public float HullPoints;
        public float ShieldPoints;
        public float ArmorPoints;
        public float EnergyDamageResistance;
        public float KineticDamageResistance;
    }

    public enum HealthType
    {
        Ship,
        Asteroid,
        Station
    }

    // у астероидов только hull
    // у кораблей hull, armor, shield
    // у станций только hull, shield
}