using UnityEngine;
using DI;
using Player;

public class GameInstaller : Installer
{
    [SerializeField] PlayerShipData _playerShip;
    [SerializeField] Camera _camera;

    protected override void InstallBindings()
    {
        Container.Bind<PlayerShipData>().FromInstance(_playerShip);
        Container.Bind<Camera>().FromInstance(_camera);

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.isEditor)
        {
            Container.Bind<IPlayerInput>().To<PCInput>().AsSingleton();
        }
    }
}
