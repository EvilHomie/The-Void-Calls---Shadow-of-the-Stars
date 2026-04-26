using DI;
using GameInput;
using Registries;
using Ships;
using UnityEngine;
using MouseCursor = GameCamera.MouseCursor;

namespace GameSystems
{
    public class PlayerMovementSystem : GameSystemBase
    {
        private const float EPS = 0.001f;
        private const float _minAcceleration = 0.005f; // 0.03f минимальный коэффициент (чтобы не было "залипания") при модификации ускорения
        private const float _noDumpingThrottleMod = 4; // модификатор изменения дросселя если выключены гасители инерции. Будто чуствительность перекладывания.
        private const float _smoothZoneTime = 0.2f; // время до ключевой скорости для сглаживания ускорения
        private const float _damperMaxFactor = 2f; // усиление гасителей при макс скорости (будто выше сопротивление)
        private const float _damperMinFactor = 1f; // сила гасителей при минимальной скорости (чтобы не залипало)
        private const float _rotateSlowAngle = 20f;
        private const float _minMouseDistanceSQR = 0.1f;
        private const float _minBoostersPowerForEnableMod = 0.2f;
        private const float _throttleZeroDelay = 0.3f;

        private float _smoothZoneMod; // 1/ _smoothZoneTime. сугубо чтобы уйти от деления в логике
        private float _throttleZeroDelayTimer; // текущий таймер остановки на нуле


        private IPlayerInput _input;
        private Vector2 _inputValue;
        private MouseCursor _mouseCursor;
        private ShipRegistry _shipRegystry;

        private bool _isActive;

        [Inject]
        public void Construct(IPlayerInput playerInput, MouseCursor mouseCursor, ShipRegistry shipRegystry)
        {
            _shipRegystry = shipRegystry;
            _input = playerInput;
            _mouseCursor = mouseCursor;
            _smoothZoneMod = 1f / _smoothZoneTime;
        }

        protected override void AwakeInit()
        { 
        }

        protected override void Subscribe()
        {
            _input.MoveInputAction += OnMoveInputAction;
            _input.ToggleDamperAction += OnToggleDamper;
            _input.DisableEngineAction += DisableEngine;
            _input.ChangeBoostersState += OnToogleBoosters;
            EventBus.GameStateChangeAction += OnGameStateChange;
            GameFlowSystem.FixedGameTick += Simulate;
        }

        protected override void Unsubscribe()
        {
            _input.MoveInputAction -= OnMoveInputAction;
            _input.ToggleDamperAction -= OnToggleDamper;
            _input.DisableEngineAction -= DisableEngine;
            _input.ChangeBoostersState -= OnToogleBoosters;
            EventBus.GameStateChangeAction -= OnGameStateChange;
            GameFlowSystem.FixedGameTick -= Simulate;
        }

        private void OnGameStateChange(GameState gameState)
        {
            _isActive = gameState == GameState.CoreGameplay;
        }

        private void OnMoveInputAction(Vector2 input)
        {
            _inputValue.x = input.x;
            _inputValue.y = input.y;
        }

        private void OnToggleDamper()
        {
            var playerShip = _shipRegystry.PlayerShip;
            ref var movementRuntimeData = ref playerShip.MovementRuntimeData;
            movementRuntimeData.InertiaDampingIsActive = !movementRuntimeData.InertiaDampingIsActive;

            if (movementRuntimeData.InertiaDampingIsActive)
            {
                movementRuntimeData.DirectThrottle = movementRuntimeData.LastDampingThrottle;
            }
            else
            {
                movementRuntimeData.LastDampingThrottle = movementRuntimeData.DirectThrottle;
                movementRuntimeData.DirectThrottle = 0;
            }
        }

        private void OnToogleBoosters(bool state)
        {
            var playerShip = _shipRegystry.PlayerShip;

            ref var movementRuntimeData = ref playerShip.MovementRuntimeData;
            ref var movementStaticData = ref playerShip.MovementStaticData;

            if (state)
            {
                movementRuntimeData.DirectThrottle = 1; // всегда включаем двигатель на макс есть был запрос через буст

                var boostersPower = movementRuntimeData.BoostersPower;
                var minPowerForEnable = movementStaticData.BoostersMaxPower * _minBoostersPowerForEnableMod;

                if (boostersPower < minPowerForEnable)
                {
                    return;
                }
            }

            movementRuntimeData.BoostersIsActive = state;
        }

        private void Simulate(float fixedDT)
        {
            if (!_isActive)
            {
                return;
            }

            var playerShip = _shipRegystry.PlayerShip;
            var mousePos = _mouseCursor.WorldPostition;

            var rb = playerShip.Rigidbody;
            var rotation = rb.rotation;
            var angularVelocity = rb.angularVelocity;
            var linearVelocity = rb.linearVelocity;
            var shipPos = rb.position;

            ref var movementRuntimeData = ref playerShip.MovementRuntimeData;
            ref var movementStaticData = ref playerShip.MovementStaticData;

            var rad = rotation * Mathf.Deg2Rad;
            var shipForward = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
            var shipRight = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            var directionToTarget = mousePos - shipPos;

            UpdateBoostersPower(fixedDT, movementStaticData, ref movementRuntimeData);
            HandleInput(fixedDT, ref movementRuntimeData);
            HandleRotation(fixedDT, ref angularVelocity, directionToTarget, shipForward, movementStaticData, ref movementRuntimeData);
            HandleMovement(fixedDT, ref linearVelocity, ref movementRuntimeData, movementStaticData, shipForward, shipRight);

            rb.angularVelocity = angularVelocity;
            rb.linearVelocity = linearVelocity;
        }

        private void HandleInput(float fixedDT, ref MovementRuntimeData movementRuntimeData)
        {
            movementRuntimeData.StrafeThrottle = _inputValue.x; // боковое движение ровно инпуту

            if (movementRuntimeData.BoostersIsActive)
            {
                return;
            }

            if (!movementRuntimeData.InertiaDampingIsActive) // если гаситель выключен то дросель всегда равен инпуту (будто отстреливает в ноль если нет инпута)
            {
                movementRuntimeData.DirectThrottle = Mathf.MoveTowards(movementRuntimeData.DirectThrottle, _inputValue.y, _noDumpingThrottleMod * fixedDT);
                return;
            }

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

            var prevThrottle = movementRuntimeData.DirectThrottle;
            var newThrottle = prevThrottle + _inputValue.y * fixedDT;

            if ((prevThrottle < 0f && newThrottle >= 0f) || (prevThrottle > 0f && newThrottle <= 0f))
            {
                newThrottle = 0;
                _throttleZeroDelayTimer = _throttleZeroDelay;
            }

            movementRuntimeData.DirectThrottle = Mathf.Clamp(newThrottle, -1, 1);
        }

        private void UpdateBoostersPower(float fixedDT, in MovementStaticData movementStaticData, ref MovementRuntimeData movementRuntimeData)
        {
            var boostersPower = movementRuntimeData.BoostersPower;

            if (!movementRuntimeData.BoostersIsActive)
            {
                boostersPower += fixedDT;
            }
            else
            {
                boostersPower -= fixedDT;

                if (boostersPower <= 0)
                {
                    movementRuntimeData.BoostersIsActive = false;
                }
            }

            boostersPower = Mathf.Clamp(boostersPower, 0, movementStaticData.BoostersMaxPower);
            movementRuntimeData.BoostersPower = boostersPower;
        }

        private void DisableEngine()
        {
            var playerShip = _shipRegystry.PlayerShip;
            ref var movementRuntimeData = ref playerShip.MovementRuntimeData;
            movementRuntimeData.DirectThrottle = 0;
        }

        private void HandleRotation(float fixedDT, ref float angularVelocity, Vector2 direction, Vector2 shipForward, in MovementStaticData movementStaticData, ref MovementRuntimeData movementRuntimeData)
        {
            float maxSpeed = movementStaticData.RotateSpeed;
            bool targetOutSideShip = direction.sqrMagnitude >= _minMouseDistanceSQR;

            if (Mathf.Abs(angularVelocity) > maxSpeed || !targetOutSideShip) // только гашение если больше максимального или мышка на корабле
            {
                angularVelocity = Mathf.MoveTowards(angularVelocity, 0f, maxSpeed * fixedDT);


                movementRuntimeData.RotatePower = Mathf.Abs(angularVelocity) < EPS ? 0f : angularVelocity / maxSpeed;
                return;
            }

            var angle = Vector2.SignedAngle(shipForward, direction);
            var absAngle = Mathf.Abs(angle);

            var power = Mathf.InverseLerp(0f, _rotateSlowAngle, absAngle) * Mathf.Sign(angle);
            var newVelocity = power * maxSpeed;
            angularVelocity = newVelocity;

            movementRuntimeData.RotatePower = Mathf.Abs(newVelocity) < EPS ? 0f : newVelocity / maxSpeed;
        }

        private void HandleMovement(float fixedDT, ref Vector2 linearVelocity, ref MovementRuntimeData movementRuntimeData, in MovementStaticData movementStaticData, Vector2 forward, Vector2 right)
        {
            var forwardVel = Vector2.Dot(linearVelocity, forward);
            var sideVel = Vector2.Dot(linearVelocity, right);

            var directThrottle = movementRuntimeData.DirectThrottle;
            var strafeThrottle = movementRuntimeData.StrafeThrottle;
            var boostersIsActive = movementRuntimeData.BoostersIsActive;

            if (directThrottle != 0 || boostersIsActive)
            {
                if (movementRuntimeData.InertiaDampingIsActive)
                {
                    ApplyMainEngineForce(fixedDT, directThrottle, ref forwardVel, movementStaticData, boostersIsActive);
                }
                else
                {
                    AddMainEngineForce(fixedDT, directThrottle, ref forwardVel, movementStaticData, boostersIsActive);
                }
            }

            if (strafeThrottle != 0)
            {
                if (movementRuntimeData.InertiaDampingIsActive)
                {
                    ApplyThrustersForce(fixedDT, strafeThrottle, ref sideVel, movementStaticData);
                }
                else
                {
                    AddThrustersForce(fixedDT, strafeThrottle, ref sideVel, movementStaticData);
                }
            }

            if (movementRuntimeData.InertiaDampingIsActive)
            {
                ApplyDirectDamping(fixedDT, directThrottle, ref forwardVel, movementStaticData, boostersIsActive);
                ApplyStrafeDamping(fixedDT, strafeThrottle, ref sideVel, movementStaticData);
            }

            movementRuntimeData.MainEnginePower = boostersIsActive ? 1 : directThrottle;
            movementRuntimeData.ThrustersPower = strafeThrottle;
            linearVelocity = right * sideVel + forward * forwardVel;
        }

        private void ApplyDirectDamping(float fixedDT, float throttle, ref float forwardVel, in MovementStaticData movementStaticData, bool boostersIsActive)
        {
            if (Mathf.Abs(forwardVel) < EPS) // если не двигаемся. 
            {
                forwardVel = 0;
                return;
            }

            if (boostersIsActive)
            {
                return;
            }

            var acceleration = forwardVel > 0
                ? movementStaticData.DirectDampingAcceleration
                : movementStaticData.ReverseDampingAcceleration;

            var maxSpeed = forwardVel > 0
                ? movementStaticData.DirectMaxSpeed
                : movementStaticData.ReverseMaxSpeed;

            var targetSpeed = throttle * maxSpeed;
            float desiredSpeed;

            if (throttle == 0f || forwardVel * throttle < 0f) // Нет дроссель в нуле или в контртяге то гасим до нуля
            {
                desiredSpeed = 0f;
            }
            else
            {
                var speedDiff = targetSpeed - forwardVel;

                if (speedDiff * throttle < -EPS) // Гасим если превысили
                {
                    desiredSpeed = targetSpeed;
                }
                else
                {
                    return;
                }
            }

            ApplyDampingSmooth(ref acceleration, forwardVel, maxSpeed);
            forwardVel = Mathf.MoveTowards(forwardVel, desiredSpeed, acceleration * fixedDT);
        }

        private void ApplyStrafeDamping(float fixedDT, float throttle, ref float sideVel, in MovementStaticData movementStaticData)
        {
            if (Mathf.Abs(sideVel) < EPS) // если не двигаемся. 
            {
                sideVel = 0;
                return;
            }

            var acceleration = movementStaticData.StrafeAcceleration;
            var maxSpeed = movementStaticData.StrafeMaxSpeed;

            var targetSpeed = throttle * maxSpeed;
            float desiredSpeed;

            if (throttle == 0f || sideVel * throttle < 0f) // Нет дроссель в нуле или в контртяге то гасим до нуля
            {
                desiredSpeed = 0f;
            }
            else
            {
                var speedDiff = targetSpeed - sideVel;

                if (speedDiff * throttle < -EPS) // Гасим если превысили
                {
                    desiredSpeed = targetSpeed;
                }
                else
                {
                    return;
                }
            }

            ApplyDampingSmooth(ref acceleration, sideVel, maxSpeed);
            sideVel = Mathf.MoveTowards(sideVel, desiredSpeed, acceleration * fixedDT);
        }

        private void ApplyMainEngineForce(float fixedDT, float throttle, ref float forwardVel, in MovementStaticData movementStaticData, bool boostersIsActive)
        {
            float acceleration;
            float maxSpeed;

            if (boostersIsActive)
            {
                acceleration = movementStaticData.BoostersAcceleration;
                maxSpeed = movementStaticData.BoostersMaxSpeed;
            }
            else
            {
                acceleration = throttle > 0
                      ? movementStaticData.DirectAcceleration
                      : -movementStaticData.ReverseAcceleration;

                maxSpeed = throttle > 0
                          ? movementStaticData.DirectMaxSpeed
                          : movementStaticData.ReverseMaxSpeed;
            }

            var targetSpeed = throttle * maxSpeed;
            var speedDiff = targetSpeed - forwardVel;

            if (speedDiff * throttle <= 0f) // проверка если скорость достигла максимальной
            {
                return;
            }

            ApplyAccelerationSmooth(ref acceleration, speedDiff);
            forwardVel += acceleration * fixedDT;
        }

        private void ApplyThrustersForce(float fixedDT, float throttle, ref float sideVel, in MovementStaticData movementStaticData)
        {
            var acceleration = throttle > 0
                      ? movementStaticData.StrafeAcceleration
                      : -movementStaticData.StrafeAcceleration;

            var maxSpeed = movementStaticData.StrafeMaxSpeed;

            var targetSpeed = throttle * maxSpeed;
            var speedDiff = targetSpeed - sideVel;

            if (speedDiff * throttle <= 0f) // проверка если скорость достигла максимальной
            {
                return;
            }

            ApplyAccelerationSmooth(ref acceleration, speedDiff);
            sideVel += acceleration * fixedDT;
        }

        private void AddMainEngineForce(float fixedDT, float throttle, ref float forwardVel, in MovementStaticData movementStaticData, bool boostersIsActive)
        {
            float acceleration;

            if (boostersIsActive)
            {
                acceleration = movementStaticData.BoostersAcceleration;
            }
            else
            {
                acceleration = throttle > 0
                ? movementStaticData.DirectAcceleration
                : movementStaticData.ReverseAcceleration;
            }

            forwardVel += acceleration * throttle * fixedDT;
        }
        private void AddThrustersForce(float fixedDT, float throttle, ref float sideVel, in MovementStaticData movementStaticData)
        {
            var acceleration = movementStaticData.StrafeAcceleration;
            sideVel += acceleration * throttle * fixedDT;
        }

        private void ApplyAccelerationSmooth(ref float acceleration, float speedDiff)
        {
            float timeToTarget = Mathf.Abs(speedDiff / acceleration);

            if (timeToTarget > _smoothZoneTime) return;

            float t = timeToTarget * _smoothZoneMod;
            float curve = t * t;
            float accelFactor = Mathf.Lerp(_minAcceleration, 1f, curve);
            acceleration *= accelFactor;
        }

        private void ApplyDampingSmooth(ref float acceleration, float velocity, float maxSpeed)
        {
            var k = Mathf.Abs(velocity) / maxSpeed;
            var curve = k * k;
            var factor = Mathf.Lerp(_damperMinFactor, _damperMaxFactor, curve); // Усиливаем гашение на макс скорости и уменьнаем при минимальной
            acceleration *= factor;
        }
    }
}











//    //Чуть более грязный но рабочий вариант поворота. Нужно потестировать и удалить если не будет нареканий
//    private void HandleRotation(float fixedDT, Vector2 shipForward, in MovementStaticData movementStaticData, ref MovementRuntimeData movementRuntimeData)
//{
//    var rb = _playerShip.Rigidbody;
//    var angularVelocity = rb.angularVelocity;
//    var maxRotateSpeed = movementStaticData.RotateSpeed;

//    if (angularVelocity > maxRotateSpeed)  // только гашение если больше максимального
//    {
//        angularVelocity = Mathf.MoveTowards(angularVelocity, 0f, maxRotateSpeed * fixedDT);
//        movementRuntimeData.RotateThrottle = -Mathf.Sign(angularVelocity);
//        return;
//    }

//    Vector2 mouseWorld = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
//    var mouseDirection = mouseWorld - rb.position;

//    if (mouseDirection.sqrMagnitude < 0.1f) // только гашение если курсор наведен на свой корабль
//    {
//        angularVelocity = Mathf.MoveTowards(angularVelocity, 0f, maxRotateSpeed * fixedDT);
//        rb.angularVelocity = angularVelocity;

//        movementRuntimeData.RotateThrottle = Mathf.Abs(angularVelocity) < EPS ? 0 : -Mathf.Sign(angularVelocity);
//        return;
//    }

//    var angle = Vector2.SignedAngle(shipForward, mouseDirection);
//    var absAngle = Mathf.Abs(angle);
//    var angleSign = Mathf.Sign(angle);

//    var rotatePowerMod = Mathf.InverseLerp(0f, 20f, absAngle) * angleSign;
//    var rotateSpeed = maxRotateSpeed * rotatePowerMod;

//    angularVelocity = Mathf.Clamp(rotateSpeed, -maxRotateSpeed, maxRotateSpeed);
//    rb.angularVelocity = angularVelocity;

//    if (Mathf.Abs(angularVelocity) > maxRotateSpeed - EPS) // если достигли макс вращения
//    {
//        movementRuntimeData.RotateThrottle = Mathf.Sign(angularVelocity);
//        return;
//    }

//    movementRuntimeData.RotateThrottle = rotatePowerMod < EPS ? 0 : rotatePowerMod;
//}


// Старые версии
//private void SimulateMovement(float fixedDT)
//{
//    Vector2 mouseWorld = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
//    var targetDirection = mouseWorld - (Vector2)_playerShip.Transform.position;

//    ref var movementData = ref _playerShip.MovementRuntimeData;
//    ref var movementStaticData = ref _playerShip.MovementStaticData;
//    ref var movementVisualData = ref _playerShip.MovementVisual;

//    float rad = _playerRotation * Mathf.Deg2Rad;
//    Vector2 right = new(Mathf.Cos(rad), Mathf.Sin(rad));
//    Vector2 forward = new(-right.y, right.x);

//    HandleThrottle(fixedDT, ref movementData, movementStaticData);
//    HandleMovement(fixedDT, ref movementData, movementStaticData, forward, right);
//    HandleRotation(fixedDT, ref movementData, ref movementVisualData, movementStaticData, targetDirection, forward);
//}



//private readonly float _throttleZeroDelay = 0.3f; // продолжительность задерки на нуле.
//private float _throttleZeroDelayTimer = 0f; // текущий таймер задержки

//private void HandleThrottle(float fixedDT, ref MovementRuntimeData movementData, in MovementStaticData movementCharacteristicsData)
//{
//    if (_inputValue.y == 0) // если нет инпута на изменение дросселя то сбросс таймера остановки на нуле
//    {
//        _throttleZeroDelayTimer = 0;
//        return;
//    }

//    if (_throttleZeroDelayTimer > 0)  // игнор если запущен таймер остановки на нуле
//    {
//        _throttleZeroDelayTimer -= fixedDT;
//        return;
//    }



//    float prevValue = movementData.Throttle;
//    movementData.Throttle += _inputValue.y * fixedDT;
//    movementData.Throttle = Mathf.Clamp(movementData.Throttle, -1, 1);

//    if ((prevValue < 0f && movementData.Throttle >= 0f) || (prevValue > 0f && movementData.Throttle <= 0f))
//    {
//        movementData.Throttle = 0;
//        _throttleZeroDelayTimer = _throttleZeroDelay;
//    }

//    movementData.TargetSpeed = (movementData.Throttle >= 0
//      ? movementCharacteristicsData.DirectMaxSpeed
//      : movementCharacteristicsData.ReverseMaxSpeed)
//      * movementData.Throttle;
//}



//private void HandleMovement(float fixedDT, ref MovementRuntimeData movementData, in MovementStaticData movementCharacteristicsData, Vector2 forward, Vector2 right)
//{
//    float forwardVel = Vector2.Dot(_playerLinearVelocity, forward);
//    float sideVel = Vector2.Dot(_playerLinearVelocity, right);
//    CalcForwardVelocity(fixedDT, ref forwardVel, ref movementData, movementCharacteristicsData);
//    CalcSideVelocity(fixedDT, ref sideVel, ref movementData, movementCharacteristicsData);

//    _playerLinearVelocity = right * sideVel + forward * forwardVel;
//}

//private void CalcForwardVelocity(float fixedDT, ref float forwardVel, ref MovementRuntimeData movementData, in MovementStaticData movementCharacteristicsData)
//{

//    if (movementData.InertiaDampingState)
//    {
//        float speedDiff = movementData.TargetSpeed - forwardVel;
//        float absSpeedDiff = Mathf.Abs(speedDiff);
//        float stabilityMod = movementData.TargetSpeed == 0 ? 0 : _stabilizationPower; // если цель остановится то минимальное значение мощности ноль иначе значение стабилизации
//        float passivePower = stabilityMod * movementData.Throttle;

//        if (absSpeedDiff < 0.0001f) // если изменение скорости около нулевое
//        {
//            forwardVel = movementData.TargetSpeed;
//            movementData.DirectMovePower = passivePower;
//        }
//        else
//        {
//            float disiredMoveDir = Mathf.Sign(speedDiff);
//            float baseAccel = (disiredMoveDir >= 0f ? movementCharacteristicsData.DirectMaxAcceleration : movementCharacteristicsData.ReverseMaxAcceleration) * fixedDT;
//            float smoothAccel = SmoothAcceleration(baseAccel, absSpeedDiff); // логика сглаживания ускорения при скорости близкой к желаемой
//            forwardVel = Mathf.MoveTowards(forwardVel, movementData.TargetSpeed, smoothAccel);
//            movementData.DirectMovePower = Mathf.Lerp(passivePower, disiredMoveDir, (smoothAccel / baseAccel) - _maxSmooth);
//        }
//    }
//    else
//    {
//        if (movementData.Throttle == 0) // если нет тяги то ничего не делаем
//        {
//            movementData.DirectMovePower = 0;
//            return;
//        }

//        float maxSpeed = movementData.Throttle > 0f
//                ? movementCharacteristicsData.DirectMaxSpeed
//                : -movementCharacteristicsData.ReverseMaxSpeed;

//        float speedDiff = maxSpeed - forwardVel;
//        float absSpeedDiff = Mathf.Abs(speedDiff);

//        if (absSpeedDiff < 0.0001f) // если изменение скорости около нулевое
//        {
//            forwardVel = maxSpeed;
//            movementData.DirectMovePower = 0;
//        }
//        else
//        {
//            float baseAccel = (movementData.Throttle > 0f ? movementCharacteristicsData.DirectMaxAcceleration : -movementCharacteristicsData.ReverseMaxAcceleration) * fixedDT * movementData.Throttle;
//            float smoothAccel = SmoothAcceleration(baseAccel, absSpeedDiff); // логика сглаживания ускорения при скорости близкой к максимальной
//            forwardVel = Mathf.MoveTowards(forwardVel, maxSpeed, smoothAccel);

//            // если скорость далека от максимальной (если нет сглаживания)
//            if (baseAccel == smoothAccel) movementData.DirectMovePower = movementData.Throttle;
//            else movementData.DirectMovePower = Mathf.Lerp(0, Mathf.Sign(movementData.Throttle), (smoothAccel / baseAccel) - _maxSmooth);
//        }
//    }
//}

//private float SmoothAcceleration(float acceleration, float absSpeedDiff)
//{
//    if (absSpeedDiff < _smoothZone) acceleration *= Mathf.Lerp(_maxSmooth, 1, absSpeedDiff / _smoothZone);
//    return acceleration;
//}


//// нужна логика быстрого гашения боковой скорости
//private void CalcSideVelocity(float fixedDT, ref float sideVel, ref MovementRuntimeData movementData, in MovementStaticData movementCharacteristicsData)
//{
//    if (movementData.InertiaDampingState)
//    {
//        float targetSpeed = 0;
//        float accelBase = movementCharacteristicsData.StrafeMaxAcceleration * fixedDT;

//        if (_inputValue.x != 0) //если есть боковой инпут
//        {
//            targetSpeed = _inputValue.x > 0 ? movementCharacteristicsData.StrafeMaxSpeed : -movementCharacteristicsData.StrafeMaxSpeed;
//            movementData.StrafeMovePower = _inputValue.x;
//        }
//        else
//        {
//            float speedDiff = targetSpeed - sideVel;

//            if (Mathf.Abs(speedDiff) < 0.0001f) // если изменение скорости около нулевое
//            {
//                sideVel = targetSpeed;
//                movementData.StrafeMovePower = 0;
//                return;
//            }

//            movementData.StrafeMovePower = Mathf.Sign(-sideVel);
//        }

//        sideVel = Mathf.MoveTowards(sideVel, targetSpeed, accelBase);
//    }
//    else
//    {
//        if (_inputValue.x != 0) //если есть боковой инпут
//        {
//            float accelBase = movementCharacteristicsData.StrafeMaxAcceleration * fixedDT;
//            float targetSpeed = _inputValue.x > 0 ? movementCharacteristicsData.StrafeMaxSpeed : -movementCharacteristicsData.StrafeMaxSpeed;
//            float speedDiff = targetSpeed - sideVel;

//            if (Mathf.Abs(speedDiff) < 0.0001f) // если изменение скорости около нулевое
//            {
//                sideVel = targetSpeed;
//                movementData.StrafeMovePower = 0;
//                return;
//            }

//            sideVel = Mathf.MoveTowards(sideVel, targetSpeed, accelBase);
//            movementData.StrafeMovePower = _inputValue.x;
//        }
//        else
//        {
//            movementData.StrafeMovePower = 0;
//        }
//    }
//}