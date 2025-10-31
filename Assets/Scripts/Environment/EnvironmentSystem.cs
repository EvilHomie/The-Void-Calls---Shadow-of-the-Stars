using DI;
using Enviroment;
using Player;
using System.Linq;
using UnityEngine;

namespace GameSystem
{
    public class EnvironmentSystem : GameSystemBase
    {
        [SerializeField] StarryCanvasTwinkleView[] _starryCanvasTwinkleViews;
        private Rigidbody2D _shipRB;
        private Transform _shipT;
        private PlayerShipData _shipData;
        private Transform _starryCanvasParent;

        [Inject]
        public void Construct(PlayerShipData ship, Camera camera)
        {
            _shipData = ship;
        }

        protected override void Init()
        {
            foreach (var starryCanvas in _starryCanvasTwinkleViews)
            {
                starryCanvas.Init();
            }

            _starryCanvasParent = _starryCanvasTwinkleViews.First().transform.parent;

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

