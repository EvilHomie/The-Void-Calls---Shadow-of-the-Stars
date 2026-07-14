using GamePools;
using UnityEngine;

namespace DefenseLayers
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class DefenseLayerBase : MonoBehaviour
    {
        public float CurrentBasePoints;
        public float MaxBasePoints;
        
        public PolygonCollider2D Collider { get; private set; }
        public Transform Transform { get; private set; }
        public GameObject GameObject { get; private set; }

        protected void SetupBase(float points)
        {
            CurrentBasePoints = points;
            MaxBasePoints = points;
        }

        public virtual void CacheDependencies()
        {
            Collider = GetComponent<PolygonCollider2D>();
            Transform = transform;
            GameObject = gameObject;
        }
    }
}