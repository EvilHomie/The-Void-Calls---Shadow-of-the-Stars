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
            ref var weaponStats = ref weapon.Stats;

            if (GameFlowSystem.CoreTime <= weaponStats.NextShootTime)
            {
                return;
            }
            ref var baseStats = ref weapon.BaseStats;

            var direction = WeaponSystemHelper.GetDirectionWithSpreadBrookTaylor(weapon.Transform.up, weaponStats.SpreadAngle);
            var shipRB = weapon.ShipRigidBody;
            var shipVelocity = shipRB.linearVelocity;
            var shipForwardVel = Vector2.Dot(shipVelocity, shipRB.transform.up);
            var velocity = shipVelocity + direction * weaponStats.ProjectileSpeed;
            var destroyTime = GameFlowSystem.CoreTime + baseStats.MaxDistance * weaponStats.InvProjectileSpeed;
            var spawnPos = weapon.ShootPoint.position;
            var shootData = new BoltWeaponShootData(weapon.PoolReference, destroyTime, spawnPos, velocity, direction, weapon.HitLayers, weapon.DamageData);

            weapon.ShootSpotPS.Emit(1);
            weaponStats.NextShootTime = GameFlowSystem.CoreTime + weaponStats.ShootDelay;

            EventBus.BoltWeaponShootAction?.Invoke(in shootData);
        }
    }
}