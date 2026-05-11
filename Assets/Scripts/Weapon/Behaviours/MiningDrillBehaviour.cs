using GameSystems;
using UnityEngine;

namespace Weapons
{
    public class MiningDrillBehaviour : IWeaponBehaviour<MiningDrill>
    {
        public void HandleStartShoot(MiningDrill weapon)
        {
            weapon.BeamLineLR.enabled = true;
            weapon.ShootSpotPS.Play();
            ProcessShooting(weapon);
        }

        public void HandleCancelShoot(MiningDrill weapon)
        {
            weapon.BeamLineLR.enabled = false;
            weapon.ShootSpotPS.Stop();
        }

        public void ProcessShooting(MiningDrill weapon)
        {
            ref var aimStats = ref weapon.AimStats;

            var beamTransform = weapon.BeamLineTransform;
            var startPosition = beamTransform.position;
            var weaponDirection = beamTransform.up;

            var hit = Physics2D.Raycast(startPosition, weaponDirection, aimStats.MaxDistance, weapon.HitLayers);

            Vector2 hitPos;

            if (hit)
            {
                Vector2 worldHitPos = hit.point;
                ref var runTime = ref weapon.WeaponStats.Runtime;

                if (runTime.NextHitTime < GameFlowSystem.CoreTime)
                {
                    var hitDelay = weapon.WeaponStats.Cached.HitDelay;
                    runTime.NextHitTime = GameFlowSystem.CoreTime + hitDelay;
                    var damage = weapon.Damage;
                    damage.Energy *= hitDelay;
                    damage.Kinetic *= hitDelay;
                    damage.Asteroid *= hitDelay;

                    EventBus.BeamHitAction?.Invoke(damage, hit.collider, worldHitPos);
                }

                hitPos = beamTransform.InverseTransformPoint(worldHitPos);
            }
            else
            {
                hitPos = weapon.WeaponStats.Cached.NoHitTargetPoint;
            }

            weapon.BeamLineLR.SetPosition(1, hitPos);
        }
    }
}