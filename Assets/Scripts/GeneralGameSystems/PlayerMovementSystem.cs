using DI;
using GameInput;
using Ships;
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
        private Vector2 _inputValue;
        private Camera _camera;
        private ShipsStorage _objectsStorage;

        [Inject]
        public void Construct(IPlayerInput playerInput, ShipsStorage objectsStorage, Camera camera)
        {
            _input = playerInput;
            _objectsStorage = objectsStorage;
            _camera = camera;
        }

        protected override void AwakeInit()
        {
            _input.Init(_camera, _objectsStorage);
        }

        protected override void Subscribe()
        {
            _input.Subscrube();
            _input.MoveInputAction += OnMoveInputAction;
            _input.ToggleDamperAction += OnToggleDamper;
            GameFlow.FixedGameTick += OnFixedGameTick;
        }

        protected override void Unsubscribe()
        {
            _input.Unsubscribe();
            _input.MoveInputAction -= OnMoveInputAction;
            _input.ToggleDamperAction -= OnToggleDamper;
            GameFlow.FixedGameTick -= OnFixedGameTick;
        }

        private void OnMoveInputAction(Vector2 input) { _inputValue = input; }
        private void OnToggleDamper()
        {
            var playerIndex = _objectsStorage.PlayerIndex;
            ref var movementData = ref _objectsStorage.MovementDatas[playerIndex];
            ref var chassisData = ref _objectsStorage.ChassisDatas[playerIndex];
            movementData.InertiaDampingState = !movementData.InertiaDampingState;

            if (movementData.InertiaDampingState)
            {
                movementData.Throttle = movementData.LastThrottleWithDamping;

                movementData.TargetSpeed = (movementData.Throttle >= 0
                     ? chassisData.DirectMaxSpeed
                     : chassisData.ReverseMaxSpeed)
                     * movementData.Throttle;
            }
            else
            {
                movementData.LastThrottleWithDamping = movementData.Throttle;
                movementData.Throttle = 0;
                movementData.DirectMovePower = 0;
            }
        }

        private void OnFixedGameTick(float fixedDT)
        {
            HandleThrottle(fixedDT);
            HandleMovement(fixedDT);
            HandleRotation(fixedDT);
        }

        private readonly float _throttleZeroDelay = 0.3f; // продолжительность задерки на нуле.
        private float _throttleZeroDelayTimer = 0f; // текущий таймер задержки

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

            var playerIndex = _objectsStorage.PlayerIndex;
            ref var movementData = ref _objectsStorage.MovementDatas[playerIndex];
            ref var chassisData = ref _objectsStorage.ChassisDatas[playerIndex];

            float prevValue = movementData.Throttle;
            movementData.Throttle += _inputValue.y * fixedDT;
            movementData.Throttle = Mathf.Clamp(movementData.Throttle, -1, 1);

            if ((prevValue < 0f && movementData.Throttle >= 0f) || (prevValue > 0f && movementData.Throttle <= 0f))
            {
                movementData.Throttle = 0;
                _throttleZeroDelayTimer = _throttleZeroDelay;
            }

            movementData.TargetSpeed = (movementData.Throttle >= 0
              ? chassisData.DirectMaxSpeed
              : chassisData.ReverseMaxSpeed)
              * movementData.Throttle;
        }

        private void HandleRotation(float fixedDT)
        {
            var playerIndex = _objectsStorage.PlayerIndex;
            ref var movementData = ref _objectsStorage.MovementDatas[playerIndex];
            ref var chassisData = ref _objectsStorage.ChassisDatas[playerIndex];
            ref var viewData = ref _objectsStorage.ViewsDatas[playerIndex];

            var rigidBody = viewData.Rigidbody;

            // логика торможения если скорость вращение выше контролируемой
            if (Mathf.Abs(rigidBody.angularVelocity) > chassisData.RotateMaxSpeed)
            {
                float rotateDirection = Mathf.Sign(rigidBody.angularVelocity);
                rigidBody.angularVelocity -= chassisData.RotateMaxSpeed * rotateDirection * fixedDT;
                movementData.RotatePower = -Mathf.Sign(rigidBody.angularVelocity);
                return;
            }

            ref var targetPos = ref _objectsStorage.AimPositions[playerIndex];
            ref var shipPosition = ref _objectsStorage.Positions[playerIndex];

            var direction = targetPos - shipPosition;

            float angleDiff = Vector2.SignedAngle(viewData.Transform.up, direction);
            float absAngleDiff = Mathf.Abs(angleDiff);
            float directionSign = Mathf.Sign(angleDiff);

            // --- плавное уменьшение скорости в начале и в конце ---
            float angleMod = Mathf.InverseLerp(0f, 10f, absAngleDiff);
            float power = angleMod * directionSign;
            float targetTorque = chassisData.RotateMaxSpeed * power;
            rigidBody.angularVelocity = Mathf.MoveTowards(rigidBody.angularVelocity, targetTorque, chassisData.RotateMaxSpeed);
            movementData.RotatePower = power;
        }

        private void HandleMovement(float fixedDT)
        {
            ref var view = ref _objectsStorage.ViewsDatas[_objectsStorage.PlayerIndex];

            float forwardVel = Vector2.Dot(view.Rigidbody.linearVelocity, view.Transform.up);
            float sideVel = Vector2.Dot(view.Rigidbody.linearVelocity, view.Transform.right);
            CalcForwardVelocity(fixedDT, ref forwardVel);
            CalcSideVelocity(fixedDT, ref sideVel);

            view.Rigidbody.linearVelocity = view.Transform.right * sideVel + view.Transform.up * forwardVel;
        }

        private void CalcForwardVelocity(float fixedDT, ref float forwardVel)
        {
            var playerIndex = _objectsStorage.PlayerIndex;
            ref var movementData = ref _objectsStorage.MovementDatas[playerIndex];
            ref var chassisData = ref _objectsStorage.ChassisDatas[playerIndex];

            if (movementData.InertiaDampingState)
            {
                float speedDiff = movementData.TargetSpeed - forwardVel;
                float absSpeedDiff = Mathf.Abs(speedDiff);
                float stabilityMod = movementData.TargetSpeed == 0 ? 0 : _stabilizationPower; // если цель остановится то минимальное значение мощности ноль иначе значение стабилизации
                float passivePower = stabilityMod * movementData.Throttle;

                if (absSpeedDiff < 0.0001f) // если изменение скорости около нулевое
                {
                    forwardVel = movementData.TargetSpeed;
                    movementData.DirectMovePower = passivePower;
                }
                else
                {
                    float disiredMoveDir = Mathf.Sign(speedDiff);
                    float baseAccel = (disiredMoveDir >= 0f ? chassisData.DirectMaxAcceleration : chassisData.ReverseMaxAcceleration) * fixedDT;
                    float smoothAccel = SmoothAcceleration(baseAccel, absSpeedDiff); // логика сглаживания ускорения при скорости близкой к желаемой
                    forwardVel = Mathf.MoveTowards(forwardVel, movementData.TargetSpeed, smoothAccel);
                    movementData.DirectMovePower = Mathf.Lerp(passivePower, disiredMoveDir, (smoothAccel / baseAccel) - _maxSmooth);
                }
            }
            else
            {
                if (movementData.Throttle == 0) // если нет тяги то ничего не делаем
                {
                    movementData.DirectMovePower = 0;
                    return;
                }

                float maxSpeed = movementData.Throttle > 0f
                        ? chassisData.DirectMaxSpeed
                        : -chassisData.ReverseMaxSpeed;

                float speedDiff = maxSpeed - forwardVel;
                float absSpeedDiff = Mathf.Abs(speedDiff);

                if (absSpeedDiff < 0.0001f) // если изменение скорости около нулевое
                {
                    forwardVel = maxSpeed;
                    movementData.DirectMovePower = 0;
                }
                else
                {
                    float baseAccel = (movementData.Throttle > 0f ? chassisData.DirectMaxAcceleration : -chassisData.ReverseMaxAcceleration) * fixedDT * movementData.Throttle;
                    float smoothAccel = SmoothAcceleration(baseAccel, absSpeedDiff); // логика сглаживания ускорения при скорости близкой к максимальной
                    forwardVel = Mathf.MoveTowards(forwardVel, maxSpeed, smoothAccel);

                    // если скорость далека от максимальной (если нет сглаживания)
                    if (baseAccel == smoothAccel) movementData.DirectMovePower = movementData.Throttle;
                    else movementData.DirectMovePower = Mathf.Lerp(0, Mathf.Sign(movementData.Throttle), (smoothAccel / baseAccel) - _maxSmooth);
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
            var playerIndex = _objectsStorage.PlayerIndex;
            ref var movementData = ref _objectsStorage.MovementDatas[playerIndex];
            ref var chassisData = ref _objectsStorage.ChassisDatas[playerIndex];

            if (movementData.InertiaDampingState)
            {
                float targetSpeed = 0;
                float accelBase = chassisData.StrafeMaxAcceleration * fixedDT;

                if (_inputValue.x != 0) //если есть боковой инпут
                {
                    targetSpeed = _inputValue.x > 0 ? chassisData.StrafeMaxSpeed : -chassisData.StrafeMaxSpeed;
                    movementData.StrafeMovePower = _inputValue.x;
                }
                else
                {
                    float speedDiff = targetSpeed - sideVel;

                    if (Mathf.Abs(speedDiff) < 0.0001f) // если изменение скорости около нулевое
                    {
                        sideVel = targetSpeed;
                        movementData.StrafeMovePower = 0;
                        return;
                    }

                    movementData.StrafeMovePower = Mathf.Sign(-sideVel);
                }

                sideVel = Mathf.MoveTowards(sideVel, targetSpeed, accelBase);
            }
            else
            {
                if (_inputValue.x != 0) //если есть боковой инпут
                {
                    float accelBase = chassisData.StrafeMaxAcceleration * fixedDT;
                    float targetSpeed = _inputValue.x > 0 ? chassisData.StrafeMaxSpeed : -chassisData.StrafeMaxSpeed;
                    float speedDiff = targetSpeed - sideVel;

                    if (Mathf.Abs(speedDiff) < 0.0001f) // если изменение скорости около нулевое
                    {
                        sideVel = targetSpeed;
                        movementData.StrafeMovePower = 0;
                        return;
                    }

                    sideVel = Mathf.MoveTowards(sideVel, targetSpeed, accelBase);
                    movementData.StrafeMovePower = _inputValue.x;
                }
                else
                {
                    movementData.StrafeMovePower = 0;
                }
            }
        }
    }
}