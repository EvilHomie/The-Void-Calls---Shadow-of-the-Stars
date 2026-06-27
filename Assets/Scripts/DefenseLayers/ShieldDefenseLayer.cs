using System;
using UnityEngine;

namespace DefenseLayers
{
    public class ShieldDefenseLayer : DefenseLayerBase
    {
        public Transform Transform { get; private set; }
        public ShieldTransformRuntimeData ShieldTransformRuntimeData;
        private const float _shieldEffectDeffSize = 0.05f;

        public void Init(float basePoints)
        {
            base.InitBase(basePoints);
            var transform = this.transform;
            Transform = transform;
            var rootSize = transform.root.localScale.x;
            ShieldTransformRuntimeData.LossyScale = transform.lossyScale;
            ShieldTransformRuntimeData.HitEffectSize = _shieldEffectDeffSize / rootSize;
            ShieldTransformRuntimeData.Position = transform.position;
            ShieldTransformRuntimeData.Rotation = transform.rotation;
        }
    }

    [Serializable]
    public struct ShieldTransformRuntimeData
    {
        public float HitEffectSize;
        public Vector2 LossyScale;
        public Vector2 Position;
        public Quaternion Rotation;
    }
}