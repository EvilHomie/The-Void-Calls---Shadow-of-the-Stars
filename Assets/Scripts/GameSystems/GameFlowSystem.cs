using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class GameFlowSystem : GameSystemBase
    {
        public static float CoreTime { get; private set; }
        public static float CoreDeltaTimeTick { get; private set; }

        public Action<GameState> GameStateChanged { get; set; }

        private readonly List<IPreUpdateTickObserver> _preUpdateTickObservers = new();
        private readonly List<IUpdateTickObserver> _updateTickObservers = new();
        private readonly List<ILateUpdateTickObserver> _lateUpdateTickObservers = new();
        private readonly List<IFixedUpdateTickObserver> _fixedUpdateTickObservers = new();
        private GameState _currentGameState = GameState.None;
        private float _gameSpeed = 1;

        protected override void AwakeInit()
        {
            base.AwakeInit();
            _gameSpeed = 1;
            _currentGameState = GameState.None;
        }

        public void AddTickObserver(ITickObserver observer)
        {
            if (observer is IPreUpdateTickObserver preUpdateTickObserver) _preUpdateTickObservers.Add(preUpdateTickObserver);
            if (observer is IUpdateTickObserver updateTickObserver) _updateTickObservers.Add(updateTickObserver);
            if (observer is ILateUpdateTickObserver lateUpdateTickObserver) _lateUpdateTickObservers.Add(lateUpdateTickObserver);
            if (observer is IFixedUpdateTickObserver fixedUpdateTickObserver) _fixedUpdateTickObservers.Add(fixedUpdateTickObserver);
        }

        public void RemoveTickObserver(ITickObserver tickObserver)
        {
            if (tickObserver is IPreUpdateTickObserver preUpdateTickObserver) _preUpdateTickObservers.Remove(preUpdateTickObserver);
            if (tickObserver is IUpdateTickObserver updateTickObserver) _updateTickObservers.Remove(updateTickObserver);
            if (tickObserver is ILateUpdateTickObserver lateUpdateTickObserver) _lateUpdateTickObservers.Remove(lateUpdateTickObserver);
            if (tickObserver is IFixedUpdateTickObserver fixedUpdateTickObserver) _fixedUpdateTickObservers.Remove(fixedUpdateTickObserver);
        }

        public void ChangeGameState(GameState newState)
        {
            if (_currentGameState == newState) return;

            _currentGameState = newState;
            GameStateChanged?.Invoke(newState);
        }
        public void ChangeGameSpeed(float speed)
        {
            _gameSpeed = speed;
        }

        void Update()
        {
            var deltaTimeTick = Time.unscaledDeltaTime * _gameSpeed;

            if (_currentGameState == GameState.CoreGameplay)
            {
                CoreTime += deltaTimeTick;
                CoreDeltaTimeTick = deltaTimeTick;
            }

            foreach (var observer in _preUpdateTickObservers) observer.PreUpdateTick();
            foreach (var observer in _updateTickObservers) observer.UpdateTick(deltaTimeTick);

        }

        private void FixedUpdate()
        {
            var fixedDTTick = Time.fixedDeltaTime * _gameSpeed;

            foreach (var observer in _fixedUpdateTickObservers) observer.FixedUpdateTick(fixedDTTick);
        }

        private void LateUpdate()
        {
            foreach (var observer in _lateUpdateTickObservers) observer.LateUpdateTick();
        }
    }
}

public enum GameState
{
    None,
    CoreGameplay,
    Pause
}
public interface ITickObserver { }

public interface IPreUpdateTickObserver : ITickObserver
{
    public void PreUpdateTick();
}
public interface IUpdateTickObserver : ITickObserver
{
    public void UpdateTick(float deltaTime);
}
public interface ILateUpdateTickObserver : ITickObserver
{
    public void LateUpdateTick();
}
public interface IFixedUpdateTickObserver : ITickObserver
{
    public void FixedUpdateTick(float fixedDT);
}