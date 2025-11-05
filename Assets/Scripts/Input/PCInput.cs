using GameSystem;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PCInput : IPlayerInput
    {
        public Action<Vector2> MoveInputAction { get; set; }
        public Action<Vector2> TrackMouseAction { get; set; }
        public Action<bool> ChangeAtackState { get; set; }
        public Action ToggleDamperAction { get; set; }
        public Action<float> MouseScrollAction { get; set; }

        private InputSystem_Actions _inputActions;
        private PlayerShip _playerShip;
        private Transform _playerTransform;
        private Camera _camera;
        private bool _isActive;

        public void Init(PlayerShip ship, Camera camera)
        {
            _inputActions = new InputSystem_Actions();
            _playerShip = ship;
            _playerTransform = ship.transform;
            _camera = camera;
        }

        public void Subscrube()
        {
            GameFlow.FixedGameTick += OnFixedGameTick;
            GameFlow.GameStateChange += OnGameStateChange;
            _inputActions.Player.LeftClick.performed += OnAttack;
            _inputActions.Player.LeftClick.canceled += OnEndAttack;
            _inputActions.Player.ToggleDamper.performed += ToggleDamper;
            _inputActions.Player.MouseScroll.performed += OnMouseScroll;
        }

        public void Unsubscribe()
        {
            GameFlow.FixedGameTick -= OnFixedGameTick;
            GameFlow.GameStateChange -= OnGameStateChange;
            _inputActions.Player.LeftClick.performed -= OnAttack;
            _inputActions.Player.LeftClick.canceled -= OnEndAttack;
            _inputActions.Player.ToggleDamper.performed -= ToggleDamper;
            _inputActions.Player.MouseScroll.performed -= OnMouseScroll;
        }

        private void OnFixedGameTick(float deltaTime)
        {
            if (!_isActive) return;

            TrackMouse();
            TrackInput();
        }

        private void OnGameStateChange(GameState gameFlow)
        {
            _isActive = gameFlow == GameState.MainGameplay;

            if (_isActive) _inputActions.Player.Enable();
            else _inputActions.Player.Disable();
        }

        private void TrackMouse()
        {
            Vector3 mouseWorld = _camera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            _playerShip.MousePos = mouseWorld;
            Vector2 direction = (mouseWorld - _playerTransform.position).normalized;
            TrackMouseAction?.Invoke(direction);
        }

        private void TrackInput()
        {
            var inputDir = _inputActions.Player.Move.ReadValue<Vector2>();
            MoveInputAction?.Invoke(inputDir);
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (!_isActive) return;

            ChangeAtackState?.Invoke(true);
        }

        private void OnEndAttack(InputAction.CallbackContext context)
        {
            if (!_isActive) return;

            ChangeAtackState?.Invoke(false);
        }

        private void ToggleDamper(InputAction.CallbackContext context)
        {
            if (!_isActive) return;

            ToggleDamperAction?.Invoke();
        }

        private void OnMouseScroll(InputAction.CallbackContext context)
        {
            float scroll = context.ReadValue<Vector2>().y;
            MouseScrollAction?.Invoke(scroll);
        }
    }
}
