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
            EventBus.WeaponChangeState?.Invoke(weapon, false);
        }

        public void ProceedShoot(BoltRepeater weapon, float dTime)
        {
            weapon.FireCooldown -= dTime;

            if (weapon.FireCooldown <= 0)
            {
                weapon.ShootSpotPS.Emit(1);
                weapon.FireCooldown = weapon.TimePerShot;

                var projectile = Object.Instantiate(weapon.ProjectilePF, weapon.ShootSpotT.position, weapon.CTransform.rotation);
                projectile.RigidBody.linearVelocity = (Vector2)weapon.CTransform.up * weapon.ProjectileSpeed;
            }
        }
    }
}
