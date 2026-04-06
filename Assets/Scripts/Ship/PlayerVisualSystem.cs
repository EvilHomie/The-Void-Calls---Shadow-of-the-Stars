using DI;
using Ship;
using UnityEngine;

namespace GameSystems
{
    public class PlayerVisualSystem : GameSystemBase
    {
        private SideEnginesPower _currentThrustersPower;
        private ObjectsStorage _objectsStorage;

        [Inject]
        public void Construct(ObjectsStorage objectsStorage)
        {
            _objectsStorage = objectsStorage;
        }

        protected override void AwakeInit()
        {
        }

        protected override void Subscribe()
        {
            GameFlow.UpdateTick += OnUpdateTick;
        }

        protected override void Unsubscribe()
        {
            GameFlow.UpdateTick -= OnUpdateTick;
        }

        private void OnUpdateTick(float deltaTime)
        {
            VisualizeDirectMove(_objectsStorage.PlayerShipData, _objectsStorage.PlayerShipView);
            CalcSideEnginesPower(_objectsStorage.PlayerShipData.MovementData);
            VisualizeSideEngines(_objectsStorage.PlayerShipView);


            foreach (var shipInstance in _objectsStorage.NonPlayerShipsInstance)
            {
                var index = shipInstance.Index;
                ref var view = ref _objectsStorage.NonPlayerShipsView[index];
                ref var shipData = ref _objectsStorage.NonPlayerShipsData[index];
                VisualizeDirectMove(shipData, view);
                CalcSideEnginesPower(shipData.MovementData);
                VisualizeSideEngines(view);
            }
        }

        private void VisualizeDirectMove(in ShipData shipData, in ShipView shipView)
        {
            float directAccel = 0;
            float reversAccel = 0;

            if (shipData.MovementData.DirectAccelerationPower > 0)
            {
                directAccel = shipData.MovementData.DirectAccelerationPower;
            }
            else if (shipData.MovementData.DirectAccelerationPower < 0)
            {
                reversAccel = shipData.MovementData.DirectAccelerationPower;
            }

            foreach (var engine in shipView.ReverseEngines)
            {
                engine.SetThrustValue(-reversAccel);
            }

            foreach (var engine in shipView.DirectEngines)
            {
                engine.SetThrustValue(directAccel);
            }
        }

        private void CalcSideEnginesPower(in ShipMovementData movementData)
        {
            _currentThrustersPower = Constants.SideEnginesPowerZero;

            if (movementData.SideAcceleration != 0)
            {
                if (movementData.SideAcceleration > 0)
                {
                    _currentThrustersPower.BackLeft = movementData.SideAcceleration;
                    _currentThrustersPower.FrontLeft = movementData.SideAcceleration;
                }
                else
                {
                    _currentThrustersPower.BackRight = -movementData.SideAcceleration;
                    _currentThrustersPower.FrontRight = -movementData.SideAcceleration;
                }
            }

            if (movementData.RotatePowerValue != 0)
            {
                if (movementData.RotatePowerValue > 0)
                {
                    if (_currentThrustersPower.BackLeft == 0) _currentThrustersPower.BackLeft = movementData.RotatePowerValue;
                    if (_currentThrustersPower.FrontRight == 0) _currentThrustersPower.FrontRight = movementData.RotatePowerValue;

                }
                else
                {
                    if (_currentThrustersPower.FrontLeft == 0) _currentThrustersPower.FrontLeft = -movementData.RotatePowerValue;
                    if (_currentThrustersPower.BackRight == 0) _currentThrustersPower.BackRight = -movementData.RotatePowerValue;
                }
            }
        }

        private void VisualizeSideEngines(in ShipView view)
        {
            view.SideEngineFR.SetThrustValue(_currentThrustersPower.FrontRight);
            view.SideEngineBR.SetThrustValue(_currentThrustersPower.BackRight);
            view.SideEngineFL.SetThrustValue(_currentThrustersPower.FrontLeft);
            view.SideEngineBL.SetThrustValue(_currentThrustersPower.BackLeft);
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