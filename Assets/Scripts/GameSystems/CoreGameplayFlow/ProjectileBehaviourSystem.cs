using DI;
using General;
using Projectiles;
using Registries;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace CoreGameSystems
{
    public class ProjectileBehaviourSystem : MonoBehaviour
    {
        private ProjectileRegistry _projectileRegistry;
        private HitRegistrationSystem _hitRegistrationSystem;
        private Dictionary<SizeType, float> _projectileSizeMap;
        private Vector3 _deffProjectileSize = Vector3.one;

        [Inject]
        public void Construct(ProjectileRegistry shipRegistry, HitRegistrationSystem hitRegistrationSystem)
        {
            _projectileRegistry = shipRegistry;
            _hitRegistrationSystem = hitRegistrationSystem;

            _projectileSizeMap = new()
            {
                {SizeType.S, 1 },
                {SizeType.M, 2 },
                {SizeType.L, 6 },
                {SizeType.XL, 12 }
            };

            EventBus.BoltWeaponShootAction += SpawnBolt;
        }

        public void Execute(float deltaTime)
        {
            foreach (var projectile in _projectileRegistry.ActiveProjectiles)
            {
                if (GameFlowSystem.CoreTime >= projectile.DestroyTime)
                {
                    CheckProjectileHit(projectile);
                    _projectileRegistry.RequestRemoveActiveProjectile(projectile);
                    continue;
                }

                if (projectile is Bolt bolt)
                {
                    ProcceedBoltBehaviour(bolt, deltaTime);
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

        private void ProcceedBoltBehaviour(Bolt bolt, float deltaTime)
        {
            if (bolt.IsMissed || GameFlowSystem.CoreTime < bolt.HitTime)
            {
                MoveBolt(bolt, deltaTime);
                return;
            }

            if (!CheckProjectileHit(bolt))
            {
                bolt.IsMissed = true;
                MoveBolt(bolt, deltaTime);
            }
        }

        private void MoveBolt(Bolt bolt, float deltaTime)
        {
            var currentPosition = bolt.Position;
            var nextPos = currentPosition + bolt.Velocity * deltaTime;
            bolt.Position = nextPos;
            bolt.Transform.position = nextPos;
        }

        private bool CheckProjectileHit(ProjectileBase projectile)
        {
            var currentPosition = projectile.Position;
            var collider = Physics2D.OverlapPoint(currentPosition, projectile.HitLayers);

            if (collider == null || projectile.IgnoredColliders.Contains(collider)) return false;

            _hitRegistrationSystem.RegisterHit(collider, currentPosition, projectile.Size, projectile.DamageData);
            _projectileRegistry.RequestRemoveActiveProjectile(projectile);
            return true;
        }



        private void ProcessStraightMissileBehaviour(StraightMissile straightMissile)
        {

        }

        private void ProcessHomingMissileBehaviourv(HomingMissile homingMissile)
        {

        }
    }
}