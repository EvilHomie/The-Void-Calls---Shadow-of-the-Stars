using System;

namespace Ships
{
    [Serializable]
    public struct HealthData
    {
        public float HullPoints;
        public float ShieldPoints;
        public float ArmorPoints;
        public float EnergyDamageResistance;
        public float KineticDamageResistance;
    }
}