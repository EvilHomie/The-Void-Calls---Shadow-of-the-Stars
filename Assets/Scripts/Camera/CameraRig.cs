using DI;
using GameSystem;
using Player;
using UnityEngine;

public class CameraRig : GameSystemBase
{
    private Camera _mainCamera;
    private Transform _mouseCursor;

    [Inject]
    public void Construct(Camera camera, MouseCursor mouseCursor)
    {
        _mainCamera = camera;
        _mouseCursor = mouseCursor.transform;
    }

    protected override void Init()
    {
        Cursor.visible = false;
        UpdateCursorPos();
    }

    protected override void Subscribe()
    {
        GameFlow.FixedGameTickStandart += OnFixedGameTick;
    }

    protected override void Unsubscribe()
    {
        GameFlow.FixedGameTickStandart -= OnFixedGameTick;
    }

    private void OnFixedGameTick()
    {
        UpdateCursorPos();
    }

    private void UpdateCursorPos()
    {
        var pos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;

        _mouseCursor.position = pos;
    }
}
