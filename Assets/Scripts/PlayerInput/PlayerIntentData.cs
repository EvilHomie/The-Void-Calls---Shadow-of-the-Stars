using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInput
{
    public class PlayerIntentData : MonoBehaviour
    {                
        public InputData InputSnapshot;
    }

    public struct InputData
    {
        public Vector2 MoveDirection;
        public SignalState ToggleAttackSignal;
        public bool DamperEnabled;
        public bool EngineDisabled;
        public bool BoostersEnabled;
        public Key NewWeaponsGroupKey;
        public SignalState NewTargetSignal;
        public float ChangeZoomValue;
    }
}