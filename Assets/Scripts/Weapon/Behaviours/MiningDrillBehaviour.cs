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
        }

        public void ProcessShooting(MiningDrill weapon)
        {
            RaycastHit2D hit = Physics2D.Raycast(weapon.Transform.position, weapon.Transform.up, weapon.MaxDistance, weapon.HitLayers); 

            if (hit.collider != null)
            {
                weapon.HitPos = hit.point;
                weapon.HitSpotT.position = hit.point;
                float coreTime = GameFlowSystem.CoreTime;

                if (coreTime <= weapon.NextHitTime)
                {
                    return;
                }

                weapon.HitSpotPS.Emit(1);
                EventBus.BeamHit?.Invoke(weapon, hit.collider);
                weapon.NextHitTime = coreTime + weapon.HitDelay;
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