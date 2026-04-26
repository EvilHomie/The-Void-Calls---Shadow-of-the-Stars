using DI;
using GameInput;
using GameSystems;
using Ships;
using Unity.Cinemachine;
using UnityEngine;

namespace GameCamera
{
    public class CameraRig : GameSystemBase
    {
        [MinMaxRangeSlider(1f, 10f)]
        [SerializeField] Vector2 _minMaxViewDistance;
        [SerializeField] CinemachineTargetGroup _cinemachineTargetGroup;
        [SerializeField] float _mouseCursorWeight = 0.8f;
        [SerializeField] CinemachineCamera _cinemachineCamera;
        [SerializeField] float _changeOrtSizeSpeed;
        [SerializeField] float _changeOrtSizeStep;

        private MouseCursor _mouseCursor;

        private IPlayerInput _input;
        private float _targetOrthographicSize;
        private float _deffOrthographicSize = 3f;
        private float _currentOrthographicSize;

        [Inject]
        public void Construct(MouseCursor mouseCursor, IPlayerInput playerInput)
        {
            _mouseCursor = mouseCursor;
            _input = playerInput;
        }

        protected override void AwakeInit()
        {           
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
            if (_currentOrthographicSize != _targetOrthographicSize)
            {
                _currentOrthographicSize = Mathf.MoveTowards(_currentOrthographicSize, _targetOrthographicSize, dTime * _changeOrtSizeSpeed);
                _cinemachineCamera.Lens.OrthographicSize = _currentOrthographicSize;
                float orthorelative = _currentOrthographicSize / _deffOrthographicSize;
                EventBus.ChangeCameraOrtoSize?.Invoke(orthorelative);
            }
        }

        private void OnSpawnPlayerShip(ShipInstance shipInstance)
        {
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
                Object = _mouseCursor.transform,
                Weight = _mouseCursorWeight,
                Radius = 1,
            };

            _cinemachineTargetGroup.Targets.Add(playerTarget);
            _cinemachineTargetGroup.Targets.Add(cursorTarget);
        }

        private void OnMouseScroll(float value)
        {
            _targetOrthographicSize -= value * _changeOrtSizeStep;
            _targetOrthographicSize = Mathf.Clamp(_targetOrthographicSize, _minMaxViewDistance.x, _minMaxViewDistance.y);
        }
    }
}