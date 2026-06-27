using CoreGameSystems;
using General;
using Helpers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Weapons
{
    public class MiningDrillStrategy : IWeaponBehaviour<ConstantBeamWeapon>
    {
        private HitRegistrationSystem _hitRegistrationSystem;

        public MiningDrillStrategy(HitRegistrationSystem hitRegistrationSystem)
        {
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

            var startLinePos = shootPointData.Position;
            var distanceToAimPosition = Vector2.Distance(aimPosition, startLinePos);
            distanceToAimPosition = Mathf.Min(aimStats.MaxDistance, distanceToAimPosition);

            var endLinePos = startLinePos + shootPointData.Direction * distanceToAimPosition;

            var hitResult = WeaponHelper.TryGetBeamHit(startLinePos, endLinePos, weapon.IgnoredColliders);

            weapon.BeamLineLR.SetPosition(0, startLinePos);
            weapon.BeamLineLR.SetPosition(1, hitResult.Point);

            var coreTime = GameFlowSystem.CoreTime;

            if (coreTime < nextHitTime || !hitResult.HasHit) return;

            var hitDelay = weapon.HitDelay;
            weapon.NextHitTime = coreTime + hitDelay;

            _hitRegistrationSystem.RegisterHitDynamic(hitResult, weapon.Size, weapon.RuntimeDamage, hitDelay);
        }
    }
}