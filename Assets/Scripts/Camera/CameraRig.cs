using DI;
using GameInput;
using GameSystems;
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
    private Transform _lookAheadCursor;
    private ShipsStorage _objectsStorage;
    private IPlayerInput _input;
    private float _targetOrtSize;
    private float _deffCameraOrtoSize = 3f;

    [Inject]
    public void Construct(LookAheadCursor lookAheadCursor, ShipsStorage objectsStorage, IPlayerInput playerInput)
    {
        _lookAheadCursor = lookAheadCursor.transform;
        _objectsStorage = objectsStorage;
        _input = playerInput;
    }

    protected override void AwakeInit()
    {

    }

    protected override void Subscribe()
    {
        GameFlow.UpdateTick += OnUpdateTick;
        _input.ChangeZoomAction += OnMouseScroll;
        EventBus.PlayerChangeShip += OnChangeShip;
    }

    protected override void Unsubscribe()
    {
        GameFlow.UpdateTick -= OnUpdateTick;
        _input.ChangeZoomAction -= OnMouseScroll;
        EventBus.PlayerChangeShip -= OnChangeShip;
    }

    private void OnUpdateTick(float dTime)
    {
        float ortho = _cinemachineCamera.Lens.OrthographicSize;
        float orthoDelta = 1;

        if (ortho != _targetOrtSize)
        {
            ortho = Mathf.Lerp(ortho, _targetOrtSize, dTime * _changeOrtSizeSpeed);
            //ortho = Mathf.MoveTowards(ortho, _targetOrtSize, dTime * _changeOrtSizeSpeed);
            orthoDelta = _cinemachineCamera.Lens.OrthographicSize / ortho;
            _cinemachineCamera.Lens.OrthographicSize = ortho;
            float orthorelative = ortho / _deffCameraOrtoSize;
            EventBus.ChangeCameraOrtoSize?.Invoke(orthorelative);
        }

        UpdateLookAheadCursorPos(orthoDelta);
    }

    private void OnChangeShip()
    {
        UpdateTargetGroup();
        _targetOrtSize = (_minMaxViewDistance.x + _minMaxViewDistance.y) / 2;
        _deffCameraOrtoSize = _targetOrtSize;
        _cinemachineCamera.Lens.OrthographicSize = _targetOrtSize;
    }

    private void UpdateTargetGroup()
    {
        _cinemachineTargetGroup.Targets.Clear();

        var shipTransform = _objectsStorage.ViewDatas[_objectsStorage.PlayerIndex].Transform;

        var playerTarget = new CinemachineTargetGroup.Target()
        {
            Object = shipTransform,
            Weight = 1,
            Radius = 1,
        };

        var cursorTarget = new CinemachineTargetGroup.Target()
        {
            Object = _lookAheadCursor,
            Weight = _mouseCursorWeight,
            Radius = 1,
        };

        _cinemachineTargetGroup.Targets.Add(playerTarget);
        _cinemachineTargetGroup.Targets.Add(cursorTarget);
    }

    private void UpdateLookAheadCursorPos(float orthoDelta)
    {
        ref var playerShipPosition = ref _objectsStorage.Positions[_objectsStorage.PlayerIndex];
        float ortho = _cinemachineCamera.Lens.OrthographicSize;
        float height = Screen.height;
        float width = Screen.width;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 screenCenter = new Vector2(width, height) * 0.5f;
        Vector2 mouseOffset = mousePos - screenCenter;
        Vector2 relativeOffset = mouseOffset / height;
        relativeOffset /= orthoDelta;
        _lookAheadCursor.position = playerShipPosition + ortho * _lookAheadDistanceMod * relativeOffset;
    }

    private void OnMouseScroll(float value)
    {
        float ortSize = _cinemachineCamera.Lens.OrthographicSize;
        ortSize -= value * _changeOrtSizeStep;
        _targetOrtSize = Mathf.Clamp(ortSize, _minMaxViewDistance.x, _minMaxViewDistance.y);
    }
}
