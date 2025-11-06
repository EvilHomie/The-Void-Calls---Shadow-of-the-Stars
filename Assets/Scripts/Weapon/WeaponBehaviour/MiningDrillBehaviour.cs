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
            EventBus.WeaponChangeState?.Invoke(weapon, true);
        }

        public void CancelShoot(MiningDrill weapon)
        {
            EventBus.WeaponChangeState?.Invoke(weapon, false);
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
                weapon.HitParticleAccumulator += weapon.SparksRate * dTime;
                int emitCount = Mathf.FloorToInt(weapon.HitParticleAccumulator);
                weapon.HitSpotPS.Emit(emitCount);
                weapon.HitParticleAccumulator -= emitCount;
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