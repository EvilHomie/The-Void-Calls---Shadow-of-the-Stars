using UnityEngine;

namespace Environment
{
    public class StarryCanvasLayer : MonoBehaviour
    {
        public static readonly int OffsetID = Shader.PropertyToID("_Offset");
        [field: SerializeField] public float SpeedMod { get; private set; }
        [field: SerializeField] public SpriteRenderer Renderer { get; set; }
        [field: SerializeField] public Transform Transform { get; set; }
        public MaterialPropertyBlock PropertyBlock;
        public Vector2 LastOffset;
    }
}