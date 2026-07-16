using CoreGameSystems;
using Environment;
using GamePools;
using General;
using PlayerInput;
using Registries;
using Unity.Cinemachine;
using UnityEngine;

namespace DI
{
    public class GameInstaller : Installer
    {
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
        [SerializeField] WeaponBehaviourSystem weaponBehaviourSystem;
        [SerializeField] EnvironmentSystem environmentSystem;
        [SerializeField] CinemachineTargetGroup cinemachineTargetGroup;
        [SerializeField] CinemachineCamera cinemachineCamera;
        [SerializeField] MovementVisualizeSystem movementVisualizeSystem;
        [SerializeField] PlayerAimSystem aimSystem;
        [SerializeField] ProjectileBehaviourSystem projectileBehaviourSystem;
        [SerializeField] HitRegistrationSystem hitRegistrationSystem;
        [SerializeField] DamageVizualizeSystem damageVizualizeSystem;
        [SerializeField] ShieldsControlSystem  shieldsControlSystem;
        [SerializeField] ShipsPool  shipsPool;


        protected override void InstallBindings()
        {
            Container.Bind<GameFlowSystem>().FromInstance(gameFlowSystem).AsSingleton();
            Container.Bind<ShipRegistry>().FromInstance(shipRegistry).AsSingleton();
            Container.Bind<WeaponRegistry>().FromInstance(weaponRegistry).AsSingleton();
            Container.Bind<ProjectileRegistry>().FromInstance(projectileRegistry).AsSingleton();
            Container.Bind<HitEffectRegistry>().FromInstance(hitParticleRegistry).AsSingleton();
            Container.Bind<ShieldEffectsPool>().FromInstance(shieldEffectsPool).AsSingleton();
            Container.Bind<ShieldsEffectRegistry>().FromInstance(shieldsEffectRegistry).AsSingleton();
            Container.Bind<PlayerControlSystem>().FromInstance(playerControlSystem).AsSingleton();
            Container.Bind<PlayerMovementSystem>().FromInstance(playerMovementSystem).AsSingleton();
            Container.Bind<WeaponBehaviourSystem>().FromInstance(weaponBehaviourSystem).AsSingleton();
            Container.Bind<EnvironmentSystem>().FromInstance(environmentSystem).AsSingleton();
            Container.Bind<CinemachineTargetGroup>().FromInstance(cinemachineTargetGroup).AsSingleton();
            Container.Bind<CinemachineCamera>().FromInstance(cinemachineCamera).AsSingleton();
            Container.Bind<MovementVisualizeSystem>().FromInstance(movementVisualizeSystem).AsSingleton();
            Container.Bind<PlayerAimSystem>().FromInstance(aimSystem).AsSingleton();
            Container.Bind<ProjectileBehaviourSystem>().FromInstance(projectileBehaviourSystem).AsSingleton();
            Container.Bind<HitRegistrationSystem>().FromInstance(hitRegistrationSystem).AsSingleton();
            Container.Bind<DamageVizualizeSystem>().FromInstance(damageVizualizeSystem).AsSingleton();
            Container.Bind<ShieldsControlSystem>().FromInstance(shieldsControlSystem).AsSingleton();
            Container.Bind<ShipsPool>().FromInstance(shipsPool).AsSingleton();
            Container.Bind<PlayerWeaponControler>().AsSingleton();

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