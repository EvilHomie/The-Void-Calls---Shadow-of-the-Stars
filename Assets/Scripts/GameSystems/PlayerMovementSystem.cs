using DI;
using GameInput;
using Ships;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameSystems
{
    public class PlayerMovementSystem : GameSystemBase
    {
        private const float EPS = 0.001f;
        private readonly float _minAcceleration = 0.005f; // 0.03f минимальный коэффициент (чтобы не было "залипания") при модификации ускорения
        private readonly float _noDumpingThrottleMod = 4; // модификатор изменения дросселя если выключены гасители инерции. Будто чуствительность перекладывания.
        private readonly float _smoothZoneTime = 0.2f; // время до ключевой скорости для сглаживания ускорения
        private float _smoothZoneMod; // 1/ _smoothZoneTime. сугубо чтобы уйти от деления в логике
        private readonly float _damperMaxFactor = 2f; // усиление гасителей при макс скорости (будто выше сопротивление)
        private readonly float _damperMinFactor = 0.2f; // сила гасителей при минимальной скорости (чтобы не залипало)
        private readonly float _rotateSlowAngle = 20f;
        private readonly float _minMouseDistanceSQR = 0.1f;


        private IPlayerInput _input;
        private Vector2 _inputValue;
        private Camera _camera;
        private ShipInstance _playerShip;
        private float _throttleZeroDelayTimer;
        private readonly float _throttleZeroDelay = 0.3f;

        [Inject]
        public void Construct(IPlayerInput playerInput, Camera camera)
        {
            _input = playerInput;
            _camera = camera;
            _smoothZoneMod = 1 / _smoothZoneTime;
        }

        protected override void AwakeInit() { }

        protected override void Subscribe()
        {
            _input.MoveInputAction += OnMoveInputAction;
            _input.ToggleDamperAction += OnToggleDamper;
            _input.DisableEngineAction += DisableEngine;
            GameFlowSystem.FixedGameTick += Simulate;
            EventBus.SpawnPlayerShip += OnSpawnPlayerShip;
        }

        protected override void Unsubscribe()
        {
            _input.MoveInputAction -= OnMoveInputAction;
            _input.ToggleDamperAction -= OnToggleDamper;
            _input.DisableEngineAction -= DisableEngine;
            GameFlowSystem.FixedGameTick -= Simulate;
            EventBus.SpawnPlayerShip -= OnSpawnPlayerShip;
        }

        private void OnSpawnPlayerShip(ShipInstance shipInstance)
        {
            _playerShip = shipInstance;
        }

        private void OnMoveInputAction(Vector2 input)
        {
            _inputValue.x = input.x;
            _inputValue.y = input.y;
        }

        private void OnToggleDamper()
        {
            ref var movementData = ref _playerShip.MovementRuntimeData;
            movementData.InertiaDampingActive = !movementData.InertiaDampingActive;

            if (movementData.InertiaDampingActive)
            {
                movementData.DirectThrottle = movementData.LastDampingThrottle;
            }
            else
            {
                movementData.LastDampingThrottle = movementData.DirectThrottle;
                movementData.DirectThrottle = 0;
            }
        }

        private void Simulate(float fixedDT)
        {
            ref var movementRuntimeData = ref _playerShip.MovementRuntimeData;
            ref var movementStaticData = ref _playerShip.MovementStaticData;

            var rad = _playerShip.Rigidbody.rotation * Mathf.Deg2Rad;
            var shipForward = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
            var shipRight = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            HandleInput(fixedDT, ref movementRuntimeData);
            HandleRotation(fixedDT, shipForward, movementStaticData, ref movementRuntimeData);
            HandleMovement(fixedDT, ref movementRuntimeData, movementStaticData, shipForward, shipRight);
        }

        private void HandleInput(float fixedDT, ref MovementRuntimeData movementRuntimeData)
        {
            movementRuntimeData.StrafeThrottle = _inputValue.x; // боковое движение ровно инпуту

            if (!movementRuntimeData.InertiaDampingActive) // если гаситель выключен то дросель всегда равен инпуту (будто отстреливает в ноль если нет инпута)
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

        private void DisableEngine()
        {
            ref var movementRuntimeData = ref _playerShip.MovementRuntimeData;
            movementRuntimeData.DirectThrottle = 0;
        }

        private void HandleRotation(float fixedDT, Vector2 shipForward, in MovementStaticData movementStaticData, ref MovementRuntimeData movementRuntimeData)
        {
            var rb = _playerShip.Rigidbody;

            float maxSpeed = movementStaticData.RotateSpeed;
            float velocity = rb.angularVelocity;

            Vector2 mouseWorld = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 dir = mouseWorld - rb.position;

            bool targetOutSideShip = dir.sqrMagnitude >= _minMouseDistanceSQR;

            if (Mathf.Abs(velocity) > maxSpeed || !targetOutSideShip) // только гашение если больше максимального или мышка на корабле
            {
                velocity = Mathf.MoveTowards(velocity, 0f, maxSpeed * fixedDT);
                rb.angularVelocity = velocity;

                movementRuntimeData.RotateThrottle = Mathf.Abs(velocity) < EPS ? 0f : velocity / maxSpeed;
                return;
            }

            var angle = Vector2.SignedAngle(shipForward, dir);
            var absAngle = Mathf.Abs(angle);

            var power = Mathf.InverseLerp(0f, _rotateSlowAngle, absAngle) * Mathf.Sign(angle);
            var newVelocity = power * maxSpeed;
            rb.angularVelocity = newVelocity;

            movementRuntimeData.RotateThrottle = Mathf.Abs(newVelocity) < EPS ? 0f : newVelocity / maxSpeed;
        }







        private void HandleMovement(float fixedDT, ref MovementRuntimeData movementRuntimeData, in MovementStaticData movementStaticData, Vector2 forward, Vector2 right)
        {
            var rb = _playerShip.Rigidbody;
            var velocity = rb.linearVelocity;
            var forwardVel = Vector2.Dot(velocity, forward);
            var sideVel = Vector2.Dot(velocity, right);

            var directThrottle = movementRuntimeData.DirectThrottle;
            var strafeThrottle = movementRuntimeData.StrafeThrottle;

            if (movementRuntimeData.InertiaDampingActive)
            {
                ApplyMainEngineDampingForce(fixedDT, directThrottle, ref forwardVel, movementStaticData);
                //CalcSideVelocity(fixedDT, ref sideVel, ref movementData, movementStaticData);
                ApplyForwardDamping(fixedDT, directThrottle, ref forwardVel, movementStaticData);
            }
            else
            {
                ApplyMainEngineAcceleration(fixedDT, directThrottle, ref forwardVel, movementStaticData);
                ApplyThrustersAcceleration(fixedDT, strafeThrottle, ref sideVel, movementStaticData);
            }

            rb.linearVelocity = right * sideVel + forward * forwardVel;

            movementRuntimeData.MainEnginePower = directThrottle;
            movementRuntimeData.ThrustersPower = strafeThrottle;
        }

        private void ApplyForwardDamping(float fixedDT, float throttle, ref float forwardVel, in MovementStaticData movementStaticData)
        {
            if (Mathf.Abs(forwardVel) < EPS) // если не двигаемся. 
            {
                forwardVel = 0;
                return;
            }

            var acceleration = forwardVel > 0
                ? movementStaticData.DirectDampingAcceleration
                : movementStaticData.ReverseDampingAcceleration;

            var maxSpeed = throttle > 0
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


            var k = Mathf.Abs(forwardVel) / maxSpeed;
            var curve = k * k;
            var factor = Mathf.Lerp(_damperMinFactor, _damperMaxFactor, curve); // Усиливаем гашение на макс скорости и уменьнаем при минимальной
            acceleration *= factor;
            forwardVel = Mathf.MoveTowards(forwardVel, desiredSpeed, acceleration * fixedDT);
























            //if (speedDiff * throttle < -EPS)
            //{
            //    Debug.LogError("гасим");
            //}

            //if (throttle == 0f)
            //{
            //    if (Mathf.Abs(forwardVel) > EPS)
            //    {
            //        Debug.LogError("гасим к нулю");
            //    }

            //    return;
            //}

            //if (Mathf.Abs(forwardVel) > Mathf.Abs(targetSpeed))
            //{
            //    Debug.LogError("ГАсим");
            //}




            //if (movementRuntimeData.InertiaDampingActive)
            //    {
            //        float maxSpeed;

            //        if (throttle == 0)
            //        {
            //            if (Mathf.Abs(forwardVel) < 0.0001f)
            //            {
            //                forwardVel = 0;
            //                movementRuntimeData.DirectPower = 0;
            //                return;
            //            }

            //            maxSpeed = forwardVel > 0
            //                ? movementStaticData.ReverseMaxSpeed
            //                : movementStaticData.DirectMaxSpeed;
            //        }
            //        else
            //        {
            //            maxSpeed = throttle > 0
            //                ? movementStaticData.DirectMaxSpeed
            //                : movementStaticData.ReverseMaxSpeed;
            //        }

            //        var targetSpeed = throttle * maxSpeed;
            //        var speedDiff = targetSpeed - forwardVel;
            //        var absSpeedDiff = Mathf.Abs(speedDiff);

            //        if (absSpeedDiff < 0.0001f) // если изменение скорости около нулевое
            //        {
            //            forwardVel = targetSpeed;
            //            return;
            //        }

            //        var speedDiffSign = Mathf.Sign(speedDiff);
            //        var acceleration = speedDiffSign > 0
            //            ? movementStaticData.DirectAcceleration
            //            : movementStaticData.ReverseAcceleration;

            //        ApplySmooth(ref acceleration, absSpeedDiff, maxSpeed);
            //        forwardVel = Mathf.MoveTowards(forwardVel, targetSpeed, acceleration * fixedDT);

            //        movementRuntimeData.DirectPower = throttle;
            //    }
            //    else
            //    {
            //        if (throttle == 0) // если дросель в нуле то ничего не делаем
            //        {
            //            return;
            //        }

            //        var acceleration = throttle > 0
            //            ? movementStaticData.DirectAcceleration
            //            : movementStaticData.ReverseAcceleration;

            //        forwardVel += acceleration * throttle * fixedDT;
            //        movementRuntimeData.DirectPower = throttle;
            //    }


        }

        private void ApplyMainEngineDampingForce(float fixedDT, float throttle, ref float forwardVel, in MovementStaticData movementStaticData)
        {
            if (throttle == 0)
            {
                return;
            }

            var acceleration = throttle > 0
                      ? movementStaticData.DirectAcceleration
                      : -movementStaticData.ReverseAcceleration;

            var maxSpeed = throttle > 0
                      ? movementStaticData.DirectMaxSpeed
                      : movementStaticData.ReverseMaxSpeed;

            // speedDiff и acceleration гарантированно одного знака
            var targetSpeed = throttle * maxSpeed;
            var speedDiff = targetSpeed - forwardVel;

            if (speedDiff * throttle <= 0f) // проверка если скорость достигла максимальной
            {
                return;
            }

            ApplySmooth(ref acceleration, speedDiff);
            forwardVel += acceleration * fixedDT;
        }

        private void ApplyMainEngineAcceleration(float fixedDT, float throttle, ref float forwardVel, in MovementStaticData movementStaticData)
        {
            if (throttle == 0) // если дросель в нуле то ничего не делаем
            {
                return;
            }

            var acceleration = throttle > 0
                ? movementStaticData.DirectAcceleration
                : movementStaticData.ReverseAcceleration;

            forwardVel += acceleration * throttle * fixedDT;
        }
        private void ApplyThrustersAcceleration(float fixedDT, float throttle, ref float sideVel, in MovementStaticData movementStaticData)
        {
            if (throttle == 0) // если дросель в нуле то ничего не делаем
            {
                return;
            }

            var acceleration = movementStaticData.StrafeAcceleration;
            sideVel += acceleration * throttle * fixedDT;
        }

        private void ApplySmooth(ref float acceleration, float speedDiff)
        {
            float timeToTarget = speedDiff / acceleration;

            if (timeToTarget > _smoothZoneTime) return;

            float t = timeToTarget * _smoothZoneMod;
            float curve = t * t;
            float accelFactor = Mathf.Lerp(_minAcceleration, 1f, curve);
            acceleration *= accelFactor;
        }


        private void ApplyThrustersDampingForce(float fixedDT, ref float sideVel, ref MovementRuntimeData movementRuntimeData, in MovementStaticData movementStaticData)
        {
            var strafeThrottle = movementRuntimeData.StrafeThrottle;

            if (movementRuntimeData.InertiaDampingActive)
            {
                float maxSpeed = movementStaticData.StrafeMaxSpeed;

                if (strafeThrottle == 0)
                {
                    if (Mathf.Abs(sideVel) < 0.0001f)
                    {
                        sideVel = 0;
                        return;
                    }
                }

                var targetSpeed = strafeThrottle * maxSpeed;
                var speedDiff = targetSpeed - sideVel;
                var absSpeedDiff = Mathf.Abs(speedDiff);

                if (absSpeedDiff < 0.0001f) // если изменение скорости около нулевое
                {
                    sideVel = targetSpeed;
                    return;
                }

                var speedDiffSign = Mathf.Sign(speedDiff);
                var acceleration = movementStaticData.StrafeAcceleration;

                ApplySmooth(ref acceleration, absSpeedDiff);
                sideVel = Mathf.MoveTowards(sideVel, targetSpeed, acceleration * fixedDT);
            }
            else
            {
                if (strafeThrottle == 0) // если дросель в нуле то ничего не делаем
                {
                    return;
                }

                var acceleration = movementStaticData.StrafeAcceleration;

                sideVel += acceleration * strafeThrottle * fixedDT;
            }


















            //if (movementData.InertiaDampingState)
            //{
            //    float targetSpeed = 0;
            //    float accelBase = movementCharacteristicsData.StrafeMaxAcceleration * fixedDT;

            //    if (_inputValue.x != 0) //если есть боковой инпут
            //    {
            //        targetSpeed = _inputValue.x > 0 ? movementCharacteristicsData.StrafeMaxSpeed : -movementCharacteristicsData.StrafeMaxSpeed;
            //        movementData.StrafeMovePower = _inputValue.x;
            //    }
            //    else
            //    {
            //        float speedDiff = targetSpeed - sideVel;

            //        if (Mathf.Abs(speedDiff) < 0.0001f) // если изменение скорости около нулевое
            //        {
            //            sideVel = targetSpeed;
            //            movementData.StrafeMovePower = 0;
            //            return;
            //        }

            //        movementData.StrafeMovePower = Mathf.Sign(-sideVel);
            //    }

            //    sideVel = Mathf.MoveTowards(sideVel, targetSpeed, accelBase);
            //}
            //else
            //{
            //    if (_inputValue.x != 0) //если есть боковой инпут
            //    {
            //        float accelBase = movementCharacteristicsData.StrafeMaxAcceleration * fixedDT;
            //        float targetSpeed = _inputValue.x > 0 ? movementCharacteristicsData.StrafeMaxSpeed : -movementCharacteristicsData.StrafeMaxSpeed;
            //        float speedDiff = targetSpeed - sideVel;

            //        if (Mathf.Abs(speedDiff) < 0.0001f) // если изменение скорости около нулевое
            //        {
            //            sideVel = targetSpeed;
            //            movementData.StrafeMovePower = 0;
            //            return;
            //        }

            //        sideVel = Mathf.MoveTowards(sideVel, targetSpeed, accelBase);
            //        movementData.StrafeMovePower = _inputValue.x;
            //    }
            //    else
            //    {
            //        movementData.StrafeMovePower = 0;
            //    }
            //}
        }

























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