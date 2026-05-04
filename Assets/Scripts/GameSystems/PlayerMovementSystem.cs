using DI;
using GameInput;
using Registries;
using Ships;
using UnityEngine;
using MouseCursor = GameCamera.MouseCursor;

namespace GameSystems
{
    public class PlayerMovementSystem : GameSystemBase, IFixedUpdateTickObserver
    {
        private const float EPS = 0.001f;
        private const float _minAcceleration = 0.005f; // 0.03f минимальный коэффициент (чтобы не было "залипания") при модификации ускорения
        private const float _noDumpingThrottleMod = 4; // модификатор изменения дросселя если выключены гасители инерции. Будто чуствительность перекладывания.
        private const float _smoothZoneTime = 0.2f; // время до ключевой скорости для сглаживания ускорения
        private const float _damperMaxFactor = 5f; // усиление гасителей при макс скорости (будто выше сопротивление)
        private const float _damperMinFactor = 0.5f; // сила гасителей при минимальной скорости (чтобы не залипало)
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

        [Inject]
        public void Construct(IPlayerInput playerInput, MouseCursor mouseCursor, ShipRegistry shipRegystry)
        {
            _shipRegystry = shipRegystry;
            _input = playerInput;
            _mouseCursor = mouseCursor;
            _smoothZoneMod = 1f / _smoothZoneTime;
            ActiveGameState = GameState.CoreGameplay;
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            _input.MoveInputAction += OnMoveInputAction;
            _input.ToggleDamperAction += OnToggleDamper;
            _input.DisableEngineAction += DisableEngine;
            _input.ChangeBoostersState += OnToogleBoosters;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            _input.MoveInputAction -= OnMoveInputAction;
            _input.ToggleDamperAction -= OnToggleDamper;
            _input.DisableEngineAction -= DisableEngine;
            _input.ChangeBoostersState -= OnToogleBoosters;
        }

        public void FixedUpdateTick(float fixedDT)
        {
            Simulate(fixedDT);
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

                if (boostersPower < minPowerForEnable) return;
            }

            movementRuntimeData.BoostersIsActive = state;
        }

        private void Simulate(float fixedDT)
        {
            if (!SystemIsActive) return;

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

            if (movementRuntimeData.BoostersIsActive) return;

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

            if (boostersIsActive) return;

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
                else return;
            }

            //ApplyDampingSmooth(ref acceleration, forwardVel, maxSpeed);
            //forwardVel = Mathf.MoveTowards(forwardVel, desiredSpeed, acceleration * fixedDT);
            forwardVel += (desiredSpeed - forwardVel) * acceleration * fixedDT;
        }

        private void ApplyStrafeDamping(float fixedDT, float throttle, ref float sideVel, in MovementStaticData movementStaticData)
        {
            if (Mathf.Abs(sideVel) < EPS) // если не двигаемся. 
            {
                sideVel = 0;
                return;
            }

            var acceleration = movementStaticData.StrafeDampingAcceleration;
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
                else return;
            }

            //ApplyDampingSmooth(ref acceleration, sideVel, maxSpeed);
            //sideVel = Mathf.MoveTowards(sideVel, desiredSpeed, acceleration * fixedDT);
            sideVel += (desiredSpeed - sideVel) * acceleration * fixedDT;
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

            if (speedDiff * throttle <= 0f) return; // проверка если скорость достигла максимальной

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

            if (speedDiff * throttle <= 0f) return;// проверка если скорость достигла максимальной

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