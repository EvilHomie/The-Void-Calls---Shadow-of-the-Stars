using DI;
using UnityEngine;

namespace CoreGameSystems
{
    public class CoreGameplayFlow : MonoBehaviour, ICoreFixedUpdateTickObserver, ICorePostLateUpdateTickObserver, ICoreUpdateTickObserver
    {
        private PlayerControlSystem _playerControlSystem;
        private PlayerMovementSystem _playerMovementSystem;
        private PlayerWeaponControlSystem _playerWeaponGroupSystem;
        private MouseCursorSystem _mouseCursorSystem;
        private WeaponBehaviourSystem _weaponBehaviourSystem;
        private EnvironmentSystem _environmentSystem;
        private CameraRigSystem _cameraRigSystem;

        [Inject]
        public void Construct(
            PlayerControlSystem playerControlSystem,
            GameFlowSystem gameFlowSystem,
            PlayerMovementSystem playerMovementSystem,
            PlayerWeaponControlSystem playerWeaponGroupSystem,
            MouseCursorSystem mouseCursorSystem,
            WeaponBehaviourSystem weaponBehaviourSystem,
            EnvironmentSystem environmentSystem,
            CameraRigSystem cameraRigSystem)
        {
            _playerControlSystem = playerControlSystem;
            _playerMovementSystem = playerMovementSystem;
            _playerWeaponGroupSystem = playerWeaponGroupSystem;
            _mouseCursorSystem = mouseCursorSystem;
            _weaponBehaviourSystem = weaponBehaviourSystem;
            _environmentSystem = environmentSystem;
            _cameraRigSystem = cameraRigSystem;
            gameFlowSystem.AddTickObserver(this);
        }

        public void CoreFixedUpdateTick(float fixedDT)
        {
            _playerMovementSystem.Execute(fixedDT);
        }

        public void CorePostLateUpdateTick(float deltaTime)
        {
            _playerControlSystem.Execute(); // Запись намерений игрока. 
        }

        public void CoreUpdateTick(float deltaTime)
        {
            _mouseCursorSystem.Execute();
            _weaponBehaviourSystem.Execute(deltaTime);
            _playerWeaponGroupSystem.Execute();



            _cameraRigSystem.Execute(deltaTime);
            _environmentSystem.Execute(deltaTime);
        }
    }
}