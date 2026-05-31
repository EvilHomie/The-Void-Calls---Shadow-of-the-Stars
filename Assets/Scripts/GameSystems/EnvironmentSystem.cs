using DI;
using Environment;
using UnityEngine;

namespace GameSystems
{
    public class EnvironmentSystem : GameSystemBase, ICoreUpdateTickObserver
    {
        private Vector3 _deffScarryCanvasScale;
        private CameraRigSystem _cameraRigSystem;
        private StarryCanvasView _starryCanvasView;
        private Camera _camera;

        private Vector2 _lastCameraPos;


        [Inject]
        public void Construct(Camera camera, CameraRigSystem cameraRig, StarryCanvasView starryCanvasView)
        {
            _camera = camera;
            _starryCanvasView = starryCanvasView;
            _cameraRigSystem = cameraRig;
            _deffScarryCanvasScale = new(0.5f, 0.5f, 0.5f);
        }

        protected override void AwakeInit()
        {
            foreach (var layer in _starryCanvasView.Layers)
            {
                Init(layer);
            }
        }
        public void CoreUpdateTick(float deltaTime)
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
        private void OnChangedCameraOrtoSize(float relativeValue)
        {
            _starryCanvasView.Transform.localScale = _deffScarryCanvasScale * relativeValue;
        }

        public void Init(StarryCanvasLayer layer)
        {
            layer.PropertyBlock = new MaterialPropertyBlock();
            Random.InitState(Random.Range(0, 100));
            Vector2 randomOffset = new()
            {
                x = Random.Range(0, 50),
                y = Random.Range(0, 50)
            };
            layer.LastOffset = randomOffset;
            layer.PropertyBlock.SetVector(StarryCanvasLayer.OffsetID, randomOffset);
            layer.Renderer.SetPropertyBlock(layer.PropertyBlock);
            //layer.Renderer.sortingOrder = SpriteSortingOrders.StarryCanvas;
        }

        private void UpdateStarView(float dTime)
        {
            var position = (Vector2)_camera.transform.position;
            var deltaPos = position - _lastCameraPos;
            _lastCameraPos = position;

            foreach (var layer in _starryCanvasView.Layers)
            {
                layer.LastOffset += dTime * layer.SpeedMod * deltaPos;
                layer.PropertyBlock.SetVector(StarryCanvasLayer.OffsetID, layer.LastOffset);

                layer.Renderer.SetPropertyBlock(layer.PropertyBlock);
            }
        }
    }
}

