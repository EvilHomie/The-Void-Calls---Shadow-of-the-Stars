using DefenseLayers;
using CoreGameSystems;
using UnityEngine;

namespace Weapons
{
    public class MiningDrillBehaviour : IWeaponBehaviour<ConstantBeamWeapon>
    {
        public void HandleStartShoot(ConstantBeamWeapon weapon)
        {
            weapon.BeamLineLR.enabled = true;
            weapon.ShootSpotPS.Play();
            ProcessShooting(weapon);
        }

        public void HandleCancelShoot(ConstantBeamWeapon weapon)
        {
            weapon.BeamLineLR.enabled = false;
            weapon.ShootSpotPS.Stop();
        }

        public void ProcessShooting(ConstantBeamWeapon weapon)
        {
            ref readonly var aimStats = ref weapon.RuntimeAimStats;
            var nextHitTime = weapon.NextHitTime;
            ref readonly var shootPointData = ref weapon.ShootPointTransformData;
            var aimPosition = weapon.AimData.AimPosition;

            Vector3 spawnLinePos = shootPointData.Position;
            spawnLinePos.z = shootPointData.ZDepth;
            var distanceToAimPosition = Vector2.Distance(aimPosition, spawnLinePos);
            var aimDistance = Mathf.Min(aimStats.MaxDistance, distanceToAimPosition);

            Vector3 worldHitPos = spawnLinePos + (Vector3)shootPointData.Direction * aimDistance;
            weapon.BeamLineLR.SetPosition(0, spawnLinePos);
            weapon.BeamLineLR.SetPosition(1, worldHitPos);

            if (GameFlowSystem.CoreTime < nextHitTime) return;

            var hit = Physics2D.OverlapPoint(worldHitPos, weapon.HitLayers);

            if (hit == null || weapon.IgnoredColliders.Contains(hit)) return;

            var hitDelay = weapon.HitDelay;
            weapon.NextHitTime = GameFlowSystem.CoreTime + hitDelay;

            var damage = weapon.RuntimeDamage;
            damage.DamageHull *= hitDelay;
            damage.DamageArmor *= hitDelay;
            damage.DamageShield *= hitDelay;
            damage.DamageAsteroid *= hitDelay;

            var hitData = new HitData
            {
                Position = worldHitPos,
                Size = weapon.Size
            };

            hit.TryGetComponent(out DefenseLayerBase defenceLayer);
            EventBus.HitAction?.Invoke(damage, defenceLayer, hitData);
        }
    }
}