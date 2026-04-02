using GameSystems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PCInput : IPlayerInput
    {
        public Action<Vector2> MoveInputAction { get; set; }
        public Action<Vector2> TrackMouseDirectionAction { get; set; }
        public Action<Vector2> TrackMouseWorldPositionAction { get; set; }
        public Action<bool> ChangeAtackState { get; set; }
        public Action ToggleDamperAction { get; set; }
        public Action<float> ChangeZoomAction { get; set; }

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
            GameFlow.UpdateTick += OnUpdateTick;
            GameFlow.GameStateChange += OnGameStateChange;
            _inputActions.Player.LeftClick.performed += OnAttack;
            _inputActions.Player.LeftClick.canceled += OnEndAttack;
            _inputActions.Player.ToggleDamper.performed += ToggleDamper;
            _inputActions.Player.MouseScroll.performed += OnMouseScroll;
        }

        public void Unsubscribe()
        {
            GameFlow.FixedGameTick -= OnUpdateTick;
            GameFlow.GameStateChange -= OnGameStateChange;
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

        private void OnGameStateChange(GameState gameFlow)
        {
            _isActive = gameFlow == GameState.MainGameplay;

            if (_isActive) _inputActions.Player.Enable();
            else _inputActions.Player.Disable();
        }

        private void TrackMouse()
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 shipScreen = _camera.WorldToScreenPoint(_playerTransform.position);
            Vector2 direction = (mouseScreen - shipScreen).normalized;
            _playerShip.MousePos = mouseWorld;
            TrackMouseDirectionAction?.Invoke(direction);
            //TrackMouseWorldPositionAction?.Invoke(mouseWorld);
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
