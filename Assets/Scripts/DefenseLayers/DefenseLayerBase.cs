using System;
using UnityEngine;

namespace DefenseLayers
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class DefenseLayerBase : MonoBehaviour
    {
        public abstract DefenseLayerType LayerType { get; }
        public Collider2D Collider { get; private set; }
        public Transform Transform { get; private set; }

        public float CurrentHealthPoints;
        public float MaxHealthPoints;
        public ResistanceMultipliers ResistanceMultipliers;

        public virtual void Init()
        {
            Collider = GetComponent<Collider2D>();
            Transform = transform;
        }
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

