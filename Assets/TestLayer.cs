using DefenseLayers;
using UnityEngine;

public class TestLayer : MonoBehaviour
{
    private void Start()
    {
        TryGetComponent(out DefenseLayerBase component);
        component.Init(500);
    }
}
