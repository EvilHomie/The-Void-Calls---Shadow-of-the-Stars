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
        private static readonly Collider2D[] _colliders = new Collider2D[16];
        private static readonly RaycastHit2D[] _hits = new RaycastHit2D[8];

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
            var projectile = _projectileRegistry.GetBolt(boltShootData.PoolId);
            projectile.Position = boltShootData.FirePointPosition;
            projectile.Transform.position = boltShootData.FirePointPosition;
            projectile.Transform.up = boltShootData.Direction;
            projectile.Velocity = boltShootData.Velocity;
            projectile.VelocityNorm = boltShootData.Velocity.normalized;
            projectile.DestroyTime = boltShootData.DestroyTime;
            projectile.HitLayers = boltShootData.HitLayers;
            projectile.DamageData = boltShootData.DamageData;
            //projectile.OwnerId = boltShootData.OwnerId;

            projectile.IgnoredCount = 0;
            int count = Physics2D.OverlapPointNonAlloc(boltShootData.FirePointPosition, _colliders);

            for (int i = 0; i < count; i++)
            {
                var collider = _colliders[i];

                if (collider.TryGetComponent(out DefenseLayerBase defenseLayer))
                {
                    if (defenseLayer.OwnerId == boltShootData.OwnerId)
                    {
                        projectile.IgnoredColliders[projectile.IgnoredCount] = collider;
                        projectile.IgnoredCount++;
                        continue;
                    }

                    if (defenseLayer.LayerType == DefenseLayerType.Shield)
                    {
                        projectile.IgnoredColliders[projectile.IgnoredCount] = collider;
                        projectile.IgnoredCount++;
                    }
                }
            }
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

            var hitCount = Physics2D.LinecastNonAlloc(prevTip, nextTip, _hits, bolt.HitLayers);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = _hits[i];

                for (int j = 0; j < bolt.IgnoredCount; j++)
                {
                    if (bolt.IgnoredColliders[j] == hit.collider)
                    {
                        bolt.Position = nextCenter;
                        bolt.Transform.position = nextCenter;
                        return;
                    }
                }

                int colliderLayer = hit.collider.gameObject.layer;

                if (colliderLayer == LayersId.ShieldLayer || colliderLayer == LayersId.ArmorLayer || colliderLayer == LayersId.HullLayer)
                {
                    hit.collider.TryGetComponent(out DefenseLayerBase defenceLayer);
                    EventBus.BoltHitAction?.Invoke(bolt, defenceLayer, hit.point);
                    _projectileRegistry.RequestRemoveActiveProjectile(bolt);
                    return;
                }
            }

            bolt.Position = nextCenter;
            bolt.Transform.position = nextCenter;
        }

        private void ProcessStraightMissile(StraightMissile straightMissile)
        {

        }

        private void ProcessHomingMissile(HomingMissile homingMissile)
        {

        }
    }
}

