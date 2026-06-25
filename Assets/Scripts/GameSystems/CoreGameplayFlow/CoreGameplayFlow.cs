using DI;
using General;
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
        private MovementVisualizeSystem _movementVisualizeSystem;
        private AimSystem _aimSystem;
        private ProjectileBehaviourSystem _projectileBehaviourSystem;
        private DamageVizualizeSystem _damageVizualizeSystem;

        [Inject]
        public void Construct(
            PlayerControlSystem playerControlSystem,
            GameFlowSystem gameFlowSystem,
            PlayerMovementSystem playerMovementSystem,
            PlayerWeaponControlSystem playerWeaponGroupSystem,
            MouseCursorSystem mouseCursorSystem,
            WeaponBehaviourSystem weaponBehaviourSystem,
            EnvironmentSystem environmentSystem,
            CameraRigSystem cameraRigSystem,
            MovementVisualizeSystem movementVisualizeSystem,
            AimSystem aimSystem,
            ProjectileBehaviourSystem projectileBehaviourSystem,
            DamageVizualizeSystem damageVizualizeSystem)
        {
            _playerControlSystem = playerControlSystem;
            _playerMovementSystem = playerMovementSystem;
            _playerWeaponGroupSystem = playerWeaponGroupSystem;
            _mouseCursorSystem = mouseCursorSystem;
            _weaponBehaviourSystem = weaponBehaviourSystem;
            _environmentSystem = environmentSystem;
            _cameraRigSystem = cameraRigSystem;
            _movementVisualizeSystem = movementVisualizeSystem;
            _aimSystem = aimSystem;
            _projectileBehaviourSystem = projectileBehaviourSystem;
            _damageVizualizeSystem = damageVizualizeSystem;
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
            _aimSystem.Execute();
            _projectileBehaviourSystem.Execute(deltaTime);

            _damageVizualizeSystem.Execute(deltaTime);
            _movementVisualizeSystem.Execute();
            _cameraRigSystem.Execute(deltaTime);
            _environmentSystem.Execute(deltaTime);
        }
    }
}