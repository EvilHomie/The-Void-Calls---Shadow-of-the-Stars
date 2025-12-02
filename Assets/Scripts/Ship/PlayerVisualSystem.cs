using DI;
using Player;
using Ship;
using UnityEngine;

namespace GameSystems
{
    public class PlayerVisualSystem : GameSystemBase
    {
        private SideEnginesPower _currentThrustersPower;
        private PlayerShip _playerShip;
        private ShipMovementView _shipMovementView;
        private ShipMovementData _shipMovementData;

        [Inject]
        public void Construct(PlayerShip ship)
        {
            _playerShip = ship;
        }

        protected override void Init()
        {
            OnUpdateShip(_playerShip.ShipData);
        }

        protected override void Subscribe()
        {
            GameFlow.FixedGameTick += OnFixedGameTick;
            EventBus.PlayerChangeShip += OnUpdateShip;
        }

        protected override void Unsubscribe()
        {
            GameFlow.FixedGameTick -= OnFixedGameTick;
            EventBus.PlayerChangeShip -= OnUpdateShip;
        }

        private void OnUpdateShip(ShipData ship)
        {
            _shipMovementData = ship.MovementData;
            _shipMovementView = ship.MovementView;
        }


        private void OnFixedGameTick(float deltaTime)
        {
            VisualizeDirectMove();
            CalcSideEnginesPower();
            VisualizeSideEngines();
        }

        private void VisualizeDirectMove()
        {
            float directAccel = 0;
            float reversAccel = 0;

            if (_shipMovementData.DirectAccelerationPower > 0) directAccel = _shipMovementData.DirectAccelerationPower;
            else if (_shipMovementData.DirectAccelerationPower < 0) reversAccel = _shipMovementData.DirectAccelerationPower;

            foreach (var engine in _shipMovementView.ReverseEngines)
            {
                engine.SetThrustValue(-reversAccel);
            }

            foreach (var engine in _shipMovementView.DirectEngines)
            {
                engine.SetThrustValue(directAccel);
            }
        }

        private void CalcSideEnginesPower()
        {
            _currentThrustersPower = Constants.SideEnginesPowerZero;

            if (_shipMovementData.SideAcceleration > 0)
            {
                _currentThrustersPower.BackLeft = _shipMovementData.SideAcceleration;
                _currentThrustersPower.FrontLeft = _shipMovementData.SideAcceleration;
            }
            else
            {
                _currentThrustersPower.BackRight = -_shipMovementData.SideAcceleration;
                _currentThrustersPower.FrontRight = -_shipMovementData.SideAcceleration;
            }

            if (_shipMovementData.RotatePowerValue != 0)
            {
                if (_shipMovementData.RotatePowerValue > 0)
                {
                    if (_currentThrustersPower.BackLeft == 0) _currentThrustersPower.BackLeft = _shipMovementData.RotatePowerValue;
                    if (_currentThrustersPower.FrontRight == 0) _currentThrustersPower.FrontRight = _shipMovementData.RotatePowerValue;

                }
                else
                {
                    if (_currentThrustersPower.FrontLeft == 0) _currentThrustersPower.FrontLeft = -_shipMovementData.RotatePowerValue;
                    if (_currentThrustersPower.BackRight == 0) _currentThrustersPower.BackRight = -_shipMovementData.RotatePowerValue;
                }
            }
        }

        private void VisualizeSideEngines()
        {
            _shipMovementView.SideEngineFR.SetThrustValue(_currentThrustersPower.FrontRight);
            _shipMovementView.SideEngineBR.SetThrustValue(_currentThrustersPower.BackRight);
            _shipMovementView.SideEngineFL.SetThrustValue(_currentThrustersPower.FrontLeft);
            _shipMovementView.SideEngineBL.SetThrustValue(_currentThrustersPower.BackLeft);
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