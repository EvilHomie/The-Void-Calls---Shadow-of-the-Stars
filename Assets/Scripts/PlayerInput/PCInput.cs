using DI;
using General;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace PlayerInput
{
    public class PCInput : IPlayerInput
    {
        private readonly InputSystem_Actions _inputActions;

        public InputData _inputData;
        public ref readonly InputData InputData => ref _inputData;

        [Inject]
        public PCInput(GameFlowSystem gameFlowSystem)
        {
            _inputActions = new InputSystem_Actions();

            // инпут по удержанию кнопки
            _inputActions.Player.LeftClick.performed += OnAttack;
            _inputActions.Player.LeftClick.canceled += OnAttack;
            _inputActions.Player.Move.performed += OnMove;
            _inputActions.Player.Move.canceled += OnMove;
            _inputActions.Player.ToggleBoosters.performed += ToggleBoosters;
            _inputActions.Player.ToggleBoosters.canceled += ToggleBoosters;
            _inputActions.Player.DisableEngine.performed += DisableEngine;
            _inputActions.Player.DisableEngine.canceled += DisableEngine;

            // инпут только при нажатии кнопки
            _inputActions.Player.ToggleDamper.performed += ToggleDamper;
            _inputActions.Player.MouseScroll.performed += OnMouseScroll;
            _inputActions.Player.SwitchWeaponsGroup.performed += SwitchWeaponsGroup;

            gameFlowSystem.GameStateChanged += OnGameStateChanged;
        }

        public void ResetComands()
        {
            _inputData.ToggleAttackSignal = SignalState.None;
            _inputData.NewTargetSignal = SignalState.None;
            _inputData.NewWeaponsGroupKey = Key.None;
            _inputData.ChangeZoomValue = 0;
        }

        private void OnGameStateChanged(GameState gameState)
        {
            if (gameState == GameState.CoreGameplay) _inputActions.Player.Enable();
            else if (_inputActions.Player.enabled)
            {
                _inputActions.Player.Disable();
                ResetComands();
                _inputData.BoostersEnabled = false;
                _inputData.EngineDisabled = false;
            }
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _inputData.MoveDirection = context.ReadValue<Vector2>();
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            _inputData.ToggleAttackSignal = context.performed ? SignalState.Performed : SignalState.Canceled;
        }

        private void ToggleDamper(InputAction.CallbackContext context)
        {
            _inputData.DamperEnabled = !_inputData.DamperEnabled;
        }

        private void ToggleBoosters(InputAction.CallbackContext context)
        {
            _inputData.BoostersEnabled = context.performed;
        }

        private void OnMouseScroll(InputAction.CallbackContext context)
        {
            var scroll = context.ReadValue<Vector2>().y;
            _inputData.ChangeZoomValue += scroll;
        }
        private void DisableEngine(InputAction.CallbackContext context)
        {
            _inputData.EngineDisabled = context.performed;
        }

        private void SwitchWeaponsGroup(InputAction.CallbackContext context)
        {
            var key = ((KeyControl)context.control).keyCode;
            _inputData.NewWeaponsGroupKey = key;
        }
    }
}

[Serializable]
public enum SignalState
{
    None,
    Performed,
    Canceled
}