using DI;
using PlayerInput;
using Registries;
using System;
using UnityEngine;

namespace CoreGameSystems
{
    public class PlayerControlSystem : MonoBehaviour
    {
        private PlayerIntentData _playerIntentData;
        private ShipRegistry _shipRegystry;

        [Inject]
        public void Construct(IPlayerInput playerInput, ShipRegistry shipRegistry)
        {
            _playerIntentData = playerInput.PlayerIntentData;
            _shipRegystry = shipRegistry;
        }
        public void Execute()
        {
            WriteIntentData();
            ResetData();
        }

        private void WriteIntentData()
        {
            var playerShip = _shipRegystry.PlayerShip;
            ref var shipIntentData = ref playerShip.IntentData;

            shipIntentData.MoveDirection = _playerIntentData.MoveInput;
            shipIntentData.DamperEnabled = _playerIntentData.DamperEnabled;
            shipIntentData.ResetThrottle = _playerIntentData.ResetThrottle;
            shipIntentData.BoostersIsActive = _playerIntentData.BoostersIsActive;

            shipIntentData.AttackChangeSignal = _playerIntentData.AttackChangeSignal;
            shipIntentData.ChangeWeaponGroup = _playerIntentData.ChangeWeaponGroup;
        }

        private void ResetData()
        {
            _playerIntentData.AttackChangeSignal = ChangeSignal.None;
            _playerIntentData.ChangeZoom = 0;
        }
    }

    [Serializable]
    public struct ShipIntentData
    {
        // Относится к движению (обрабатывается в физическом тике)
        public Vector2 MoveDirection;
        public bool DamperEnabled;
        public bool ResetThrottle;
        public bool BoostersIsActive;

        // Вне физического тика
        public ChangeSignal AttackChangeSignal;
        public WeaponGroup ChangeWeaponGroup;
        public float ChangeZoom;

    }

    [Serializable]
    public enum ChangeSignal
    {
        None,
        Performed,
        Canceled
    }
}