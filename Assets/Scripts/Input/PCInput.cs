using GameSystems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameInput
{
    public class PCInput : IPlayerInput
    {
        public Action<Vector2> MoveInputAction { get; set; }
        public Action<bool> ChangeAtackState { get; set; }
        public Action ToggleDamperAction { get; set; }
        public Action<float> ChangeZoomAction { get; set; }

        private readonly InputSystem_Actions _inputActions;

        public PCInput()
        {
            _inputActions = new InputSystem_Actions();
        }

        public void Subscrube()
        {
            EventBus.GameStateChangeAction += OnGameStateChange;
            _inputActions.Player.LeftClick.performed += OnAttack;
            _inputActions.Player.LeftClick.canceled += OnEndAttack;
            _inputActions.Player.ToggleDamper.performed += ToggleDamper;
            _inputActions.Player.MouseScroll.performed += OnMouseScroll;

            _inputActions.Player.Move.performed += OnMove;
            _inputActions.Player.Move.canceled += OnStop;
        }

        public void Unsubscribe()
        {
            EventBus.GameStateChangeAction -= OnGameStateChange;
            _inputActions.Player.LeftClick.performed -= OnAttack;
            _inputActions.Player.LeftClick.canceled -= OnEndAttack;
            _inputActions.Player.ToggleDamper.performed -= ToggleDamper;
            _inputActions.Player.MouseScroll.performed -= OnMouseScroll;

            _inputActions.Player.Move.performed -= OnMove;
            _inputActions.Player.Move.canceled -= OnStop;
        }

        private void OnGameStateChange(GameState gameState)
        {
            if (gameState == GameState.CoreGameplay) _inputActions.Player.Enable();
            else _inputActions.Player.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            var inputDir = context.ReadValue<Vector2>();
            MoveInputAction?.Invoke(inputDir);
        }

        private void OnStop(InputAction.CallbackContext context)
        {
            MoveInputAction?.Invoke(Vector2.zero);
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
