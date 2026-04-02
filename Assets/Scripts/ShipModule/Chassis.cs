using UnityEngine;

[CreateAssetMenu(fileName = "Chassis", menuName = "Scriptable Objects/Chassis")]
public class Chassis : ScriptableObject, IShipModule
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Size Size { get; private set; }
    [field: SerializeField] public float Mass { get; private set; }
    [field: SerializeField] public float DirectDrag { get; private set; }
    [field: SerializeField] public float ReverseDrag { get; private set; }
    [field: SerializeField] public float StrafeDrag { get; private set; }
    [field: SerializeField] public float RotateDrag { get; private set; }
}
