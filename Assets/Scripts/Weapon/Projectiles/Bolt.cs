using GameSystems;
using UnityEngine;

namespace Projectiles
{
    public class Bolt : ProjectileBase
    {
        public override ProjectileType ProjectileType => ProjectileType.Bolt;

        private void OnTriggerEnter2D(Collider2D other)
        {
            EventBus.BoltHitAction?.Invoke(this, other);
        }
    }
}