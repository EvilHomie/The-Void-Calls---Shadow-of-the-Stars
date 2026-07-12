using CoreGameSystems;
using DI;
using General;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace PlayerInput
{
    public class PCInput : IPlayerInput
    {
        public PlayerIntentData PlayerIntentData { get; }

        private readonly Dictionary<Key, WeaponGroup> _groupBindings = new()
        {
            { Key.Digit1, WeaponGroup.Group1 },
            { Key.Digit2, WeaponGroup.Group2 },
            { Key.Digit3, WeaponGroup.Group3 },
            { Key.Digit4, WeaponGroup.Group4 },
            { Key.Digit5, WeaponGroup.Group5 },
        };

        private readonly InputSystem_Actions _inputActions;

        [Inject]
        public PCInput(GameFlowSystem gameFlowSystem, PlayerIntentData playerIntentData)
        {
            PlayerIntentData = playerIntentData;
            _inputActions = new InputSystem_Actions();

            _inputActions.Player.LeftClick.performed += OnAttack;
            _inputActions.Player.LeftClick.canceled += OnAttack;

            _inputActions.Player.ToggleDamper.performed += ToggleDamper;
            _inputActions.Player.MouseScroll.performed += OnMouseScroll;

            _inputActions.Player.Move.performed += OnMove;
            _inputActions.Player.Move.canceled += OnMove;

            _inputActions.Player.DisableEngine.performed += DisableEngine;
            _inputActions.Player.DisableEngine.canceled += DisableEngine;

            _inputActions.Player.ToggleBoosters.performed += ToggleBoosters;
            _inputActions.Player.ToggleBoosters.canceled += ToggleBoosters;

            _inputActions.Player.SwitchWeaponsGroup.performed += SwitchWeaponsGroup;

            gameFlowSystem.GameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState gameState)
        {
            if (gameState == GameState.CoreGameplay) _inputActions.Player.Enable();
            else _inputActions.Player.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            PlayerIntentData.MoveInput = context.ReadValue<Vector2>();
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            PlayerIntentData.AttackChangeSignal = context.performed ? ChangeSignal.Performed : ChangeSignal.Canceled;
        }

        private void ToggleDamper(InputAction.CallbackContext context)
        {
            PlayerIntentData.DamperEnabled = !PlayerIntentData.DamperEnabled;
        }

        private void ToggleBoosters(InputAction.CallbackContext context)
        {
            PlayerIntentData.BoostersIsActive = !PlayerIntentData.BoostersIsActive;
        }

        private void OnMouseScroll(InputAction.CallbackContext context)
        {
            var scroll = context.ReadValue<Vector2>().y;
            PlayerIntentData.ChangeZoom = scroll;
        }
        private void DisableEngine(InputAction.CallbackContext context)
        {
            PlayerIntentData.ResetThrottle = context.performed;
        }

        private void SwitchWeaponsGroup(InputAction.CallbackContext context)
        {
            var key = ((KeyControl)context.control).keyCode;
            PlayerIntentData.ChangeWeaponGroup = _groupBindings[key];
        }
    }
}