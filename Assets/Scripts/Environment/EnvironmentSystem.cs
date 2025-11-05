using DI;
using Enviroment;
using Player;
using UnityEngine;

namespace GameSystem
{
    public class EnvironmentSystem : GameSystemBase
    {
        [SerializeField] StarryCanvasTwinkleView[] _starryCanvasTwinkleViews;
        [SerializeField] Transform _starryCanvasParent;
        private Rigidbody2D _shipRB;
        private Transform _shipT;
        private PlayerShip _playerShip;

        [Inject]
        public void Construct(PlayerShip ship)
        {
            _playerShip = ship;
        }

        protected override void Init()
        {
            foreach (var starryCanvas in _starryCanvasTwinkleViews)
            {
                starryCanvas.Init();
            }

            _shipRB = _playerShip.Rigidbody;
            _shipT = _playerShip.transform;
        }

        protected override void Subscribe()
        {
            GameFlow.GameTick += OnGameTick;
            EventBus.ChangeCameraOrtoSize += OnChangeCameraOrtoSize;
        }

        protected override void Unsubscribe()
        {
            GameFlow.GameTick -= OnGameTick;
            EventBus.ChangeCameraOrtoSize -= OnChangeCameraOrtoSize;
        }

        private void OnGameTick(float fixDeltaTime)
        {
            UpdateStarView(fixDeltaTime);
        }

        private void OnChangeCameraOrtoSize(float value)
        {
            _starryCanvasParent.localScale = Constants.Vector3One * value / Constants.DeffCameraOrtoSize;
        }


        private void UpdateStarView(float fixDeltaTime)
        {
            _starryCanvasParent.position = _shipT.position;

            if (_shipRB.linearVelocity == Constants.Vector2Zero) return;

            foreach (var starryCanvas in _starryCanvasTwinkleViews)
            {
                starryCanvas.LastOffset += fixDeltaTime * starryCanvas.SpeedMod * _shipRB.linearVelocity;
                starryCanvas.Material.SetVector(StarryCanvasTwinkleView.OffsetID, starryCanvas.LastOffset);
            }
        }
    }
}

