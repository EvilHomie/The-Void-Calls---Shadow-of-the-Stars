using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInput
{
    public interface IPlayerInput
    {
        public ref readonly InputData InputData { get; }

        public void ResetComands();
    }

    public struct InputData
    {
        // состояния
        public Vector2 MoveDirection;
        public bool DamperEnabled;
        public bool EngineDisabled;
        public bool BoostersEnabled;
        // команды
        public SignalState ToggleAttackSignal;
        public Key NewWeaponsGroupKey;
        public SignalState NewTargetSignal;
        public float ChangeZoomValue;
    }
}