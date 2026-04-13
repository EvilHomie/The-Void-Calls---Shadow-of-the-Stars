using GameSystems;
using Helper;

namespace Weapons
{
    public class BoltRepeaterBehaviour : IWeaponBehaviour<BoltRepeater>
    {
        public void StartShoot(BoltRepeater weapon)
        {
            //weapon.IsShooting = true;
            //EventBus.WeaponChangeShootState?.Invoke(weapon, true);
        }
        public void CancelShoot(BoltRepeater weapon)
        {
            //weapon.IsShooting = false;
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
            projectile.DestroyTime = GameFlowSystem.CoreTime + weapon.MaxDistance / weapon.ProjectileSpeed;
            var direction = WeaponSystemHelper.GetDirectionWithSpreadBrookTaylor(weapon.Transform.up, weapon.SpreadAngle);
            projectile.RigidBody.linearVelocity = direction * weapon.ProjectileSpeed;

            weapon.ShootSpotPS.Emit(1);
            weapon.NextShootTime = GameFlowSystem.CoreTime + weapon.ShootDelay;
        }
    }
}
