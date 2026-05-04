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
            if (GameFlowSystem.CoreTime <= weapon.NextShootTime)
            {
                return;
            }

            var direction = WeaponSystemHelper.GetDirectionWithSpreadBrookTaylor(weapon.Transform.up, weapon.SpreadAngle);
            var shipRB = weapon.ShipRigidBody;
            var shipVelocity = shipRB.linearVelocity;
            var shipForwardVel = Vector2.Dot(shipVelocity, shipRB.transform.up);
            var velocity = shipVelocity + direction * weapon.ProjectileSpeed;
            var destroyTime = GameFlowSystem.CoreTime + weapon.MaxDistance * weapon.InvProjectileSpeed;
            var spawnPos = weapon.ShootPoint.position;
            var shootData = new BoltWeaponShootData(weapon.PoolReference, destroyTime, spawnPos, velocity, direction, weapon.HitLayers);

            weapon.ShootSpotPS.Emit(1);
            weapon.NextShootTime = GameFlowSystem.CoreTime + weapon.ShootDelay;

            EventBus.BoltWeaponShootAction?.Invoke(in shootData);
        }
    }
}