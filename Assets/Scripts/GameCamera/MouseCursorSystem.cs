using DI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreGameSystems
{
    public class MouseCursorSystem : MonoBehaviour
    {
        [SerializeField] Texture2D cursorTexture;
        [field: SerializeField] public Transform CursorTransform { get; private set; }

        public Vector2 ScreenPostition;
        public Vector2 WorldPostition;
        private Camera _camera;

        [Inject]
        public void Construct(Camera camera)
        {
            _camera = camera;
            var cursorHotSpot = new Vector2(cursorTexture.width, cursorTexture.height) * 0.5f;
            Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);
        }

        public void Execute()
        {
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            ScreenPostition = Mouse.current.position.ReadValue();
            WorldPostition = _camera.ScreenToWorldPoint(ScreenPostition);
            CursorTransform.position = WorldPostition;
        }
    }
}