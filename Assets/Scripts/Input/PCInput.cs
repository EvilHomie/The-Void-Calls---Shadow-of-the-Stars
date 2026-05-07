using DI;
using GameSystems;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace GameInput
{
    public class PCInput : IPlayerInput
    {
        public Action<Vector2> MoveInputAction { get; set; }
        public Action<bool> ChangeAttackState { get; set; }
        public Action<bool> ChangeBoostersState { get; set; }
        public Action ToggleDamperAction { get; set; }
        public Action DisableEngineAction { get; set; }
        public Action<float> ChangeZoomAction { get; set; }
        public Action<WeaponGroup> SwitchWeaponGroupAction { get; set; }

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
        public PCInput(GameFlowSystem gameFlowSystem)
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
            var inputDir = context.ReadValue<Vector2>();
            MoveInputAction?.Invoke(inputDir);
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            ChangeAttackState?.Invoke(context.performed);
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

        private void SwitchWeaponsGroup(InputAction.CallbackContext context)
        {
            var key = ((KeyControl)context.control).keyCode;
            SwitchWeaponGroupAction?.Invoke(_groupBindings[key]);
        }
    }
}
