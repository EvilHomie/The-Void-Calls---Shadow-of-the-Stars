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
            projectile.OwnerId = boltShootData.OwnerId;

            projectile.IgnoredLayers.Clear();
            int count = Physics2D.OverlapPointNonAlloc(boltShootData.FirePointPosition, _colliders);


            for (int i = 0; i < count; i++)
            {
                if (_colliders[i].TryGetComponent(out DefenseLayerBase defenseLayer))
                {
                    if (defenseLayer.OwnerId == boltShootData.OwnerId)
                    {
                        projectile.IgnoredLayers.Add(defenseLayer);
                    }
                    else if (defenseLayer is ShieldLayer)
                    {
                        projectile.IgnoredLayers.Add(defenseLayer);
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

            float closestFraction = float.MaxValue;
            DefenseLayerBase closestLayer = null;
            Vector2 closestHit = default;

            var hitCount = Physics2D.LinecastNonAlloc(prevTip, nextTip, _hits, bolt.HitLayers);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = _hits[i];

                if (!hit.collider.TryGetComponent(out DefenseLayerBase layer))
                    continue;

                if (bolt.IgnoredLayers.Contains(layer))
                    continue;

                if (layer is ArmorLayer)
                {
                    closestLayer = layer;
                    closestHit = hit.point;
                    break;
                }

                if (hit.fraction < closestFraction)
                {
                    closestFraction = hit.fraction;
                    closestLayer = layer;
                    closestHit = hit.point;
                }
            }

            if (closestLayer != null)
            {
                EventBus.BoltHitAction?.Invoke(bolt, closestLayer, closestHit);
                _projectileRegistry.RequestRemoveActiveProjectile(bolt);
                return;
            }

            bolt.Position = nextCenter;
            bolt.Transform.position = bolt.Position;

            //if (!hit)
            //{
            //    bolt.Position = nextCenter;
            //    bolt.Transform.position = bolt.Position;
            //    return;
            //}

            //if (hit.collider.TryGetComponent(out DefenseLayerBase defenseLayer))
            //{
            //    if (bolt.IgnoredLayers.Contains(defenseLayer))
            //    {
            //        bolt.Position = nextCenter;
            //        bolt.Transform.position = bolt.Position;
            //        return;
            //    }

            //    EventBus.BoltHitAction?.Invoke(bolt, defenseLayer, hit.point);
            //    _projectileRegistry.RequestRemoveActiveProjectile(bolt);
            //}
        }

        private void ProcessStraightMissile(StraightMissile straightMissile)
        {

        }

        private void ProcessHomingMissile(HomingMissile homingMissile)
        {

        }
    }
}

