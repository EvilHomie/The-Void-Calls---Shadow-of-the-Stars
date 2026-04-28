using DI;
using Projectiles;
using Registries;
using UnityEngine;
using Weapons;

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
            EventBus.ProjectileHitAction += OnProjectileHit;
            EventBus.SpawnBoltAction += SpawnBolt;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.ProjectileHitAction -= OnProjectileHit;
            EventBus.SpawnBoltAction -= SpawnBolt;
        }

        private void SpawnBolt(in BoltSpawnData boltShootData)
        {
            var projectile = _projectileRegistry.GetBolt(boltShootData.PoolReference);

            projectile.Transform.position = boltShootData.Position;
            projectile.Transform.up = boltShootData.Velocity;
            projectile.RigidBody.linearVelocity = boltShootData.Velocity;
            projectile.DestroyTime = boltShootData.DestroyTime;
        }

        private void OnProjectileHit(ProjectileBase projectile, Collider2D hitCollider)
        {
            Vector2 hitPoint = hitCollider.ClosestPoint(projectile.HitCollider.position);
            var projectileHitData = new HitEffectSpawnData(projectile.PoolReference, hitPoint);
            _projectileRegistry.RequestRemoveActiveProjectile(projectile);

            EventBus.SpawnHitEffectAction?.Invoke(projectileHitData);
        }

        private void OnGameTick()
        {
            foreach (var projectile in _projectileRegistry.ActiveProjectiles)
            {
                if (GameFlowSystem.CoreTime >= projectile.DestroyTime)
                {
                    _projectileRegistry.RequestRemoveActiveProjectile(projectile);
                    continue;
                }

                //if (projectile is StraightMissile straightMissile)
                //{
                //    ProcessStraightMissile(straightMissile);
                //}
                //else if (projectile is HomingMissile homingMissile)
                //{
                //    ProcessHomingMissile (homingMissile);
                //}
            }
        }

        private void ProcessStraightMissile(StraightMissile straightMissile)
        {

        }

        private void ProcessHomingMissile(HomingMissile homingMissile)
        {

        }
    }
}

