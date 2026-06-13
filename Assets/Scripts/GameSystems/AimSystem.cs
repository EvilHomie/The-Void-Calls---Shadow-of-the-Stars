using DI;
using GameCamera;
using Helpers;
using Registries;
using Ships;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Weapons;

namespace GameSystems
{
    public class AimSystem : GameSystemBase, ICorePreUpdateTickObserver
    {
        private ShipRegistry _shipRegistry;
        private MouseCursor _mouseCursor;
        private GameFlowSystem _gameFlowSystem;

        [SerializeField] Transform leadMarkerPrefab;
        [SerializeField] Transform weaponAimMarkerPrefab;

        private Transform _leadMarker;
        private bool _showLeadMarker;
        private List<Transform> _weaponsAimMarkers;


        [Inject]
        public void Construct(ShipRegistry shipRegistry, MouseCursor mouseCursor, GameFlowSystem gameFlowSystem)
        {
            _mouseCursor = mouseCursor;
            _shipRegistry = shipRegistry;
            _gameFlowSystem = gameFlowSystem;
        }

        protected override void AwakeInit()
        {
            base.AwakeInit();
            _weaponsAimMarkers = new();

            for (int i = 0; i < WorldConfig.MaxMainWeaponSlotsCount; i++)
            {
                var weaponMarker = Instantiate(weaponAimMarkerPrefab, transform);
                weaponMarker.gameObject.SetActive(false);
                _weaponsAimMarkers.Add(weaponMarker);
            }

            _leadMarker = Instantiate(leadMarkerPrefab, transform);
            _leadMarker.gameObject.SetActive(false);
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            _gameFlowSystem.GameStateChanged += OnGameStateChanged;
            EventBus.PlayerSwitchWeaponsGroupAction += OnPlayerSwitchWeaponGroup;
            EventBus.ChangeTargetAction += OnChangeTarget;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            _gameFlowSystem.GameStateChanged -= OnGameStateChanged;
            EventBus.PlayerSwitchWeaponsGroupAction -= OnPlayerSwitchWeaponGroup;
            EventBus.ChangeTargetAction -= OnChangeTarget;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state != GameState.CoreGameplay) DisablePlayerAimMarkers();
            else ActivatePlayerAimMarkers();
        }

        private void OnChangeTarget(ShipInstance instance, Rigidbody2D targetRB)
        {
            var aimData = instance.AimData;
            aimData.TargetRigidBody = targetRB;
        }

        private void OnPlayerSwitchWeaponGroup()
        {
            DisablePlayerAimMarkers();
            ActivatePlayerAimMarkers();
        }

        private void DisablePlayerAimMarkers()
        {
            foreach (var marker in _weaponsAimMarkers) marker.gameObject.SetActive(false);
            _leadMarker.gameObject.SetActive(false);
        }

        private void ActivatePlayerAimMarkers()
        {
            var playerShip = _shipRegistry.PlayerShip;
            var aimData = playerShip.AimData;

            ActiveMainWeaponsMarkers(playerShip.WeaponSlots, aimData);

            if (aimData.FastestBoltSpeed == 0 || aimData.TargetRigidBody == null) return;

            _leadMarker.gameObject.SetActive(true);
            _showLeadMarker = true;
        }

        private void ActiveMainWeaponsMarkers(List<WeaponSlot> weaponSlots, AimData aimData)
        {
            var fastestProjectileSpeed = 0f;

            for (int i = 0; i < weaponSlots.Count; i++)
            {
                var weaponSlot = weaponSlots[i];

                if (!weaponSlot.IsInActiveGroup) continue;

                var aimMarker = _weaponsAimMarkers[i];
                aimMarker.gameObject.SetActive(true);

                var weapon = weaponSlot.Weapon;

                if (weapon is not IBoltWeapon boltWeapon) continue;

                if (fastestProjectileSpeed < boltWeapon.ProjectileSpeed)
                {
                    fastestProjectileSpeed = boltWeapon.ProjectileSpeed;
                    aimData.FastetsBoltWeapon = weapon;
                }
            }

            aimData.FastestBoltSpeed = fastestProjectileSpeed;
        }



        public void CorePreUpdateTick()
        {
            var playerShip = _shipRegistry.PlayerShip;
            var playerAimData = playerShip.AimData;
            playerAimData.AimPosition = _mouseCursor.WorldPostition;

            UpdatePlayerMainWeaponsAimMarkers(playerAimData, playerShip.WeaponSlots);

            if (_showLeadMarker)
            {
                _leadMarker.position = GetTargetLeadPosition(playerShip);
            }

            foreach (var ship in _shipRegistry.ShipsInFight)
            {
                // нужна будет логика по расчету точки прицеливания = упреждению как у игрока
                //ref var shipAimData = ref ship.AimData; 
                //var targetRigidBody = shipAimData.TargetRigidBody;
                //shipAimData.AimPosition = targetRigidBody.position;
            }
        }

        private void UpdatePlayerMainWeaponsAimMarkers(AimData aimData, List<WeaponSlot> weaponSlots)
        {
            for (int i = 0; i < weaponSlots.Count; i++)
            {
                var weapon = weaponSlots[i].Weapon;
                ref var shootPointData = ref weapon.ShootPointData;
                ref var aimStats = ref weapon.AimStats;
                var distanceToAimPosition = Vector2.Distance(aimData.AimPosition, shootPointData.Position);
                var aimDistance = Mathf.Min(aimStats.MaxDistance, distanceToAimPosition);
                _weaponsAimMarkers[i].position = shootPointData.Position + shootPointData.Direction * aimDistance;
            }
        }

        private Vector2 GetTargetLeadPosition(ShipInstance shipInstance)
        {
            //var weaponShooterPos = Vector2.zero;
            //int activeWeaponsCount = 0;

            //for (int i = 0; i < shipInstance.WeaponSlots.Count; i++)
            //{
            //    var weaponSlot = shipInstance.WeaponSlots[i];

            //    if (!weaponSlot.IsInActiveGroup) continue;

            //    var weapon = weaponSlot.Weapon;

            //    if (weapon.WeaponType == WeaponType.BoltRepeater)
            //    {
            //        bool isInActiveGroup = weaponSlot.WeaponGroup.ContainsAny(shipInstance.ActiveWeaponGroup);

            //        if (isInActiveGroup)
            //        {
            //            activeWeaponsCount++;
            //            ref var shootPointData = ref weapon.ShootPointData;
            //            shooterPos += shootPointData.Position;
            //        }
            //    }
            //}

            //if (activeWeaponsCount == 0) return false;

            //shooterPos /= activeWeaponsCount;

            var aimData = shipInstance.AimData;

            var weaponShooterPos = aimData.FastetsBoltWeapon.ShootPointData.Position;
            var targetRB = aimData.TargetRigidBody;
            var shooterRB = shipInstance.Rigidbody;

            var shooterVelocity = shooterRB.linearVelocity;
            var targetVelocity = targetRB.linearVelocity;
            var targetPosition = targetRB.position;

            var toTarget = (targetPosition - weaponShooterPos).normalized;
            var distanceToTarget = Vector2.Distance(targetPosition, weaponShooterPos);

            // Полная скорость снаряда после выстрела
            var projectileVelocity = shooterVelocity + toTarget * aimData.FastestBoltSpeed;
            // Расчет времени перехвата
            var projectileToTargetSpeed = Vector2.Dot(projectileVelocity, toTarget);

            if (projectileToTargetSpeed <= 0.001f) // Если снаряд не может достигнуть цели.
            {
                return targetPosition;
            }

            var timeToReach = distanceToTarget / projectileToTargetSpeed;
            // Скорость цели относительно снаряда
            var relativeVelocity = targetVelocity - projectileVelocity;
            // Боковая составляющая относительно линии выстрела
            var tangentialVelocity = relativeVelocity - Vector2.Dot(relativeVelocity, toTarget) * toTarget;
            // Продольная составляющая цели
            var targetRadialVelocity = Vector2.Dot(targetVelocity, toTarget) * toTarget;
            return targetPosition + (tangentialVelocity + targetRadialVelocity) * timeToReach;
        }
    }
}