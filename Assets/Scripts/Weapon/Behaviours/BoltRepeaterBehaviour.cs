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

            var direction = WeaponSystemHelper.GetDirectionWithSpreadBrookTaylor(weapon.Transform.up, config.SpreadAngle);
            var shipRB = weapon.ShipRB;
            var shipVelocity = shipRB.linearVelocity;
            var shipForwardVel = Vector2.Dot(shipVelocity, shipRB.transform.up);
            var velocity = shipVelocity + direction * config.ProjectileSpeed;
            var destroyTime = GameFlowSystem.CoreTime + aim.MaxDistance * cached.InvProjectileSpeed;
            var spawnPos = weapon.ShootPoint.position;
            var shootData = new BoltWeaponShootData(weapon.PoolReference, weapon.OwnerId, destroyTime, spawnPos, velocity, direction, weapon.HitLayers, weapon.Damage);

            weapon.ShootSpotPS.Emit(1);
            runTime.NextShootTime = GameFlowSystem.CoreTime + cached.ShootDelay;

            EventBus.BoltWeaponShootAction?.Invoke(in shootData);
        }
    }
}