using UnityEngine;

[CreateAssetMenu(fileName = "Chassis", menuName = "Scriptable Objects/Chassis")]
public class Chassis : ScriptableObject, IShipModule
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Size Size { get; private set; }
    [field: SerializeField] public int Hull { get; private set; }
    [field: SerializeField] public float Mass { get; private set; }
    [field: SerializeField] public float DirectDrag { get; private set; }
    [field: SerializeField] public float ReverseDrag { get; private set; }
    [field: SerializeField] public float StrafeDrag { get; private set; }
    [field: SerializeField] public float RotateDrag { get; private set; }

     public ChassisModificators ChassisModificators;    
}

public struct ChassisModificators
{
    public int MassModPercent;
    public int HullModPercent;
    public int DirectDragModPercent;
    public int ReverseDragModPercent;
    public int StrafeDragModPercent;
    public int RotateDragModPercent;
}