using System;
using UnityEngine;

namespace GameInput
{
    public interface IPlayerInput
    {
        public Action<Vector2> MoveInputAction { get; set; }
        public Action<bool> ChangeAttackState { get; set; }
        public Action<bool> ChangeBoostersState { get; set; }
        public Action ToggleDamperAction { get; set; }
        public Action DisableEngineAction { get; set; }
        public Action<float> ChangeZoomAction { get; set; }
        public Action<WeaponGroup> SwitchWeaponGroupAction { get; set; }
    }
}