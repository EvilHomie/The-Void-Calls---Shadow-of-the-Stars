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
        private ShipsStorage _objectsStorage;

        [Inject]
        public void Construct(ShipsStorage objectsStorage)
        {
            _objectsStorage = objectsStorage;
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
            GameFlow.UpdateTick += UpdateStarView;
            EventBus.ChangeCameraOrtoSize += OnChangeCameraOrtoSize;
        }

        protected override void Unsubscribe()
        {
            GameFlow.UpdateTick -= UpdateStarView;
            EventBus.ChangeCameraOrtoSize -= OnChangeCameraOrtoSize;
        }

        private void OnChangeCameraOrtoSize(float dTime)
        {
            _starryCanvasParent.localScale = Constants.Vector3One * dTime;
        }

        private void UpdateStarView(float dTime)
        {
            var playerIndex = _objectsStorage.PlayerIndex;
            ref var playerPosition = ref _objectsStorage.Positions[playerIndex];
            var playerView = _objectsStorage.ViewDatas[playerIndex];
            _starryCanvasParent.position = playerPosition;
            var shipRB = playerView.Rigidbody;

            if (shipRB.linearVelocity.sqrMagnitude < 0.0001f)
            {
                return;
            }

            foreach (var starryCanvas in _starryCanvasTwinkleViews)
            {
                starryCanvas.LastOffset += dTime * starryCanvas.SpeedMod * shipRB.linearVelocity;
                starryCanvas.Material.SetVector(StarryCanvasTwinkleView.OffsetID, starryCanvas.LastOffset);
            }
        }
    }
}

