using GameSystems;
using Helpers;
using UnityEngine;

namespace Weapons
{
    public class BoltRepeaterBehaviour : IWeaponBehaviour<BoltRepeater>
    {
        public void StartShoot(BoltRepeater weapon)
        {
        }
        public void CancelShoot(BoltRepeater weapon)
        {
        }

        public void ProceedShoot(BoltRepeater weapon)
        {
            if (GameFlowSystem.CoreTime <= weapon.NextShootTime)
            {
                return;
            }

            var direction = WeaponSystemHelper.GetDirectionWithSpreadBrookTaylor(weapon.Transform.up, weapon.SpreadAngle);
            var shipRB = weapon.ShipRigidBody;
            var shipForwardVel = Vector2.Dot(shipRB.linearVelocity, shipRB.transform.up);
            var velocity = direction * (weapon.ProjectileSpeed + shipForwardVel);
            var destroyTime = GameFlowSystem.CoreTime + weapon.MaxDistance / weapon.ProjectileSpeed;
            var spawnPos = weapon.ShootPoint.position;
            var shootData = new BoltSpawnData(weapon.PoolReference, destroyTime, spawnPos, velocity);

            weapon.ShootSpotPS.Emit(1);
            weapon.NextShootTime = GameFlowSystem.CoreTime + weapon.ShootDelay;

            EventBus.SpawnBoltAction?.Invoke(in shootData);
        }
    }
}