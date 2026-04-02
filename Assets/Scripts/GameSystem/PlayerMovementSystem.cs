using DI;
using Player;
using Ship;
using UnityEngine;

namespace GameSystems
{
    public class PlayerMovementSystem : GameSystemBase
    {
        public const int _rotateMod = 45; // базовая скорость поворота при силе равной сопротивлению
        public const int _rotateAngleTreshhold = 2; // отбраковка минимального угла поворота
        private const float _stabilizationPower = 0.6f; // модификатор при движении без ускорения при включеном гасителе инерции. Будто мощность для поддержания скорости
        private const float _smoothZone = 0.5f; // чем больше тем раньше начнется плавность
        private const float _maxSmooth = 0.05f; // чем меньше тем более плавно (дольше) добираются последние "метры" скорости

        private IPlayerInput _input;
        private Vector2 _mouseDirection;
        private Vector2 _inputValue;
        private Camera _camera;
        private PlayerShip _playerShip;
        private Rigidbody2D _shipRB;
        private Transform _shipTransform;
        private ShipMovementData _movementData;

        private float _directMaxSpeed;
        private float _directAcceleration;
        private float _reverseMaxSpeed;
        private float _reverseAcceleration;
        private float _strafeMaxSpeed;
        private float _strafeAcceleration;
        private float _rotateSpeed;

        private float _targetSpeed;

        private bool _inertiaDampingLastState;
        private float _lastThrottleWithDamping;

        [Inject]
        public void Construct(IPlayerInput playerInput, PlayerShip ship, Camera camera)
        {
            _input = playerInput;
            _playerShip = ship;
            _camera = camera;
        }

        protected override void Init()
        {
            _input.Init(_playerShip, _camera);
            _shipRB = _playerShip.Rigidbody;
            _shipTransform = _shipRB.transform;

            OnPlayerChangeShip(_playerShip);
        }

        protected override void Subscribe()
        {
            _input.Subscrube();
            _input.MoveInputAction += OnMoveInputAction;
            _input.TrackMouseAction += OnTrackMouseAction;
            _input.ToggleDamperAction += OnToggleDamper;
            GameFlow.FixedGameTick += OnFixedGameTick;
            EventBus.PlayerChangeShip += OnPlayerChangeShip;
        }

        protected override void Unsubscribe()
        {
            _input.Unsubscribe();
            _input.MoveInputAction -= OnMoveInputAction;
            _input.TrackMouseAction -= OnTrackMouseAction;
            _input.ToggleDamperAction -= OnToggleDamper;
            GameFlow.FixedGameTick -= OnFixedGameTick;
            EventBus.PlayerChangeShip -= OnPlayerChangeShip;
        }

        private void OnPlayerChangeShip(PlayerShip ship)
        {
            _movementData = ship.ShipData.MovementData;
            _inertiaDampingLastState = ship.ShipData.MovementData.InertiaDamping;
            _shipRB.mass = ship.ShipData.ChassisData.Mass;

            _directMaxSpeed = ship.ShipData.MovementData.MainEngine.DirectThrust / ship.ShipData.ChassisData.DirectDrag / Constants.WorldUnitMod;
            _directAcceleration = ship.ShipData.MovementData.MainEngine.DirectThrust / ship.ShipData.ChassisData.Mass / Constants.WorldUnitMod;
            _reverseMaxSpeed = ship.ShipData.MovementData.MainEngine.ReverseThrust / ship.ShipData.ChassisData.ReverseDrag / Constants.WorldUnitMod;
            _reverseAcceleration = ship.ShipData.MovementData.MainEngine.ReverseThrust / ship.ShipData.ChassisData.Mass / Constants.WorldUnitMod;
            _strafeMaxSpeed = ship.ShipData.MovementData.SideEngines.StrafeThrust / ship.ShipData.ChassisData.StrafeDrag / Constants.WorldUnitMod;
            _strafeAcceleration = ship.ShipData.MovementData.SideEngines.StrafeThrust / ship.ShipData.ChassisData.Mass / Constants.WorldUnitMod;
            _rotateSpeed = ship.ShipData.MovementData.SideEngines.RotateThrust / ship.ShipData.ChassisData.RotateDrag;
        }

        private void OnTrackMouseAction(Vector2 dir) { _mouseDirection = dir; }
        private void OnMoveInputAction(Vector2 input) { _inputValue = input; }
        private void OnToggleDamper()
        {
            _movementData.InertiaDamping = !_movementData.InertiaDamping;

            if (_inertiaDampingLastState == _movementData.InertiaDamping)
            {
                return;
            }

            if (_movementData.InertiaDamping)
            {
                _movementData.Throttle = _lastThrottleWithDamping;
                _inertiaDampingLastState = true;

                _targetSpeed = (_movementData.Throttle >= 0
                     ? _directMaxSpeed
                     : _reverseMaxSpeed)
                     * _movementData.Throttle;
            }
            else
            {
                _lastThrottleWithDamping = _movementData.Throttle;
                _movementData.Throttle = 0;
                _inertiaDampingLastState = false;
                _movementData.DirectAccelerationPower = 0;
            }
        }

        private void OnFixedGameTick(float fixedDT)
        {
            HandleThrottle(fixedDT);
            HandleMovement(fixedDT);
            HandleRotation(fixedDT);
        }

        private readonly float _throttleZeroDelay = 0.2f; // продолжительность задерки на нуле.
        private float _throttleZeroDelayTimer = 0f; // текущий таймер задержки

        // переменные нужны для закомментированной версии
        //private readonly float _throttleZeroSensitivity = 0.01f; // порог срабатывания задержки.
        //private bool _lockOnZero; // должна ли быть пауза при прохождении через ноль

        private void HandleThrottle(float fixedDT)
        {
            if (_inputValue.y == 0) // если нет инпута на изменение дросселя то сбросс таймера остановки на нуле
            {
                _throttleZeroDelayTimer = 0;
                return;
            }

            if (_throttleZeroDelayTimer > 0)  // игнор если запущен таймер остановки на нуле
            {
                _throttleZeroDelayTimer -= fixedDT;
                return;
            }

            float prevValue = _movementData.Throttle;
            _movementData.Throttle += _inputValue.y * fixedDT;
            _movementData.Throttle = Mathf.Clamp(_movementData.Throttle, -1, 1);

            if ((prevValue < 0f && _movementData.Throttle >= 0f) || (prevValue > 0f && _movementData.Throttle <= 0f))
            {
                _movementData.Throttle = 0;
                _throttleZeroDelayTimer = _throttleZeroDelay;
            }

            _targetSpeed = (_movementData.Throttle >= 0
              ? _directMaxSpeed
              : _reverseMaxSpeed)
              * _movementData.Throttle;

            // В версии ниже нашел косяк. если новая скорость отличается от нуля меньше чем на _throttleZeroSensitivity то при обратке не стопарится на нуле

            //if (_inputValue.y == 0) // если нет инпута на изменение дросселя то сбросс таймера остановки на нуле
            //{
            //    _lockOnZero = false;
            //    _throttleZeroDelayTimer = 0;
            //    return;
            //}

            //if (_throttleZeroDelayTimer > 0)  // игнор если запущен таймер остановки на нуле
            //{
            //    _throttleZeroDelayTimer -= fixedDT;
            //    return;
            //}

            //_movementData.Throttle += _inputValue.y * fixedDT;
            //_movementData.Throttle = Mathf.Clamp(_movementData.Throttle, -1, 1);

            //float absThrottle = Mathf.Abs(_movementData.Throttle);

            //if (absThrottle >= _throttleZeroSensitivity)
            //{
            //    _lockOnZero = true;
            //}

            //if (_lockOnZero && absThrottle < _throttleZeroSensitivity)
            //{
            //    _lockOnZero = false;
            //    _movementData.Throttle = 0;
            //    _throttleZeroDelayTimer = _throttleZeroDelay;
            //}

            //_targetSpeed = (_movementData.Throttle >= 0
            //   ? _directMaxSpeed
            //   : _reverseMaxSpeed)
            //   * _movementData.Throttle;
        }

        private void HandleRotation(float fixedDT)
        {
            // логика торможения если скорость вращение выше контролируемой
            if (Mathf.Abs(_shipRB.angularVelocity) > _rotateSpeed)
            {
                float rotateDirection = Mathf.Sign(_shipRB.angularVelocity);
                _shipRB.angularVelocity -= _rotateSpeed * rotateDirection * fixedDT;
                _movementData.RotatePowerValue = -Mathf.Sign(_shipRB.angularVelocity);
                return;
            }

            float angleDiff = Vector2.SignedAngle(_shipTransform.up, _mouseDirection);
            float absAngleDiff = Mathf.Abs(angleDiff);
            float direction = Mathf.Sign(angleDiff);

            // --- плавное уменьшение скорости в начале и в конце ---
            float anleMod = Mathf.InverseLerp(0f, 10f, absAngleDiff);
            float power = anleMod * direction;
            float targetTorque = _rotateSpeed * power;
            _shipRB.angularVelocity = Mathf.MoveTowards(_shipRB.angularVelocity, targetTorque, _rotateSpeed);
            _movementData.RotatePowerValue = power;
        }

        private void HandleMovement(float fixedDT)
        {
            float forwardVel = Vector2.Dot(_shipRB.linearVelocity, _shipTransform.up);
            float sideVel = Vector2.Dot(_shipRB.linearVelocity, _shipTransform.right);
            CalcForwardVelocity(fixedDT, ref forwardVel);
            CalcSideVelocity(fixedDT, ref sideVel);

            _shipRB.linearVelocity = _shipTransform.right * sideVel + _shipTransform.up * forwardVel;
        }

        private void CalcForwardVelocity(float fixedDT, ref float forwardVel)
        {
            if (_movementData.InertiaDamping)
            {
                float speedDiff = _targetSpeed - forwardVel;
                float absSpeedDiff = Mathf.Abs(speedDiff);
                float stabilityMod = _targetSpeed == 0 ? 0 : _stabilizationPower; // если цель остановится то минимальное значение мощности ноль иначе значение стабилизации
                float passivePower = stabilityMod * _movementData.Throttle;

                if (absSpeedDiff < 0.0001f) // если изменение скорости около нулевое
                {
                    forwardVel = _targetSpeed;
                    _movementData.DirectAccelerationPower = passivePower;
                }
                else
                {
                    float disiredMoveDir = Mathf.Sign(speedDiff);
                    float baseAccel = (disiredMoveDir >= 0f ? _directAcceleration : _reverseAcceleration) * fixedDT;
                    float smoothAccel = SmoothAcceleration(baseAccel, absSpeedDiff); // логика сглаживания ускорения при скорости близкой к желаемой
                    forwardVel = Mathf.MoveTowards(forwardVel, _targetSpeed, smoothAccel);
                    _movementData.DirectAccelerationPower = Mathf.Lerp(passivePower, disiredMoveDir, (smoothAccel / baseAccel) - _maxSmooth);
                }
            }
            else
            {
                if (_movementData.Throttle == 0) // если нет тяги то ничего не делаем
                {
                    _movementData.DirectAccelerationPower = 0;
                    return;
                }

                float maxSpeed = _movementData.Throttle > 0f
                        ? _directMaxSpeed
                        : -_reverseMaxSpeed;

                float speedDiff = maxSpeed - forwardVel;
                float absSpeedDiff = Mathf.Abs(speedDiff);

                if (absSpeedDiff < 0.0001f) // если изменение скорости около нулевое
                {
                    forwardVel = maxSpeed;
                    _movementData.DirectAccelerationPower = 0;
                }
                else
                {
                    float baseAccel = (_movementData.Throttle > 0f ? _directAcceleration : -_reverseAcceleration) * fixedDT * _movementData.Throttle;
                    float smoothAccel = SmoothAcceleration(baseAccel, absSpeedDiff); // логика сглаживания ускорения при скорости близкой к максимальной
                    forwardVel = Mathf.MoveTowards(forwardVel, maxSpeed, smoothAccel);

                    // если скорость далека от максимальной (если нет сглаживания)
                    if (baseAccel == smoothAccel) _movementData.DirectAccelerationPower = _movementData.Throttle;
                    else _movementData.DirectAccelerationPower = Mathf.Lerp(0, Mathf.Sign(_movementData.Throttle), (smoothAccel / baseAccel) - _maxSmooth);
                }
            }
        }

        private float SmoothAcceleration(float acceleration, float absSpeedDiff)
        {
            if (absSpeedDiff < _smoothZone) acceleration *= Mathf.Lerp(_maxSmooth, 1, absSpeedDiff / _smoothZone);
            return acceleration;
        }


        // нужна логика быстрого гашения боковой скорости
        private void CalcSideVelocity(float fixedDT, ref float sideVel)
        {
            if (_movementData.InertiaDamping)
            {
                float targetSpeed = 0;
                float accelBase = _strafeAcceleration * fixedDT;

                if (_inputValue.x != 0) //если есть боковой инпут
                {
                    targetSpeed = _inputValue.x > 0 ? _strafeMaxSpeed : -_strafeMaxSpeed;
                    _movementData.SideAcceleration = _inputValue.x;
                }
                else
                {
                    float speedDiff = targetSpeed - sideVel;

                    if (Mathf.Abs(speedDiff) < 0.0001f) // если изменение скорости около нулевое
                    {
                        sideVel = targetSpeed;
                        _movementData.SideAcceleration = 0;
                        return;
                    }

                    _movementData.SideAcceleration = Mathf.Sign(-sideVel);
                }

                sideVel = Mathf.MoveTowards(sideVel, targetSpeed, accelBase);
            }
            else
            {
                if (_inputValue.x != 0) //если есть боковой инпут
                {
                    float accelBase = _strafeAcceleration * fixedDT;
                    float targetSpeed = _inputValue.x > 0 ? _strafeMaxSpeed : -_strafeMaxSpeed;
                    float speedDiff = targetSpeed - sideVel;

                    if (Mathf.Abs(speedDiff) < 0.0001f) // если изменение скорости около нулевое
                    {
                        sideVel = targetSpeed;
                        _movementData.SideAcceleration = 0;
                        return;
                    }

                    sideVel = Mathf.MoveTowards(sideVel, targetSpeed, accelBase);
                    _movementData.SideAcceleration = _inputValue.x;
                }
                else
                {
                    _movementData.SideAcceleration = 0;
                }
            }
        }



        //private void Update() //Тестовая часть 
        //{
        //    if (Keyboard.current.qKey.wasPressedThisFrame)
        //    {
        //        _shipRB.AddTorque(200);
        //    }

        //    if (Keyboard.current.eKey.wasPressedThisFrame)
        //    {
        //        _shipRB.AddTorque(-200);
        //    }

        //    if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        //    {
        //        _shipRB.AddRelativeForce(Vector2.up * 500);
        //    }

        //    if (Keyboard.current.leftCtrlKey.wasPressedThisFrame)
        //    {
        //        _shipRB.AddRelativeForce(Vector2.down * 500);
        //    }
        //}
    }
}