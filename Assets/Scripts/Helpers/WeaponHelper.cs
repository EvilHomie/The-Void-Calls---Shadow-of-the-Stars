using Ships;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Helpers
{
    public class WeaponHelper
    {
        //public static void AimAtTarget(ShipInstance shipInstance, float dTime)
        //{
        //    Vector2 aimPos = shipInstance.AimData.AimPosition;

        //    foreach (var slot in shipInstance.WeaponSlots)
        //    {
        //        var weapon = slot.Weapon;
        //        ref readonly var aimStats = ref weapon.RuntimeAimStats;
        //        var weaponTransform = weapon.Transform;
        //        Vector2 weaponPosition = weaponTransform.position;
        //        Vector2 targetDir = aimPos - weaponPosition;

        //        var targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90f;
        //var currentWorldAngle = Mathf.DeltaAngle(0f, weaponTransform.eulerAngles.z);
        //        var newWorldAngle = Mathf.MoveTowardsAngle(currentWorldAngle, targetAngle, aimStats.RotateSpeed * dTime);
        //        var parentAngle = weaponTransform.parent.eulerAngles.z;
        //        var localAngle = Mathf.DeltaAngle(0f, newWorldAngle - parentAngle);
        //        localAngle = Mathf.Clamp(localAngle, -aimStats.MaxRotateAngle, aimStats.MaxRotateAngle);
        //        var finalWorldAngle = parentAngle + localAngle;
        //        weaponTransform.rotation = Quaternion.Euler(0f, 0f, finalWorldAngle);

        //        ref var shootPointData = ref weapon.ShootPointRuntimeData;
        //        shootPointData.Direction = weaponTransform.up;
        //        shootPointData.Position = weapon.ShootPointTransform.position;
        //    }
        //}

        //public static void AimAtTarget(ShipInstance shipInstance, float dTime)
        //{
        //    Vector2 aimPos = shipInstance.AimData.AimPosition;

        //    foreach (var slot in shipInstance.MainWeaponsSlots)
        //    {
        //        var weapon = slot.Weapon;

        //        if (weapon == null) continue;

        //        var weaponTransform = weapon.Transform;
        //        ref readonly var aimStats = ref weapon.RuntimeAimStats;
        //        var localTargetPos = weapon.SlotTransform.InverseTransformPoint(aimPos);

        //        var localTargetAngle = Vector2.SignedAngle(Vector2.up, localTargetPos);
        //        var maxAngle = aimStats.MaxRotateAngle;
        //        //localTargetAngle = Mathf.Clamp(localTargetAngle, -maxAngle, maxAngle);

        //        var newAngle = Mathf.MoveTowardsAngle(weapon.RotateAngle, localTargetAngle, aimStats.RotateSpeed * dTime);
        //        weapon.RotateAngle = newAngle;
        //        weaponTransform.localRotation = Quaternion.Euler(0, 0, newAngle);

        //        ref var shootPointData = ref weapon.ShootPointRuntimeData;
        //        shootPointData.Direction = weaponTransform.up;
        //        shootPointData.Position = weapon.ShootPointTransform.position;
        //    }
        //}

        public static void AimAtTarget(ShipInstance shipInstance, float dTime) // избавился от зависимости от слота
        {
            Vector2 aimPos = shipInstance.AimData.AimPosition;

            foreach (var slot in shipInstance.MainWeaponsSlots)
            {
                var weapon = slot.Weapon;

                if (weapon == null) continue;

                var weaponTransform = weapon.Transform;
                ref readonly var aimStats = ref weapon.RuntimeAimStats;
                Vector2 weaponPos = weaponTransform.position;
                var toTarget = aimPos - weaponPos;

                var angleDelta = Vector2.SignedAngle(weaponTransform.up, toTarget);
                var maxRotationDelta = aimStats.RotateSpeed * dTime;
                var rotationDelta = Mathf.Clamp(angleDelta, -maxRotationDelta, maxRotationDelta);

                var maxRotateAngle = aimStats.MaxRotateAngle;
                ref var rotateAngle = ref weapon.RotateAngle;
                rotateAngle += rotationDelta;
                rotateAngle = Mathf.Clamp(rotateAngle, -maxRotateAngle, maxRotateAngle);

                weaponTransform.localRotation = Quaternion.Euler(0, 0, rotateAngle);

                ref var shootPointData = ref weapon.ShootPointRuntimeData;
                shootPointData.Direction = weaponTransform.up;
                shootPointData.Position = weapon.ShootPointTransform.position;
            }
        }

        public static HitResult TryGetBeamHit(Vector2 startPos, Vector2 aimPos, HashSet<Collider2D> ignoredColliders)
        {
            var interceptHit = Physics2D.Linecast(startPos, aimPos, GameLayers.InterceptMask);

            if (interceptHit && !ignoredColliders.Contains(interceptHit.collider))
            {
                var collider = interceptHit.collider;
                var hitPoint = collider.OverlapPoint(aimPos) ? aimPos : interceptHit.point;

                return new HitResult(true, collider, hitPoint);
            }

            var defenseCollider = Physics2D.OverlapPoint(aimPos, GameLayers.DamageableMask);

            if (!defenseCollider || ignoredColliders.Contains(defenseCollider))
            {
                return new HitResult(false, null, aimPos);
            }

            return new HitResult(true, defenseCollider, aimPos);
        }

        public static HitResult TryGetProjectileHit(Vector2 currentPos, Vector2 nextPos, Vector2 aimPos, HashSet<Collider2D> ignoredColliders)
        {
            var interceptHit = Physics2D.Linecast(currentPos, nextPos, GameLayers.InterceptMask);

            if (!interceptHit || ignoredColliders.Contains(interceptHit.collider))
            {
                return HitResult.NoHit;
            }

            if (interceptHit.collider.OverlapPoint(aimPos))
            {
                return HitResult.NoHit;
            }

            return new HitResult(true, interceptHit.collider, interceptHit.point);
        }

        public static HitResult TryGetHitInPoint(Vector2 point, HashSet<Collider2D> ignoredColliders)
        {
            var defenseCollider = Physics2D.OverlapPoint(point, GameLayers.DamageableMask);
            var hasHit = defenseCollider && !ignoredColliders.Contains(defenseCollider);
            return new HitResult(hasHit, defenseCollider, point);
        }

        public readonly struct HitResult
        {
            public readonly bool HasHit;
            public readonly Collider2D Collider;
            public readonly Vector2 Point;

            public static readonly HitResult NoHit = new(false, null, Vector2.zero);
            public HitResult(bool hasHit, Collider2D collider, Vector2 point)
            {
                HasHit = hasHit;
                Collider = collider;
                Point = point;
            }
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

