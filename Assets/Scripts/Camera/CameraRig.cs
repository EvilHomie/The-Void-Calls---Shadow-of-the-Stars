using DI;
using GameSystem;
using Player;
using Ship;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraRig : GameSystemBase
{
    [SerializeField] CinemachineTargetGroup _cinemachineTargetGroup;
    [SerializeField] float _mouseCursorWeight = 0.8f;
    [SerializeField] CinemachineCamera _cinemachineCamera;
    [SerializeField] float _changeOrtSizeSpeed;
    [SerializeField] float _changeOrtSizeStep;
    private Camera _mainCamera;
    private Transform _mouseCursor;
    private PlayerShip _playerShip;
    private IPlayerInput _input;
    private Vector2 _minMaxOrtSize;
    private float targetOrtSize;

    [Inject]
    public void Construct(Camera camera, MouseCursor mouseCursor, PlayerShip playerShip, IPlayerInput playerInput)
    {
        _mainCamera = camera;
        _mouseCursor = mouseCursor.transform;
        _playerShip = playerShip;
        _input = playerInput;
    }

    protected override void Init()
    {
        _cinemachineTargetGroup.Targets.Clear();

        var playerTarget = new CinemachineTargetGroup.Target()
        {
            Object = _playerShip.transform,
            Weight = 1,
            Radius = 1,
        };

        var cursorTarget = new CinemachineTargetGroup.Target()
        {
            Object = _mouseCursor,
            Weight = _mouseCursorWeight,
            Radius = 1,
        };

        _cinemachineTargetGroup.Targets.Add(playerTarget);
        _cinemachineTargetGroup.Targets.Add(cursorTarget);
        Cursor.visible = false;
       
    }

    private void Start()
    {
        UpdateCursorPos();
        OnChangeShip(_playerShip.ShipData);
    }

    protected override void Subscribe()
    {
        GameFlow.LateGameTick += OnLateGameTick;
        GameFlow.FixedGameTick += OnFixedGameTick;
        _input.MouseScrollAction += OnMouseScroll;
        EventBus.PlayerChangeShip += OnChangeShip;
    }

    protected override void Unsubscribe()
    {
        GameFlow.LateGameTick -= OnLateGameTick;
        GameFlow.FixedGameTick -= OnFixedGameTick;
        _input.MouseScrollAction -= OnMouseScroll;
        EventBus.PlayerChangeShip -= OnChangeShip;
    }

    private void OnFixedGameTick(float dTime)
    {
        if (_cinemachineCamera.Lens.OrthographicSize != targetOrtSize)
        {
            _cinemachineCamera.Lens.OrthographicSize = Mathf.MoveTowards(_cinemachineCamera.Lens.OrthographicSize, targetOrtSize, dTime * _changeOrtSizeSpeed);
            EventBus.ChangeCameraOrtoSize?.Invoke(_cinemachineCamera.Lens.OrthographicSize);
        }
    }

    private void OnChangeShip(ShipData data)
    {
        _minMaxOrtSize = data.MinMaxViewDistance;
        targetOrtSize = (_minMaxOrtSize.x + _minMaxOrtSize.y) / 2;
        _cinemachineCamera.Lens.OrthographicSize = targetOrtSize;
        EventBus.ChangeCameraOrtoSize?.Invoke(_cinemachineCamera.Lens.OrthographicSize);
    }

    private void OnLateGameTick(float dTime)
    {
        UpdateCursorPos();
    }

    private void UpdateCursorPos()
    {
        var pos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;

        _mouseCursor.position = pos;
    }
    private void OnMouseScroll(float value)
    {
        float ortSize = _cinemachineCamera.Lens.OrthographicSize;
        ortSize -= value * _changeOrtSizeStep;
        targetOrtSize = Mathf.Clamp(ortSize, _minMaxOrtSize.x, _minMaxOrtSize.y);
    }
}
