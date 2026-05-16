using UnityEngine;

namespace Projectiles
{
    public class Bolt : ProjectileBase
    {
        public override ProjectileType ProjectileType => ProjectileType.Bolt;

        [field: SerializeField] public float TipOffset { get; private set; }

        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsMissed;
        //public Vector2 VelocityNorm;
    }
}