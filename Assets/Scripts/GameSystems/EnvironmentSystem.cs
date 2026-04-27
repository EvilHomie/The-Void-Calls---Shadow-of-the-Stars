using DI;
using Environment;
using GameCamera;
using Registries;
using UnityEngine;

namespace GameSystems
{
    public class EnvironmentSystem : GameSystemBase , IUpdateTickObserver
    {
        private Vector3 Vector3One;
        private CameraRigSystem _cameraRigSystem;
        private ShipRegistry _shipRegistry;
        private StarryCanvasView _starryCanvasView;

        [Inject]
        public void Construct(CameraRigSystem cameraRig, ShipRegistry shipRegistry, StarryCanvasView starryCanvasView)
        {
            _starryCanvasView = starryCanvasView;
            _shipRegistry = shipRegistry;
            _cameraRigSystem = cameraRig;
            Vector3One = Vector3.one;
            ActiveGameState = GameState.CoreGameplay;
        }

        protected override void AwakeInit( )
        {
            foreach (var  layer in _starryCanvasView.Layers)
            {
                Init(layer);
            }
        }
        public void UpdateTick(float deltaTime)
        {
            UpdateStarView(deltaTime);
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            _cameraRigSystem.CameraOrtoSizeChanged += OnChangedCameraOrtoSize;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            _cameraRigSystem.CameraOrtoSizeChanged -= OnChangedCameraOrtoSize;
        }

        public void Init(StarryCanvasLayer layer)
        {
            layer.Material = layer.Renderer.material;
            Random.InitState(Random.Range(0, 100));
            Vector2 randomOffset = new()
            {
                x = Random.Range(0, 50),
                y = Random.Range(0, 50)
            };
            layer.LastOffset = randomOffset;
            layer.Material.SetVector(StarryCanvasLayer.OffsetID, randomOffset);
        }

        private void OnChangedCameraOrtoSize(float relativeValue)
        {
            _starryCanvasView.Transform.localScale = Vector3One * relativeValue;
        }

        private void UpdateStarView(float dTime)
        {
            var playerShip = _shipRegistry.PlayerShip;
            var playerPosition = playerShip.Transform.position;
            var playerVelocity = playerShip.Rigidbody.linearVelocity;
            _starryCanvasView.Transform.position = playerPosition;                      

            foreach (var layer in _starryCanvasView.Layers)
            {
                layer.LastOffset += dTime * layer.SpeedMod * playerVelocity;
                layer.Material.SetVector(StarryCanvasLayer.OffsetID, layer.LastOffset);
            }
        }
    }
}

