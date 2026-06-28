using System;
using UnityEngine;

namespace DefenseLayers
{
    public class ShieldDefenseLayer : DefenseLayerBase
    {
        public SizeType Size { get; private set; }
        public Transform Transform { get; private set; }
        public Transform ParentTransform { get; private set; }
        public ShieldTransformRuntimeData ShieldTransformRuntimeData;

        public float RegRate;

        public void Init(float basePoints, float regRate, SizeType size)
        {
            base.InitBase(basePoints);
            var transform = this.transform;
            Transform = transform;
            ParentTransform = transform.parent;
            RegRate = regRate;
            Size = size;
        }
    }

    [Serializable]
    public struct ShieldTransformRuntimeData
    {     
        public Vector2 LossyScale;
        public Vector2 Position;
        public Quaternion Rotation;
    }
}