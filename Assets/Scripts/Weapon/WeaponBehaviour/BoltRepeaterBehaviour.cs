using GameSystem;
using UnityEngine;

namespace Weapon
{
    public class BoltRepeaterBehaviour : IWeaponBehaviour<BoltRepeater>
    {
        public void StartShoot(BoltRepeater weapon)
        {
            weapon.IsShooting = true;
            EventBus.WeaponChangeState?.Invoke(weapon, true);
        }
        public void CancelShoot(BoltRepeater weapon)
        {
            weapon.IsShooting = false;
            
        }

        public void ProceedShoot(BoltRepeater weapon, float dTime)
        {
            if (weapon.FireCooldown <= 0)
            {
                if (!weapon.IsShooting)
                {
                    EventBus.WeaponChangeState?.Invoke(weapon, false);
                    return;
                }

                weapon.ShootSpotPS.Emit(1);
                weapon.FireCooldown = weapon.TimePerShot;

                var projectile = EventBus.GetProjectile(weapon.ProjectilePF.PoolData.PoolName);
                projectile.CachedTransform.SetPositionAndRotation(weapon.ShootSpotT.position, weapon.CTransform.rotation);
                projectile.RigidBody.linearVelocity = (Vector2)weapon.CTransform.up * weapon.ProjectileSpeed;
                projectile.Weapon = weapon;
                projectile.LifeTime = weapon.MaxDistance / weapon.ProjectileSpeed;
            }

            weapon.FireCooldown -= dTime;
        }
    }
}
