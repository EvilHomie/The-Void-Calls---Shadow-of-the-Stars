using System;
using UnityEngine;

namespace GameInput
{
    public interface IPlayerInput
    {
        public Action<Vector2> MoveInputAction { get; set; }
        public Action<bool> ChangeAtackState { get; set; }
        public Action ToggleDamperAction { get; set; }
        public Action<float> ChangeZoomAction { get; set; }
        public void Subscrube();
        public void Unsubscribe();
    }
}