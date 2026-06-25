using UnityEngine;

namespace PlayerInput
{
    public class PlayerIntentData : MonoBehaviour
    {
        public Vector2 MoveInput { get; set; }
        public ChangeSignal AttackChangeSignal { get; set; }
        public bool DamperEnabled { get; set; } = true;
        public bool ResetThrottle { get; set; }
        public bool BoostersIsActive { get; set; }
        public WeaponGroup ChangeWeaponGroup { get; set; }
        public float ChangeZoom { get; set; }
    }
}