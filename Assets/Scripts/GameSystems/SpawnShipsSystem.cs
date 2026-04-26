using DI;
using Helpers;
using Ships;
using UnityEngine;

namespace GameSystems
{
    public class SpawnShipsSystem : GameSystemBase
    {
        private ShipInstance _playerShip;

        [Inject]
        public void Construct(ShipInstance playerShip)
        {
            _playerShip = playerShip;
        }

        protected override void AwakeInit()
        {
        }

        protected override void Subscribe()
        {

        }

        protected override void Unsubscribe()
        {

        }

        private void Start()
        {
            ShipInitHelper.InitShip(_playerShip);
            EventBus.SpawnPlayerShip?.Invoke(_playerShip);
            EventBus.GameStateChangeAction?.Invoke(GameState.CoreGameplay);
        }


    }
}

