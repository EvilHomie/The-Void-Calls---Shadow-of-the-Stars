using UnityEngine;

namespace Weapon
{
    public class MiningDrillBehaviour : IWeaponBehaviour<MiningDrill>
    {
        public void StartShoot(MiningDrill weapon)
        {
            ProceedShoot(weapon, 0);
            weapon.BeamLineGO.SetActive(true);
            weapon.ShootSpot.Play();

        }

        public void CancelShoot(MiningDrill weapon)
        {
            weapon.BeamLineGO.SetActive(false);
            weapon.ShootSpot.Stop();
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
                weapon.HitSpot.Emit(emitCount);
                weapon.HitParticleAccumulator -= emitCount;
            }
            else
            {
                weapon.HitPos = weapon.CTransform.up * weapon.MaxDistance;
                
            }

            weapon.BeamLine.SetPosition(0, weapon.CTransform.position);
            weapon.BeamLine.SetPosition(1, weapon.HitPos);
        }
    }
}