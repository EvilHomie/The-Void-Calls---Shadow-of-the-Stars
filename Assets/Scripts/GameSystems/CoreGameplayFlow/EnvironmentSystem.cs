using DI;
using Environment;
using UnityEngine;

namespace CoreGameSystems
{
    public class EnvironmentSystem : MonoBehaviour
    {
        private Vector3 _deffScarryCanvasScale = Vector3.one * 0.5f;
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
            _cameraRigSystem.CameraOrtoSizeChanged += OnChangedCameraOrtoSize;
            Init(_starryCanvasView.Layer);
        }

        public void Execute(float deltaTime)
        {
            UpdateStarView(deltaTime);
        }

        private void OnChangedCameraOrtoSize(float orthoSize)
        {
            _starryCanvasView.Transform.localScale = _deffScarryCanvasScale * orthoSize;
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
        }

        private void UpdateStarView(float dTime)
        {
            var position = (Vector2)_camera.transform.position;
            var deltaPos = position - _lastCameraPos;
            _lastCameraPos = position;

            var starryCanvasLayer = _starryCanvasView.Layer;
            starryCanvasLayer.LastOffset += dTime * starryCanvasLayer.SpeedMod * deltaPos;
            starryCanvasLayer.PropertyBlock.SetVector(StarryCanvasLayer.OffsetID, starryCanvasLayer.LastOffset);
            starryCanvasLayer.Renderer.SetPropertyBlock(starryCanvasLayer.PropertyBlock);
        }
    }
}

