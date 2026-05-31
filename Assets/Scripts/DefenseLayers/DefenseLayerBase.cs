using System;
using UnityEngine;

namespace DefenseLayers
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class DefenseLayerBase : MonoBehaviour
    {
        [field: SerializeField] public SizeType Size { get; private set; }
        public abstract DefenseLayerType LayerType { get; }
        public Collider2D Collider { get; private set; }
        public Transform Transform { get; private set; }

        public float CurrentHealthPoints;
        public float MaxHealthPoints { get; private set; }

        public ResistanceMultipliers ResistanceMultipliers;

        public virtual void Init(float HP)
        {
            Collider = GetComponent<Collider2D>();
            Transform = transform;
            CurrentHealthPoints = HP;
            MaxHealthPoints = HP;
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

