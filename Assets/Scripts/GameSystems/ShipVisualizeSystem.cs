using DI;
using Ships;
using UnityEngine;

namespace GameSystems
{
    public class ShipVisualizeSystem : GameSystemBase
    {
        private SideEnginesPower _currentThrustersPower;
        private ShipsDataStorage _objectsStorage;

        [Inject]
        public void Construct(ShipsDataStorage objectsStorage)
        {
            _objectsStorage = objectsStorage;
        }

        protected override void AwakeInit()
        {
        }

        protected override void Subscribe()
        {
            GameFlowSystem.UpdateTick += OnUpdateTick;
        }

        protected override void Unsubscribe()
        {
            GameFlowSystem.UpdateTick -= OnUpdateTick;
        }

        private void OnUpdateTick(float deltaTime)
        {

            for (int i = 0; i <= _objectsStorage.LastUsedIndex; i++)
            {
                ref var movementData = ref _objectsStorage.MovementRuntimeDatas[i];
                ref var viewData = ref _objectsStorage.Views[i];

                VisualizeDirectMove(movementData, viewData);
                CalcSideEnginesPower(movementData);
                VisualizeSideEngines(viewData);
            }
        }

        private void VisualizeDirectMove(in MovementData movementData, in ViewData shipView)
        {
            foreach (var engine in shipView.MainEnginesPlumes)
            {
                engine.SetPowerValue(movementData.DirectMovePower);
            }
        }

        private void CalcSideEnginesPower(in MovementData movementData)
        {
            _currentThrustersPower = Constants.SideEnginesPowerZero;

            if (movementData.StrafeMovePower != 0)
            {
                if (movementData.StrafeMovePower > 0)
                {
                    _currentThrustersPower.BackLeft = movementData.StrafeMovePower;
                    _currentThrustersPower.FrontLeft = movementData.StrafeMovePower;
                }
                else
                {
                    _currentThrustersPower.BackRight = -movementData.StrafeMovePower;
                    _currentThrustersPower.FrontRight = -movementData.StrafeMovePower;
                }
            }

            if (movementData.RotatePower != 0)
            {
                if (movementData.RotatePower > 0)
                {
                    if (_currentThrustersPower.BackLeft == 0) _currentThrustersPower.BackLeft = movementData.RotatePower;
                    if (_currentThrustersPower.FrontRight == 0) _currentThrustersPower.FrontRight = movementData.RotatePower;

                }
                else
                {
                    if (_currentThrustersPower.FrontLeft == 0) _currentThrustersPower.FrontLeft = -movementData.RotatePower;
                    if (_currentThrustersPower.BackRight == 0) _currentThrustersPower.BackRight = -movementData.RotatePower;
                }
            }
        }

        private void VisualizeSideEngines(in ViewData view)
        {
            view.SideEngineFR.SetPowerValue(_currentThrustersPower.FrontRight);
            view.SideEngineBR.SetPowerValue(_currentThrustersPower.BackRight);
            view.SideEngineFL.SetPowerValue(_currentThrustersPower.FrontLeft);
            view.SideEngineBL.SetPowerValue(_currentThrustersPower.BackLeft);
        }
    }
}

public struct SideEnginesPower
{
    public float FrontLeft;
    public float FrontRight;
    public float BackLeft;
    public float BackRight;
}

public static class SideEnginesPowerExtensions
{
    public static void Lerp(this ref SideEnginesPower current, in SideEnginesPower target, float t)
    {
        t = Mathf.Clamp01(t);
        float it = 1f - t;

        current.FrontLeft = current.FrontLeft * it + target.FrontLeft * t;
        current.FrontRight = current.FrontRight * it + target.FrontRight * t;
        current.BackLeft = current.BackLeft * it + target.BackLeft * t;
        current.BackRight = current.BackRight * it + target.BackRight * t;
    }
    public static void MoveTowards(ref this SideEnginesPower current, in SideEnginesPower target, float maxDelta)
    {
        current.FrontLeft = Mathf.MoveTowards(current.FrontLeft, target.FrontLeft, maxDelta);
        current.FrontRight = Mathf.MoveTowards(current.FrontRight, target.FrontRight, maxDelta);
        current.BackLeft = Mathf.MoveTowards(current.BackLeft, target.BackLeft, maxDelta);
        current.BackRight = Mathf.MoveTowards(current.BackRight, target.BackRight, maxDelta);
    }
}