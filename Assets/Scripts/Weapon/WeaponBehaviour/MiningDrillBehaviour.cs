using GameSystems;
using UnityEngine;

namespace Weapons
{
    public class MiningDrillBehaviour : IWeaponBehaviour<MiningDrill>
    {
        public void StartShoot(MiningDrill weapon)
        {
            ProceedShoot(weapon);
            weapon.BeamLineGO.SetActive(true);
            weapon.ShootSpotPS.Play();
        }

        public void CancelShoot(MiningDrill weapon)
        {
            weapon.BeamLineGO.SetActive(false);
            weapon.ShootSpotPS.Stop();
        }

        public void ProceedShoot(MiningDrill weapon)
        {
            RaycastHit2D hit = Physics2D.Raycast(weapon.Transform.position, weapon.Transform.up, weapon.MaxDistance, weapon.HitLayers); 

            if (hit.collider != null)
            {
                weapon.HitPos = hit.point;
                weapon.HitSpotT.position = hit.point;

                if (GameFlowSystem.CoreTime <= weapon.NextHitTime)
                {
                    return;
                }

                weapon.HitSpotPS.Emit(1);
                float hitTime = weapon.NextHitTime - GameFlowSystem.CoreTime;
                EventBus.BeamHit?.Invoke(weapon, hit.collider, hitTime);
                weapon.NextHitTime = GameFlowSystem.CoreTime + weapon.HitDelay;
            }
            else
            {
                weapon.HitPos = weapon.Transform.position + weapon.Transform.up * weapon.MaxDistance;
            }

            weapon.BeamLineLR.SetPosition(0, weapon.Transform.position);
            weapon.BeamLineLR.SetPosition(1, weapon.HitPos);
        }
    }
}