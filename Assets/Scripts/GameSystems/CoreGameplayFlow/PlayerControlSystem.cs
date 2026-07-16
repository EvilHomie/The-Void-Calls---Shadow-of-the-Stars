using DI;
using PlayerInput;
using Registries;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreGameSystems
{
    public class PlayerControlSystem : MonoBehaviour
    {
        private IPlayerInput _playerInput;
        private ShipRegistry _shipRegistry;
        private PlayerIntentData _playerIntentData;


        [Inject]
        public void Construct(IPlayerInput playerInput, ShipRegistry shipRegistry, PlayerIntentData playerIntentData)
        {
            _playerInput = playerInput;
            _shipRegistry = shipRegistry;
            _playerIntentData = playerIntentData;
        }

        public void Execute()
        {
            CreateSnapshot();
            ResetSignals();
        }

        private void CreateSnapshot()
        {
            ref readonly var inputSnapshot = ref _playerInput.InputData;
            _playerIntentData.InputSnapshot = inputSnapshot;

            var playerShip = _shipRegistry.PlayerShip;
            ref var shipIntentData = ref playerShip.ControlData;

            shipIntentData.MoveDirection = inputSnapshot.MoveDirection;
            shipIntentData.DamperEnabled = inputSnapshot.DamperEnabled;
            shipIntentData.EngineDisabled = inputSnapshot.EngineDisabled;
            shipIntentData.BoostersEnabled = inputSnapshot.BoostersEnabled;
            shipIntentData.NewTarget = null;
        }
        private void ResetSignals()
        {
            ref var inputData = ref _playerInput.InputData;
            inputData.ToggleAttackSignal = SignalState.None;
            inputData.NewWeaponsGroupKey = Key.None;
            inputData.NewTargetSignal = SignalState.None;
            inputData.ChangeZoomValue = 0;
        }
    }
}