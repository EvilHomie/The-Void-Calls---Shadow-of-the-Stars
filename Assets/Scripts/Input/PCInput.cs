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

        private InputSystem_Actions _inputActions;
        private Transform _playerTransform;
        private Camera _camera;
        private bool _isActive;

        public void Init(PlayerShipData ship, Camera camera)
        {
            _inputActions = new InputSystem_Actions();
            _playerTransform = ship.transform;
            _camera = camera;
        }

        public void Subscrube()
        {
            GameFlow.FixedGameTick += OnFixedGameTick;
            GameFlow.GameStateChange += OnGameStateChange;
            _inputActions.Player.LeftClick.performed += OnAttack;
            _inputActions.Player.LeftClick.canceled += OnEndAttack;
            EventBus.UpdateShip += OnUpdateShip;
        }

        public void Unsubscribe()
        {
            GameFlow.FixedGameTick -= OnFixedGameTick;
            GameFlow.GameStateChange -= OnGameStateChange;
            _inputActions.Player.LeftClick.performed -= OnAttack;
            _inputActions.Player.LeftClick.canceled -= OnEndAttack;
            EventBus.UpdateShip -= OnUpdateShip;
        }

        private void OnUpdateShip(PlayerShipData ship)
        {
            _playerTransform = ship.transform;
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
            ChangeAtackState?.Invoke(true);
        }

        private void OnEndAttack(InputAction.CallbackContext context)
        {
            ChangeAtackState?.Invoke(false);
        }
    }
}
