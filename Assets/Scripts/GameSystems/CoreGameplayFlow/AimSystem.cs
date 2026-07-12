using DI;
using General;
using Helpers;
using Registries;
using Ships;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace CoreGameSystems
{
    public class AimSystem : MonoBehaviour
    {
        private Vector3 _deffWeaponMarkerScale = Vector3.one * 0.03f;
        private Vector3 _deffLeadMarkerScale = Vector3.one * 0.075f;

        private ShipRegistry _shipRegistry;
        private MouseCursorSystem _mouseCursor;
        private GameFlowSystem _gameFlowSystem;
        private CameraRigSystem _cameraRigSystem;

        [SerializeField] Transform leadMarkerPrefab;
        [SerializeField] Transform weaponAimMarkerPrefab;
        [SerializeField] Transform markerConteiner;

        private Transform _leadMarker;
        private bool _showLeadMarker;
        private List<Transform> _weaponsAimMarkers;


        [Inject]
        public void Construct(ShipRegistry shipRegistry, CameraRigSystem cameraRig, MouseCursorSystem mouseCursor, GameFlowSystem gameFlowSystem)
        {
            _mouseCursor = mouseCursor;
            _shipRegistry = shipRegistry;
            _gameFlowSystem = gameFlowSystem;
            _cameraRigSystem = cameraRig;
            _cameraRigSystem.CameraOrtoSizeChanged += UpdateMarkersSizes;
            _gameFlowSystem.GameStateChanged += OnGameStateChanged;
            EventBus.PlayerSwitchWeaponsGroupAction += OnPlayerSwitchWeaponGroup;
            EventBus.ChangeTargetAction += OnChangeTarget;
            Init();
        }

        private void Init()
        {
            _weaponsAimMarkers = new();

            for (int i = 0; i < GameConfig.MaxMainWeaponSlotsCount; i++)
            {
                var weaponMarker = Instantiate(weaponAimMarkerPrefab, markerConteiner);
                weaponMarker.gameObject.SetActive(false);
                _weaponsAimMarkers.Add(weaponMarker);
            }

            _leadMarker = Instantiate(leadMarkerPrefab, markerConteiner);
            _leadMarker.gameObject.SetActive(false);
        }

        public void Execute(float deltaTime)
        {
            ProccedPlayerAim(deltaTime);
            ProceedEnemyAim(deltaTime);
        }

        private void ProccedPlayerAim(float deltaTime)
        {
            var playerShip = _shipRegistry.PlayerShip;
            var aimData = playerShip.AimData;
            aimData.AimPosition = _mouseCursor.WorldPostition;
            WeaponHelper.AimAtTarget(playerShip, deltaTime);
            UpdateMarkersPosition(playerShip);
        }

        private void ProceedEnemyAim(float deltaTime)
        {
            foreach (var ship in _shipRegistry.Lod0Ships)
            {
                var aimData = ship.AimData;
                aimData.AimPosition = aimData.TargetRigidBody.position;
                WeaponHelper.AimAtTarget(ship, deltaTime);
                // пока что боты глупые и всегда целятся в центр игрока без упреждения
            }
        }

        private void UpdateMarkersSizes(float orthoSize)
        {
            foreach (var marker in _weaponsAimMarkers) marker.localScale = _deffWeaponMarkerScale * orthoSize;
            _leadMarker.localScale = _deffLeadMarkerScale * orthoSize;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state != GameState.CoreGameplay) DisableMarkers();
            else EnableMarkers();
        }

        private void OnChangeTarget(ShipInstance instance, Rigidbody2D newTargetRB)
        {
            var aimData = instance.AimData;
            aimData.TargetRigidBody = newTargetRB;
        }

        private void OnPlayerSwitchWeaponGroup()
        {
            DisableMarkers();
            EnableMarkers();
        }

        private void DisableMarkers()
        {
            foreach (var marker in _weaponsAimMarkers) marker.gameObject.SetActive(false);
            _leadMarker.gameObject.SetActive(false);
        }

        private void EnableMarkers()
        {
            var playerShip = _shipRegistry.PlayerShip;
            var aimData = playerShip.AimData;
            var fastestProjectileSpeed = 0f;

            for (int i = 0; i < playerShip.MainWeaponsSlots.Count; i++)
            {
                var weaponSlot = playerShip.MainWeaponsSlots[i];

                if (!weaponSlot.IsInActiveGroup) continue;

                var aimMarker = _weaponsAimMarkers[i];
                aimMarker.gameObject.SetActive(true);

                var weapon = weaponSlot.Weapon;

                if (weapon is not ProjectileWeapon boltWeapon) continue;

                ref readonly var fireStats = ref boltWeapon.RuntimeFireStats;
                var projectileSpeed = fireStats.ProjectileSpeed;

                if (fastestProjectileSpeed < projectileSpeed)
                {
                    fastestProjectileSpeed = projectileSpeed;
                    aimData.FastetsBoltWeapon = weapon;
                }
            }

            aimData.FastestBoltSpeed = fastestProjectileSpeed;

            if (fastestProjectileSpeed == 0 || aimData.TargetRigidBody == null) return;

            _leadMarker.gameObject.SetActive(true);
            _showLeadMarker = true;
        }

        private void UpdateMarkersPosition(ShipInstance shipInstance)
        {
            var aimData = shipInstance.AimData;

            for (int i = 0; i < shipInstance.MainWeaponsSlots.Count; i++)
            {
                var weaponSlot = shipInstance.MainWeaponsSlots[i];

                if (!weaponSlot.IsInActiveGroup) continue;

                var weapon = weaponSlot.Weapon;
                ref readonly var shootPointData = ref weapon.ShootPointRuntimeData;
                var shootPosition = shootPointData.Position;
                var direction = shootPointData.Direction;
                ref readonly var aimStats = ref weapon.RuntimeAimStats;

                var distanceToAimPosition = Vector2.Distance(shipInstance.AimData.AimPosition, shootPosition);
                var aimDistance = Mathf.Min(aimStats.MaxDistance, distanceToAimPosition);
                _weaponsAimMarkers[i].position = shootPosition + direction * aimDistance;
            }

            if (_showLeadMarker)
            {
                _leadMarker.position = GetTargetLeadPosition(aimData, shipInstance.Rigidbody);
            }
        }

        private Vector2 GetTargetLeadPosition(AimData aimData, Rigidbody2D ownRigidBody)
        {
            var fastestBoltWeapon = aimData.FastetsBoltWeapon;
            ref readonly var shootPointRuntimeData = ref fastestBoltWeapon.ShootPointRuntimeData;
            var weaponShooterPos = shootPointRuntimeData.Position;
            var targetRB = aimData.TargetRigidBody;

            var shooterVelocity = ownRigidBody.linearVelocity;
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