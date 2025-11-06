using UnityEngine;
using Weapon;

namespace Helper
{
    public class WeaponSystemHelper
    {
        public static void AimAtTarget(WeaponBase weapon, float dTime, in Vector3 targetPos)
        {
            Vector2 dir = targetPos - weapon.CTransform.position;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

            float newAngle = Mathf.MoveTowardsAngle(
                weapon.CTransform.eulerAngles.z,
                targetAngle,
                weapon.RotateSpeed * dTime
            );

            weapon.CTransform.rotation = Quaternion.Euler(0, 0, newAngle);

            float localZ = Mathf.DeltaAngle(0, weapon.CTransform.localEulerAngles.z);

            if (Mathf.Abs(localZ) > weapon.MaxRotateAngle)
            {
                float clamped = Mathf.Clamp(localZ, -weapon.MaxRotateAngle, weapon.MaxRotateAngle);
                weapon.CTransform.localRotation = Quaternion.Euler(0, 0, clamped);
            }
        }

        public static bool CanFire(WeaponBase weapon, LayerMask hitLayers)
        {
            Ray ray = new(weapon.CTransform.position, weapon.CTransform.up);
            return Physics.Raycast(ray, weapon.MaxDistance, hitLayers);
        }    
    }
}

