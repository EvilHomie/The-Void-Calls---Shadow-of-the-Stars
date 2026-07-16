using DI;
using General;
using Registries;
using UnityEngine;

namespace CoreGameSystems
{
    public class CoreGameplayFlow : MonoBehaviour, ICoreFixedUpdateTickObserver, ICorePostLateUpdateTickObserver, ICoreUpdateTickObserver, ICorePreUpdateTickObserver
    {
        private PlayerControlSystem _playerControlSystem;
        private PlayerMovementSystem _playerMovementSystem;
        private MouseCursorSystem _mouseCursorSystem;
        private WeaponBehaviourSystem _weaponBehaviourSystem;
        private EnvironmentSystem _environmentSystem;
        private CameraRigSystem _cameraRigSystem;
        private MovementVisualizeSystem _movementVisualizeSystem;
        private PlayerAimSystem _aimSystem;
        private ProjectileBehaviourSystem _projectileBehaviourSystem;
        private DamageVizualizeSystem _damageVizualizeSystem;
        private ShieldsControlSystem _shieldsControlSystem;

        [Inject]
        public void Construct(
            PlayerControlSystem playerControlSystem,
            GameFlowSystem gameFlowSystem,
            PlayerMovementSystem playerMovementSystem,             
            MouseCursorSystem mouseCursorSystem,
            WeaponBehaviourSystem weaponBehaviourSystem,
            EnvironmentSystem environmentSystem,
            CameraRigSystem cameraRigSystem,
            MovementVisualizeSystem movementVisualizeSystem,
            PlayerAimSystem aimSystem,
            ProjectileBehaviourSystem projectileBehaviourSystem,
            DamageVizualizeSystem damageVizualizeSystem,
            ShieldsControlSystem shieldsControlSystem)
        {
            _playerControlSystem = playerControlSystem;
            _playerMovementSystem = playerMovementSystem;
            _mouseCursorSystem = mouseCursorSystem;
            _weaponBehaviourSystem = weaponBehaviourSystem;
            _environmentSystem = environmentSystem;
            _cameraRigSystem = cameraRigSystem;
            _movementVisualizeSystem = movementVisualizeSystem;
            _aimSystem = aimSystem;
            _projectileBehaviourSystem = projectileBehaviourSystem;
            _damageVizualizeSystem = damageVizualizeSystem;
            _shieldsControlSystem = shieldsControlSystem;
            gameFlowSystem.AddTickObserver(this);
        }

        public void CoreFixedUpdateTick(float fixedDT)
        {
            _playerMovementSystem.Execute(fixedDT);
        }

        public void CorePostLateUpdateTick(float deltaTime)
        {
            _playerControlSystem.Execute(); // Считываение и применение инпута
        }

        public void CorePreUpdateTick()
        {
        }

        public void CoreUpdateTick(float deltaTime)
        {
            _mouseCursorSystem.Execute();
            _aimSystem.Execute(deltaTime);
            _weaponBehaviourSystem.Execute();
            _projectileBehaviourSystem.Execute(deltaTime);

            _shieldsControlSystem.Execute(deltaTime);
            _damageVizualizeSystem.Execute(deltaTime);
            _movementVisualizeSystem.Execute();
            _cameraRigSystem.Execute(deltaTime);
            _environmentSystem.Execute(deltaTime);
        }
    }
}