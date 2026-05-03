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
            EventBus.BoltHitAction += OnBoltHit;
            EventBus.BoltWeaponShootAction += SpawnBolt;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.BoltHitAction -= OnBoltHit;
            EventBus.BoltWeaponShootAction -= SpawnBolt;
        }

        private void SpawnBolt(in BoltWeaponShootData boltShootData)
        {
            var projectile = _projectileRegistry.GetBolt(boltShootData.PoolReference);

            projectile.Transform.position = boltShootData.FirePointPosition;
            projectile.Transform.up = boltShootData.Velocity;
            projectile.RigidBody.linearVelocity = boltShootData.Velocity;
            projectile.DestroyTime = boltShootData.DestroyTime;
        }

        private void OnBoltHit(Bolt bolt, Collider2D hitCollider)
        {
            _projectileRegistry.RequestRemoveActiveProjectile(bolt);
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

