using DI;
using GameSystems;
using Registries;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCamera
{
    public class MouseCursor : MonoBehaviour , ISyncable
    {
        [SerializeField] CursorConfig CoreGamePlayConfig;        
        
        public Vector2 ScreenPostition;
        public Vector2 WorldPostition;
        private Camera _mainCamera;
        private Transform _transform;

        [Inject]
        public void Construct(Camera camera, RegistrySyncSystem registrySyncSystem)
        {
            _mainCamera = camera;
            _transform = transform;
            registrySyncSystem.Add(this);
            Cursor.SetCursor(CoreGamePlayConfig.CursorTexture, CoreGamePlayConfig.CursorHotspot, CursorMode.Auto);
        }


        //private void OnEnable()
        //{
        //    GameFlowSystem.PreUpdateTick += UpdatePositions;            
        //}

        //private void OnDisable()
        //{
        //    GameFlowSystem.PreUpdateTick -= UpdatePositions;
        //}

        private void UpdatePositions()
        {
            ScreenPostition = Mouse.current.position.ReadValue();
            WorldPostition = _mainCamera.ScreenToWorldPoint(ScreenPostition);
            _transform.position = WorldPostition;
        }

        public void Sync()
        {
            UpdatePositions();
        }
    }

    [Serializable]
    public struct CursorConfig
    {
       public Texture2D CursorTexture;
       public Vector2 CursorHotspot;
       
    }
}