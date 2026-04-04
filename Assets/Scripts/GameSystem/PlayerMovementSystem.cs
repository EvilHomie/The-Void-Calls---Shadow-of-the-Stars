using DI;
using GameInput;
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
        private ShipInstance _playerShip;

        [Inject]
        public void Construct(IPlayerInput playerInput, ShipInstance ship, Camera camera)
        {
            _input = playerInput;
            _playerShip = ship;
            _camera = camera;
        }

        protected override void Init()
        {
            _input.Init(_playerShip, _camera);
            OnPlayerChangeShip(_playerShip);
        }

        protected override void Subscribe()
        {
            _input.Subscrube();
            _input.MoveInputAction += OnMoveInputAction;
            _input.TrackMouseDirectionAction += OnTrackMouseAction;
            _input.ToggleDamperAction += OnToggleDamper;
            GameFlow.FixedGameTick += OnFixedGameTick;
            EventBus.PlayerChangeShip += OnPlayerChangeShip;
        }

        protected override void Unsubscribe()
        {
            _input.Unsubscribe();
            _input.MoveInputAction -= OnMoveInputAction;
            _input.TrackMouseDirectionAction -= OnTrackMouseAction;
            _input.ToggleDamperAction -= OnToggleDamper;
            GameFlow.FixedGameTick -= OnFixedGameTick;
            EventBus.PlayerChangeShip -= OnPlayerChangeShip;
        }

        private void OnPlayerChangeShip(ShipInstance ship)
        {
            var shipMovementData = _playerShip.ShipData.MovementData;
            var shipChassisData = _playerShip.ShipData.ChassisData;
            var shipEquipData = _playerShip.ShipData.EquipData;

            shipMovementData.InertiaDampingLastState = shipMovementData.InertiaDamping;
            shipMovementData.DirectMaxSpeed = shipEquipData.MainEngine.DirectThrust / shipChassisData.DirectDrag * Constants.WorldUnitMod;
            shipMovementData.DirectAcceleration = shipEquipData.MainEngine.DirectThrust / shipChassisData.Mass * Constants.WorldUnitMod;
            shipMovementData.ReverseMaxSpeed = shipEquipData.MainEngine.ReverseThrust / shipChassisData.ReverseDrag * Constants.WorldUnitMod;
            shipMovementData.ReverseAcceleration = shipEquipData.MainEngine.ReverseThrust / shipChassisData.Mass * Constants.WorldUnitMod;
            shipMovementData.StrafeMaxSpeed = shipEquipData.SideEngines.StrafeThrust / shipChassisData.StrafeDrag * Constants.WorldUnitMod;
            shipMovementData.StrafeAcceleration = shipEquipData.SideEngines.StrafeThrust / shipChassisData.Mass * Constants.WorldUnitMod;
            shipMovementData.RotateSpeed = shipEquipData.SideEngines.RotateThrust / shipChassisData.RotateDrag;
        }

        private void OnTrackMouseAction(Vector2 dir) { _mouseDirection = dir; }
        private void OnMoveInputAction(Vector2 input) { _inputValue = input; }
        private void OnToggleDamper()
        {
            var shipMovementData = _playerShip.ShipData.MovementData;
            shipMovementData.InertiaDamping = !shipMovementData.InertiaDamping;

            if (shipMovementData.InertiaDampingLastState == shipMovementData.InertiaDamping)
            {
                return;
            }

            if (shipMovementData.InertiaDamping)
            {
                shipMovementData.Throttle = shipMovementData.LastThrottleWithDamping;
                shipMovementData.InertiaDampingLastState = true;

                shipMovementData.TargetSpeed = (shipMovementData.Throttle >= 0
                     ? shipMovementData.DirectMaxSpeed
                     : shipMovementData.ReverseMaxSpeed)
                     * shipMovementData.Throttle;
            }
            else
            {
                shipMovementData.LastThrottleWithDamping = shipMovementData.Throttle;
                shipMovementData.Throttle = 0;
                shipMovementData.InertiaDampingLastState = false;
                shipMovementData.DirectAccelerationPower = 0;
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

            var movementData = _playerShip.ShipData.MovementData;

            float prevValue = movementData.Throttle;
            movementData.Throttle += _inputValue.y * fixedDT;
            movementData.Throttle = Mathf.Clamp(movementData.Throttle, -1, 1);

            if ((prevValue < 0f && movementData.Throttle >= 0f) || (prevValue > 0f && movementData.Throttle <= 0f))
            {
                movementData.Throttle = 0;
                _throttleZeroDelayTimer = _throttleZeroDelay;
            }

            movementData.TargetSpeed = (movementData.Throttle >= 0
              ? movementData.DirectMaxSpeed
              : movementData.ReverseMaxSpeed)
              * movementData.Throttle;

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
            var view = _playerShip.View;
            var movementData = _playerShip.ShipData.MovementData;

            // логика торможения если скорость вращение выше контролируемой
            if (Mathf.Abs(view.Rigidbody.angularVelocity) > movementData.RotateSpeed)
            {
                float rotateDirection = Mathf.Sign(view.Rigidbody.angularVelocity);
                view.Rigidbody.angularVelocity -= movementData.RotateSpeed * rotateDirection * fixedDT;
                movementData.RotatePowerValue = -Mathf.Sign(view.Rigidbody.angularVelocity);
                return;
            }

            float angleDiff = Vector2.SignedAngle(view.Transform.up, _mouseDirection);
            float absAngleDiff = Mathf.Abs(angleDiff);
            float direction = Mathf.Sign(angleDiff);

            // --- плавное уменьшение скорости в начале и в конце ---
            float anleMod = Mathf.InverseLerp(0f, 10f, absAngleDiff);
            float power = anleMod * direction;
            float targetTorque = movementData.RotateSpeed * power;
            view.Rigidbody.angularVelocity = Mathf.MoveTowards(view.Rigidbody.angularVelocity, targetTorque, movementData.RotateSpeed);
            movementData.RotatePowerValue = power;
        }

        private void HandleMovement(float fixedDT)
        {
            var view = _playerShip.View;

            float forwardVel = Vector2.Dot(view.Rigidbody.linearVelocity, view.Transform.up);
            float sideVel = Vector2.Dot(view.Rigidbody.linearVelocity, view.Transform.right);
            CalcForwardVelocity(fixedDT, ref forwardVel);
            CalcSideVelocity(fixedDT, ref sideVel);

            view.Rigidbody.linearVelocity = view.Transform.right * sideVel + view.Transform.up * forwardVel;
        }

        private void CalcForwardVelocity(float fixedDT, ref float forwardVel)
        {
            var movementData = _playerShip.ShipData.MovementData;

            if (movementData.InertiaDamping)
            {
                float speedDiff = movementData.TargetSpeed - forwardVel;
                float absSpeedDiff = Mathf.Abs(speedDiff);
                float stabilityMod = movementData.TargetSpeed == 0 ? 0 : _stabilizationPower; // если цель остановится то минимальное значение мощности ноль иначе значение стабилизации
                float passivePower = stabilityMod * movementData.Throttle;

                if (absSpeedDiff < 0.0001f) // если изменение скорости около нулевое
                {
                    forwardVel = movementData.TargetSpeed;
                    movementData.DirectAccelerationPower = passivePower;
                }
                else
                {
                    float disiredMoveDir = Mathf.Sign(speedDiff);
                    float baseAccel = (disiredMoveDir >= 0f ? movementData.DirectAcceleration : movementData.ReverseAcceleration) * fixedDT;
                    float smoothAccel = SmoothAcceleration(baseAccel, absSpeedDiff); // логика сглаживания ускорения при скорости близкой к желаемой
                    forwardVel = Mathf.MoveTowards(forwardVel, movementData.TargetSpeed, smoothAccel);
                    movementData.DirectAccelerationPower = Mathf.Lerp(passivePower, disiredMoveDir, (smoothAccel / baseAccel) - _maxSmooth);
                }
            }
            else
            {
                if (movementData.Throttle == 0) // если нет тяги то ничего не делаем
                {
                    movementData.DirectAccelerationPower = 0;
                    return;
                }

                float maxSpeed = movementData.Throttle > 0f
                        ? movementData.DirectMaxSpeed
                        : -movementData.ReverseMaxSpeed;

                float speedDiff = maxSpeed - forwardVel;
                float absSpeedDiff = Mathf.Abs(speedDiff);

                if (absSpeedDiff < 0.0001f) // если изменение скорости около нулевое
                {
                    forwardVel = maxSpeed;
                    movementData.DirectAccelerationPower = 0;
                }
                else
                {
                    float baseAccel = (movementData.Throttle > 0f ? movementData.DirectAcceleration : -movementData.ReverseAcceleration) * fixedDT * movementData.Throttle;
                    float smoothAccel = SmoothAcceleration(baseAccel, absSpeedDiff); // логика сглаживания ускорения при скорости близкой к максимальной
                    forwardVel = Mathf.MoveTowards(forwardVel, maxSpeed, smoothAccel);

                    // если скорость далека от максимальной (если нет сглаживания)
                    if (baseAccel == smoothAccel) movementData.DirectAccelerationPower = movementData.Throttle;
                    else movementData.DirectAccelerationPower = Mathf.Lerp(0, Mathf.Sign(movementData.Throttle), (smoothAccel / baseAccel) - _maxSmooth);
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
            var movementData = _playerShip.ShipData.MovementData;

            if (movementData.InertiaDamping)
            {
                float targetSpeed = 0;
                float accelBase = movementData.StrafeAcceleration * fixedDT;

                if (_inputValue.x != 0) //если есть боковой инпут
                {
                    targetSpeed = _inputValue.x > 0 ? movementData.StrafeMaxSpeed : -movementData.StrafeMaxSpeed;
                    movementData.SideAcceleration = _inputValue.x;
                }
                else
                {
                    float speedDiff = targetSpeed - sideVel;

                    if (Mathf.Abs(speedDiff) < 0.0001f) // если изменение скорости около нулевое
                    {
                        sideVel = targetSpeed;
                        movementData.SideAcceleration = 0;
                        return;
                    }

                    movementData.SideAcceleration = Mathf.Sign(-sideVel);
                }

                sideVel = Mathf.MoveTowards(sideVel, targetSpeed, accelBase);
            }
            else
            {
                if (_inputValue.x != 0) //если есть боковой инпут
                {
                    float accelBase = movementData.StrafeAcceleration * fixedDT;
                    float targetSpeed = _inputValue.x > 0 ? movementData.StrafeMaxSpeed : -movementData.StrafeMaxSpeed;
                    float speedDiff = targetSpeed - sideVel;

                    if (Mathf.Abs(speedDiff) < 0.0001f) // если изменение скорости около нулевое
                    {
                        sideVel = targetSpeed;
                        movementData.SideAcceleration = 0;
                        return;
                    }

                    sideVel = Mathf.MoveTowards(sideVel, targetSpeed, accelBase);
                    movementData.SideAcceleration = _inputValue.x;
                }
                else
                {
                    movementData.SideAcceleration = 0;
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