using Projectiles;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class ProjectileBehaviourSystem : GameSystemBase
    {
        private HashSet<ProjectileBase> _activeProjectiles;
        private HashSet<ProjectileBase> _destroyedProjectiles;

        protected override void AwakeInit()
        {
            _activeProjectiles = new(500);
            _destroyedProjectiles = new(500);
        }

        protected override void Subscribe()
        {
            GameFlow.UpdateTick += OnGameTick;
            EventBus.ProjectileFetched += OnProjectileFetched;
            EventBus.ProjectileHit += OnProjectileHit;

        }

        protected override void Unsubscribe()
        {
            GameFlow.UpdateTick -= OnGameTick;
            EventBus.ProjectileFetched -= OnProjectileFetched;
            EventBus.ProjectileHit -= OnProjectileHit;
        }

        private void OnProjectileHit(ProjectileBase projectile, Collider2D hitCollider)
        {
            var hitEffect = EventBus.GetHitParticle(projectile.HitData.PoolName);
            Vector2 hitPoint = hitCollider.ClosestPoint(projectile.HitCollider.position);
            hitEffect.CachedTransform.position = hitPoint;
            _destroyedProjectiles.Add(projectile);
        }

        private void OnProjectileFetched(ProjectileBase projectile)
        {
            _activeProjectiles.Add(projectile);
        }
        private void OnGameTick(float dTime)
        {
            ReturnDestroyed();

            foreach (var projectile in _activeProjectiles)
            {
                if (GameFlow.CoreTime >= projectile.DestroyTime)
                {
                    _destroyedProjectiles.Add(projectile);
                    continue;
                }
            }
        }
        private void ReturnDestroyed()
        {
            foreach (var projectile in _destroyedProjectiles)
            {
                _activeProjectiles.Remove(projectile);
                EventBus.ReturnProjectile(projectile);
            }

            _destroyedProjectiles.Clear();
        }
    }
}

