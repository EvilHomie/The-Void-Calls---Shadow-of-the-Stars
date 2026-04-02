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
        private ShipsDataStorage _objectsStorage;
        private Camera _camera;

        [Inject]
        public void Construct(ShipsDataStorage objectsStorage, Camera camera)
        {
            _objectsStorage = objectsStorage;
            _camera = camera;
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
        }

        protected override void Unsubscribe()
        {
            GameFlowSystem.UpdateTick -= UpdateStarView;
            EventBus.ChangeCameraOrtoSize -= OnChangeCameraOrtoSize;
        }

        private void OnChangeCameraOrtoSize(float dTime)
        {
            _starryCanvasParent.localScale = Constants.Vector3One * dTime;
        }

        private void UpdateStarView(float dTime)
        {
            var playerIndex = _objectsStorage.PlayerIndex;
            var playerPosition = _objectsStorage.Positions[playerIndex];
            var playerVelocity = _objectsStorage.MovementRuntimeDatas[playerIndex].LinearVelocity;
            _starryCanvasParent.position = playerPosition;                      

            foreach (var starryCanvas in _starryCanvasTwinkleViews)
            {
                starryCanvas.LastOffset += dTime * starryCanvas.SpeedMod * playerVelocity;
                starryCanvas.Material.SetVector(StarryCanvasTwinkleView.OffsetID, starryCanvas.LastOffset);
            }
        }
    }
}

