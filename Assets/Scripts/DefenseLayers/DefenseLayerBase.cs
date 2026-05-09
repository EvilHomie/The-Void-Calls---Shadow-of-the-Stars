using System;
using UnityEngine;

namespace DefenseLayers
{
    public abstract class DefenseLayerBase : MonoBehaviour
    {
        public abstract DefenseLayerType LayerType { get; }
        [field: SerializeField] public Collider2D Collider { get; private set; }

        public float CurrentHealthPoints;
        public float MaxHealthPoints;
        public ResistanceMultipliers ResistanceMultipliers;
    }

    public enum DefenseLayerType
    {
        Shield,
        Armor,
        Hull,
        AsteroidHull
    }

    [Serializable]
    public struct ResistanceMultipliers
    {
        public float Energy;
        public float Kinetic;
    }
}

