using System;
using System.Collections.Generic;

namespace Weapons
{
    public class WeaponsStrategy
    {
        private readonly Dictionary<WeaponType, WeaponInvoker> _invokers;

        public WeaponsStrategy()
        {
            _invokers = new();
            RegisterInvokers();
        }

        public void HandleStartShoot(WeaponBase weapon)
            => _invokers[weapon.WeaponType].HandleStartShoot(weapon);

        public void HandleCancelShoot(WeaponBase weapon)
            => _invokers[weapon.WeaponType].HandleCancelShoot(weapon);

        public void ProcessShooting(WeaponBase weapon)
            => _invokers[weapon.WeaponType].ProcessShooting(weapon);

        private void RegisterInvokers()
        {
            _invokers[WeaponType.MiningDrill] = CreateInvoker(new MiningDrillStrategy());
            _invokers[WeaponType.BoltRepeater] = CreateInvoker(new BoltRepeaterStrategy());
        }

        private WeaponInvoker CreateInvoker<TWeapon>(IWeaponBehaviour<TWeapon> behaviour) where TWeapon : WeaponBase
        {
            return new WeaponInvoker(
                w => behaviour.HandleStartShoot((TWeapon)w),
                w => behaviour.HandleCancelShoot((TWeapon)w),
                w => behaviour.ProcessShooting((TWeapon)w)
            );
        }
    }

    public interface IWeaponBehaviour<TProjectile> where TProjectile : WeaponBase
    {
        void HandleStartShoot(TProjectile weapon);
        void HandleCancelShoot(TProjectile weapon);
        void ProcessShooting(TProjectile weapon);
    }

    public class WeaponInvoker
    {
        private readonly Action<WeaponBase> _start;
        private readonly Action<WeaponBase> _cancel;
        private readonly Action<WeaponBase> _proceed;

        public WeaponInvoker(Action<WeaponBase> start, Action<WeaponBase> cancel, Action<WeaponBase> proceed)
        {
            _start = start;
            _cancel = cancel;
            _proceed = proceed;
        }

        public void HandleStartShoot(WeaponBase w) => _start(w);
        public void HandleCancelShoot(WeaponBase w) => _cancel(w);
        public void ProcessShooting(WeaponBase w) => _proceed(w);
    }
}
