using Ships;
using System.Collections.Generic;
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

        public static void AimAtTarget(ShipInstance shipInstance, float dTime)
        {
            Vector2 aimPos = shipInstance.AimData.AimPosition;

            foreach (var slot in shipInstance.WeaponSlots)
            {
                var weapon = slot.Weapon;

                if(weapon == null) continue;

                var weaponTransform = weapon.Transform;
                ref readonly var aimStats = ref weapon.RuntimeAimStats;
                var localTargetPos = weapon.SlotTransform.InverseTransformPoint(aimPos);

                var localTargetAngle = Vector2.SignedAngle(Vector2.up, localTargetPos);
                var maxAngle = aimStats.MaxRotateAngle;
                localTargetAngle = Mathf.Clamp(localTargetAngle, -maxAngle, maxAngle);

                var newAngle = Mathf.MoveTowardsAngle(weapon.RotateAngle, localTargetAngle, aimStats.RotateSpeed * dTime);
                weapon.RotateAngle = newAngle;
                weaponTransform.localRotation = Quaternion.Euler(0, 0, newAngle);

                ref var shootPointData = ref weapon.ShootPointRuntimeData;
                shootPointData.Direction = weaponTransform.up;
                shootPointData.Position = weapon.ShootPointTransform.position;
            }
        }

        //public static HitResult TryGetBeamHit(Vector2 startPos, Vector2 aimPos, HashSet<Collider2D> ignoredColliders)
        //{
        //    Collider2D defenseCollider;
        //    bool hasHit;
        //    Vector2 hitPoint;

        //    var interceptedHit = Physics2D.Linecast(startPos, aimPos, LayersId.AsteroidsMask);

        //    if (interceptedHit)
        //    {
        //        hasHit = true;
        //        defenseCollider = interceptedHit.collider;
        //        hitPoint = defenseCollider.OverlapPoint(aimPos) ? aimPos : interceptedHit.point;
        //    }
        //    else
        //    {
        //        defenseCollider = Physics2D.OverlapPoint(aimPos, LayersId.DefenseMask);
        //        hitPoint = aimPos;
        //        hasHit = defenseCollider && !ignoredColliders.Contains(defenseCollider);
        //    }

        //    return new HitResult(hasHit, defenseCollider, hitPoint);
        //}

        public static HitResult TryGetBeamHit(Vector2 startPos, Vector2 aimPos, HashSet<Collider2D> ignoredColliders)
        {
            var interceptedHit = Physics2D.Linecast(startPos, aimPos, GameLayers.InterceptMask);

            if (interceptedHit)
            {
                var collider = interceptedHit.collider;
                var hitPoint = collider.OverlapPoint(aimPos) ? aimPos : interceptedHit.point;

                return new HitResult(true, collider, hitPoint);
            }

            var defenseCollider = Physics2D.OverlapPoint(aimPos, GameLayers.ShipMask);

            if (!defenseCollider || ignoredColliders.Contains(defenseCollider))
            {
                return new HitResult(false, null, aimPos);
            }

            return new HitResult(true, defenseCollider, aimPos);
        }

        //public static HitResult TryGetProjectileHit(Vector2 currentPos, Vector2 nextPos, Vector2 aimPos)
        //{
        //    Collider2D defenseCollider;
        //    bool hasHit;
        //    Vector2 hitPoint;
        //    var interceptedHit = Physics2D.Linecast(currentPos, nextPos, LayersId.AsteroidsMask);            

        //    if (interceptedHit)
        //    {
        //        defenseCollider = interceptedHit.collider;
        //        hasHit = !defenseCollider.OverlapPoint(aimPos);
        //        hitPoint = interceptedHit.point;
        //    }
        //    else
        //    {
        //        hasHit = false;
        //        defenseCollider = null;
        //        hitPoint = Vector2.zero;
        //    }

        //    return new HitResult(hasHit, defenseCollider, hitPoint);
        //}

        public static HitResult TryGetProjectileHit(Vector2 currentPos, Vector2 nextPos, Vector2 aimPos)
        {
            var hit = Physics2D.Linecast(currentPos, nextPos, GameLayers.InterceptMask);

            if (!hit)
            {
                return HitResult.NoHit;
            }

            if (hit.collider.OverlapPoint(aimPos))
            {
                return HitResult.NoHit;
            }

            return new HitResult(true, hit.collider, hit.point);
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


        // первая версия
        /*
        private bool IsHitOther(Vector2 startPos, Vector2 endPos, HashSet<Collider2D> ignoredColliders, out Vector2 hitPos, out Collider2D hitedCollider)
        { 
            var asteroidHit = Physics2D.Linecast(startPos, endPos, LayersId.AsteroidsMask);
            hitedCollider = asteroidHit.collider;

            if (hitedCollider != null) 
            { 
                hitPos = hitedCollider.OverlapPoint(endPos) ? endPos : asteroidHit.point;
                return true;
            }

            hitedCollider = Physics2D.OverlapPoint(endPos, LayersId.DefenseMask);
            hitPos = endPos;

            if (!hitedCollider || ignoredColliders.Contains(hitedCollider))
            { 
                return false;
            } 
            else
            { 
                return true;
            } 
        }
        */


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

