using DI;
using GamePools;
using CoreGameSystems;
using Projectiles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Registries
{
    public class ProjectileRegistry : MonoBehaviour, ICorePreUpdateTickObserver
    {
        public IReadOnlyCollection<ProjectileBase> ActiveProjectiles => _activeProjectiles;

        private readonly HashSet<ProjectileBase> _activeProjectiles = new(500);
        private readonly HashSet<ProjectileBase> _activeProjectilesToAdd = new(100);
        private readonly HashSet<ProjectileBase> _activeProjectilesToRemove = new(100);

        private ProjectilesPool _projectilesPool;

        [Inject]
        public void Construct(GameFlowSystem gameFlowSystem, ProjectilesPool projectilesPool)
        {
            _projectilesPool = projectilesPool;
            gameFlowSystem.AddTickObserver(this);
        }

        public void CorePreUpdateTick()
        {
            Sync();
        }

        public Bolt GetBolt(uint poolId)
        {
            var projectile = _projectilesPool.Getitem(poolId);

#if UNITY_EDITOR
            if (projectile is not Bolt) throw new Exception($"Expected Bolt, got {projectile.GetType()} from pool {poolId}");
#endif

            RequestAddActiveProjectile(projectile);
            return (Bolt)projectile;
        }

        public HomingMissile GetHomingMissile(uint poolId)
        {
            var projectile = _projectilesPool.Getitem(poolId);

#if UNITY_EDITOR
            if (projectile is not Bolt) throw new Exception($"Expected HomingMissile, got {projectile.GetType()} from pool {poolId}");
#endif

            RequestAddActiveProjectile(projectile);
            return (HomingMissile)projectile;
        }

        public StraightMissile GetStraightMissile(uint poolId)
        {
            var projectile = _projectilesPool.Getitem(poolId);

#if UNITY_EDITOR
            if (projectile is not Bolt) throw new Exception($"Expected StraightMissile, got {projectile.GetType()} from pool {poolId}");
#endif

            RequestAddActiveProjectile(projectile);
            return (StraightMissile)projectile;
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
                _projectilesPool.ReleaseItem(projectile);
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