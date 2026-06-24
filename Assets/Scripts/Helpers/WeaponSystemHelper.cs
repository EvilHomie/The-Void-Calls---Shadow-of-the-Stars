using Ships;
using UnityEngine;

namespace Helpers
{
    public class WeaponSystemHelper
    {
        public static void AimAtTarget(ShipInstance shipInstance, float dTime)
        {
            Vector2 aimPos = shipInstance.AimData.AimPosition;

            foreach (var slot in shipInstance.WeaponSlots)
            {
                var weapon = slot.Weapon;
                ref readonly var aimStats = ref weapon.RuntimeAimStats;
                var weaponTransform = weapon.WeaponTransform;
                Vector2 weaponPosition = weaponTransform.position;
                Vector2 targetDir = aimPos - weaponPosition;

                var targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90f;
                var currentWorldAngle = Mathf.DeltaAngle(0f, weaponTransform.eulerAngles.z);
                var worldAngle = Mathf.MoveTowardsAngle(currentWorldAngle, targetAngle, aimStats.RotateSpeed * dTime);
                var parentAngle = weaponTransform.parent.eulerAngles.z;
                var localAngle = Mathf.DeltaAngle(0f, worldAngle - parentAngle);
                localAngle = Mathf.Clamp(localAngle, -aimStats.MaxRotateAngle, aimStats.MaxRotateAngle);
                var finalWorldAngle = parentAngle + localAngle;
                weaponTransform.rotation = Quaternion.Euler(0f, 0f, finalWorldAngle);

                ref var shootPointData = ref weapon.ShootPointTransformData;
                shootPointData.Direction = weaponTransform.up;
                shootPointData.Position = weapon.ShootPointTransform.position;
            }
        }



        //public static bool CanFire(WeaponBase weapon, LayerMask hitLayers)
        //{
        //    ref var aimStats = ref weapon.AimStats;
        //    Ray ray = new(weapon.Transform.position, weapon.Transform.up);
        //    return Physics.Raycast(ray, aimStats.MaxDistance, hitLayers);
        //}

        public static Vector2 GetDirectionWithSpreadBrookTaylor(Vector2 baseDir, float spreadAngleDeg)
        {
            if (spreadAngleDeg <= 0f)
                return baseDir;

            float r = (Random.value * 2f - 1f);
            float angleRad = r * spreadAngleDeg * Mathf.Deg2Rad;

            return new Vector2(
                baseDir.x * 1 - baseDir.y * angleRad,
                baseDir.x * angleRad + baseDir.y
            );
        }

        public static Vector2 GetDirectionWithSpreadQuaternion(Vector2 baseDir, float spreadAngleDeg)
        {
            if (spreadAngleDeg <= 0f)
                return baseDir;

            var randomAngle = Random.Range(-spreadAngleDeg, spreadAngleDeg);

            return Quaternion.Euler(0, 0, randomAngle) * baseDir;
        }

        public static Vector2 GetDirectionWithSpreadTrig(Vector2 baseDir, float spreadAngleDeg)
        {
            if (spreadAngleDeg <= 0f)
                return baseDir;

            float angle = Random.Range(-spreadAngleDeg, spreadAngleDeg) * Mathf.Deg2Rad;

            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            return new Vector2(
                baseDir.x * cos - baseDir.y * sin,
                baseDir.x * sin + baseDir.y * cos
            );
        }
    }
}

