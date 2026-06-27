using UnityEngine;

namespace DefenseLayers
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class DefenseLayerBase : MonoBehaviour
    {
        public float CurrentBasePoints;
        public float MaxBasePoints;
        public Collider2D Collider { get; private set; }

        protected void InitBase(float points)
        {
            Collider = GetComponent<Collider2D>();
            CurrentBasePoints = points;
            MaxBasePoints = points;
        }
    }
}