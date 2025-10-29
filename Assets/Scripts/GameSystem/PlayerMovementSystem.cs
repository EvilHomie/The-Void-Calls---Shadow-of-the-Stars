using DI;
using Player;
using Ship;
using UnityEngine;

namespace GameSystem
{
    public class PlayerMovementSystem : GameSystemBase
    {
        public const int _rotateMod = 90; // базовая скорость поворота при силе равной массе
        private const float _stabilizationPower = 0.6f; // модификатор при движении без ускорения при включеном гасителе инерции. Будто мощность для поддержания скорости
        private const float _smoothZone = 0.1f; // чем больше тем раньше начнется плавность
        private const float _maxSmooth = 0.05f; // чем меньше тем более плавно (дольше) добираются последние "метры" скорости

        private IPlayerInput _input;
        private Vector2 _mouseDirection;
        private Vector2 _inputValue;
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
        private float _prevDirectSpeed;
        private float _prevSideSpeed;

        private bool _inertiaDampingLastState;
        private float _lastThrottleWithDamping;

        [Inject]
        public void Construct(IPlayerInput playerInput, PlayerShipData ship, Camera camera)
        {
            _input = playerInput;
            _input.Init(ship, camera);
            OnUpdateShip(ship);
        }

        protected override void Subscribe()
        {
            _input.Subscrube();
            _input.MoveInputAction += OnMoveInputAction;
            _input.TrackMouseAction += OnTrackMouseAction;
            GameFlow.FixedGameTick += OnFixedGameTick;
            EventBus.UpdateShip += OnUpdateShip;
            EventBus.ToggleDamper += OnToggleDamper;
        }

        protected override void Unsubscribe()
        {
            _input.Unsubscribe();
            _input.MoveInputAction -= OnMoveInputAction;
            _input.TrackMouseAction -= OnTrackMouseAction;
            GameFlow.FixedGameTick -= OnFixedGameTick;
            EventBus.UpdateShip -= OnUpdateShip;
            EventBus.ToggleDamper -= OnToggleDamper;
        }

        private void OnUpdateShip(PlayerShipData ship)
        {
            _shipRB = ship.Rigidbody;
            _shipTransform = _shipRB.transform;
            _movementData = ship.MovementData;
            _inertiaDampingLastState = ship.MovementData.InertiaDamping;

            _directMaxSpeed = ship.MovementData.MainEngine.DirectThrust / ship.ChassisData.DirectDrag;
            _directAcceleration = ship.MovementData.MainEngine.DirectThrust / ship.ChassisData.Mass;
            _reverseMaxSpeed = ship.MovementData.MainEngine.ReverseThrust / ship.ChassisData.ReverseDrag;
            _reverseAcceleration = ship.MovementData.MainEngine.ReverseThrust / ship.ChassisData.Mass;
            _strafeMaxSpeed = ship.MovementData.SideEngines.StrafeThrust / ship.ChassisData.StrafeDrag;
            _strafeAcceleration = ship.MovementData.SideEngines.StrafeThrust / ship.ChassisData.Mass;
            _rotateSpeed = _rotateMod * ship.MovementData.SideEngines.RotateThrust / ship.ChassisData.RotateDrag;
        }

        private void OnTrackMouseAction(Vector2 dir) { _mouseDirection = dir; }
        private void OnMoveInputAction(Vector2 input) { _inputValue = input; }
        private void OnToggleDamper()
        {
            _movementData.InertiaDamping = !_movementData.InertiaDamping;

            if (_inertiaDampingLastState != _movementData.InertiaDamping)
            {
                if (_movementData.InertiaDamping)
                {
                    _movementData.Throttle = _lastThrottleWithDamping;
                    _inertiaDampingLastState = true;
                }
                else
                {
                    _movementData.Throttle = 0;
                    _inertiaDampingLastState = false;
                    _movementData.DirectAccelerationPower = 0;
                }
            }
        }

        private void OnFixedGameTick(float fixedDT)
        {
            HandleThrottle(fixedDT);
            HandleMovement(fixedDT);
            HandleRotation(fixedDT);
        }

        private readonly float _throttleZeroSensitivity = 0.05f; // порог срабатывания задержки.
        private readonly float _throttleZeroDelay = 1f; // продолжительность задерки на нуле.
        private float _throttleZeroDelayTimer = 0f; // текущий таймер задержки
        private bool _zeroCrossIgnored; // должна ли быть пауза при прохождении через ноль

        private void HandleThrottle(float fixedDT)
        {
            if (_movementData.InertiaDamping)
            {
                _lastThrottleWithDamping = _movementData.Throttle;
            }

            if (_inputValue.y == 0)
            {
                _zeroCrossIgnored = true;
                _throttleZeroDelayTimer = 0;
                return;
            }

            if (_throttleZeroDelayTimer > 0)
            {
                _throttleZeroDelayTimer -= fixedDT;
                return;
            }

            _movementData.Throttle += _inputValue.y * fixedDT;
            _movementData.Throttle = Mathf.Clamp(_movementData.Throttle, -1, 1);

            float absThrottle = Mathf.Abs(_movementData.Throttle);

            if (absThrottle > _throttleZeroSensitivity)
            {
                _zeroCrossIgnored = false;
            }

            if (!_zeroCrossIgnored && absThrottle < _throttleZeroSensitivity)
            {
                _zeroCrossIgnored = true;
                _movementData.Throttle = 0;
                _throttleZeroDelayTimer = _throttleZeroDelay;
            }

            _targetSpeed = (_movementData.Throttle >= 0
               ? _directMaxSpeed
               : _reverseMaxSpeed)
               * _movementData.Throttle;
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
            float absAngle = Mathf.Abs(angleDiff);
            float direction = Mathf.Sign(angleDiff);

            // --- плавное уменьшение скорости в начале и в конце ---
            float mod = Mathf.InverseLerp(0f, 10f, absAngle);
            _movementData.RotatePowerValue = mod * direction;
            _shipRB.angularVelocity = _movementData.RotatePowerValue * _rotateSpeed;
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

/* Старая логика (вдруг пригодится)
 //// регулятор мощностей двигателя
        //private void UpdatePower(in float input, EngineBase engine, in float fixedDT)
        //{
        //    if (input != 0)
        //    {
        //        float targetSign = Mathf.Sign(input);

        //        if (Mathf.Sign(engine.PowerValue) == targetSign || engine.PowerValue == 0f)
        //        {
        //            // направление совпадает → обычный разгон
        //            engine.PowerValue += targetSign * fixedDT / engine.FullPowerTime;
        //        }
        //        else
        //        {
        //            // направление противоположное → ускоренное торможение
        //            // скорость сброса = DisableTime + сила в противоположном направлении
        //            float brakeRate = fixedDT / engine.DisableTime + fixedDT / engine.FullPowerTime;
        //            engine.PowerValue = Mathf.MoveTowards(engine.PowerValue, 0f, brakeRate);
        //        }

        //        engine.PowerValue = Mathf.Clamp(engine.PowerValue, -1f, 1f);
        //    }
        //    else
        //    {
        //        // кнопка отпущена → сброс к 0 через DisableTime
        //        engine.PowerValue = Mathf.MoveTowards(engine.PowerValue, 0f, fixedDT / engine.DisableTime);
        //    }
        //}


логика приложения сил с учетом linearDamping
private const float _dampMod = 10; // модификатор набора скорости при dampinge меньше 1
private void ApplyMoveForces()
        {
            Vector2 localForce = Vector2.zero;

            // --- Главный двигатель ---
            if (_inputDirection.y != 0)
            {
                float forwardPower = _shipMovementData.ForvardPowerValue;
                float mainPower = forwardPower > 0
              ? forwardPower * _mainEngine.ForwardForce
              : forwardPower * _mainEngine.ReverseForce;

                if (_shipRB.linearDamping >= 1f)
                {
                    mainPower *= _shipRB.linearDamping; // просто усиливаем силу
                }
                else if (_shipRB.linearDamping > 0f && mainPower != 0)
                {
                    float t = Mathf.Abs(_shipRB.linearVelocity.y) / Mathf.Abs(mainPower);
                    float modifier = Mathf.Lerp(_dampMod, 1f, t);
                    mainPower *= modifier * _shipRB.linearDamping; // адаптивный модификатор
                }
                // else: linearDamping == 0 → mainForce напрямую

                localForce += Vector2Up * mainPower;
            }

            // --- Боковые ускорители ---
            if (_inputDirection.x != 0)
            {
                float sideForce = _shipMovementData.SidePowerValue * _sideEngines.SideForce;

                // модификатор для боковых
                if (_shipRB.linearDamping >= 1f)
                {
                    sideForce *= _shipRB.linearDamping;
                }
                else if (_shipRB.linearDamping > 0f && sideForce != 0)
                {
                    float t = Mathf.Abs(_shipRB.linearVelocity.x) / Mathf.Abs(sideForce);
                    float modifier = Mathf.Lerp(_dampMod, 1f, t);
                    sideForce *= modifier * _shipRB.linearDamping;
                }
                // else: linearDamping == 0 → sideForce напрямую

                localForce += Vector2Right * sideForce;
            }


            _shipRB.AddRelativeForce(localForce, ForceMode2D.Force);
        }

private void ApplyThrust(in Vector2 input, in float fixedDT, ref float forwardVel, ref float sideVel)
        {
            // --- Главный двигатель ---
            if (input.y != 0)
            {
                float desiredAccel = (input.y > 0f
                    ? _relativeForwardForce
                    : -_relativeReverseForce)
                    * fixedDT / _mainEngine.FullPowerTime;

                forwardVel += desiredAccel;
            }

            // --- Боковые ускорители ---
            if (input.x != 0)
            {
                float desiredAccel = (input.x > 0f
                    ? _relativeSideForce
                    : -_relativeSideForce)
                    * fixedDT / _sideEngines.FullPowerTime;

                sideVel += desiredAccel;
            }
        }

логика движения (основной двигатель) как в X4 до рефактора
private void ApplyMainEngineThrust(float fixedDT, ref float forwardVel)
        {

            if (_movementData.InertiaDamping)
            {
                float speedDiff = _targetSpeed - forwardVel;

                if (speedDiff == 0) return;

                float absSpeedDiff = Mathf.Abs(speedDiff);
                float targetSpeedDir = Mathf.Sign(speedDiff);

                float baseAccelStep = (targetSpeedDir > 0f
                    ? _directAcceleration
                    : _reverseAcceleration)
                    * fixedDT;

                // логика сглаживания ускорения при скорости близкой к желаемой
                float smoothAccelStep = SmoothAcceleration(baseAccelStep, absSpeedDiff);
                forwardVel = Mathf.MoveTowards(forwardVel, _targetSpeed, smoothAccelStep);

                // логика записи данных о движении 
                float currentAcceleration = forwardVel - _prevSpeed;
                float accelerationRatio = currentAcceleration / baseAccelStep;


                if (_targetSpeed != 0f && Mathf.Abs(accelerationRatio) < _stabilizationSpeedMod)
                {
                    _movementData.ForwardAccelerationMod = _stabilizationSpeedMod * targetSpeedDir;
                }
                else
                {
                    _movementData.ForwardAccelerationMod = accelerationRatio;
                }

                _prevSpeed = forwardVel;
                return;
            }

            if (_movementData.Throttle == 0)
            {
                _movementData.ForwardAccelerationMod = _movementData.Throttle;
            }
            else
            {
                float maxSpeed = _movementData.Throttle > 0f
                    ? _directMaxSpeed
                    : -_reverseMaxSpeed;

                float speedDiff = maxSpeed - forwardVel;

                if (speedDiff == 0f) return;

                float absSpeedDiff = Mathf.Abs(speedDiff);

                float baseAccelStep = (_movementData.Throttle > 0f
                    ? _directAcceleration
                    : -_reverseAcceleration)
                    * fixedDT * _movementData.Throttle;

                // логика сглаживания ускорения при скорости близкой к максимальной
                float smoothAccelStep = SmoothAcceleration(baseAccelStep, absSpeedDiff);
                forwardVel = Mathf.MoveTowards(forwardVel, maxSpeed, smoothAccelStep);

                // логика записи данных о движении 
                _movementData.ForwardAccelerationMod = _movementData.Throttle * smoothAccelStep / baseAccelStep;

                _prevSpeed = forwardVel;
            }
        }




        //private void ApplyMainEngineThrust(float fixedDT, ref float forwardVel)
        //{
        //    //TODO Может довавить проверку что если разгон от нуля то более вязко, будто преодолевает инерцию
        //    float targetSpeed;
        //    float accelBase;
        //    float accelSign;
        //    float throttle = _movementData.Throttle;

        //    if (_movementData.InertiaDamping)
        //    {
        //        targetSpeed = _targetSpeed;
        //        accelSign = Mathf.Sign(targetSpeed - forwardVel);
        //        accelBase = accelSign > 0f ? _directAcceleration : _reverseAcceleration;
        //    }
        //    else
        //    {
        //        if (throttle == 0)
        //        {
        //            _movementData.DirectAcceleration = 0f;
        //            return;
        //        }

        //        accelSign = Mathf.Sign(throttle);
        //        targetSpeed = accelSign > 0f ? _directMaxSpeed : -_reverseMaxSpeed;
        //        accelBase = accelSign > 0f ? _directAcceleration : _reverseAcceleration;
        //        accelBase *= Mathf.Abs(throttle);
        //    }

        //    float speedDiff = targetSpeed - forwardVel;
        //    if (speedDiff == 0)           // можно раскомитить для лучшей производительности, но тогда не показывается поддержание мощности двигателя при включенном гасителе и при достижении макс скорости
        //        return;

        //    float absDiff = Mathf.Abs(speedDiff);
        //    float baseStep = accelBase * fixedDT;

        //    // --- Сглаживание ---
        //    float smoothStep = SmoothAcceleration(baseStep, absDiff);
        //    forwardVel = Mathf.MoveTowards(forwardVel, targetSpeed, smoothStep);

        //    // --- Расчет ускорения ---
        //    float currentAccel = forwardVel - _prevDirectSpeed;
        //    float accelRatio = baseStep != 0f ? currentAccel / baseStep : 0f;

        //    // --- Стабилизация ---
        //    if (_movementData.InertiaDamping)
        //    {
        //        if (targetSpeed != 0f && Mathf.Abs(accelRatio) < _stabilizationSpeedMod)
        //            accelRatio = _stabilizationSpeedMod * accelSign;
        //    }
        //    else
        //    {
        //        accelRatio = throttle * smoothStep / baseStep;
        //    }

        //    _movementData.DirectAcceleration = accelRatio;
        //    _prevDirectSpeed = forwardVel;
        //}
*/