using DI;
using General;
using Helpers;
using Projectiles;
using Registries;
using System.Collections.Generic;
using UnityEngine;
using Weapons;
using static Helpers.WeaponHelper;

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
                    if (ProjectileIsHit(projectile)) continue;
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
            projectile.CurrentPos = boltShootData.SpawnPosition; ;
            projectile.Velocity = boltShootData.Velocity; ;
            projectile.DestroyTime = boltShootData.DestroyTime;
            projectile.HitTime = boltShootData.HitTime; ;
            projectile.HitLayers = boltShootData.HitLayers;
            projectile.DamageData = boltShootData.DamageData;
            projectile.IgnoredColliders = boltShootData.IgnoredColliders;
            projectile.IsMissed = false;
            projectile.Size = boltShootData.Size;
            projectile.AimPos = boltShootData.AimPos;

            projectile.Transform.up = boltShootData.Direction;
            projectile.Transform.position = boltShootData.SpawnPosition; ;
        }

        private void ProcceedBoltBehaviour(Bolt bolt, float deltaTime)
        {
            if (!bolt.IsMissed && GameFlowSystem.CoreTime >= bolt.HitTime)
            {
                if (ProjectileIsHit(bolt)) return;
            }

            var pos = bolt.CurrentPos;
            var nextPos = pos + bolt.Velocity * deltaTime;

            var hitResult = WeaponHelper.TryGetProjectileHit(pos, nextPos, bolt.AimPos);


            var interceptedHit = Physics2D.Linecast(pos, nextPos, LayersId.AsteroidsMask);
            var defenseCollider = interceptedHit.collider;

            if (defenseCollider)
            {
                var isOverlapPoint = defenseCollider.OverlapPoint(bolt.AimPos);

                if (isOverlapPoint)
                {
                    bolt.CurrentPos = nextPos;
                    bolt.Transform.position = nextPos;
                    return;
                }

                var hitPoint = interceptedHit.point;
                var hitReusult = new HitResult(true, defenseCollider, hitPoint);
                _hitRegistrationSystem.RegisterHit(hitReusult, bolt.Size, bolt.DamageData);
                _projectileRegistry.RequestRemoveActiveProjectile(bolt);
                return;
            }

            bolt.CurrentPos = nextPos;
            bolt.Transform.position = nextPos;
        }

        private bool ProjectileIsHit(ProjectileBase projectile)
        {
            var position = projectile.CurrentPos;
            var defenseCollider = Physics2D.OverlapPoint(position, LayersId.HitMask);
            var hasHit = defenseCollider && !projectile.IgnoredColliders.Contains(defenseCollider);

            if (hasHit)
            {
                var hitResult = new HitResult(hasHit, defenseCollider, position);
                _hitRegistrationSystem.RegisterHit(hitResult, projectile.Size, projectile.DamageData);
                _projectileRegistry.RequestRemoveActiveProjectile(projectile);
                return true;
            }
            projectile.IsMissed = true;
            return false;
        }





        private void ProcessStraightMissileBehaviour(StraightMissile straightMissile)
        {

        }

        private void ProcessHomingMissileBehaviourv(HomingMissile homingMissile)
        {

        }
    }
}