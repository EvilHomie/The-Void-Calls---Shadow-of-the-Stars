using DefenseLayers;
using DI;
using Projectiles;
using Registries;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace GameSystems
{
    public class ProjectileBehaviourSystem : GameSystemBase, ICoreUpdateTickObserver
    {
        private ProjectileRegistry _projectileRegistry;
        private Dictionary<SizeType, float> _projectileSizeMap;
        private Vector3 _deffProjectileSize = Vector3.one;

        [Inject]
        public void Construct(ProjectileRegistry shipRegistry)
        {
            _projectileRegistry = shipRegistry;
            _projectileSizeMap = new()
            {
                {SizeType.S, 1 },
                {SizeType.M, 5 },
                {SizeType.L, 25 },
                {SizeType.XL, 125 }
            };
        }

        public void CoreUpdateTick(float deltaTime)
        {
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
            var projectile = _projectileRegistry.GetBolt(boltShootData.ProjectilePoolId);
            projectile.Transform.localScale = _deffProjectileSize * _projectileSizeMap[boltShootData.Size];
            projectile.Position = boltShootData.SpawnPosition;
            projectile.Transform.position = boltShootData.SpawnPosition;
            projectile.Transform.up = boltShootData.Direction;
            projectile.Velocity = boltShootData.Velocity;
            projectile.DestroyTime = boltShootData.DestroyTime;
            projectile.HitTime = boltShootData.HitTime;
            projectile.HitLayers = boltShootData.HitLayers;
            projectile.DamageData = boltShootData.DamageData;
            projectile.IgnoredColliders = boltShootData.IgnoredColliders;
            projectile.IsMissed = false;
            projectile.Size = boltShootData.Size;
        }

        private void MoveBolts(Bolt bolt, float deltaTime)
        {
            var currentPosition = bolt.Position;

            if (bolt.IsMissed || GameFlowSystem.CoreTime < bolt.HitTime)
            {
                var nextPos = currentPosition + bolt.Velocity * deltaTime;
                bolt.Position = nextPos;
                bolt.Transform.position = nextPos;
                return;
            }

            var hit = Physics2D.OverlapPoint(currentPosition, bolt.HitLayers);

            if (hit == null || bolt.IgnoredColliders.Contains(hit))
            {
                bolt.IsMissed = true;
                var nextPos = currentPosition + bolt.Velocity * deltaTime;
                bolt.Position = nextPos;
                bolt.Transform.position = nextPos;
                return;
            }

            hit.TryGetComponent(out DefenseLayerBase defenceLayer);

            var hitData = new HitData
            {
                Position = currentPosition,
                Size = bolt.Size
            };

            EventBus.BoltHitAction?.Invoke(bolt, defenceLayer, hitData);
            _projectileRegistry.RequestRemoveActiveProjectile(bolt);
        }

        private void ProcessStraightMissile(StraightMissile straightMissile)
        {

        }

        private void ProcessHomingMissile(HomingMissile homingMissile)
        {

        }
    }
}

