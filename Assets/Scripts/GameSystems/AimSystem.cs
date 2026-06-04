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
        private bool _leadMarkerIsActive;
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
            EventBus.PlayerSwitchWeaponGroupAction += OnPlayerSwitchWeaponGroup;
            EventBus.ChangeTargetAction += OnChangeTarget;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            _gameFlowSystem.GameStateChanged -= OnGameStateChanged;
            EventBus.PlayerSwitchWeaponGroupAction -= OnPlayerSwitchWeaponGroup;
            EventBus.ChangeTargetAction -= OnChangeTarget;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state != GameState.CoreGameplay) DisableMarkers();
            else EnableActiveWeaponsMarkers();
        }

        private void OnChangeTarget(ShipInstance instance, Rigidbody2D targetRB)
        {
            var aimData = instance.AimData;
            aimData.TargetRigidBody = targetRB;
        }

        private void DisableMarkers()
        {
            foreach (var marker in _weaponsAimMarkers) marker.gameObject.SetActive(false);
            _leadMarker.gameObject.SetActive(false);
        }

        private void EnableActiveWeaponsMarkers()
        {
            var playerShip = _shipRegistry.PlayerShip;
            var aimData = playerShip.AimData;
            aimData.FastestProjectileSpeed = 0;

            for (int i = 0; i < playerShip.WeaponSlots.Count; i++)
            {
                var weaponSlot = playerShip.WeaponSlots[i];
                var weapon = weaponSlot.Weapon;

                bool isInActiveGroup = weaponSlot.WeaponGroup.ContainsAny(playerShip.ActiveWeaponGroup);

                if (isInActiveGroup)
                {
                    var aimMarker = _weaponsAimMarkers[i];
                    aimMarker.gameObject.SetActive(true);
                }

                if (weapon is IProjectileWeapon projectileWeapon)
                {
                    if (isInActiveGroup && aimData.FastestProjectileSpeed < projectileWeapon.ProjectileSpeed)
                    {
                        aimData.FastestProjectileSpeed = projectileWeapon.ProjectileSpeed;
                    }
                }
            }

            if (aimData.FastestProjectileSpeed != 0)
            {
                _leadMarker.gameObject.SetActive(true);
                _leadMarkerIsActive = true;
            }
        }

        private void OnPlayerSwitchWeaponGroup()
        {
            DisableMarkers();
            EnableActiveWeaponsMarkers();
        }

        public void CorePreUpdateTick()
        {
            var playerShip = _shipRegistry.PlayerShip;
            var playerAimData = playerShip.AimData;
            playerAimData.AimPosition = _mouseCursor.WorldPostition;

            ShowWeaponsAimMarkers(playerAimData, playerShip.WeaponSlots);
            ShowTargetLeadMarker(playerAimData, playerShip);

            foreach (var ship in _shipRegistry.ShipsInFight)
            {
                // нужна будет логика по расчету точки прицеливания = упреждению как у игрока
                //ref var shipAimData = ref ship.AimData; 
                //var targetRigidBody = shipAimData.TargetRigidBody;
                //shipAimData.AimPosition = targetRigidBody.position;
            }
        }

        private void ShowWeaponsAimMarkers(AimData aimData, List<WeaponSlot> weaponSlots)
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

        private void ShowTargetLeadMarker(AimData aimData, ShipInstance shipInstance)
        {
            bool showLeadMarker;
            if (aimData.TargetRigidBody == null || aimData.FastestProjectileSpeed == 0)
            {
                showLeadMarker = false;
            }
            else
            {
                if (GetTargetLeadPosition(aimData, shipInstance, out Vector2 leadPosition))
                {
                    _leadMarker.position = leadPosition;
                    showLeadMarker = true;
                }
                else
                {
                    showLeadMarker = false;
                }
            }

            _leadMarker.gameObject.SetActive(showLeadMarker);
            _leadMarkerIsActive = showLeadMarker;
        }

        private bool GetTargetLeadPosition(AimData aimData, ShipInstance shipInstance, out Vector2 leadMarkerPos)
        {
            leadMarkerPos = Vector2.zero;
            var shooterAverageWeaponPos = Vector2.zero;
            int activeWeaponsCount = 0;

            for (int i = 0; i < shipInstance.WeaponSlots.Count; i++)
            {
                var weaponSlot = shipInstance.WeaponSlots[i];
                var weapon = weaponSlot.Weapon;

                if (weapon.WeaponType == WeaponType.BoltRepeater)
                {
                    bool isInActiveGroup = weaponSlot.WeaponGroup.ContainsAny(shipInstance.ActiveWeaponGroup);

                    if (isInActiveGroup)
                    {
                        activeWeaponsCount++;
                        ref var shootPointData = ref weapon.ShootPointData;
                        shooterAverageWeaponPos += shootPointData.Position;
                    }
                }
            }

            if (activeWeaponsCount == 0) return false;

            shooterAverageWeaponPos /= activeWeaponsCount;


            var targetRB = aimData.TargetRigidBody;
            var shooterRB = shipInstance.Rigidbody;

            var shooterVelocity = shooterRB.linearVelocity;
            var targetVelocity = targetRB.linearVelocity;
            var targetPosition = targetRB.position;

            var toTarget = (targetPosition - shooterAverageWeaponPos).normalized;
            var distanceToTarget = Vector2.Distance(targetPosition, shooterAverageWeaponPos);

            // Полная скорость снаряда после выстрела
            var projectileVelocity = shooterVelocity + toTarget * aimData.FastestProjectileSpeed;

            // Расчет времени перехвата
            var projectileToTargetSpeed = Vector2.Dot(projectileVelocity, toTarget);
            projectileToTargetSpeed = Mathf.Min(0.001f, projectileToTargetSpeed);
            var timeToReach = distanceToTarget / projectileToTargetSpeed;

            // Скорость цели относительно снаряда
            var relativeVelocity = targetVelocity - projectileVelocity;

            // Боковая составляющая относительно линии выстрела
            var tangentialVelocity = relativeVelocity - Vector2.Dot(relativeVelocity, toTarget) * toTarget;

            // Продольная составляющая цели
            var targetRadialVelocity = Vector2.Dot(targetVelocity, toTarget) * toTarget;

            leadMarkerPos = targetPosition + (tangentialVelocity + targetRadialVelocity) * timeToReach;

            return true;
        }
    }
}