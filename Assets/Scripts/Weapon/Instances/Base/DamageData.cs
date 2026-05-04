using System;

namespace Weapons
{
    [Serializable]
    public struct DamageData
    {
        public float ShieldDamage;
        public float ArmorDamage;
        public float HullDamage;

        public float ShieldMultiplier;
        public float ArmorMultiplier;
        public float HullMultiplier;

        public float AsteroidMultipler;
    }
}