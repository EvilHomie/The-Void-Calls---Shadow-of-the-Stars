using UnityEngine;

[CreateAssetMenu(fileName = "MainEngine", menuName = "Scriptable Objects/MainEngine")]
public class MainEngine : ScriptableObject
{
    [field: SerializeField] public float DirectThrust { get; private set; }
    [field: SerializeField] public float ReverseThrust { get; private set; }
}
