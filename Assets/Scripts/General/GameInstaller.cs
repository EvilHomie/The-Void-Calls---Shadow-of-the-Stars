using UnityEngine;
using DI;
using Player;

public class GameInstaller : Installer
{
    [SerializeField] PlayerShip _playerShip;
    [SerializeField] Camera _camera;
    [SerializeField] LookAheadCursor _mouseCursor;

    protected override void InstallBindings()
    {
        Container.Bind<PlayerShip>().FromInstance(_playerShip);
        Container.Bind<Camera>().FromInstance(_camera);
        Container.Bind<LookAheadCursor>().FromInstance(_mouseCursor);

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.isEditor)
        {
            Container.Bind<IPlayerInput>().To<PCInput>().AsSingleton();
        }
    }
}
