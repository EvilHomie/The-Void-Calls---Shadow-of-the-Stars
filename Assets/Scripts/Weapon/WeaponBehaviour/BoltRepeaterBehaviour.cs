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

            var projectile = EventBus.GetProjectile?.Invoke(weapon.ProjectilePoolData.PoolName);
            projectile.CachedTransform.SetPositionAndRotation(weapon.ShootPoint.position, weapon.Transform.rotation);
            projectile.Weapon = weapon;
            var weapontTransformUp = weapon.Transform.up;
            projectile.DestroyTime = GameFlowSystem.CoreTime + weapon.MaxDistance / weapon.ProjectileSpeed;
            var direction = WeaponSystemHelper.GetDirectionWithSpreadBrookTaylor(weapontTransformUp, weapon.SpreadAngle);
            var shipRB = weapon.ShipRigidBody;
            float shipForwardVel = Vector2.Dot(shipRB.linearVelocity, shipRB.transform.up);
            projectile.RigidBody.linearVelocity = (weapon.ProjectileSpeed + shipForwardVel) * direction;

            weapon.ShootSpotPS.Emit(1);
            weapon.NextShootTime = GameFlowSystem.CoreTime + weapon.ShootDelay;
        }
    }
}
