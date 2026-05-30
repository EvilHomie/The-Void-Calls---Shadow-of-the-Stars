using DefenseLayers;
using GameSystems;
using UnityEngine;

namespace Weapons
{
    public class MiningDrillBehaviour : IWeaponBehaviour<MiningDrill>
    {
        private Vector2 _vector2Up = Vector2.up;
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
            ref var data = ref weapon.RuntimeData;
            ref var shootPointData = ref weapon.ShootPointData;
            var aimData = weapon.AimData;

            var distanceToAimPosition = Vector2.Distance(aimData.AimPosition, shootPointData.Position);
            var aimDistance = Mathf.Min(aimStats.MaxDistance, distanceToAimPosition);
            var localHitPos = _vector2Up * aimDistance;
            weapon.BeamLineLR.SetPosition(1, localHitPos);

            if (GameFlowSystem.CoreTime < data.NextHitTime) return;

            Vector2 worldHitPos = weapon.ShootPoint.TransformPoint(localHitPos);
            var hit = Physics2D.OverlapPoint(worldHitPos, weapon.HitLayers);

            if (hit == null || weapon.IgnoredColliders.Contains(hit)) return;
            
            var hitDelay = data.HitDelay;
            data.NextHitTime = GameFlowSystem.CoreTime + hitDelay;

            var damage = weapon.DamageData;
            damage.Energy *= hitDelay;
            damage.Kinetic *= hitDelay;
            damage.Asteroid *= hitDelay;

            hit.TryGetComponent(out DefenseLayerBase defenceLayer);
            EventBus.BeamHitAction?.Invoke(damage, defenceLayer, worldHitPos);
        }
    }
}