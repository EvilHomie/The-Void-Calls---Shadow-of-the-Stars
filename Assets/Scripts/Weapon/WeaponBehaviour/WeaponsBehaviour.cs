using System;
using System.Collections.Generic;
using Weapons;

namespace GameSystems
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

        public void ProceedShoot(WeaponBase weapon, float dTime)
            => _invokers[weapon.WeaponType].ProceedShoot(weapon, dTime);

        private void RegisterInvokers()
        {
            MiningDrillBehaviour miningDrillBehaviour = new();
            var miningDrillInvoker = CreateInvoker(WeaponType.MiningDrill, miningDrillBehaviour);
            _invokers[WeaponType.MiningDrill] = miningDrillInvoker;
            BoltRepeaterBehaviour boltRepeaterBehaviour = new();
            var boltRepeaterInvoker = CreateInvoker(WeaponType.BoltRepeater, boltRepeaterBehaviour);
            _invokers[WeaponType.BoltRepeater] = boltRepeaterInvoker;
        }

        private WeaponInvoker CreateInvoker(WeaponType type, object behaviour)
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

                case WeaponType.BoltRepeater:
                    var BoltRepeaterBehaviour = (IWeaponBehaviour<BoltRepeater>)behaviour;
                    return new WeaponInvoker(
                        w => BoltRepeaterBehaviour.StartShoot((BoltRepeater)w),
                        w => BoltRepeaterBehaviour.CancelShoot((BoltRepeater)w),
                        (w, dt) => BoltRepeaterBehaviour.ProceedShoot((BoltRepeater)w, dt)
                    );

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
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
