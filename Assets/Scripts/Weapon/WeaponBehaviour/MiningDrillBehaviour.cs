using GameSystem;
using UnityEngine;

namespace Weapon
{
    public class MiningDrillBehaviour : IWeaponBehaviour<MiningDrill>
    {
        public void StartShoot(MiningDrill weapon)
        {
            ProceedShoot(weapon, 0);
            weapon.BeamLineGO.SetActive(true);
            weapon.ShootSpotPS.Play();
            weapon.IsShooting = true;
            EventBus.WeaponChangeShootState?.Invoke(weapon, true);
        }

        public void CancelShoot(MiningDrill weapon)
        {
            EventBus.WeaponChangeShootState?.Invoke(weapon, false);
            weapon.BeamLineGO.SetActive(false);
            weapon.ShootSpotPS.Stop();
            weapon.IsShooting = false;
        }

        public void ProceedShoot(MiningDrill weapon, float dTime)
        {
            RaycastHit2D hit = Physics2D.Raycast(weapon.CTransform.position, weapon.CTransform.up, weapon.MaxDistance, weapon.HitLayers);

            if (hit.collider != null)
            {
                weapon.HitPos = hit.point;
                weapon.HitSpotT.position = hit.point;
                weapon.HitParticleAccumulator += weapon.HitRate * dTime;

                if (weapon.HitParticleAccumulator >= 1)
                {
                    weapon.HitSpotPS.Emit(1);
                    weapon.HitParticleAccumulator -= 1;
                    EventBus.BeamHit?.Invoke(weapon, hit.collider);
                }
            }
            else
            {
                weapon.HitPos = weapon.CTransform.position + weapon.CTransform.up * weapon.MaxDistance;
            }

            weapon.BeamLineLR.SetPosition(0, weapon.CTransform.position);
            weapon.BeamLineLR.SetPosition(1, weapon.HitPos);
        }
    }
}