using System;
using UnityEngine;

namespace GameSystem
{
    public class GameFlow : MonoBehaviour
    {
        public static Action<float> GameTick { get; set; }
        public static Action<float> FixedGameTick { get; set; }
        public static Action<float> LateGameTick { get; set; }
        public static Action<GameState> GameStateChange { get; set; }

        void Update()
        {
            GameTick?.Invoke(Time.deltaTime);
        }
        private void FixedUpdate()
        {
            FixedGameTick?.Invoke(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            LateGameTick?.Invoke(Time.deltaTime);
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