using DI;
using Ships;
using UnityEngine;

namespace GameSystems
{
    public class ShipVisualizeSystem : GameSystemBase
    {
        private ThrustersPower currentThrustersPower;

        private ShipInstance _playerShip;


        protected override void AwakeInit()
        {
        }

        protected override void Subscribe()
        {
            GameFlowSystem.UpdateTick += OnUpdateTick;
            EventBus.SpawnPlayerShip += OnSpawnPlayerShip;
        }

        protected override void Unsubscribe()
        {
            GameFlowSystem.UpdateTick -= OnUpdateTick;
            EventBus.SpawnPlayerShip -= OnSpawnPlayerShip;
        }

        private void OnSpawnPlayerShip(ShipInstance shipInstance)
        {
            _playerShip = shipInstance;
        }

        private void OnUpdateTick(float deltaTime)
        {
            ref var movementRuntimeData = ref _playerShip.MovementRuntimeData;
            ref var view = ref _playerShip.View;
            VisualizeMainEngine(movementRuntimeData, view);
            VisualizeThrusters(movementRuntimeData, view);
        }

        private void VisualizeMainEngine(in MovementRuntimeData movementVisualData, in View view)
        {
            foreach (var engine in view.MainEnginesPlumes)
            {
                engine.SetPowerValue(movementVisualData.DirectPower);
            }
        }

        private void VisualizeThrusters(in MovementRuntimeData movementRuntimeData, in View view)
        {
            var currentThrustersPower = new ThrustersPower();

            if (movementRuntimeData.StrafePower > 0)
            {
                currentThrustersPower.BackLeft = movementRuntimeData.StrafePower;
                currentThrustersPower.FrontLeft = movementRuntimeData.StrafePower;
            }
            else if (movementRuntimeData.StrafePower < 0)
            {
                currentThrustersPower.BackRight = -movementRuntimeData.StrafePower;
                currentThrustersPower.FrontRight = -movementRuntimeData.StrafePower;
            }

            if (movementRuntimeData.RotatePower > 0)
            {
                if (currentThrustersPower.BackLeft == 0) currentThrustersPower.BackLeft = movementRuntimeData.RotatePower;
                if (currentThrustersPower.FrontRight == 0) currentThrustersPower.FrontRight = movementRuntimeData.RotatePower;

            }
            else if (movementRuntimeData.RotatePower < 0)
            {
                if (currentThrustersPower.FrontLeft == 0) currentThrustersPower.FrontLeft = -movementRuntimeData.RotatePower;
                if (currentThrustersPower.BackRight == 0) currentThrustersPower.BackRight = -movementRuntimeData.RotatePower;
            }

            view.ThrusterFR.SetPowerValue(currentThrustersPower.FrontRight);
            view.ThrusterBR.SetPowerValue(currentThrustersPower.BackRight);
            view.ThrusterFL.SetPowerValue(currentThrustersPower.FrontLeft);
            view.ThrusterBL.SetPowerValue(currentThrustersPower.BackLeft);
        }
    }
}

public struct ThrustersPower
{
    public float FrontLeft;
    public float FrontRight;
    public float BackLeft;
    public float BackRight;
}

public static class SideEnginesPowerExtensions
{
    public static void Lerp(this ref ThrustersPower current, in ThrustersPower target, float t)
    {
        t = Mathf.Clamp01(t);
        float it = 1f - t;

        current.FrontLeft = current.FrontLeft * it + target.FrontLeft * t;
        current.FrontRight = current.FrontRight * it + target.FrontRight * t;
        current.BackLeft = current.BackLeft * it + target.BackLeft * t;
        current.BackRight = current.BackRight * it + target.BackRight * t;
    }
    public static void MoveTowards(ref this ThrustersPower current, in ThrustersPower target, float maxDelta)
    {
        current.FrontLeft = Mathf.MoveTowards(current.FrontLeft, target.FrontLeft, maxDelta);
        current.FrontRight = Mathf.MoveTowards(current.FrontRight, target.FrontRight, maxDelta);
        current.BackLeft = Mathf.MoveTowards(current.BackLeft, target.BackLeft, maxDelta);
        current.BackRight = Mathf.MoveTowards(current.BackRight, target.BackRight, maxDelta);
    }
}