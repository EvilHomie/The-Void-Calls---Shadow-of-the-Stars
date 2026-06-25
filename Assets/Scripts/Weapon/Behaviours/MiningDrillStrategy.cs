using CoreGameSystems;
using DI;
using UnityEngine;

namespace Weapons
{
    public class MiningDrillStrategy : IWeaponBehaviour<ConstantBeamWeapon>
    {
        private HitRegistrationSystem _hitRegistrationSystem;

        public MiningDrillStrategy() { Debug.LogError(1); }

        [Inject]
        public MiningDrillStrategy(HitRegistrationSystem hitRegistrationSystem)
        {
            Debug.LogError(2);
            _hitRegistrationSystem = hitRegistrationSystem;
        }

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

            var collider = Physics2D.OverlapPoint(worldHitPos, weapon.HitLayers);

            if (collider == null || weapon.IgnoredColliders.Contains(collider)) return;

            var hitDelay = weapon.HitDelay;
            weapon.NextHitTime = GameFlowSystem.CoreTime + hitDelay;

            _hitRegistrationSystem.RegisterHitDynamic(collider, worldHitPos, weapon.Size, weapon.RuntimeDamage, hitDelay);
        }
    }
}