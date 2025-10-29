using DI;
using Enviroment;
using Player;
using UnityEngine;

namespace GameSystem
{
    public class EnvironmentSystem : GameSystemBase
    {
        [SerializeField] StarryCanvasTwinkleView[] _starryCanvasTwinkleViews;
        private Rigidbody2D _shipRB;
        private Transform _shipT;
        private PlayerShipData _shipData;

        [Inject]
        public void Construct(PlayerShipData ship)
        {
            _shipData = ship;
        }

        protected override void Init()
        {
            foreach (var starryCanvas in _starryCanvasTwinkleViews)
            {
                starryCanvas.Init();
            }

            OnUpdateShip(_shipData);
        }

        protected override void Subscribe()
        {
            GameFlow.GameTick += OnGameTick;
        }
        protected override void Unsubscribe()
        {
            GameFlow.GameTick -= OnGameTick;
        }
        private void OnUpdateShip(PlayerShipData ship)
        {
            _shipRB = ship.Rigidbody;
            _shipT = _shipRB.transform;
        }

        private void OnGameTick(float fixDeltaTime)
        {
            UpdateStarView(fixDeltaTime);
        }


        private void UpdateStarView(float fixDeltaTime)
        {
            if (_shipRB.linearVelocity == Constants.Vector2Zero) return;

            foreach (var starryCanvas in _starryCanvasTwinkleViews)
            {
                starryCanvas.LastOffset += fixDeltaTime * starryCanvas.SpeedMod * _shipRB.linearVelocity;
                starryCanvas.Transform.position = _shipT.position;
                starryCanvas.Material.SetVector(StarryCanvasTwinkleView.OffsetID, starryCanvas.LastOffset);
            }
        }
    }
}

