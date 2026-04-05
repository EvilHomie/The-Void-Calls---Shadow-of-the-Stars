using GameInput;
using GameSystems;
using Ship;
using UnityEngine;

namespace DI
{
    public class GameInstaller : Installer
    {
        [SerializeField] ShipInstance _playerShip;
        [SerializeField] Camera _mainCamera;
        [SerializeField] LookAheadCursor _mouseCursor;
        [SerializeField] ObjectsStorage _objectsStorage;

        protected override void InstallBindings()
        {
            Container.Bind<ShipInstance>().FromInstance(_playerShip);
            Container.Bind<Camera>().FromInstance(_mainCamera);
            Container.Bind<LookAheadCursor>().FromInstance(_mouseCursor);
            Container.Bind<ObjectsStorage>().FromInstance(_objectsStorage);

            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.isEditor)
            {
                Container.Bind<IPlayerInput>().To<PCInput>().AsSingleton();
            }
        }
    }
}