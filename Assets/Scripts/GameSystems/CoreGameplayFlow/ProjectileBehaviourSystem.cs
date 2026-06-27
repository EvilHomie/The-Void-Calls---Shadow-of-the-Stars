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
                    if (ProceedProjectileHit(projectile)) continue;
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

            var spawnPos = boltShootData.SpawnPos;
            var aimPos = boltShootData.AimPos;
            var velocity = boltShootData.Velocity;
            var distanceToAimPos = Vector2.Distance(aimPos, spawnPos);

            var toAim = (aimPos - spawnPos).normalized;
            var toAimSpeed = Vector2.Dot(velocity, toAim);
            toAimSpeed = Mathf.Max(0.001f, toAimSpeed);
            var timeToAimPos = distanceToAimPos / toAimSpeed;
            var coreTime = GameFlowSystem.CoreTime;

            projectile.Transform.localScale = _deffProjectileSize * _projectileSizeMap[boltShootData.Size];
            projectile.CurrentPos = spawnPos;
            projectile.Velocity = velocity;
            projectile.DestroyTime = coreTime + boltShootData.LifeTime;
            projectile.HitTime = coreTime + timeToAimPos;
            projectile.DamageData = boltShootData.DamageData;
            projectile.IgnoredColliders = boltShootData.IgnoredColliders;
            projectile.IsMissed = false;
            projectile.Size = boltShootData.Size;
            projectile.AimPos = aimPos;

            projectile.Transform.up = boltShootData.Direction;
            projectile.Transform.position = spawnPos;
        }

        private void ProcceedBoltBehaviour(Bolt bolt, float deltaTime)
        {
            if (!bolt.IsMissed && GameFlowSystem.CoreTime >= bolt.HitTime)
            {
                if (ProceedProjectileHit(bolt)) return;
            }

            var pos = bolt.CurrentPos;
            var nextPos = pos + bolt.Velocity * deltaTime;

            var hitResult = WeaponHelper.TryGetProjectileHit(pos, nextPos, bolt.AimPos);

            if (!hitResult.HasHit)
            {
                bolt.CurrentPos = nextPos;
                bolt.Transform.position = nextPos;
            }
            else
            {
                _hitRegistrationSystem.RegisterHit(hitResult, bolt.Size, bolt.DamageData);
                _projectileRegistry.RequestRemoveActiveProjectile(bolt);
            }
        }

        private bool ProceedProjectileHit(ProjectileBase projectile)
        {
            var position = projectile.CurrentPos;
            var defenseCollider = Physics2D.OverlapPoint(position, LayersId.DamageableMask);
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