using Environment;
using GameCamera;
using GameInput;
using GameSystems;
using Registries;
using Ships;
using UnityEngine;

namespace DI
{
    public class GameInstaller : Installer
    {
        [SerializeField] ShipInstance playerShip;
        [SerializeField] Camera mainCamera;
        [SerializeField] MouseCursor mouseCursor;
        [SerializeField] GameFlowSystem gameFlowSystem;
        [SerializeField] ShipRegistry  shipRegistry;
        [SerializeField] WeaponRegistry  weaponRegistry;
        [SerializeField] StarryCanvasView  starryCanvasView;
        [SerializeField] CameraRigSystem  cameraRigSystem;

        protected override void InstallBindings()
        {
            Container.Bind<ShipInstance>().FromInstance(playerShip).AsSingleton();
            Container.Bind<GameFlowSystem>().FromInstance(gameFlowSystem).AsSingleton();
            Container.Bind<ShipRegistry>().FromInstance(shipRegistry).AsSingleton();
            Container.Bind<WeaponRegistry>().FromInstance(weaponRegistry).AsSingleton();


            Container.Bind<StarryCanvasView>().FromInstance(starryCanvasView).AsSingleton();

            Container.Bind<Camera>().FromInstance(mainCamera).AsSingleton();
            Container.Bind<CameraRigSystem>().FromInstance(cameraRigSystem).AsSingleton();
            Container.Bind<MouseCursor>().FromInstance(mouseCursor).AsSingleton();

            

            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.isEditor)
            {
                Container.Bind<IPlayerInput>().To<PCInput>().AsSingleton();
            }
        }
    }
}