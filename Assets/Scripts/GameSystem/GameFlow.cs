using System;
using UnityEngine;

namespace GameSystems
{
    public class GameFlow : MonoBehaviour
    {
        // события помеченные как Standart будто обычные Update FixedUpdate LateUpdate
        public static Action<float> UpdateTick { get; set; }
        public static Action<float> FixedGameTick { get; set; }
        public static Action<float> LateGameTick { get; set; }
        public static Action GameTickStandart { get; set; }
        public static Action FixedGameTickStandart { get; set; }
        public static Action LateGameTickStandart { get; set; }
        public static Action<GameState> GameStateChange { get; set; }

        void Update()
        {
            UpdateTick?.Invoke(Time.deltaTime);
            GameTickStandart?.Invoke();
        }

        private void FixedUpdate()
        {
            FixedGameTick?.Invoke(Time.fixedDeltaTime);
            FixedGameTickStandart?.Invoke();
        }

        private void LateUpdate()
        {
            LateGameTick?.Invoke(Time.deltaTime);
            LateGameTickStandart?.Invoke();
        }

        private void Start()
        {
            GameStateChange?.Invoke(GameState.MainGameplay);
        }
    }
}

public enum GameState
{
    MainGameplay,
    Pause
}