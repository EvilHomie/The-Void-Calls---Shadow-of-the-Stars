using GameSystems;
using Ships;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameInput
{
    public class PCInput : IPlayerInput
    {
        public Action<Vector2> MoveInputAction { get; set; }
        //public Action<Vector2> TrackMouseDirectionAction { get; set; }
        public Action<Vector2> TrackMouseWorldPositionAction { get; set; }
        public Action<bool> ChangeAtackState { get; set; }
        public Action ToggleDamperAction { get; set; }
        public Action<float> ChangeZoomAction { get; set; }

        private InputSystem_Actions _inputActions;
        private Camera _camera;
        private ShipsStorage _objectsStorage;
        private bool _isActive;

        public void Init(Camera camera, ShipsStorage objectsStorage)
        {
            _inputActions = new InputSystem_Actions();
            _objectsStorage = objectsStorage;
            _camera = camera;
        }

        public void Subscrube()
        {
            GameFlow.UpdateTick += OnUpdateTick;
            EventBus.GameStateChangeAction += OnGameStateChange;
            _inputActions.Player.LeftClick.performed += OnAttack;
            _inputActions.Player.LeftClick.canceled += OnEndAttack;
            _inputActions.Player.ToggleDamper.performed += ToggleDamper;
            _inputActions.Player.MouseScroll.performed += OnMouseScroll;
        }

        public void Unsubscribe()
        {
            GameFlow.UpdateTick -= OnUpdateTick;
            EventBus.GameStateChangeAction -= OnGameStateChange;
            _inputActions.Player.LeftClick.performed -= OnAttack;
            _inputActions.Player.LeftClick.canceled -= OnEndAttack;
            _inputActions.Player.ToggleDamper.performed -= ToggleDamper;
            _inputActions.Player.MouseScroll.performed -= OnMouseScroll;
        }

        private void OnUpdateTick(float deltaTime)
        {
            if (!_isActive) return;

            TrackMouse();
            TrackInput();
        }

        private void OnGameStateChange(GameState gameState)
        {
            _isActive = gameState == GameState.CoreGameplay;

            if (_isActive) _inputActions.Player.Enable();
            else _inputActions.Player.Disable();
        }

        private void TrackMouse()
        {
            Vector2 mouseWorld = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _objectsStorage.AimPositions[_objectsStorage.PlayerIndex] = mouseWorld;
        }

        private void TrackInput()
        {
            var inputDir = _inputActions.Player.Move.ReadValue<Vector2>();
            MoveInputAction?.Invoke(inputDir);
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            ChangeAtackState?.Invoke(true);
        }

        private void OnEndAttack(InputAction.CallbackContext context)
        {
            ChangeAtackState?.Invoke(false);
        }

        private void ToggleDamper(InputAction.CallbackContext context)
        {
            ToggleDamperAction?.Invoke();
        }

        private void OnMouseScroll(InputAction.CallbackContext context)
        {
            float scroll = context.ReadValue<Vector2>().y;
            ChangeZoomAction?.Invoke(scroll);
        }
    }
}
