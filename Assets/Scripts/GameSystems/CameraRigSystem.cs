using DI;
using GameInput;
using Registries;
using Ships;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using MouseCursor = GameCamera.MouseCursor;

namespace GameSystems
{
    public class CameraRigSystem : GameSystemBase, ICoreUpdateTickObserver
    {
        [SerializeField] CinemachineTargetGroup _cinemachineTargetGroup;
        [SerializeField] float _mouseCursorWeight = 0.8f;
        [SerializeField] CinemachineCamera _cinemachineCamera;
        [SerializeField] float _changeOrtSizeSpeed;

        public Action<float> CameraOrtoSizeChanged { get; set; }
        private MouseCursor _mouseCursor;
        private ShipRegistry _shipRegistry;

        private IPlayerInput _input;
        private float _targetOrthographicSize;
        private float _currentOrthographicSize;
        private Dictionary<SizeType, Vector2> _viewDistanceMap;
        private Vector2 _currentViewDistance;

        [Inject]
        public void Construct(MouseCursor mouseCursor, IPlayerInput playerInput, ShipRegistry shipRegistry)
        {
            _shipRegistry = shipRegistry;
            _mouseCursor = mouseCursor;
            _input = playerInput;

            _viewDistanceMap = new()
            {
                {SizeType.S, new (1,5) },
                {SizeType.M, new (2,10) },
                {SizeType.L, new (8,20) },
                {SizeType.XL, new (1,30) }
            };
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
                CameraOrtoSizeChanged?.Invoke(_currentOrthographicSize);
            }
        }

        private void Init(ShipInstance shipInstance)
        {
            UpdateTargetGroup(shipInstance);
            _currentViewDistance = _viewDistanceMap[shipInstance.Size];
            _targetOrthographicSize = (_currentViewDistance.x + _currentViewDistance.y) / 2;
            _currentOrthographicSize = _cinemachineCamera.Lens.OrthographicSize;
            _changeOrtSizeSpeed = _currentViewDistance.y;
            CameraOrtoSizeChanged?.Invoke(_currentOrthographicSize);
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
            _targetOrthographicSize -= value;
            _targetOrthographicSize = Mathf.Clamp(_targetOrthographicSize, _currentViewDistance.x, _currentViewDistance.y);
        }
    }
}