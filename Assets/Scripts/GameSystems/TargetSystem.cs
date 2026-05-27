using DI;
using GameCamera;
using Registries;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

        [Inject]
        public void Construct(ShipRegistry shipRegistry, MouseCursor mouseCursor, GameFlowSystem gameFlowSystem)
        {
            _mouseCursor = mouseCursor;
            _shipRegistry = shipRegistry;
            _gameFlowSystem = gameFlowSystem;


            

            gameFlowSystem.GameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.CoreGameplay)
            {
                if(_leadMarker == null) _leadMarker = Instantiate(leadMarkerPrefab);

                if (_weaponsAimMarkers == null)
                {
                    _weaponsAimMarkers = new();
                    var playerShip = _shipRegistry.PlayerShip;

                    for (int i = 0; i < playerShip.WeaponSlots.Length; i++)
                    {

                    }
                }
            }
            else
            {
                
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

