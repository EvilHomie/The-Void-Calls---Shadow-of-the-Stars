using GameSystem;
using UnityEngine;

namespace Projectile
{
    public class BoltRepeaterProjectile : ProjectileBase
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            EventBus.ProjectileHit?.Invoke(this, other);
        }
    }
}