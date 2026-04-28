//using GameSystems;
//using Registries;
//using System.Collections.Generic;
//using System.Diagnostics;
//using UnityEngine;
//using WeaponBehaviours;
//using Weapons;

//public class WeaponBenchmarkRunner : MonoBehaviour
//{
//    [SerializeField] private int weaponsCount = 200;
//    [SerializeField] private int warmupIterations = 2000;
//    [SerializeField] private int benchmarkIterations = 200_000;

//    private WeaponsBehaviour _weaponsBehaviour;
//    private List<WeaponBase> _weapons;

//    private void Start()
//    {
//        Setup();
//        RunBenchmark();
//    }

//    private void Setup()
//    {
//        _weaponsBehaviour = new WeaponsBehaviour();
//        _weapons = new List<WeaponBase>(weaponsCount);

//        // создаём реальные MonoBehaviour объекты
//        for (int i = 0; i < weaponsCount; i++)
//        {
//            if (i % 2 == 0)
//            {
//                var go = new GameObject("Drill");
//                var w = go.AddComponent<DummyMiningDrill>();
//                //w.WeaponType = WeaponType.MiningDrill;
//                _weapons.Add(w);
//            }
//            else
//            {
//                var go = new GameObject("Repeater");
//                var w = go.AddComponent<DummyBoltRepeater>();
//                //w.WeaponType = WeaponType.BoltRepeater;
//                _weapons.Add(w);
//            }
//        }
//    }

//    private void RunBenchmark()
//    {
//        // 🔥 прогрев
//        for (int i = 0; i < warmupIterations; i++)
//        {
//            for (int j = 0; j < _weapons.Count; j++)
//            {
//                _weaponsBehaviour.ProceedShoot(_weapons[j]);
//            }
//        }

//        var sw = Stopwatch.StartNew();

//        for (int i = 0; i < benchmarkIterations; i++)
//        {
//            for (int j = 0; j < _weapons.Count; j++)
//            {
//                _weaponsBehaviour.ProceedShoot(_weapons[j]);
//            }
//        }

//        sw.Stop();

//        long totalCalls = (long)benchmarkIterations * _weapons.Count;
//        double nsPerCall = (sw.Elapsed.TotalMilliseconds * 1_000_000) / totalCalls;

//        UnityEngine.Debug.Log(
//            $"=== BENCH RESULT ===\n" +
//            $"Weapons: {_weapons.Count}\n" +
//            $"Total calls: {totalCalls}\n" +
//            $"Total time: {sw.Elapsed.TotalMilliseconds:F3} ms\n" +
//            $"Per call: {nsPerCall:F2} ns"
//        );
//    }
//}

//public class DummyMiningDrill : WeaponBase
//{
//    public override WeaponType WeaponType => WeaponType.DummyMiningDrill;

//    public override void Init()
//    {
//    }
//}
//public class DummyBoltRepeater : WeaponBase
//{
//    public override WeaponType WeaponType => WeaponType.DummyBoltRepeater;

//    public override void Init()
//    {
//    }
//}

//public class DummyMiningDrillBehaviour : IWeaponBehaviour<DummyMiningDrill>
//{
//    public void StartShoot(DummyMiningDrill weapon)
//    {
//    }

//    public void CancelShoot(DummyMiningDrill weapon)
//    {
//    }

//    public void ProceedShoot(DummyMiningDrill weapon)
//    {
//    }
//}

//public class DummyBoltRepeaterBehaviour : IWeaponBehaviour<DummyBoltRepeater>
//{
//    public void StartShoot(DummyBoltRepeater weapon)
//    {
//    }

//    public void CancelShoot(DummyBoltRepeater weapon)
//    {
//    }

//    public void ProceedShoot(DummyBoltRepeater weapon)
//    {
//    }
//}

///*тестовые куски в WeaponsBehaviour но текущая реализация быстрее
// * 
// * добавить в WeaponType
//WeaponType.DummyBoltRepeater
//WeaponType.DummyMiningDrill

//    //изменить регистрацию вот так
//private void RegisterInvokers()
//{
//    _invokers[WeaponType.MiningDrill] = new WeaponInvoker<MiningDrill>(new MiningDrillBehaviour());
//    _invokers[WeaponType.BoltRepeater] = new WeaponInvoker<BoltRepeater>(new BoltRepeaterBehaviour());

//    _invokers[WeaponType.DummyBoltRepeater] = new WeaponInvoker<DummyBoltRepeater>(new DummyBoltRepeaterBehaviour());
//    _invokers[WeaponType.DummyMiningDrill] = new WeaponInvoker<DummyMiningDrill>(new DummyMiningDrillBehaviour());
//}

////заменить инвокеры на это

//public abstract class WeaponInvoker
//{
//    public abstract void StartShoot(WeaponBase w);
//    public abstract void CancelShoot(WeaponBase w);
//    public abstract void ProceedShoot(WeaponBase w);
//}

//public class WeaponInvoker<TWeapon> : WeaponInvoker
//where TWeapon : WeaponBase
//{
//    private readonly IWeaponBehaviour<TWeapon> _behaviour;

//    public WeaponInvoker(IWeaponBehaviour<TWeapon> behaviour)
//    {
//        _behaviour = behaviour;
//    }

//    public override void StartShoot(WeaponBase w) => _behaviour.StartShoot((TWeapon)w);
//    public override void CancelShoot(WeaponBase w) => _behaviour.CancelShoot((TWeapon)w);
//    public override void ProceedShoot(WeaponBase w) => _behaviour.ProceedShoot((TWeapon)w);
//}
//*/