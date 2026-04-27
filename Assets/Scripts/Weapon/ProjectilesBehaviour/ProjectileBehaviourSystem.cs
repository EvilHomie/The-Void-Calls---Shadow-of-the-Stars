using Projectiles;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class ProjectileBehaviourSystem : GameSystemBase, IUpdateTickObserver
    {
        private HashSet<ProjectileBase> _activeProjectiles;
        private HashSet<ProjectileBase> _destroyedProjectiles;

        protected override void AwakeInit()
        {
            _activeProjectiles = new(500);
            _destroyedProjectiles = new(500);
            ActiveGameState = GameState.CoreGameplay;
        }

        public void UpdateTick(float deltaTime)
        {
            if (!SystemIsActive) return;

            OnGameTick(deltaTime);
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            EventBus.ProjectileFetched += OnProjectileFetched;
            EventBus.ProjectileHit += OnProjectileHit;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.ProjectileFetched -= OnProjectileFetched;
            EventBus.ProjectileHit -= OnProjectileHit;
        }

        private void OnProjectileHit(ProjectileBase projectile, Collider2D hitCollider)
        {
            var hitEffect = EventBus.GetHitParticle(projectile.HitData.PoolReference);
            Vector2 hitPoint = hitCollider.ClosestPoint(projectile.HitCollider.position);
            hitEffect.CachedTransform.position = hitPoint;
            _destroyedProjectiles.Add(projectile);

            Debug.LogError(hitCollider.name);
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
                if (GameFlowSystem.CoreTime >= projectile.DestroyTime)
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

