using System;
using UnityEngine;

namespace GameSystems
{
    public class GameFlow : MonoBehaviour
    {
        // события помеченные как Standart будто обычные Update FixedUpdate LateUpdate
        public static Action<float> FixedGameTick { get; set; }
        public static Action<float> PreUpdateTick { get; set; }
        public static Action<float> UpdateTick { get; set; }
        public static Action PostUpdateTick { get; set; }
        public static Action<float> LateGameTick { get; set; }
        //public static Action UnityUpdateTick { get; set; }
        //public static Action UnityFixedUpdateTick { get; set; }
        //public static Action UnityLateUpdateTick { get; set; }
        public static Action<GameState> GameStateChange { get; set; }
        public static float CoreTime { get; private set; }

        private GameState _currentGameState;
        private float _gameSpeed = 1;

        void Update()
        {
            var deltaTime = Time.unscaledDeltaTime * _gameSpeed;
            PreUpdateTick?.Invoke(deltaTime);
            UpdateTick?.Invoke(deltaTime);
            PostUpdateTick?.Invoke();
            //UnityUpdateTick?.Invoke();

            if (_currentGameState == GameState.CoreGameplay)
            {
                CoreTime += deltaTime;
            }
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime * _gameSpeed;

            FixedGameTick?.Invoke(deltaTime);
            //UnityFixedUpdateTick?.Invoke();
        }

        private void LateUpdate()
        {
            var deltaTime = Time.unscaledDeltaTime * _gameSpeed;

            LateGameTick?.Invoke(deltaTime);
            //UnityLateUpdateTick?.Invoke();
        }

        private void Start()
        {
            GameStateChange?.Invoke(GameState.CoreGameplay);
            _currentGameState = GameState.CoreGameplay;
        }
    }
}

public enum GameState
{
    CoreGameplay,
    Pause
}