using Projectile;
using System;
using System.Collections.Generic;

namespace GameSystem
{
    public class ProjectileBehaviour : GameSystemBase
    {
        private HashSet<ProjectileBase> _activeProjectiles;
        private HashSet<ProjectileBase> _projectilesPendingRemoval;
        private HashSet<ProjectileBase> _projectilesPendingReturn;

        protected override void Init()
        {
            _activeProjectiles = new(500);
            _projectilesPendingRemoval = new(500);
            _projectilesPendingReturn = new(500);
        }

        protected override void Subscribe()
        {
            EventBus.ProjectileFetched += OnProjectileFetched;
            GameFlow.FixedGameTick += OnGameTick;
        }

        protected override void Unsubscribe()
        {
            EventBus.ProjectileFetched -= OnProjectileFetched;
        }

        private void OnProjectileFetched(ProjectileBase projectile)
        {
            _activeProjectiles.Add(projectile);
        }
        private void OnGameTick(float dTime)
        {
            foreach (var projectile in _projectilesPendingRemoval)
            {
                _activeProjectiles.Remove(projectile);
            }

            _projectilesPendingRemoval.Clear();

            foreach (var projectile in _projectilesPendingReturn)
            {
                _activeProjectiles.Remove(projectile);
                EventBus.ReturnProjectile(projectile);
            }

            _projectilesPendingReturn.Clear();

            foreach (var projectile in _activeProjectiles)
            {
                if (projectile.InPool)
                {
                    _projectilesPendingRemoval.Add(projectile);
                    continue;
                }

                if (projectile.LifeTime <= 0)
                {
                    _projectilesPendingReturn.Add(projectile);
                    continue;
                }

                projectile.LifeTime -= dTime;
            }
        }
    }
}

