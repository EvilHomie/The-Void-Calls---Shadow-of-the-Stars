using UnityEngine;

namespace Environment
{
    public class StarryCanvasView : MonoBehaviour
    {
        [field: SerializeField] public Transform Transform {  get; private set; }
        [field: SerializeField] public StarryCanvasLayer[]  Layers {  get; private set; }
    }
}
