using DI;
using GameSystems;
using Projectiles;
using System.Collections.Generic;
using UnityEngine;

namespace Registries
{
    public class ProjectileRegistry : MonoBehaviour, IPreUpdateTickObserver
    {
        public IReadOnlyCollection<ProjectileBase> ActiveProjectiles => _activeProjectiles;

        private readonly HashSet<ProjectileBase> _activeProjectiles = new(200);
        private readonly HashSet<ProjectileBase> _activeProjectilesToAdd = new(20);
        private readonly HashSet<ProjectileBase> _activeProjectilesToRemove = new(20);

        [Inject]
        public void Construct(GameFlowSystem gameFlowSystem)
        {
            gameFlowSystem.AddTickObserver(this);
        }

        public void PreUpdateTick()
        {
            Sync();
        }

        public void RequestAddActiveProjectile(ProjectileBase projectile)
        {
            _activeProjectilesToRemove.Remove(projectile);
            _activeProjectilesToAdd.Add(projectile);
        }

        public void RequestRemoveActiveProjectile(ProjectileBase projectile)
        {
            _activeProjectilesToAdd.Remove(projectile);
            _activeProjectilesToRemove.Add(projectile);
        }

        public void Sync()
        {
            foreach (var projectile in _activeProjectilesToRemove)
            {
                EventBus.ReturnProjectileInPull(projectile);
                _activeProjectiles.Remove(projectile);
            }

            _activeProjectilesToRemove.Clear();

            foreach (var projectile in _activeProjectilesToAdd)
            {
                _activeProjectiles.Add(projectile);
            }

            _activeProjectilesToAdd.Clear();
        }
    }
}