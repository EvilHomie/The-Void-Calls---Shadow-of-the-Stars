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

        private readonly List<ICorePreUpdateTickObserver> _corePreUpdateTickObservers = new();
        private readonly List<ICoreUpdateTickObserver> _coreUpdateTickObservers = new();
        private readonly List<ICoreLateUpdateTickObserver> _coreLateUpdateTickObservers = new();
        private readonly List<ICoreFixedUpdateTickObserver> _coreFixedUpdateTickObservers = new();
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
            if (observer is ICorePreUpdateTickObserver preUpdateTickObserver) _corePreUpdateTickObservers.Add(preUpdateTickObserver);
            if (observer is ICoreUpdateTickObserver updateTickObserver) _coreUpdateTickObservers.Add(updateTickObserver);
            if (observer is ICoreLateUpdateTickObserver lateUpdateTickObserver) _coreLateUpdateTickObservers.Add(lateUpdateTickObserver);
            if (observer is ICoreFixedUpdateTickObserver fixedUpdateTickObserver) _coreFixedUpdateTickObservers.Add(fixedUpdateTickObserver);
        }

        public void RemoveTickObserver(ITickObserver tickObserver)
        {
            if (tickObserver is ICorePreUpdateTickObserver preUpdateTickObserver) _corePreUpdateTickObservers.Remove(preUpdateTickObserver);
            if (tickObserver is ICoreUpdateTickObserver updateTickObserver) _coreUpdateTickObservers.Remove(updateTickObserver);
            if (tickObserver is ICoreLateUpdateTickObserver lateUpdateTickObserver) _coreLateUpdateTickObservers.Remove(lateUpdateTickObserver);
            if (tickObserver is ICoreFixedUpdateTickObserver fixedUpdateTickObserver) _coreFixedUpdateTickObservers.Remove(fixedUpdateTickObserver);
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
            if (_currentGameState == GameState.CoreGameplay)
            {
                var coreDeltaTimeTick = Time.unscaledDeltaTime * _gameSpeed;
                CoreTime += coreDeltaTimeTick;
                CoreDeltaTimeTick = coreDeltaTimeTick;
                foreach (var observer in _corePreUpdateTickObservers) observer.CorePreUpdateTick();
                foreach (var observer in _coreUpdateTickObservers) observer.CoreUpdateTick(coreDeltaTimeTick);
            }
        }

        private void FixedUpdate()
        {
            if (_currentGameState == GameState.CoreGameplay)
            {
                var coreFixedDTTick = Time.fixedUnscaledDeltaTime * _gameSpeed;

                foreach (var observer in _coreFixedUpdateTickObservers) observer.CoreFixedUpdateTick(coreFixedDTTick);
            }  
        }

        private void LateUpdate()
        {
            if (_currentGameState == GameState.CoreGameplay)
            {
                foreach (var observer in _coreLateUpdateTickObservers) observer.CoreLateUpdateTick();
            }
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

public interface ICorePreUpdateTickObserver : ITickObserver
{
    public void CorePreUpdateTick();
}
public interface ICoreUpdateTickObserver : ITickObserver
{
    public void CoreUpdateTick(float deltaTime);
}
public interface ICoreLateUpdateTickObserver : ITickObserver
{
    public void CoreLateUpdateTick();
}
public interface ICoreFixedUpdateTickObserver : ITickObserver
{
    public void CoreFixedUpdateTick(float fixedDT);
}