using GameInput;
using Ships;
using UnityEngine;

namespace DI
{
    public class GameInstaller : Installer
    {
        [SerializeField] ShipInstance _playerShip;
        [SerializeField] Camera _mainCamera;
        [SerializeField] LookAheadCursor _mouseCursor;

        protected override void InstallBindings()
        {
            Container.Bind<ShipInstance>().FromInstance(_playerShip);
            Container.Bind<Camera>().FromInstance(_mainCamera);
            Container.Bind<LookAheadCursor>().FromInstance(_mouseCursor);

            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.isEditor)
            {
                var input = new PCInput();
                Container.Bind<IPlayerInput>().To<PCInput>().FromInstance(input);
            }
        }
    }
}