using DI;
using Projectiles;
using Registries;
using UnityEngine;

namespace GameSystems
{
    public class ProjectileBehaviourSystem : GameSystemBase, IUpdateTickObserver
    {
        private ProjectileRegistry _projectileRegistry;

        [Inject]
        public void Construct(ProjectileRegistry shipRegistry)
        {
            _projectileRegistry = shipRegistry;
             ActiveGameState = GameState.CoreGameplay;
        }

        public void UpdateTick(float deltaTime)
        {
            if (!SystemIsActive) return;

            OnGameTick();
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
            _projectileRegistry.RequestRemoveActiveProjectile(projectile);
        }

        private void OnProjectileFetched(ProjectileBase projectile)
        {
            _projectileRegistry.RequestAddActiveProjectile(projectile);
        }
        private void OnGameTick()
        {
            foreach (var projectile in _projectileRegistry.ActiveProjectiles)
            {
                if (GameFlowSystem.CoreTime >= projectile.DestroyTime)
                {
                    _projectileRegistry.RequestRemoveActiveProjectile(projectile);
                }
            }
        }
    }
}

