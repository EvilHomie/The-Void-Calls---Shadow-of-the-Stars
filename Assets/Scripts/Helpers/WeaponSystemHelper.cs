using Ships;
using UnityEngine;
using Weapons;

namespace Helpers
{
    public class WeaponSystemHelper
    {
        public static void AimAtTarget(WeaponBase weapon, float dTime, in Vector3 targetPos)
        {
            ref var baseStats = ref weapon.BaseStats;
            Vector2 dir = targetPos - weapon.Transform.position;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

            float newAngle = Mathf.MoveTowardsAngle(
                weapon.Transform.eulerAngles.z,
                targetAngle,
                baseStats.RotateSpeed * dTime
            );

            weapon.Transform.rotation = Quaternion.Euler(0, 0, newAngle);

            float localZ = Mathf.DeltaAngle(0, weapon.Transform.localEulerAngles.z);

            if (Mathf.Abs(localZ) > baseStats.MaxRotateAngle)
            {
                float clamped = Mathf.Clamp(localZ, -baseStats.MaxRotateAngle, baseStats.MaxRotateAngle);
                weapon.Transform.localRotation = Quaternion.Euler(0, 0, clamped);
            }
        }

        public static void AimAtTarget(ShipInstance shipInstance, float dTime)
        {
            Vector3 targetPos = shipInstance.TargetData.Position;

            foreach (var slot in shipInstance.WeaponSlots)
            {
                var weapon = slot.Weapon;
                ref var baseStats = ref weapon.BaseStats;
                var transform = weapon.Transform;
                var maxRotateAngle = baseStats.MaxRotateAngle;

                Vector3 targetDir = targetPos - transform.position;
                float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90f;

                float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, baseStats.RotateSpeed * dTime);

                transform.rotation = Quaternion.Euler(0, 0, newAngle);

                float localZ = Mathf.DeltaAngle(0, transform.localEulerAngles.z);

                if (Mathf.Abs(localZ) > maxRotateAngle)
                {
                    float clamped = Mathf.Clamp(localZ, -maxRotateAngle, maxRotateAngle);
                    transform.localRotation = Quaternion.Euler(0, 0, clamped);
                }
            }
        }

        public static bool CanFire(WeaponBase weapon, LayerMask hitLayers)
        {
            ref var baseStats = ref weapon.BaseStats;
            Ray ray = new(weapon.Transform.position, weapon.Transform.up);
            return Physics.Raycast(ray, baseStats.MaxDistance, hitLayers);
        }

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

