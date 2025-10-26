using Player;
using System;
using UnityEngine;

public interface IPlayerInput
{
    public Action<Vector2> MoveInputAction { get; set; }
    public Action<Vector2> TrackMouseAction { get; set; }
    public Action<bool> ChangeAtackState { get; set; }

    public void Init(PlayerShipData playerShip, Camera camera);
    public void Subscrube();
    public void Unsubscribe();
}