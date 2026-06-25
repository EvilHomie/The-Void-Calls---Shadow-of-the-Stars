using DI;
using Registries;
using Ships;
using UnityEngine;

namespace CoreGameSystems
{
    public class PlayerMovementSystem : MonoBehaviour/*, ICoreFixedUpdateTickObserver*/
    {
        private const float EPS = 0.001f;
        private const float _minAcceleration = 0.005f; // 0.03f минимальный коэффициент (чтобы не было "залипания") при модификации ускорения
        private const float _noDumpingThrottleMod = 4; // модификатор изменения дросселя если выключены гасители инерции. Будто чуствительность перекладывания.
        private const float _smoothZoneTime = 0.2f; // время до ключевой скорости для сглаживания ускорения
        private const float _damperMaxFactor = 5f; // усиление гасителей при макс скорости (будто выше сопротивление)
        private const float _damperMinFactor = 0.5f; // сила гасителей при минимальной скорости (чтобы не залипало)
        private const float _rotateSlowAngle = 20f;
        private const float _minBoostersPowerForEnableMod = 0.2f;
        private const float _throttleZeroDelay = 0.3f;

        private float _smoothZoneMod; // 1/ _smoothZoneTime. сугубо чтобы уйти от деления в логике
        private float _throttleZeroDelayTimer; // текущий таймер остановки на нуле

        private ShipRegistry _shipRegystry;

        [Inject]
        public void Construct(ShipRegistry shipRegystry)
        {
            _shipRegystry = shipRegystry;
            _smoothZoneMod = 1f / _smoothZoneTime;
        }

        public void Execute(float fixedDT)
        {
            var playerShip = _shipRegystry.PlayerShip;
            var aimPos = playerShip.AimData.AimPosition;

            var rb = playerShip.Rigidbody;
            var rotation = rb.rotation;
            var angularVelocity = rb.angularVelocity;
            var linearVelocity = rb.linearVelocity;
            var shipPos = rb.position;

            ref var movementRuntimeData = ref playerShip.MovementRuntimeData;
            ref var movementStats = ref playerShip.MovementStats;
            ref readonly var intentData = ref playerShip.IntentData;

            var rad = rotation * Mathf.Deg2Rad;
            var shipForward = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
            var shipRight = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            var directionToTarget = aimPos - shipPos;
            var targetInsideShip = playerShip.HullCollider.OverlapPoint(aimPos);

            if (intentData.DamperEnabled != movementRuntimeData.InertiaDampingIsActive)
            {
                OnToggleDamper(ref movementRuntimeData, intentData.DamperEnabled);
            }

            var moveDirection = intentData.MoveDirection;

            if (intentData.ResetThrottle)
            {
                moveDirection.y = 0;
                movementRuntimeData.DirectThrottle = 0;
            }

            if (intentData.BoostersIsActive != movementRuntimeData.BoostersIsActive)
            {
                OnToogleBoosters(intentData.BoostersIsActive);
            }

            UpdateBoostersPower(fixedDT, movementStats, ref movementRuntimeData);
            HandleInput(fixedDT, ref movementRuntimeData, moveDirection);
            HandleRotation(fixedDT, ref angularVelocity, targetInsideShip, directionToTarget, shipForward, movementStats, ref movementRuntimeData);
            HandleMovement(fixedDT, ref linearVelocity, ref movementRuntimeData, movementStats, shipForward, shipRight);

            rb.angularVelocity = angularVelocity;
            rb.linearVelocity = linearVelocity;
        }

        private void OnToggleDamper(ref MovementRuntimeData movementRuntimeData, bool newState)
        {
            movementRuntimeData.InertiaDampingIsActive = newState;

            if (newState)
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
            ref var movementStats = ref playerShip.MovementStats;

            if (state)
            {
                movementRuntimeData.DirectThrottle = 1; // всегда включаем двигатель на макс есть был запрос через буст

                var boostersPower = movementRuntimeData.BoostersPower;
                var minPowerForEnable = movementStats.BoostersMaxPower * _minBoostersPowerForEnableMod;

                if (boostersPower < minPowerForEnable) return;
            }

            movementRuntimeData.BoostersIsActive = state;
        }

        private void HandleInput(float fixedDT, ref MovementRuntimeData movementRuntimeData, in Vector2 moveDirection)
        {
            movementRuntimeData.StrafeThrottle = moveDirection.x; // боковое движение ровно инпуту

            if (movementRuntimeData.BoostersIsActive) return;

            if (!movementRuntimeData.InertiaDampingIsActive) // если гаситель выключен то дросель всегда равен инпуту (будто отстреливает в ноль если нет инпута)
            {
                movementRuntimeData.DirectThrottle = Mathf.MoveTowards(movementRuntimeData.DirectThrottle, moveDirection.y, _noDumpingThrottleMod * fixedDT);
                return;
            }

            if (moveDirection.y == 0) // если нет инпута на изменение дросселя то сбросс таймера остановки на нуле
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
            var newThrottle = prevThrottle + moveDirection.y * fixedDT;

            if ((prevThrottle < 0f && newThrottle >= 0f) || (prevThrottle > 0f && newThrottle <= 0f))
            {
                newThrottle = 0;
                _throttleZeroDelayTimer = _throttleZeroDelay;
            }

            movementRuntimeData.DirectThrottle = Mathf.Clamp(newThrottle, -1, 1);
        }

        private void UpdateBoostersPower(float fixedDT, in MovementStats movementStats, ref MovementRuntimeData movementRuntimeData)
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

            boostersPower = Mathf.Clamp(boostersPower, 0, movementStats.BoostersMaxPower);
            movementRuntimeData.BoostersPower = boostersPower;
        }

        private void HandleRotation(float fixedDT, ref float angularVelocity, bool targetInsideShip, Vector2 direction, Vector2 shipForward, in MovementStats movementStats, ref MovementRuntimeData movementRuntimeData)
        {
            float maxSpeed = movementStats.RotateSpeed;

            if (Mathf.Abs(angularVelocity) > maxSpeed || targetInsideShip) // только гашение если больше максимального или мышка на корабле
            {
                angularVelocity = Mathf.MoveTowards(angularVelocity, 0f, maxSpeed * fixedDT);
                //movementRuntimeData.RotatePower = -Mathf.Sign(angularVelocity);
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

        private void HandleMovement(float fixedDT, ref Vector2 linearVelocity, ref MovementRuntimeData movementRuntimeData, in MovementStats movementStats, Vector2 forward, Vector2 right)
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
                    ApplyMainEngineForce(fixedDT, directThrottle, ref forwardVel, movementStats, boostersIsActive);
                }
                else
                {
                    AddMainEngineForce(fixedDT, directThrottle, ref forwardVel, movementStats, boostersIsActive);
                }
            }

            if (strafeThrottle != 0)
            {
                if (movementRuntimeData.InertiaDampingIsActive)
                {
                    ApplyThrustersForce(fixedDT, strafeThrottle, ref sideVel, movementStats);
                }
                else
                {
                    AddThrustersForce(fixedDT, strafeThrottle, ref sideVel, movementStats);
                }
            }

            if (movementRuntimeData.InertiaDampingIsActive)
            {
                ApplyDirectDamping(fixedDT, directThrottle, ref forwardVel, movementStats, boostersIsActive);
                ApplyStrafeDamping(fixedDT, strafeThrottle, ref sideVel, movementStats);
            }

            movementRuntimeData.MainEnginePower = boostersIsActive ? 1 : directThrottle;
            movementRuntimeData.StrafePower = strafeThrottle;
            linearVelocity = right * sideVel + forward * forwardVel;
        }

        private void ApplyDirectDamping(float fixedDT, float throttle, ref float forwardVel, in MovementStats movementStats, bool boostersIsActive)
        {
            if (Mathf.Abs(forwardVel) < EPS) // если не двигаемся. 
            {
                forwardVel = 0;
                return;
            }

            if (boostersIsActive) return;

            var acceleration = forwardVel > 0
                ? movementStats.DirectDampingAcceleration
                : movementStats.ReverseDampingAcceleration;

            var maxSpeed = forwardVel > 0
                ? movementStats.DirectMaxSpeed
                : movementStats.ReverseMaxSpeed;

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

        private void ApplyStrafeDamping(float fixedDT, float throttle, ref float sideVel, in MovementStats movementStats)
        {
            if (Mathf.Abs(sideVel) < EPS) // если не двигаемся. 
            {
                sideVel = 0;
                return;
            }

            var acceleration = movementStats.StrafeDampingAcceleration;
            var maxSpeed = movementStats.StrafeMaxSpeed;

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

        private void ApplyMainEngineForce(float fixedDT, float throttle, ref float forwardVel, in MovementStats movementStats, bool boostersIsActive)
        {
            float acceleration;
            float maxSpeed;

            if (boostersIsActive)
            {
                acceleration = movementStats.BoostersAcceleration;
                maxSpeed = movementStats.BoostersMaxSpeed;
            }
            else
            {
                acceleration = throttle > 0
                      ? movementStats.DirectAcceleration
                      : -movementStats.ReverseAcceleration;

                maxSpeed = throttle > 0
                          ? movementStats.DirectMaxSpeed
                          : movementStats.ReverseMaxSpeed;
            }

            var targetSpeed = throttle * maxSpeed;
            var speedDiff = targetSpeed - forwardVel;

            if (speedDiff * throttle <= 0f) return; // проверка если скорость достигла максимальной

            ApplyAccelerationSmooth(ref acceleration, speedDiff);
            forwardVel += acceleration * fixedDT;
        }

        private void ApplyThrustersForce(float fixedDT, float throttle, ref float sideVel, in MovementStats movementStats)
        {
            var acceleration = throttle > 0
                      ? movementStats.StrafeAcceleration
                      : -movementStats.StrafeAcceleration;

            var maxSpeed = movementStats.StrafeMaxSpeed;

            var targetSpeed = throttle * maxSpeed;
            var speedDiff = targetSpeed - sideVel;

            if (speedDiff * throttle <= 0f) return;// проверка если скорость достигла максимальной

            ApplyAccelerationSmooth(ref acceleration, speedDiff);
            sideVel += acceleration * fixedDT;
        }

        private void AddMainEngineForce(float fixedDT, float throttle, ref float forwardVel, in MovementStats movementStats, bool boostersIsActive)
        {
            float acceleration;

            if (boostersIsActive)
            {
                acceleration = movementStats.BoostersAcceleration;
            }
            else
            {
                acceleration = throttle > 0
                ? movementStats.DirectAcceleration
                : movementStats.ReverseAcceleration;
            }

            forwardVel += acceleration * throttle * fixedDT;
        }
        private void AddThrustersForce(float fixedDT, float throttle, ref float sideVel, in MovementStats movementStats)
        {
            var acceleration = movementStats.StrafeAcceleration;
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