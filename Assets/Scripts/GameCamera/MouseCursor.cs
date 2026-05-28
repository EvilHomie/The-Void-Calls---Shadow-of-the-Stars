using DI;
using GameSystems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCamera
{
    public class MouseCursor : MonoBehaviour , ICorePreUpdateTickObserver
    {
        [SerializeField] CursorConfig CursorConfig;        
        
        public Vector2 ScreenPostition;
        public Vector2 WorldPostition;
        private Camera _camera;
        private Transform _transform;

        [Inject]
        public void Construct(Camera camera, GameFlowSystem gameFlowSystem)
        {
            _camera = camera;
            _transform = transform;
            gameFlowSystem.AddTickObserver(this);
            Cursor.SetCursor(CursorConfig.CursorTexture, CursorConfig.CursorHotspot, CursorMode.Auto);
        }

        public void CorePreUpdateTick()
        {
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            ScreenPostition = Mouse.current.position.ReadValue();
            WorldPostition = _camera.ScreenToWorldPoint(ScreenPostition);
            _transform.position = WorldPostition;
        }
    }

    [Serializable]
    public struct CursorConfig
    {
       public Texture2D CursorTexture;
       public Vector2 CursorHotspot;
       
    }
}