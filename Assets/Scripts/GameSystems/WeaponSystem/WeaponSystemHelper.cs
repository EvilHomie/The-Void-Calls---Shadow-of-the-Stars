using UnityEngine;
using Weapons;

namespace Helper
{
    public class WeaponSystemHelper
    {
        public static void AimAtTarget(WeaponBase weapon, float dTime, in Vector3 targetPos)
        {
            Vector2 dir = targetPos - weapon.Transform.position;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

            float newAngle = Mathf.MoveTowardsAngle(
                weapon.Transform.eulerAngles.z,
                targetAngle,
                weapon.RotateSpeed * dTime
            );

            weapon.Transform.rotation = Quaternion.Euler(0, 0, newAngle);

            float localZ = Mathf.DeltaAngle(0, weapon.Transform.localEulerAngles.z);

            if (Mathf.Abs(localZ) > weapon.MaxRotateAngle)
            {
                float clamped = Mathf.Clamp(localZ, -weapon.MaxRotateAngle, weapon.MaxRotateAngle);
                weapon.Transform.localRotation = Quaternion.Euler(0, 0, clamped);
            }
        }

        public static bool CanFire(WeaponBase weapon, LayerMask hitLayers)
        {
            Ray ray = new(weapon.Transform.position, weapon.Transform.up);
            return Physics.Raycast(ray, weapon.MaxDistance, hitLayers);
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

