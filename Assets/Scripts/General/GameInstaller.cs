using GameCamera;
using GameInput;
using Ships;
using UnityEngine;

namespace DI
{
    public class GameInstaller : Installer
    {
        [SerializeField] ShipInstance playerShip;
        [SerializeField] Camera mainCamera;
        [SerializeField] MouseCursor  mouseCursor;

        protected override void InstallBindings()
        {
            Container.Bind<ShipInstance>().FromInstance(playerShip).AsSingleton();
            Container.Bind<Camera>().FromInstance(mainCamera).AsSingleton();
            Container.Bind<MouseCursor>().FromInstance(mouseCursor).AsSingleton();

            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.isEditor)
            {
                Container.Bind<IPlayerInput>().To<PCInput>().AsSingleton();
            }
        }
    }
}