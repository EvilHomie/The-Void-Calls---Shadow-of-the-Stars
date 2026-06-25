using DI;
using PlayerInput;
using Registries;
using UnityEngine;

namespace CoreGameSystems
{
    public class PlayerControlSystem : MonoBehaviour
    {
        private PlayerIntentData _playerIntentData;
        private ShipRegistry _shipRegystry;

        [Inject]
        public void Construct(IPlayerInput playerInput, ShipRegistry shipRegistry)
        {
            _playerIntentData = playerInput.PlayerIntentData;
            _shipRegystry = shipRegistry;
        }
        public void Execute()
        {
            WriteIntentData();
            ResetData();
        }

        private void WriteIntentData()
        {
            var playerShip = _shipRegystry.PlayerShip;
            ref var intentData = ref playerShip.IntentData;

            intentData.MoveDirection = _playerIntentData.MoveInput;
            intentData.DamperEnabled = _playerIntentData.DamperEnabled;
            intentData.ResetThrottle = _playerIntentData.ResetThrottle;
            intentData.BoostersIsActive = _playerIntentData.BoostersIsActive;

            intentData.AttackChangeSignal = _playerIntentData.AttackChangeSignal;
            intentData.ChangeWeaponGroup = _playerIntentData.ChangeWeaponGroup;
        }

        private void ResetData()
        {
            _playerIntentData.AttackChangeSignal = ChangeSignal.None;
            _playerIntentData.ChangeZoom = 0;
        }
    }
}