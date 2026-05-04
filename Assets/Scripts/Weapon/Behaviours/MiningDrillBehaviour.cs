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
            ref var baseStats = ref weapon.BaseStats;

            var weaponPosition = weapon.Transform.position;
            var weaponDirection = weapon.Transform.up;

            RaycastHit2D hit = Physics2D.Raycast(weaponPosition, weaponDirection, baseStats.MaxDistance, weapon.HitLayers); 

            if (hit.collider != null)
            {
                if (!weapon.IsHit)
                {
                    weapon.HitSpotPS.Play();
                    weapon.IsHit = true;
                }

                weapon.HitPos = hit.point;
                weapon.HitSpotT.position = hit.point;
                EventBus.BeamHit?.Invoke(weapon, hit.collider);
            }
            else
            {
                if (weapon.IsHit)
                {
                    weapon.HitSpotPS.Stop();
                    weapon.IsHit = false;
                }

                weapon.HitPos = weaponPosition + weaponDirection * baseStats.MaxDistance;
            }

            weapon.BeamLineLR.SetPosition(0, weaponPosition);
            weapon.BeamLineLR.SetPosition(1, weapon.HitPos);
        }
    }
}