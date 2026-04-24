using DI;
using Enviroment;
using Ships;
using UnityEngine;

namespace GameSystems
{
    public class EnvironmentSystem : GameSystemBase
    {
        [SerializeField] StarryCanvasTwinkleView[] _starryCanvasTwinkleViews;
        [SerializeField] Transform _starryCanvasParent;
        private ShipInstance _playerShip;
        private Camera _camera;
        private Vector3 Vector3One;

        [Inject]
        public void Construct(Camera camera)
        {
            _camera = camera;
            Vector3One = Vector3.one;
        }

        protected override void AwakeInit()
        {
            foreach (var starryCanvas in _starryCanvasTwinkleViews)
            {
                starryCanvas.Init();
            }
        }

        protected override void Subscribe()
        {
            GameFlowSystem.UpdateTick += UpdateStarView;
            EventBus.ChangeCameraOrtoSize += OnChangeCameraOrtoSize;
            EventBus.SpawnPlayerShip += OnSpawnPlayerShip;
        }

        protected override void Unsubscribe()
        {
            GameFlowSystem.UpdateTick -= UpdateStarView;
            EventBus.ChangeCameraOrtoSize -= OnChangeCameraOrtoSize;
            EventBus.SpawnPlayerShip -= OnSpawnPlayerShip;
        }

        private void OnSpawnPlayerShip(ShipInstance shipInstance)
        {
            _playerShip = shipInstance;
        }

        private void OnChangeCameraOrtoSize(float relativeValue)
        {
            _starryCanvasParent.localScale = Vector3One * relativeValue;
        }

        private void UpdateStarView(float dTime)
        {
            var playerPosition = _playerShip.Transform.position;
            var playerVelocity = _playerShip.Rigidbody.linearVelocity;
            _starryCanvasParent.position = playerPosition;                      

            foreach (var starryCanvas in _starryCanvasTwinkleViews)
            {
                starryCanvas.LastOffset += dTime * starryCanvas.SpeedMod * playerVelocity;
                starryCanvas.Material.SetVector(StarryCanvasTwinkleView.OffsetID, starryCanvas.LastOffset);
            }
        }
    }
}

