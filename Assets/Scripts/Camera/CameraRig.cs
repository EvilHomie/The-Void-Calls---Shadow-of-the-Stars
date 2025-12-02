using DI;
using GameSystems;
using Player;
using Ship;
using Unity.Cinemachine;
using UnityEngine;

public class CameraRig : GameSystemBase
{
    [SerializeField] CinemachineTargetGroup _cinemachineTargetGroup;
    [SerializeField] float _mouseCursorWeight = 0.8f;
    [SerializeField] CinemachineCamera _cinemachineCamera;
    [SerializeField] float _changeOrtSizeSpeed;
    [SerializeField] float _changeOrtSizeStep;
    private Transform _mouseCursor;
    private PlayerShip _playerShip;
    private IPlayerInput _input;
    private Vector2 _minMaxOrtSize;
    private float targetOrtSize;

    [Inject]
    public void Construct(MouseCursor mouseCursor, PlayerShip playerShip, IPlayerInput playerInput)
    {
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
            OnChangeOrtoSize();
        }
    }

    private void OnChangeShip(ShipData data)
    {
        _minMaxOrtSize = data.MinMaxViewDistance;
        targetOrtSize = (_minMaxOrtSize.x + _minMaxOrtSize.y) / 2;
        _cinemachineCamera.Lens.OrthographicSize = targetOrtSize;
        OnChangeOrtoSize();
    }

    private void OnChangeOrtoSize()
    {
        EventBus.ChangeCameraOrtoSize?.Invoke(_cinemachineCamera.Lens.OrthographicSize);
        _mouseCursor.localScale = Constants.Vector3One * _cinemachineCamera.Lens.OrthographicSize / Constants.DeffCameraOrtoSize;
    }

    private void OnLateGameTick(float dTime)
    {
        UpdateCursorPos();
    }

    private void UpdateCursorPos()
    {
        _mouseCursor.position = _playerShip.MousePos;
    }
    private void OnMouseScroll(float value)
    {
        float ortSize = _cinemachineCamera.Lens.OrthographicSize;
        ortSize -= value * _changeOrtSizeStep;
        targetOrtSize = Mathf.Clamp(ortSize, _minMaxOrtSize.x, _minMaxOrtSize.y);
    }
}
