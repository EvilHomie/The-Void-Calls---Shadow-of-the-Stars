using System;
using UnityEngine;

namespace DefenseLayers
{
    public abstract class DefenseLayerBase : MonoBehaviour
    {
        [field: SerializeField] public SizeType Size { get; private set; }
        [field: SerializeField] public Collider2D Collider { get; private set; }
        public abstract DefenseLayerType LayerType { get; }
        public Transform Transform { get; private set; }

        public float CurrentHealthPoints;
        public float MaxHealthPoints { get; private set; }

        public virtual void Init(float HP)
        {
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
}

