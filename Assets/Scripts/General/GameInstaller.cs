using CoreGameSystems;
using Environment;
using GamePools;
using Registries;
using Ships;
using Unity.Cinemachine;
using UnityEngine;

namespace DI
{
    public class GameInstaller : Installer
    {
        [SerializeField] ShipInstance playerShip;
        [SerializeField] Camera mainCamera;
        [SerializeField] MouseCursorSystem mouseCursorSystem;
        [SerializeField] GameFlowSystem gameFlowSystem;
        [SerializeField] ShipRegistry shipRegistry;
        [SerializeField] WeaponRegistry weaponRegistry;
        [SerializeField] ProjectileRegistry projectileRegistry;
        [SerializeField] HitEffectRegistry hitParticleRegistry;
        [SerializeField] StarryCanvasView starryCanvasView;
        [SerializeField] CameraRigSystem cameraRigSystem;
        [SerializeField] HitParticlesPool hitParticlesPool;
        [SerializeField] ProjectilesPool projectilesPool;
        [SerializeField] ShieldsEffectRegistry shieldsEffectRegistry;
        [SerializeField] ShieldEffectsPool shieldEffectsPool;
        [SerializeField] PlayerControlSystem playerControlSystem;
        [SerializeField] PlayerMovementSystem playerMovementSystem;
        [SerializeField] PlayerWeaponControlSystem playerWeaponControlSystem;
        [SerializeField] WeaponBehaviourSystem weaponBehaviourSystem;
        [SerializeField] EnvironmentSystem environmentSystem;
        [SerializeField] PlayerIntentData playerIntentData;
        [SerializeField] CinemachineTargetGroup cinemachineTargetGroup;
        [SerializeField] CinemachineCamera cinemachineCamera;
        [SerializeField] MovementVisualizeSystem movementVisualizeSystem;
        [SerializeField] AimSystem aimSystem;
        [SerializeField] ProjectileBehaviourSystem projectileBehaviourSystem;
        [SerializeField] HitRegistrationSystem hitRegistrationSystem;


        protected override void InstallBindings()
        {
            Container.Bind<ShipInstance>().FromInstance(playerShip).AsSingleton();
            Container.Bind<GameFlowSystem>().FromInstance(gameFlowSystem).AsSingleton();
            Container.Bind<ShipRegistry>().FromInstance(shipRegistry).AsSingleton();
            Container.Bind<WeaponRegistry>().FromInstance(weaponRegistry).AsSingleton();
            Container.Bind<ProjectileRegistry>().FromInstance(projectileRegistry).AsSingleton();
            Container.Bind<HitEffectRegistry>().FromInstance(hitParticleRegistry).AsSingleton();
            Container.Bind<ShieldEffectsPool>().FromInstance(shieldEffectsPool).AsSingleton();
            Container.Bind<ShieldsEffectRegistry>().FromInstance(shieldsEffectRegistry).AsSingleton();
            Container.Bind<PlayerControlSystem>().FromInstance(playerControlSystem).AsSingleton();
            Container.Bind<PlayerMovementSystem>().FromInstance(playerMovementSystem).AsSingleton();
            Container.Bind<PlayerWeaponControlSystem>().FromInstance(playerWeaponControlSystem).AsSingleton();
            Container.Bind<WeaponBehaviourSystem>().FromInstance(weaponBehaviourSystem).AsSingleton();
            Container.Bind<EnvironmentSystem>().FromInstance(environmentSystem).AsSingleton();
            Container.Bind<PlayerIntentData>().FromInstance(playerIntentData).AsSingleton();
            Container.Bind<CinemachineTargetGroup>().FromInstance(cinemachineTargetGroup).AsSingleton();
            Container.Bind<CinemachineCamera>().FromInstance(cinemachineCamera).AsSingleton();
            Container.Bind<MovementVisualizeSystem>().FromInstance(movementVisualizeSystem).AsSingleton();
            Container.Bind<AimSystem>().FromInstance(aimSystem).AsSingleton();
            Container.Bind<ProjectileBehaviourSystem>().FromInstance(projectileBehaviourSystem).AsSingleton();
            Container.Bind<HitRegistrationSystem>().FromInstance(hitRegistrationSystem).AsSingleton();

            Container.Bind<HitParticlesPool>().FromInstance(hitParticlesPool).AsSingleton();
            Container.Bind<ProjectilesPool>().FromInstance(projectilesPool).AsSingleton();


            Container.Bind<StarryCanvasView>().FromInstance(starryCanvasView).AsSingleton();

            Container.Bind<Camera>().FromInstance(mainCamera).AsSingleton();
            Container.Bind<CameraRigSystem>().FromInstance(cameraRigSystem).AsSingleton();
            Container.Bind<MouseCursorSystem>().FromInstance(mouseCursorSystem).AsSingleton();



            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.isEditor)
            {
                Container.Bind<IPlayerInput>().To<PCInput>().AsSingleton();
            }
        }
    }
}