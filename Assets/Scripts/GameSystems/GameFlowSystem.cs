using System;
using UnityEngine;

namespace GameSystems
{
    public class GameFlowSystem : GameSystemBase
    {
        // события помеченные как Standart будто обычные Update FixedUpdate LateUpdate
        public static Action PreFixedGameTick { get; set; }
        public static Action<float> FixedGameTick { get; set; }
        public static Action PostFixedGameTick { get; set; }

        public static Action PreUpdateTick { get; set; }
        public static Action<float> UpdateTick { get; set; }
        public static Action PostUpdateTick { get; set; }
        public static Action<float> LateGameTick { get; set; }
        public static Action<GameState> GameStateChange { get; set; }
        public static float CoreTime { get; private set; }

        private GameState _currentGameState = GameState.None;
        private float _gameSpeed = 1;

        protected override void AwakeInit()
        {

        }

        protected override void Subscribe()
        {
            EventBus.GameStateChangeAction += OnGameStateChanged;
        }

        protected override void Unsubscribe()
        {
            EventBus.GameStateChangeAction -= OnGameStateChanged;
        }

        void Update()
        {
            var deltaTime = Time.unscaledDeltaTime * _gameSpeed;

            if (_currentGameState == GameState.CoreGameplay)
            {
                CoreTime += deltaTime;
            }

            PreUpdateTick?.Invoke();
            UpdateTick?.Invoke(deltaTime);
            PostUpdateTick?.Invoke();
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime * _gameSpeed;
            PreFixedGameTick?.Invoke();
            FixedGameTick?.Invoke(deltaTime);
            PostFixedGameTick?.Invoke();
        }

        private void LateUpdate()
        {
            var deltaTime = Time.unscaledDeltaTime * _gameSpeed;
            LateGameTick?.Invoke(deltaTime);
        }
        private void OnGameStateChanged(GameState state)
        {
            _currentGameState = state;
        }
    }
}

public enum GameState
{
    None,
    CoreGameplay,
    Pause
}