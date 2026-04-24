using DI;
using GameInput;
using GameSystems;
using Ships;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRig : GameSystemBase
{
    [MinMaxRangeSlider(1f, 10f)]
    [SerializeField] Vector2 _minMaxViewDistance;
    [SerializeField] CinemachineTargetGroup _cinemachineTargetGroup;
    [SerializeField] float _mouseCursorWeight = 0.8f;
    [SerializeField] CinemachineCamera _cinemachineCamera;
    [SerializeField] float _changeOrtSizeSpeed;
    [SerializeField] float _changeOrtSizeStep;
    [SerializeField] float _lookAheadDistanceMod = 1f;
    private LookAheadCursor _lookAheadCursor;
    private IPlayerInput _input;
    private float _targetOrthographicSize;
    private float _deffOrthographicSize = 3f;
    private ShipInstance _playerShipInstance;


    private float _currentOrthographicSize;
    private float _screenHeight;
    private float _screenWidth;

    [Inject]
    public void Construct(LookAheadCursor lookAheadCursor, IPlayerInput playerInput)
    {
        _lookAheadCursor = lookAheadCursor;
        _input = playerInput;
    }

    protected override void AwakeInit()
    {
        _screenHeight = Screen.height;
        _screenWidth = Screen.width;
    }

    protected override void Subscribe()
    {
        GameFlowSystem.UpdateTick += OnUpdateTick;
        _input.ChangeZoomAction += OnMouseScroll;
        EventBus.SpawnPlayerShip += OnSpawnPlayerShip;
    }

    protected override void Unsubscribe()
    {
        GameFlowSystem.UpdateTick -= OnUpdateTick;
        _input.ChangeZoomAction -= OnMouseScroll;
        EventBus.SpawnPlayerShip -= OnSpawnPlayerShip;
    }

    private void OnUpdateTick(float dTime)
    {
        var orthoDelta = 1f;

        if (_currentOrthographicSize != _targetOrthographicSize)
        {
            //_currentOrthographicSize = Mathf.Lerp(_currentOrthographicSize, _targetOrthographicSize, dTime * _changeOrtSizeSpeed);
            _currentOrthographicSize = Mathf.MoveTowards(_currentOrthographicSize, _targetOrthographicSize, dTime * _changeOrtSizeSpeed);
            //orthoDelta = _cinemachineCamera.Lens.OrthographicSize / ortho;
            _cinemachineCamera.Lens.OrthographicSize = _currentOrthographicSize;
            float orthorelative = _currentOrthographicSize / _deffOrthographicSize;
            EventBus.ChangeCameraOrtoSize?.Invoke(orthorelative);
        }

        UpdateLookAheadCursorPos(orthoDelta);
    }

    private void OnSpawnPlayerShip(ShipInstance shipInstance)
    {
        _playerShipInstance = shipInstance;
        UpdateTargetGroup(shipInstance);
        _targetOrthographicSize = (_minMaxViewDistance.x + _minMaxViewDistance.y) / 2;
        _deffOrthographicSize = _targetOrthographicSize;
        _currentOrthographicSize = _cinemachineCamera.Lens.OrthographicSize;
    }

    private void UpdateTargetGroup(ShipInstance shipInstance)
    {
        _cinemachineTargetGroup.Targets.Clear();

        var shipTransform = shipInstance.transform;

        var playerTarget = new CinemachineTargetGroup.Target()
        {
            Object = shipTransform,
            Weight = 1,
            Radius = 1,
        };

        var cursorTarget = new CinemachineTargetGroup.Target()
        {
            Object = _lookAheadCursor.Transform,
            Weight = _mouseCursorWeight,
            Radius = 1,
        };

        _cinemachineTargetGroup.Targets.Add(playerTarget);
        _cinemachineTargetGroup.Targets.Add(cursorTarget);
    }

    private void UpdateLookAheadCursorPos(float orthoDelta)
    {
        var playerShipPosition = _playerShipInstance.Rigidbody.position;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 screenCenter = new Vector2(_screenWidth, _screenHeight) * 0.5f;
        Vector2 mouseOffset = mousePos - screenCenter;
        Vector2 relativeOffset = mouseOffset / _screenHeight;
        //relativeOffset /= orthoDelta;
        _lookAheadCursor.Transform.position = playerShipPosition + _currentOrthographicSize * _lookAheadDistanceMod * relativeOffset;
    }

    private void OnMouseScroll(float value)
    {
        _targetOrthographicSize -= value * _changeOrtSizeStep;
        _targetOrthographicSize = Mathf.Clamp(_targetOrthographicSize, _minMaxViewDistance.x, _minMaxViewDistance.y);
    }
}
