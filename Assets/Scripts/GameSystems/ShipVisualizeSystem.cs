using DI;
using Registries;
using Ships;
using UnityEngine;

namespace GameSystems
{
    public class ShipVisualizeSystem : GameSystemBase, IPreUpdateTickObserver
    {
        private ShipRegistry _shipRegistry;

        [Inject]
        public void Construct(ShipRegistry shipRegystry)
        {
            _shipRegistry = shipRegystry;
            ActiveGameState = GameState.CoreGameplay;
        }

        public void PreUpdateTick()
        {
            if (!SystemIsActive) return;

            Visualize();
        }        

        private void Visualize()
        {
            var playerShip = _shipRegistry.PlayerShip;
            ref var movementRuntimeData = ref playerShip.MovementRuntimeData;
            ref var view = ref playerShip.View;
            VisualizeMainEngine(movementRuntimeData, view);
            VisualizeThrusters(movementRuntimeData, view);
            VisualizeBoosters(movementRuntimeData, view);
        }

        private void VisualizeMainEngine(in MovementRuntimeData movementVisualData, in View view)
        {
            foreach (var engine in view.MainEnginesPlumes)
            {
                engine.SetPowerValue(movementVisualData.MainEnginePower);
            }
        }

        private void VisualizeThrusters(in MovementRuntimeData movementRuntimeData, in View view)
        {
            var currentThrustersPower = new ThrustersPower();

            if (movementRuntimeData.ThrustersPower > 0)
            {
                currentThrustersPower.BackLeft = movementRuntimeData.ThrustersPower;
                currentThrustersPower.FrontLeft = movementRuntimeData.ThrustersPower;
            }
            else if (movementRuntimeData.ThrustersPower < 0)
            {
                currentThrustersPower.BackRight = -movementRuntimeData.ThrustersPower;
                currentThrustersPower.FrontRight = -movementRuntimeData.ThrustersPower;
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

        private void VisualizeBoosters(in MovementRuntimeData movementVisualData, in View view)
        {
            foreach (var booster in view.BoostersPlumes)
            {
                booster.SetPowerValue(movementVisualData.BoostersIsActive ? 1 : 0);
            }
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