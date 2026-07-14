using System;
using UnityEngine;

namespace DefenseLayers
{
    public class ShieldDefenseLayer : DefenseLayerBase
    {
        public Transform Transform { get; private set; }
        public ShieldTransformRuntimeData ShieldTransformRuntimeData;

        public float RegRate;

        public void Init(float basePoints, float regRate, float lossySize)
        {
            base.InitBase(basePoints);
            var transform = this.transform;
            Transform = transform;
            RegRate = regRate;

            var localScale = transform.localScale;

            ShieldTransformRuntimeData = new ShieldTransformRuntimeData()
            {
                RadiusMod = 1,
                lossySize = lossySize,
                CurrentLocalScale = localScale,
                DeffaultLocalScale = localScale,
                WorldPosition = transform.position,
                WorldRotation = transform.rotation
            };
        }
    }

    [Serializable]
    public struct ShieldTransformRuntimeData
    {
        public float RadiusMod;
        public float lossySize;
        public Vector2 CurrentLocalScale;
        public Vector2 CurrentLossyScale;
        public Vector2 DeffaultLocalScale;
        public Vector2 WorldPosition;
        public Quaternion WorldRotation;
    }
}