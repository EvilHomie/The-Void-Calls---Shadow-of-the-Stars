using DI;
using General;
using PlayerInput;
using Ships;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace CoreGameSystems
{
    public class CameraRigSystem : MonoBehaviour
    {        
        [SerializeField] float _mouseCursorWeight;
        [SerializeField] float _changeOrtSizeSpeed;

        public Action<float> CameraOrtoSizeChanged { get; set; }
        private MouseCursorSystem _mouseCursorSystem;
        CinemachineTargetGroup _cinemachineTargetGroup;
        CinemachineCamera _cinemachineCamera;
        private PlayerIntentData _playerIntentData;
        private float _targetOrthographicSize;
        private float _currentOrthographicSize;
        private Dictionary<SizeType, Vector2> _viewDistanceMap;
        private Vector2 _currentViewDistance;

        [Inject]
        public void Construct(
            MouseCursorSystem mouseCursorSystem,
            PlayerIntentData playerIntentData,
            CinemachineCamera cinemachineCamera,
            CinemachineTargetGroup cinemachineTargetGroup)
        {
            _mouseCursorSystem = mouseCursorSystem;
            _playerIntentData = playerIntentData;
            _cinemachineTargetGroup = cinemachineTargetGroup;
            _cinemachineCamera = cinemachineCamera;

            _viewDistanceMap = new()
            {
                {SizeType.S, new (1,2) },
                {SizeType.M, new (1,4) },
                {SizeType.L, new (1,8) },
                {SizeType.XL, new (1,16) }
            };
            EventBus.PlayerShipSpawned += Init;
        }

        public void Execute(float deltaTime)
        {
            if (_playerIntentData.ChangeZoom != 0)
            {
                OnMouseScroll(_playerIntentData.ChangeZoom);
            }


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
                Object = _mouseCursorSystem.CursorTransform,
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