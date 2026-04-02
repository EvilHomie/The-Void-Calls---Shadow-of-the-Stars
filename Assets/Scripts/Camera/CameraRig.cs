using DI;
using GameSystems;
using Player;
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
    private Transform _playerShipTransform;
    private PlayerShip _playerShip;
    private IPlayerInput _input;
    private float _targetOrtSize;
    private Camera _camera;
    private float _deffCameraOrtoSize = 3f;

    [Inject]
    public void Construct(LookAheadCursor lookAheadCursor, PlayerShip playerShip, IPlayerInput playerInput, Camera camera)
    {
        _lookAheadCursor = lookAheadCursor.transform;
        _playerShip = playerShip;
        _playerShipTransform = playerShip.transform;
        _input = playerInput;
        _camera = camera;
    }

    protected override void Init()
    {
        _cinemachineTargetGroup.Targets.Clear();

        var playerTarget = new CinemachineTargetGroup.Target()
        {
            Object = _playerShipTransform,
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

    private void Start()
    {
        //UpdateLookAheadCursorPos();
        OnChangeShip(_playerShip);
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

    private void OnChangeShip(PlayerShip ship)
    {
        _targetOrtSize = (_minMaxViewDistance.x + _minMaxViewDistance.y) / 2;
        _deffCameraOrtoSize = _targetOrtSize;
        _cinemachineCamera.Lens.OrthographicSize = _targetOrtSize;
        OnChangeOrtoSize();
    }

    private void OnChangeOrtoSize()
    {

    }
    private void UpdateLookAheadCursorPos(float orthoDelta)
    {
        float ortho = _cinemachineCamera.Lens.OrthographicSize;
        float height = Screen.height;
        float width = Screen.width;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 screenCenter = new Vector2(width, height) * 0.5f;
        Vector2 mouseOffset = mousePos - screenCenter;
        Vector3 relativeOffset = mouseOffset / height;
        relativeOffset /= orthoDelta;
        _lookAheadCursor.position = _playerShipTransform.position + ortho * _lookAheadDistanceMod * relativeOffset;
    }

    private void OnMouseScroll(float value)
    {
        float ortSize = _cinemachineCamera.Lens.OrthographicSize;
        ortSize -= value * _changeOrtSizeStep;
        _targetOrtSize = Mathf.Clamp(ortSize, _minMaxViewDistance.x, _minMaxViewDistance.y);
    }
}


//Vector2 mouse = Mouse.current.position.ReadValue();
//Vector3 viewport = new(mouse.x / width, mouse.y / height, 0f);

//Vector3 world = _camera.ViewportToWorldPoint(new (viewport.x, viewport.y, _camera.nearClipPlane));

//_lookAheadCursor.position = new Vector3(world.x, world.y, 0f);
