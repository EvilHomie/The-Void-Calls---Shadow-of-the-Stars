using DI;
using Enviroment;
using Ship;
using UnityEngine;

namespace GameSystems
{
    public class EnvironmentSystem : GameSystemBase
    {
        [SerializeField] StarryCanvasTwinkleView[] _starryCanvasTwinkleViews;
        [SerializeField] Transform _starryCanvasParent;
        private ObjectsStorage _objectsStorage;

        [Inject]
        public void Construct(ObjectsStorage objectsStorage)
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
            _starryCanvasParent.position = _objectsStorage.PlayerShipData.Position;
            var shipRB = _objectsStorage.PlayerShipView.Rigidbody;

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

