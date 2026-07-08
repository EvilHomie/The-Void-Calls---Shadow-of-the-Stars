using Ships;
using System;
using UnityEngine;

namespace DefenseLayers
{
    public class ShieldDefenseLayer : DefenseLayerBase
    {
        public SizeType Size { get; private set; }
        public Transform Transform { get; private set; }
        public Transform RadiusTransform { get; private set; }
        public ShieldTransformRuntimeData ShieldTransformRuntimeData;

        public float RegRate;

        public void Init(float basePoints, float regRate, SizeType size)
        {
            base.InitBase(basePoints);
            var transform = this.transform;
            Transform = transform;
            RadiusTransform = transform.parent;
            RegRate = regRate;
            Size = size;

            var deffLossyScale = transform.localScale * GameConfig.SizeMap[size];

            ShieldTransformRuntimeData = new ShieldTransformRuntimeData()
            {
                Radius = 1,
                DeffaultLossyScale = deffLossyScale,
                CurrentLossyScale = deffLossyScale,
                WorldPosition = transform.position,
                WorldRotation = transform.rotation
            };
        }
    }

    [Serializable]
    public struct ShieldTransformRuntimeData
    {
        public float Radius;
        public Vector2 CurrentLossyScale;
        public Vector2 DeffaultLossyScale;
        public Vector2 WorldPosition;
        public Quaternion WorldRotation;
    }
}