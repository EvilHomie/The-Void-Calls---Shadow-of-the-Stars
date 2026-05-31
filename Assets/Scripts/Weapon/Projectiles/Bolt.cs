using UnityEngine;

namespace Projectiles
{
    public class Bolt : ProjectileBase
    {
        public override ProjectileType ProjectileType => ProjectileType.Bolt;

        public Vector3 Position;
        public Vector3 Velocity;
        public bool IsMissed;
    }
}