using DefenseLayers;
using GameSystems;
using UnityEngine;

namespace Weapons
{
    public class MiningDrillBehaviour : IWeaponBehaviour<MiningDrill>
    {
        private static readonly RaycastHit2D[] _beamHits = new RaycastHit2D[16];
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

            int hitCount = Physics2D.RaycastNonAlloc(startPosition, weaponDirection, _beamHits, aimStats.MaxDistance, weapon.HitLayers);
            DefenseLayerBase hitLayer = null;
            Vector2 hitPos = weapon.WeaponStats.Cached.NoHitTargetPoint;

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _beamHits[i];

                if (!hit.collider.TryGetComponent(out DefenseLayerBase layer)) continue;
                if (layer.OwnerId == weapon.OwnerId) continue;

                hitPos = hit.point;
                hitLayer = layer;
                break;
            }


            if (hitLayer == null)
            {
                weapon.BeamLineLR.SetPosition(1, hitPos);
                return;
            }

            ref var runTime = ref weapon.WeaponStats.Runtime;

            if (runTime.NextHitTime < GameFlowSystem.CoreTime)
            {
                var hitDelay = weapon.WeaponStats.Cached.HitDelay;
                runTime.NextHitTime = GameFlowSystem.CoreTime + hitDelay;
                var damage = weapon.Damage;
                damage.Energy *= hitDelay;
                damage.Kinetic *= hitDelay;
                damage.Asteroid *= hitDelay;

                EventBus.BeamHitAction?.Invoke(damage, hitLayer, hitPos);
            }

            Vector2 localHitPos = beamTransform.InverseTransformPoint(hitPos);
            weapon.BeamLineLR.SetPosition(1, localHitPos);
        }
    }
}