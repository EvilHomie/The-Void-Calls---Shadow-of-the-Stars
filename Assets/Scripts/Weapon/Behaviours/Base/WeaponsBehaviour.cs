using System;
using System.Collections.Generic;
using Weapons;

namespace Weapons
{
    public class WeaponsBehaviour
    {
        private readonly Dictionary<WeaponType, WeaponInvoker> _invokers;

        public WeaponsBehaviour()
        {
            _invokers = new();
            RegisterInvokers();
        }

        public void StartShoot(WeaponBase weapon)
            => _invokers[weapon.WeaponType].StartShoot(weapon);

        public void CancelShoot(WeaponBase weapon)
            => _invokers[weapon.WeaponType].CancelShoot(weapon);

        public void ProceedShoot(WeaponBase weapon)
            => _invokers[weapon.WeaponType].ProceedShoot(weapon);

        private void RegisterInvokers()
        {
            _invokers[WeaponType.MiningDrill] = CreateInvoker(new MiningDrillBehaviour());
            _invokers[WeaponType.BoltRepeater] = CreateInvoker(new BoltRepeaterBehaviour());
        }

        private WeaponInvoker CreateInvoker<TWeapon>(IWeaponBehaviour<TWeapon> behaviour) where TWeapon : WeaponBase
        {
            return new WeaponInvoker(
                w => behaviour.StartShoot((TWeapon)w),
                w => behaviour.CancelShoot((TWeapon)w),
                w => behaviour.ProceedShoot((TWeapon)w)
            );
        }
    }

    public interface IWeaponBehaviour<TWeapon> where TWeapon : WeaponBase
    {
        void StartShoot(TWeapon weapon);
        void CancelShoot(TWeapon weapon);
        void ProceedShoot(TWeapon weapon);
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

        public void StartShoot(WeaponBase w) => _start(w);
        public void CancelShoot(WeaponBase w) => _cancel(w);
        public void ProceedShoot(WeaponBase w) => _proceed(w);
    }
}
