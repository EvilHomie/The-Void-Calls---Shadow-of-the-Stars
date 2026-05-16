using GameSystems;
using Helpers;
using UnityEngine;

namespace Weapons
{
    public class BoltRepeaterBehaviour : IWeaponBehaviour<BoltRepeater>
    {
        public void HandleStartShoot(BoltRepeater weapon)
        {
            ProcessShooting(weapon);
        }
        public void HandleCancelShoot(BoltRepeater weapon)
        {
        }

        public void ProcessShooting(BoltRepeater weapon)
        {
            ref var runTime = ref weapon.WeaponStats.Runtime;

            if (GameFlowSystem.CoreTime <= runTime.NextShootTime)
            {
                return;
            }

            var aim = weapon.AimStats;
            var config = weapon.WeaponStats.Config;
            var cached = weapon.WeaponStats.Cached;
            var shootPosition = weapon.TargetData.ShootPosition;

            var direction = WeaponSystemHelper.GetDirectionWithSpreadBrookTaylor(weapon.Transform.up, config.SpreadAngle);
            Vector2 spawnPos = weapon.ShootPoint.position;
            var shipVelocity = weapon.ShipRB.linearVelocity;

            var boltVelocity = shipVelocity + direction * config.ProjectileSpeed;
            var distance = Vector2.Distance(shootPosition, spawnPos);
            float speed = boltVelocity.magnitude;

            float timeToTarget = distance / speed;
            var hitTime = GameFlowSystem.CoreTime + timeToTarget;
            var destroyTime = GameFlowSystem.CoreTime + cached.ProjectileLifeTime;

            var shootData = new BoltWeaponShootData(weapon.PoolId, weapon.IgnoredColliders, destroyTime, hitTime, spawnPos, boltVelocity, direction, weapon.HitLayers, weapon.Damage);

            weapon.ShootSpotPS.Emit(1);
            runTime.NextShootTime = GameFlowSystem.CoreTime + cached.ShootDelay;

            EventBus.BoltWeaponShootAction?.Invoke(in shootData);
        }
    }
}