using GameSystems;
using UnityEngine;

namespace Weapons
{
    public class MiningDrillBehaviour : IWeaponBehaviour<MiningDrill>
    {
        public void HandleStartShoot(MiningDrill weapon)
        {
            ProcessShooting(weapon);
            weapon.BeamLineGO.SetActive(true);
            weapon.ShootSpotPS.Play();
        }

        public void HandleCancelShoot(MiningDrill weapon)
        {
            weapon.BeamLineGO.SetActive(false);
            weapon.ShootSpotPS.Stop();

            if (weapon.IsHit)
            {
                weapon.HitSpotPS.Stop();
                weapon.IsHit = false;
            }
        }

        public void ProcessShooting(MiningDrill weapon)
        {
            ref var aimStats = ref weapon.AimStats;

            var weaponPosition = weapon.Transform.position;
            var weaponDirection = weapon.Transform.up;

            RaycastHit2D hit = Physics2D.Raycast(weaponPosition, weaponDirection, aimStats.MaxDistance, weapon.HitLayers); 

            if (hit.collider != null)
            {
                if (!weapon.IsHit)
                {
                    weapon.HitSpotPS.Play();
                    weapon.IsHit = true;
                }

                weapon.HitPos = hit.point;
                weapon.HitSpotT.position = hit.point;

                var damage = weapon.Damage;
                var coreDeltaTime = GameFlowSystem.CoreDeltaTimeTick;
                damage.Energy *= coreDeltaTime;
                damage.Kinetic *= coreDeltaTime;
                damage.Asteroid *= coreDeltaTime;

                EventBus.BeamHitAction?.Invoke(damage, hit.collider, weapon.HitPos);
            }
            else
            {
                if (weapon.IsHit)
                {
                    weapon.HitSpotPS.Stop();
                    weapon.IsHit = false;
                }

                weapon.HitPos = weaponPosition + weaponDirection * aimStats.MaxDistance;
            }

            weapon.BeamLineLR.SetPosition(0, weaponPosition);
            weapon.BeamLineLR.SetPosition(1, weapon.HitPos);
        }
    }
}