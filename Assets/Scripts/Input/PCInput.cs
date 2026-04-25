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
        public Action<bool> ChangeBoostersState { get; set; }
        public Action ToggleDamperAction { get; set; }
        public Action DisableEngineAction { get; set; }
        public Action<float> ChangeZoomAction { get; set; }

        private readonly InputSystem_Actions _inputActions;

        public PCInput()
        {
            _inputActions = new InputSystem_Actions();

            _inputActions.Player.LeftClick.performed += OnAttack;
            _inputActions.Player.LeftClick.canceled += OnAttack;

            _inputActions.Player.ToggleDamper.performed += ToggleDamper;
            _inputActions.Player.MouseScroll.performed += OnMouseScroll;

            _inputActions.Player.Move.performed += OnMove;
            _inputActions.Player.Move.canceled += OnMove;

            _inputActions.Player.DisableEngine.performed += DisableEngine;

            _inputActions.Player.ToggleBoosters.performed += ToggleBoosters;
            _inputActions.Player.ToggleBoosters.canceled += ToggleBoosters;

            EventBus.GameStateChangeAction += OnGameStateChange;
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

        private void OnAttack(InputAction.CallbackContext context)
        {
            ChangeAtackState?.Invoke(context.performed);
        }

        private void ToggleDamper(InputAction.CallbackContext context)
        {
            ToggleDamperAction?.Invoke();
        }

        private void ToggleBoosters(InputAction.CallbackContext context)
        {
            ChangeBoostersState?.Invoke(context.performed);
        }

        private void OnMouseScroll(InputAction.CallbackContext context)
        {
            var scroll = context.ReadValue<Vector2>().y;
            ChangeZoomAction?.Invoke(scroll);
        }
        private void DisableEngine(InputAction.CallbackContext context)
        {
            DisableEngineAction?.Invoke();
        }
    }
}
