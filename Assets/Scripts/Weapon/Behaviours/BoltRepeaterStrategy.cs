using General;
using Helpers;
using UnityEngine;

namespace Weapons
{
    public class BoltRepeaterStrategy : IWeaponBehaviour<BoltWeapon>
    {
        public void HandleStartShoot(BoltWeapon weapon)
        {
            ProcessShooting(weapon);
        }
        public void HandleCancelShoot(BoltWeapon weapon)
        {
        }

        public void ProcessShooting(BoltWeapon weapon)
        {
            var nextShootTime = weapon.NextShootTime;
            var coreTime = GameFlowSystem.CoreTime;

            if (coreTime <= nextShootTime)
            {
                return;
            }

            ref readonly var fireStats = ref weapon.RuntimeFireStats;
            ref readonly var logicStats = ref weapon.LogicStats;

            var shootPointData = weapon.ShootPointTransformData;
            var spawnPos = shootPointData.Position;
            var baseAimPos = weapon.AimData.AimPosition;
            var distanceToAimPos = Vector2.Distance(baseAimPos, spawnPos);
            var spreadedDirection = WeaponHelper.GetDirectionWithSpreadBrookTaylor(shootPointData.Direction, fireStats.SpreadAngle);
            var spreadTimeMultiplier = logicStats.SpreadTimeMultiplier;
            var distanceMultiplier = 1 + Random.Range(-spreadTimeMultiplier, spreadTimeMultiplier);
            var spreadedAimPos = spawnPos + spreadedDirection * (distanceToAimPos * distanceMultiplier);

            var boltSelfVelocity = spreadedDirection * fireStats.ProjectileSpeed;
            var shipVelocity = weapon.ShipRB.linearVelocity;
            var boltTotalVelocity = shipVelocity + boltSelfVelocity;

            var shootData = new BoltWeaponShootData(
                weapon.ProjectilePoolId,
                weapon.Size,
                weapon.IgnoredColliders,
                logicStats.ProjectileLifeTime,
                spawnPos,
                spreadedAimPos,
                boltTotalVelocity,
                spreadedDirection,
                weapon.RuntimeDamage);

            weapon.ShootSpotPS.Emit(1);
            weapon.NextShootTime = GameFlowSystem.CoreTime + logicStats.ShootDelay;

            EventBus.BoltWeaponShootAction?.Invoke(in shootData);
        }
    }
}




//var distance = Vector2.Distance(aimPosition, spawnPos);

//Vector2 toAim = (aimPosition - spawnPos).normalized;
//float toAimSpeed = Vector2.Dot(boltVelocity, toAim);
//var timeToAimPos = distance / toAimSpeed;

//var timeToAimPos = (aimPosition - spawnPos).magnitude / boltVelocity.magnitude;

/*
 * 
 * var boltForwardSpeed = Vector2.Dot(targetRelativeVelocity, direction);
            var distance = Vector2.Distance(aimPosition, spawnPos);

            var timeToAimPos = distance / boltForwardSpeed;
 * 
 * */


/*
 *  public void ProcessShooting(BoltRepeater weapon)
        {
            ref var runTime = ref weapon.WeaponStats.Runtime;
            var coreTime = GameFlowSystem.CoreTime;

            if (coreTime <= runTime.NextShootTime)
            {
                return;
            }

            var aim = weapon.AimStats;
            var config = weapon.WeaponStats.Config;
            var cached = weapon.WeaponStats.Cached;
            var targetData = weapon.TargetData;

            var direction = WeaponSystemHelper.GetDirectionWithSpreadBrookTaylor(weapon.Transform.up, config.SpreadAngle);
            var spawnPos = weapon.ShootPoint.position;
            var shipVelocity = weapon.ShipRB.linearVelocity;
            var boltSelfVelocity = direction * config.ProjectileSpeed;

            var boltVelocity = shipVelocity + boltSelfVelocity;
            var targetRelativeVelocity = boltVelocity - targetData.TargetVelocity;

            float relativeSpeed = targetRelativeVelocity.magnitude;
            float timeToAimPos;

            if (relativeSpeed < 0.001f) timeToAimPos = cached.ProjectileLifeTime;
            else timeToAimPos = (targetData.TargetPosition - (Vector2)spawnPos).magnitude / relativeSpeed;

            var hitTime = coreTime + timeToAimPos;
            var destroyTime = coreTime + cached.ProjectileLifeTime;

            var shootData = new BoltWeaponShootData(weapon.PoolId, weapon.IgnoredColliders, destroyTime, hitTime, spawnPos, boltVelocity, direction, weapon.HitLayers, weapon.Damage);

            weapon.ShootSpotPS.Emit(1);
            runTime.NextShootTime = GameFlowSystem.CoreTime + cached.ShootDelay;

            EventBus.BoltWeaponShootAction?.Invoke(in shootData);
        }
 * */