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
        private CameraRigSystem _cameraRigSystem;
        private PlayerWeaponControler _playerWeaponControler;
        private PlayerAimSystem _playerAimSystem;


        [Inject]
        public void Construct(IPlayerInput playerInput, ShipRegistry shipRegistry, CameraRigSystem cameraRigSystem, PlayerAimSystem playerAimSystem, PlayerWeaponControler playerWeaponControler)
        {
            _playerInput = playerInput;
            _shipRegistry = shipRegistry;
            _cameraRigSystem = cameraRigSystem;
            _playerWeaponControler = playerWeaponControler;
            _playerAimSystem = playerAimSystem;
        }

        public void Execute()
        {
            ApplyInput();
            _playerInput.ResetComands();
        }

        private void ApplyInput()
        {
            ref readonly var inputSnapShot = ref _playerInput.InputData;

            var playerShip = _shipRegistry.PlayerShip;
            ref var shipControl = ref playerShip.Control;
            

            shipControl.MoveDirection = inputSnapShot.MoveDirection;
            shipControl.DamperEnabled = inputSnapShot.DamperEnabled;
            shipControl.EngineDisabled = inputSnapShot.EngineDisabled;
            shipControl.BoostersEnabled = inputSnapShot.BoostersEnabled;
            

            if (inputSnapShot.ChangeZoomValue != 0)
            {
                _cameraRigSystem.OnMouseScroll(inputSnapShot.ChangeZoomValue);
            }

            if (inputSnapShot.ToggleAttackSignal != SignalState.None)
            {
                var isShooting = inputSnapShot.ToggleAttackSignal == SignalState.Performed;
                shipControl.IsShooting = isShooting;
                _playerWeaponControler.OnPlayerChangeAttackState(playerShip, isShooting);
            }

            if (inputSnapShot.NewWeaponsGroupKey != Key.None)
            {
                _playerWeaponControler.OnSwitchWeaponGroupAction(inputSnapShot.NewWeaponsGroupKey, playerShip, shipControl.IsShooting);
                _playerAimSystem.OnPlayerSwitchWeaponGroup();
            }
        }
    }
}