using System;
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
        
        public static WeaponInvoker CreateInvoker(WeaponType type, object behaviour)
        {
            switch (type)
            {
                case WeaponType.MiningDrill:
                    var drillBehaviour = (IWeaponBehaviour<MiningDrill>)behaviour;
                    return new WeaponInvoker(
                        w => drillBehaviour.StartShoot((MiningDrill)w),
                        w => drillBehaviour.CancelShoot((MiningDrill)w),
                        (w, dt) => drillBehaviour.ProceedShoot((MiningDrill)w, dt)
                    );

                // case WeaponType.PlasmaRifle: ...

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public class WeaponInvoker
    {
        private readonly Action<WeaponBase> _start;
        private readonly Action<WeaponBase> _cancel;
        private readonly Action<WeaponBase, float> _proceed;

        public WeaponInvoker(Action<WeaponBase> start, Action<WeaponBase> cancel, Action<WeaponBase, float> proceed)
        {
            _start = start;
            _cancel = cancel;
            _proceed = proceed;
        }

        public void StartShoot(WeaponBase w) => _start(w);
        public void CancelShoot(WeaponBase w) => _cancel(w);
        public void ProceedShoot(WeaponBase w, float dTime) => _proceed(w, dTime);
    }
}

