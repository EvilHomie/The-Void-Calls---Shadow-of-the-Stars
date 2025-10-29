using DI;
using Player;
using Ship;
using System;
using UnityEngine;

namespace GameSystem
{
    public class PlayerVisualSystem : GameSystemBase
    {
        [SerializeField] float _rotateVelocityTreshhold;
        [SerializeField] float _sideVelocityTreshhold;
        [SerializeField] private float _thrusterSmooth = 5f;

        private float _prevAngularVel;
        private float _prevLinearVel;
        private SideEnginesPower _currentThrustersPower;
        private SideEnginesPower _targetThrustersPower;
        //private ShipChassisData _shipMovementData;
        private ShipMovementView _shipMovementView;
        private Rigidbody2D _shipRB;
        private Transform _shipTransform;

        private static readonly SideEnginesPower SideEnginesPowerZero = new();


        private ShipMovementData _shipMovementData;

        [Inject]
        public void Construct(PlayerShipData ship)
        {
            OnUpdateShip(ship);
        }

        protected override void Subscribe()
        {
            //GameFlow.FixedGameTick += OnFixedGameTick;
            GameFlow.GameTick += OnGameTick;
            //EventBus.UpdateShip += OnUpdateShip;
        }

        protected override void Unsubscribe()
        {
            //GameFlow.FixedGameTick -= OnFixedGameTick;
            GameFlow.GameTick -= OnGameTick;
            //EventBus.UpdateShip -= OnUpdateShip;
        }

        private void OnUpdateShip(PlayerShipData ship)
        {
            _shipMovementData = ship.MovementData;
            //_shipRB = ship.Rigidbody;
            //_shipTransform = _shipRB.transform;
            //_shipMovementData = ship.MovementData;
            _shipMovementView = ship.MovementView;
            _shipMovementView.Init();
        }

        private void OnFixedGameTick(float fDeltaTime)
        {
            //_targetThrustersPower = SideEnginesPowerZero;
            //CalculateRotatePower();
            //CalculateSideMovePower();
        }

        private void OnGameTick(float deltaTime)
        {
            float directAccel = 0;
            float reversAccel = 0;

            _shipMovementData.DirectAccelerationPower = Math.Clamp(_shipMovementData.DirectAccelerationPower, -1, 1);

            if (_shipMovementData.DirectAccelerationPower > 0) directAccel = _shipMovementData.DirectAccelerationPower;
            else if (_shipMovementData.DirectAccelerationPower < 0) reversAccel = _shipMovementData.DirectAccelerationPower;

            _shipMovementView.MainEngine.SetThrustValue(directAccel);
            foreach (var engine in _shipMovementView.ReverseMainEngines)
            {
                engine.SetThrustValue(-reversAccel);
            }

            _currentThrustersPower = SideEnginesPowerZero;

            if (_shipMovementData.SideAcceleration > 0)
            {
                _currentThrustersPower.BackLeft = _shipMovementData.SideAcceleration;
                _currentThrustersPower.FrontLeft = _shipMovementData.SideAcceleration;
            }
            else if (_shipMovementData.SideAcceleration < 0)
            {
                _currentThrustersPower.BackRight = -_shipMovementData.SideAcceleration;
                _currentThrustersPower.FrontRight = -_shipMovementData.SideAcceleration;
            }

            _shipMovementView.SideEngineFR.SetThrustValue(_currentThrustersPower.FrontRight);
            _shipMovementView.SideEngineBR.SetThrustValue(_currentThrustersPower.BackRight);
            _shipMovementView.SideEngineFL.SetThrustValue(_currentThrustersPower.FrontLeft);
            _shipMovementView.SideEngineBL.SetThrustValue(_currentThrustersPower.BackLeft);
        }

        //private void OnGameTick(float deltaTime)
        //{
        //    float step = _thrusterSmooth * deltaTime;
        //    _currentThrustersPower.MoveTowards(_targetThrustersPower, step);

        //    UpdateSideEnginesVisual();
        //    UpdateMainEngineVisual();
        //}

        //private void UpdateSideEnginesVisual()
        //{
        //    _shipMovementView.SideEngineFL.SetThrustValue(_currentThrustersPower.FrontLeft);
        //    _shipMovementView.SideEngineBR.SetThrustValue(_currentThrustersPower.BackRight);
        //    _shipMovementView.SideEngineFR.SetThrustValue(_currentThrustersPower.FrontRight);
        //    _shipMovementView.SideEngineBL.SetThrustValue(_currentThrustersPower.BackLeft);
        //}

        //private void UpdateMainEngineVisual()
        //{
        //    float mainThrust = _shipMovementData.ForvardPowerValue;

        //    if (mainThrust > 0)
        //    {
        //        _shipMovementView.MainEngine.SetThrustValue(mainThrust);
        //    }
        //    else if (mainThrust < 0)
        //    {
        //        foreach (var engine in _shipMovementView.ReverseMainEngines)
        //        {
        //            engine.SetThrustValue(MathF.Abs(mainThrust));
        //        }
        //    }
        //    else
        //    {
        //        _shipMovementView.MainEngine.SetThrustValue(0);

        //        foreach (var engine in _shipMovementView.ReverseMainEngines)
        //        {
        //            engine.SetThrustValue(0);
        //        }
        //    }
        //}

        //private void CalculateRotatePower()
        //{
        //    float rotateThrust = _shipMovementData.RotatePowerValue;
        //    float absRotateThrust = MathF.Abs(rotateThrust);

        //    if (rotateThrust != 0) // Если игрок управляет вращением движением
        //    {
        //        if (rotateThrust > 0) // вращение против часовой
        //        {
        //            _targetThrustersPower.FrontRight = absRotateThrust;
        //            _targetThrustersPower.BackLeft = absRotateThrust;
        //        }
        //        else if (rotateThrust < 0) // вращение по часовой
        //        {
        //            _targetThrustersPower.BackRight = absRotateThrust;
        //            _targetThrustersPower.FrontLeft = absRotateThrust;
        //        }

        //        return;
        //    }



        //    //float currVel = _shipRB.angularVelocity;
        //    //float delta = currVel - _prevAngularVel;
        //    //float direction = Mathf.Sign(currVel);
        //    //_prevAngularVel = currVel;

        //    //if (Mathf.Abs(currVel) > _rotateVelocityTreshhold)
        //    //{
        //    //    if (Mathf.Abs(delta) <= float.Epsilon)
        //    //    {
        //    //        if (Mathf.Abs(currVel) > _rotateVelocityTreshhold) // Макс Разгон"
        //    //        {
        //    //            if (direction > 0)
        //    //            {
        //    //                _targetThrustersPower.FrontRight = 1;
        //    //                _targetThrustersPower.BackLeft = 1;
        //    //            }
        //    //            else
        //    //            {
        //    //                _targetThrustersPower.BackRight = 1;
        //    //                _targetThrustersPower.FrontLeft = 1;
        //    //            }
        //    //        }
        //    //        //else простой
        //    //    }
        //    //    else
        //    //    {
        //    //        if (direction > 0) // корабль вращается против часовой
        //    //        {
        //    //            if (delta > 0) //разгон
        //    //            {
        //    //                _targetThrustersPower.FrontRight = 1;
        //    //                _targetThrustersPower.BackLeft = 1;
        //    //            }
        //    //            else if (delta < 0) // торможение
        //    //            {
        //    //                _targetThrustersPower.BackRight = 1;
        //    //                _targetThrustersPower.FrontLeft = 1;
        //    //            }
        //    //        }
        //    //        else // корабль вращается по часовой
        //    //        {
        //    //            if (delta > 0) // торможение
        //    //            {
        //    //                _targetThrustersPower.BackLeft = 1;
        //    //                _targetThrustersPower.FrontRight = 1;
        //    //            }
        //    //            else if (delta < 0) //разгон
        //    //            {
        //    //                _targetThrustersPower.BackRight = 1;
        //    //                _targetThrustersPower.FrontLeft = 1;
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}

        //private void CalculateSideMovePower()
        //{
        //    float sideThrust = _shipMovementData.SidePowerValue;

        //    if (sideThrust != 0) // Если игрок управляет боковым движением
        //    {
        //        if (sideThrust < 0) // движение влево
        //        {
        //            _targetThrustersPower.FrontRight = 1;
        //            _targetThrustersPower.BackRight = 1;
        //        }
        //        else if (sideThrust > 0) // движение вправо
        //        {
        //            _targetThrustersPower.FrontLeft = 1;
        //            _targetThrustersPower.BackLeft = 1;
        //        }

        //        return;
        //    }

        //    float currVel = Vector2.Dot(_shipRB.linearVelocity, _shipTransform.right);

        //    if (Mathf.Abs(currVel) < _sideVelocityTreshhold) // отбраковка минимального движения. также защита от значения равного нулю
        //    {                
        //        return;
        //    }

        //    float direction = Mathf.Sign(currVel);

        //    if (direction < 0) // скользит влего и тормозит
        //    {
        //        _targetThrustersPower.FrontLeft = 1;
        //        _targetThrustersPower.BackLeft = 1;
        //    }
        //    else // скользит влего и тормозит
        //    {
        //        _targetThrustersPower.FrontRight = 1;
        //        _targetThrustersPower.BackRight = 1;
        //    }
        //}
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