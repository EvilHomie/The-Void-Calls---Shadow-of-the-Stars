using DI;
using GameCamera;
using Registries;
using System;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace GameSystems
{
    public class TargetSystem : GameSystemBase, ICorePreUpdateTickObserver
    {
        private ShipRegistry _shipRegistry;
        private MouseCursor _mouseCursor;
        private GameFlowSystem _gameFlowSystem;

        [SerializeField] Transform leadMarkerPrefab;
        [SerializeField] Transform weaponAimMarkerPrefab;

        private Transform _leadMarker;
        private List<Transform> _weaponsAimMarkers;
        private List<WeaponAimData>  _weaponAimDatas;

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
            _weaponAimDatas = new();

            for (int i = 0; i < WorldConfig.MaxMainWeaponSlotsCount; i++)
            {
                var weaponMarker = Instantiate(weaponAimMarkerPrefab);
                weaponMarker.gameObject.SetActive(false);
                _weaponsAimMarkers.Add(weaponMarker);                
            }

            _leadMarker = Instantiate(leadMarkerPrefab);
            _leadMarker.gameObject.SetActive(false);
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            _gameFlowSystem.GameStateChanged += OnGameStateChanged;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            _gameFlowSystem.GameStateChanged -= OnGameStateChanged;
        }


        private void OnGameStateChanged(GameState state)
        {
            if (state != GameState.CoreGameplay)
            {
                foreach (var marker in _weaponsAimMarkers)
                {
                    marker.gameObject.SetActive(false);
                }

                _leadMarker.gameObject.SetActive(false);
            }
            else
            {
                var playerShip = _shipRegistry.PlayerShip;

                for (int i = 0; i < playerShip.WeaponSlots.Count; i++)
                {
                    var weapon = playerShip.WeaponSlots[i].Weapon;
                    var weaponAimData = new WeaponAimData()
                    {
                        ShootPoint = weapon.ShootPoint,

                    };

                    if (weapon is BoltRepeater boltRepeater)
                    {
                        var projectileSpeed = boltRepeater.WeaponStats.Config.ProjectileSpeed;
                    }
                }
            }
        }

        public void CorePreUpdateTick()
        {
            var playerShip = _shipRegistry.PlayerShip;
            playerShip.AimData.AimPosition = _mouseCursor.WorldPostition;


            foreach (var ship in _shipRegistry.ShipsInFight)
            {
                // нужна будет логика по расчету точки прицеливания = упреждению как у игрока
                //ref var shipAimData = ref ship.AimData; 
                //var targetRigidBody = shipAimData.TargetRigidBody;
                //shipAimData.AimPosition = targetRigidBody.position;
            }
        }
    }
}

[Serializable]
public struct WeaponAimData
{
    public Transform ShootPoint;
    public float ProjectileSpeed;
    public float MaxDistance;
    public Transform AimPoint;
}

