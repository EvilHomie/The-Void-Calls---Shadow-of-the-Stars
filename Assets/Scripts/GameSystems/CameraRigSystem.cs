using DI;
using GameInput;
using Registries;
using Ships;
using System;
using Unity.Cinemachine;
using UnityEngine;
using MouseCursor = GameCamera.MouseCursor;

namespace GameSystems
{
    public class CameraRigSystem : GameSystemBase, ICoreUpdateTickObserver
    {
        [MinMaxRangeSlider(1f, 10f)]
        [SerializeField] Vector2 _minMaxViewDistance;
        [SerializeField] CinemachineTargetGroup _cinemachineTargetGroup;
        [SerializeField] float _mouseCursorWeight = 0.8f;
        [SerializeField] CinemachineCamera _cinemachineCamera;
        [SerializeField] float _changeOrtSizeSpeed;
        [SerializeField] float _changeOrtSizeStep;

        public  Action<float> CameraOrtoSizeChanged { get; set; }
        private MouseCursor _mouseCursor;
        private ShipRegistry _shipRegistry;

        private IPlayerInput _input;
        private float _targetOrthographicSize;
        private float _deffOrthographicSize = 3f;
        private float _currentOrthographicSize;

        [Inject]
        public void Construct(MouseCursor mouseCursor, IPlayerInput playerInput, ShipRegistry shipRegistry)
        {
            _shipRegistry = shipRegistry;
            _mouseCursor = mouseCursor;
            _input = playerInput;
        }

        private void Start()
        {
            var playerShip = _shipRegistry.PlayerShip;
            Init(playerShip);
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            _input.ChangeZoomAction += OnMouseScroll;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            _input.ChangeZoomAction -= OnMouseScroll;
        }

        public void CoreUpdateTick(float deltaTime)
        {
            if (_currentOrthographicSize != _targetOrthographicSize)
            {
                _currentOrthographicSize = Mathf.MoveTowards(_currentOrthographicSize, _targetOrthographicSize, deltaTime * _changeOrtSizeSpeed);
                _cinemachineCamera.Lens.OrthographicSize = _currentOrthographicSize;
                float orthorelative = _currentOrthographicSize / _deffOrthographicSize;
                CameraOrtoSizeChanged?.Invoke(orthorelative);
            }
        }

        private void Init(ShipInstance shipInstance)
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