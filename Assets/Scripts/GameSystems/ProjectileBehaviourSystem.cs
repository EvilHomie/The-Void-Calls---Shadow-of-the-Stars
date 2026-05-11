using DefenseLayers;
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

            OnGameTick(deltaTime);
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            EventBus.BoltWeaponShootAction += SpawnBolt;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.BoltWeaponShootAction -= SpawnBolt;
        }

        private void SpawnBolt(in BoltWeaponShootData boltShootData)
        {
            var projectile = _projectileRegistry.GetBolt(boltShootData.PoolReference);
            projectile.Position = boltShootData.FirePointPosition;
            projectile.Transform.position = boltShootData.FirePointPosition;
            projectile.Transform.up = boltShootData.Direction;
            projectile.Velocity = boltShootData.Velocity;
            projectile.VelocityNorm = boltShootData.Velocity.normalized;
            projectile.DestroyTime = boltShootData.DestroyTime;
            projectile.HitLayers = boltShootData.HitLayers;
            projectile.DamageData = boltShootData.DamageData;
            projectile.OwnerId = boltShootData.OwnerId;
        }

        private void OnGameTick(float deltaTime)
        {
            foreach (var projectile in _projectileRegistry.ActiveProjectiles)
            {
                if (GameFlowSystem.CoreTime >= projectile.DestroyTime)
                {
                    _projectileRegistry.RequestRemoveActiveProjectile(projectile);
                    continue;
                }

                if (projectile is Bolt bolt)
                {
                    MoveBolts(bolt, deltaTime);
                }
            }
        }

        private void MoveBolts(Bolt bolt, float deltaTime)
        {
            var step = bolt.Velocity * deltaTime;

            var prevCenter = bolt.Position;
            var nextCenter = prevCenter + step;
            var tipDirrectOffset = bolt.VelocityNorm * bolt.TipOffset;

            var prevTip = prevCenter + tipDirrectOffset;
            var nextTip = nextCenter + tipDirrectOffset;

            var hit = Physics2D.Linecast(prevTip, nextTip, bolt.HitLayers);

            if (!hit)
            {
                bolt.Position = nextCenter;
                bolt.Transform.position = bolt.Position;
                return;
            }

            if (hit.collider.TryGetComponent(out DefenseLayerBase defenseLayer))
            {
                if (defenseLayer.OwnerId == bolt.OwnerId) return;
                EventBus.BoltHitAction?.Invoke(bolt, defenseLayer, hit.point);
                _projectileRegistry.RequestRemoveActiveProjectile(bolt);
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

