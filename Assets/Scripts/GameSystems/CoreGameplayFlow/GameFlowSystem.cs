using System;
using System.Collections.Generic;
using UnityEngine;

namespace General
{
    public class GameFlowSystem : GameSystemBase
    {
        public static float CoreTime { get; private set; }
        public static float CoreTickDeltaTime { get; private set; }

        public Action<GameState> GameStateChanged { get; set; }

        private readonly List<ICorePreUpdateTickObserver> _corePreUpdateTickObservers = new();
        private readonly List<ICoreUpdateTickObserver> _coreUpdateTickObservers = new();
        private readonly List<ICoreLateUpdateTickObserver> _coreLateUpdateTickObservers = new();
        private readonly List<ICoreFixedUpdateTickObserver> _coreFixedUpdateTickObservers = new();
        private readonly List<ICorePreFixedUpdateTickObserver> _corePreFixedUpdateTickObservers = new();
        private readonly List<ICorePostLateUpdateTickObserver> _corePostLateUpdateTickObserver = new();
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
            if (observer is ICorePreFixedUpdateTickObserver  corePreFixedUpdateTickObserver) _corePreFixedUpdateTickObservers.Add(corePreFixedUpdateTickObserver);
            if (observer is ICorePostLateUpdateTickObserver  corePostLateUpdateTickObserver) _corePostLateUpdateTickObserver.Add(corePostLateUpdateTickObserver);
        }

        public void RemoveTickObserver(ITickObserver observer)
        {
            if (observer is ICorePreUpdateTickObserver preUpdateTickObserver) _corePreUpdateTickObservers.Remove(preUpdateTickObserver);
            if (observer is ICoreUpdateTickObserver updateTickObserver) _coreUpdateTickObservers.Remove(updateTickObserver);
            if (observer is ICoreLateUpdateTickObserver lateUpdateTickObserver) _coreLateUpdateTickObservers.Remove(lateUpdateTickObserver);
            if (observer is ICoreFixedUpdateTickObserver fixedUpdateTickObserver) _coreFixedUpdateTickObservers.Remove(fixedUpdateTickObserver);
            if (observer is ICorePreFixedUpdateTickObserver corePreFixedUpdateTickObserver) _corePreFixedUpdateTickObservers.Remove(corePreFixedUpdateTickObserver);
            if (observer is ICorePostLateUpdateTickObserver corePostLateUpdateTickObserver) _corePostLateUpdateTickObserver.Remove(corePostLateUpdateTickObserver);
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
                CoreTickDeltaTime = coreDeltaTimeTick;
                foreach (var observer in _corePreUpdateTickObservers) observer.CorePreUpdateTick();
                foreach (var observer in _coreUpdateTickObservers) observer.CoreUpdateTick(coreDeltaTimeTick);
            }            
        }

        private void FixedUpdate()
        {
            if (_currentGameState == GameState.CoreGameplay)
            {
                var coreFixedDTTick = Time.fixedUnscaledDeltaTime * _gameSpeed;

                foreach (var observer in _corePreFixedUpdateTickObservers) observer.CorePreFixedUpdateTick(coreFixedDTTick);
                foreach (var observer in _coreFixedUpdateTickObservers) observer.CoreFixedUpdateTick(coreFixedDTTick);
            }  
        }

        private void LateUpdate()
        {
            if (_currentGameState == GameState.CoreGameplay)
            {
                foreach (var observer in _coreLateUpdateTickObservers) observer.CoreLateUpdateTick();
                foreach (var observer in _corePostLateUpdateTickObserver) observer.CorePostLateUpdateTick(CoreTickDeltaTime);
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
public interface ICorePostLateUpdateTickObserver : ITickObserver
{
    public void CorePostLateUpdateTick(float deltaTime);
}
public interface ICorePreFixedUpdateTickObserver : ITickObserver
{
    public void CorePreFixedUpdateTick(float fixedDT);
}
public interface ICoreFixedUpdateTickObserver : ITickObserver
{
    public void CoreFixedUpdateTick(float fixedDT);
}