using UnityEngine;

namespace Enviroment
{
    public class StarryCanvasTwinkleView : MonoBehaviour
    {
        public static readonly int OffsetID = Shader.PropertyToID("_Offset");
        [field: SerializeField] public float SpeedMod { get; private set; }
        [field: SerializeField] public SpriteRenderer Renderer { get; set; }
        [field: SerializeField] public Transform Transform { get; set; }
        public Material Material { get; private set; }
        public Vector2 LastOffset { get; set; }
        public void Init()
        {
            Material = Renderer.material;
            Random.InitState(Random.Range(0, 100));
            Vector2 randomOffset = new()
            {
                x = Random.Range(0, 50),
                y = Random.Range(0, 50)
            };
            LastOffset = randomOffset;
            Material.SetVector(OffsetID, randomOffset);
        }
    }
}