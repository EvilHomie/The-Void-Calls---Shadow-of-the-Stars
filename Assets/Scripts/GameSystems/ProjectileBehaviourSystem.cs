using DefenseLayers;
using DI;
using Projectiles;
using Registries;
using UnityEngine;
using Weapons;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace GameSystems
{
    public class ProjectileBehaviourSystem : GameSystemBase, IUpdateTickObserver
    {
        private ProjectileRegistry _projectileRegistry;
        private static readonly Collider2D[] _colliders = new Collider2D[16];
        private static readonly RaycastHit2D[] _hits = new RaycastHit2D[16];

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
                    if (defenseLayer.OwnerId == boltShootData.OwnerId || defenseLayer.LayerType == DefenseLayerType.Shield)
                    {
                        projectile.IgnoredColliders[projectile.IgnoredCount] = collider;
                        projectile.IgnoredCount++;
                    }
                }
            }
        }

        private void MoveBolts(Bolt bolt, float deltaTime)
        {
            var prevPos = bolt.Position;
            var nextPos = prevPos + bolt.Velocity * deltaTime;

            var hitCount = Physics2D.LinecastNonAlloc(prevPos, nextPos, _hits, bolt.HitLayers);

            if (hitCount == 0)
            {
                bolt.Position = nextPos;
                bolt.Transform.position = nextPos;
            }

            Collider2D bestCollider = null;
            Vector2 bestHitPos = default;
            int bestPriority = int.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = _hits[i];
                var collider = hit.collider;
                bool ignored = false;

                for (int j = 0; j < bolt.IgnoredCount; j++)
                {
                    if (bolt.IgnoredColliders[j] == collider)
                    {
                        ignored = true;
                        break;
                    }
                }

                if (ignored) continue;

                int priority;
                int layer = collider.gameObject.layer;

                if (layer == LayersId.ShieldLayer) priority = 0;
                else if (layer == LayersId.ArmorLayer) priority = 1;
                else priority = 2;

                if (priority < bestPriority)
                {
                    bestPriority = priority;
                    bestCollider = hit.collider;
                    bestHitPos = hit.point;
                }
            }

            if (bestCollider != null)
            {
                bestCollider.TryGetComponent(out DefenseLayerBase defenceLayer);
                EventBus.BoltHitAction?.Invoke(bolt, defenceLayer, bestHitPos);
                _projectileRegistry.RequestRemoveActiveProjectile(bolt);
            }
            else
            {
                bolt.Position = nextPos;
                bolt.Transform.position = nextPos;
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

