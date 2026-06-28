using System;
using UnityEngine;

namespace DefenseLayers
{
    public class ShieldDefenseLayer : DefenseLayerBase
    {
        public Transform Transform { get; private set; }
        public Transform ParentTransform { get; private set; }
        public ShieldTransformRuntimeData ShieldTransformRuntimeData;
        
        public float RootSize;

        public float RegRate;

        public void Init(float basePoints, float regRate)
        {
            base.InitBase(basePoints);
            var transform = this.transform;
            Transform = transform;
            RootSize = transform.root.localScale.x;
            ParentTransform = transform.parent;
            RegRate = regRate;
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