using UnityEngine;

namespace DefenseLayers
{
    public class ShieldDefenseLayer : DefenseLayerBase
    {
        [field: SerializeField] public Transform Transform { get; private set; }

        public float CurrentPoints;
        public float MaxPoints;
        public ShieldTransformData TransformData;
    }
     
    public struct ShieldTransformData
    {
        float LossyScale;
        Vector2 Position;
        Quaternion Rotation;
    }
}